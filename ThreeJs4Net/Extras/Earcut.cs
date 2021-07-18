using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreeJs4Net.Math;

namespace ThreeJs4Net.Extras
{
    public static class Earcut
    {
        public static List<int> Triangulate(List<float> data, List<int> holeIndices, int dim = 2)
        {
            var hasHoles = holeIndices != null && holeIndices.Any();
            var outerLen = hasHoles ? holeIndices[0] * dim : data.Count;
            var outerNode = LinkedList(data, 0, outerLen, dim, true);
            var triangles = new List<int>();

            if (outerNode == null || outerNode.next == outerNode.prev)
                return triangles;

            float minX = 0;
            float minY = 0;
            float maxX, maxY, x, y;
            float invSize = 0;

            if (hasHoles) outerNode = EliminateHoles(data, holeIndices, outerNode, dim);

            // if the shape is not too simple, we'll use z-order curve hash later; calculate polygon bbox
            if (data.Count > 80 * dim)
            {
                minX = maxX = data[0];
                minY = maxY = data[1];

                for (var i = dim; i < outerLen; i += dim)
                {
                    x = data[i];
                    y = data[i + 1];
                    if (x < minX) minX = x;
                    if (y < minY) minY = y;
                    if (x > maxX) maxX = x;
                    if (y > maxY) maxY = y;
                }

                // minX, minY and invSize are later used to transform coords into integers for z-order calculation
                invSize = Mathf.Max(maxX - minX, maxY - minY);
                invSize = invSize != 0 ? 1 / invSize : 0;
            }

            EarcutLinked(outerNode, triangles, dim, minX, minY, invSize, 0);

            return triangles;

        }

        // check whether a polygon node forms a valid ear with adjacent nodes
        private static bool IsEar(Node ear)
        {
            var a = ear.prev;
            var b = ear;
            var c = ear.next;

            if (Area(a, b, c) >= 0)
            {
                return false; // reflex, can't be an ear
            }

            // now make sure we don't have other points inside the potential ear
            var p = ear.next.next;

            while (p != ear.prev)
            {
                if (PointInTriangle(a.x, a.y, b.x, b.y, c.x, c.y, p.x, p.y) && Area(p.prev, p, p.next) >= 0)
                {
                    return false;
                }
                p = p.next;

            }

            return true;
        }

        private static bool IsEarHashed(Node ear, float minX, float minY, float invSize)
        {
            var a = ear.prev;
            var b = ear;
            var c = ear.next;

            if (Area(a, b, c) >= 0)
            {
                return false; // reflex, can't be an ear
            }

            // triangle bbox; min & max are calculated like this for speed
            var minTX = a.x < b.x ? (a.x < c.x ? a.x : c.x) : (b.x < c.x ? b.x : c.x);
            var minTY = a.y < b.y ? (a.y < c.y ? a.y : c.y) : (b.y < c.y ? b.y : c.y);
            var maxTX = a.x > b.x ? (a.x > c.x ? a.x : c.x) : (b.x > c.x ? b.x : c.x);
            var maxTY = a.y > b.y ? (a.y > c.y ? a.y : c.y) : (b.y > c.y ? b.y : c.y);

            // z-order range for the current triangle bbox;
            var minZ = zOrder(minTX, minTY, minX, minY, invSize);
            var maxZ = zOrder(maxTX, maxTY, minX, minY, invSize);

            var p = ear.prevZ;
            var n = ear.nextZ;

            // look for points inside the triangle in both directions
            while (p != null && p.z >= minZ && n != null && n.z <= maxZ)
            {
                if (p != ear.prev && p != ear.next &&
                    PointInTriangle(a.x, a.y, b.x, b.y, c.x, c.y, p.x, p.y) &&
                    Area(p.prev, p, p.next) >= 0)
                {
                    return false;
                }
                p = p.prevZ;

                if (n != ear.prev && n != ear.next &&
                    PointInTriangle(a.x, a.y, b.x, b.y, c.x, c.y, n.x, n.y) &&
                    Area(n.prev, n, n.next) >= 0)
                {
                    return false;
                }
                n = n.nextZ;
            }

            // look for remaining points in decreasing z-order
            while (p != null && p.z >= minZ)
            {
                if (p != ear.prev && p != ear.next &&
                    PointInTriangle(a.x, a.y, b.x, b.y, c.x, c.y, p.x, p.y) &&
                    Area(p.prev, p, p.next) >= 0)
                {
                    return false;
                }
                p = p.prevZ;
            }

            // look for remaining points in increasing z-order
            while (n != null && n.z <= maxZ)
            {
                if (n != ear.prev && n != ear.next &&
                    PointInTriangle(a.x, a.y, b.x, b.y, c.x, c.y, n.x, n.y) &&
                    Area(n.prev, n, n.next) >= 0)
                {
                    return false;
                }
                n = n.nextZ;
            }

            return true;
        }

        // go through all polygon nodes and cure small local self-intersections
        private static Node CureLocalIntersections(Node start, List<int> triangles, int dim)
        {
            var p = start;
            do
            {
                var a = p.prev;
                var b = p.next.next;

                if (!Equals(a, b) && Intersects(a, p, p.next, b) && LocallyInside(a, b) && LocallyInside(b, a))
                {
                    triangles.Add(a.i / dim);
                    triangles.Add(p.i / dim);
                    triangles.Add(b.i / dim);

                    // remove two nodes involved
                    RemoveNode(p);
                    RemoveNode(p.next);

                    p = start = b;
                }

                p = p.next;
            } while (p != start);

            return FilterPoints(p, null);
        }

        // link every hole into the outer loop, producing a single-ring polygon without holes
        private static Node EliminateHoles(List<float> data, List<int> holeIndices, Node outerNode, int dim)
        {
            List<Node> queue = new List<Node>();
            //var queue = [],
            //    , start, end, list;

            var len = holeIndices.Count;
            for (var i = 0; i < len; i++)
            {
                var start = holeIndices[i] * dim;
                var end = i < len - 1 ? holeIndices[i + 1] * dim : data.Count;
                var list = LinkedList(data, start, end, dim, false);
                if (list == list.next)
                {
                    list.steiner = true;
                }
                queue.Add(getLeftmost(list));
            }

            queue.Sort((a, b) =>
            {
                if (a.x == b.x) return 0;
                if (a.x < b.x) return -1;
                return 1;
            });

            // process holes from left to right
            for (var i = 0; i < queue.Count; i++)
            {
                EliminateHole(queue[i], outerNode);
                outerNode = FilterPoints(outerNode, outerNode.next);
            }

            return outerNode;
        }

        // find a bridge between vertices that connects hole with an outer ring and and link it
        private static void EliminateHole(Node hole, Node outerNode)
        {
            outerNode = FindHoleBridge(hole, outerNode);
            if (outerNode != null)
            {
                var b = SplitPolygon(outerNode, hole);

                // filter collinear points around the cuts
                FilterPoints(outerNode, outerNode.next);
                FilterPoints(b, b.next);
            }
        }

        // main ear slicing loop which triangulates a polygon (given as a linked list)
        private static void EarcutLinked(Node ear, List<int> triangles, int dim, float minX, float minY, float invSize, int pass)
        {
            if (ear == null)
            {
                return;
            }

            // interlink polygon nodes in z-order
            if (pass == 0 && invSize == 0)
            {
                IndexCurve(ear, minX, minY, invSize);
            }

            var stop = ear;
            Node prev, next;

            // iterate through ears, slicing them one by one
            while (ear.prev != ear.next)
            {
                prev = ear.prev;
                next = ear.next;

                if (invSize == 0 ? IsEarHashed(ear, minX, minY, invSize) : IsEar(ear))
                {
                    // cut off the triangle
                    triangles.Add(prev.i / dim);
                    triangles.Add(ear.i / dim);
                    triangles.Add(next.i / dim);

                    RemoveNode(ear);

                    // skipping the next vertex leads to less sliver triangles
                    ear = next.next;
                    stop = next.next;

                    continue;
                }

                ear = next;

                // if we looped through the whole remaining polygon and can't find any more ears
                if (ear == stop)
                {
                    // try filtering points and slicing again
                    if (pass == 0)
                    {
                        EarcutLinked(FilterPoints(ear, null), triangles, dim, minX, minY, invSize, 1);
                        // if this didn't work, try curing all small self-intersections locally
                    }
                    else if (pass == 1)
                    {
                        ear = CureLocalIntersections(FilterPoints(ear, null), triangles, dim);
                        EarcutLinked(ear, triangles, dim, minX, minY, invSize, 2);
                        // as a last resort, try splitting the remaining polygon into two
                    }
                    else if (pass == 2)
                    {
                        EarcutLinked(ear, triangles, dim, minX, minY, invSize, 0);
                    }
                    break;
                }
            }
        }

        // try splitting polygon into two and triangulate them independently
        private static void SplitEarcut(Node start, List<int> triangles, int dim, float minX, float minY, int invSize)
        {
            // look for a valid diagonal that divides the polygon into two
            var a = start;
            do
            {
                var b = a.next.next;

                while (b != a.prev)
                {
                    if (a.i != b.i && IsValidDiagonal(a, b))
                    {
                        // split the polygon in two by the diagonal
                        var c = SplitPolygon(a, b);

                        // filter colinear points around the cuts
                        a = FilterPoints(a, a.next);
                        c = FilterPoints(c, c.next);

                        // run earcut on each half
                        EarcutLinked(a, triangles, dim, minX, minY, invSize, 0);
                        EarcutLinked(c, triangles, dim, minX, minY, invSize, 0);
                        return;
                    }

                    b = b.next;

                }

                a = a.next;
            } while (a != start);
        }


        // David Eberly's algorithm for finding a bridge between hole and outer polygon
        private static Node FindHoleBridge(Node hole, Node outerNode)
        {
            var p = outerNode;
            var hx = hole.x;
            var hy = hole.y;
            var qx = float.NegativeInfinity;
            Node m = null;

            // find a segment intersected by a ray from the hole's leftmost point to the left;
            // segment's endpoint with lesser x will be potential connection point
            do
            {
                if (hy <= p.y && hy >= p.next.y && p.next.y != p.y)
                {
                    var x = p.x + (hy - p.y) * (p.next.x - p.x) / (p.next.y - p.y);
                    if (x <= hx && x > qx)
                    {
                        qx = x;
                        if (x == hx)
                        {
                            if (hy == p.y) return p;
                            if (hy == p.next.y) return p.next;
                        }

                        m = p.x < p.next.x ? p : p.next;
                    }
                }
                p = p.next;
            } while (p != outerNode);

            if (m == null) return null;

            if (hx == qx) return m; // hole touches outer segment; pick leftmost endpoint

            // look for points inside the triangle of hole point, segment intersection and endpoint;
            // if there are no points found, we have a valid connection;
            // otherwise choose the point of the minimum angle with the ray as connection point

            var stop = m;
            var mx = m.x;
            var my = m.y;
            var tanMin = float.PositiveInfinity;
            float tan;
            p = m;

            do
            {
                if (hx >= p.x && p.x >= mx && hx != p.x && PointInTriangle(hy < my ? hx : qx, hy, mx, my, hy < my ? qx : hx, hy, p.x, p.y))
                {
                    tan = Mathf.Abs(hy - p.y) / (hx - p.x); // tangential

                    if (LocallyInside(p, hole) && (tan < tanMin || (tan == tanMin && (p.x > m.x || (p.x == m.x && SectorContainsSector(m, p))))))
                    {
                        m = p;
                        tanMin = tan;
                    }
                }

                p = p.next;
            } while (p != stop);

            return m;
        }

        // eliminate colinear or duplicate points
        private static Node FilterPoints(Node start, Node end)
        {
            if (start == null) return start;
            if (end == null) end = start;

            var p = start;
            bool again;

            do
            {
                again = false;

                if (!p.steiner && (Equals(p, p.next) || Area(p.prev, p, p.next) == 0))
                {
                    RemoveNode(p);
                    p = end = p.prev;
                    if (p == p.next) break;
                    again = true;
                }
                else
                {
                    p = p.next;
                }
            } while (again || p != end);

            return end;
        }

        // create a circular doubly linked list from polygon points in the specified winding order
        private static Node LinkedList(List<float> data, int start, int end, int dim, bool clockwise)
        {
            Node last = null;
            int i;

            if (clockwise == (SignedArea(data, start, end, dim) > 0))
            {
                for (i = start; i < end; i += dim)
                {
                    last = InsertNode(i, data[i], data[i + 1], last);
                }
            }
            else
            {
                for (i = end - dim; i >= start; i -= dim)
                {
                    last = InsertNode(i, data[i], data[i + 1], last);
                }
            }

            if (last != null && Equals(last, last.next))
            {
                RemoveNode(last);
                last = last.next;
            }

            return last;

        }

        // signed area of a triangle
        private static float Area(Node p, Node q, Node r)
        {
            return (q.y - p.y) * (r.x - q.x) - (q.x - p.x) * (r.y - q.y);
        }

        // check if two points are equal
        private static bool Equals(Node p1, Node p2)
        {
            return p1.x == p2.x && p1.y == p2.y;
        }

        private static float SignedArea(List<float> data, int start, int end, int dim)
        {
            float sum = 0;
            var j = end - dim;
            for (var i = start; i < end; i += dim)
            {
                sum += (data[j] - data[i]) * (data[i + 1] + data[j + 1]);
                j = i;
            }

            return sum;
        }

        // create a node and optionally link it with previous one (in a circular doubly linked list)
        private static Node InsertNode(int i, float x, float y, Node last)
        {
            var p = new Node(i, x, y);

            if (last == null)
            {
                p.prev = p;
                p.next = p;
            }
            else
            {
                p.next = last.next;
                p.prev = last;
                last.next.prev = p;
                last.next = p;
            }

            return p;
        }

        private static void RemoveNode(Node p)
        {
            p.next.prev = p.prev;
            p.prev.next = p.next;

            if (p.prevZ != null) p.prevZ.nextZ = p.nextZ;
            if (p.nextZ != null) p.nextZ.prevZ = p.prevZ;
        }


        // link two polygon vertices with a bridge; if the vertices belong to the same ring, it splits polygon into two;
        // if one belongs to the outer ring and another to a hole, it merges it into a single ring
        private static Node SplitPolygon(Node a, Node b)
        {
            var a2 = new Node(a.i, a.x, a.y);
            var b2 = new Node(b.i, b.x, b.y);
            var an = a.next;
            var bp = b.prev;

            a.next = b;
            b.prev = a;

            a2.next = an;
            an.prev = a2;

            b2.next = a2;
            a2.prev = b2;

            bp.next = b2;
            b2.prev = bp;

            return b2;
        }

        // check if a polygon diagonal is locally inside the polygon
        private static bool LocallyInside(Node a, Node b)
        {
            return Area(a.prev, a, a.next) < 0 ?
                Area(a, b, a.next) >= 0 && Area(a, a.prev, b) >= 0 :
                Area(a, b, a.prev) < 0 || Area(a, a.next, b) < 0;

        }

        // check if the middle point of a polygon diagonal is inside the polygon
        private static bool MiddleInside(Node a, Node b)
        {
            var p = a;
            var inside = false;
            var px = (a.x + b.x) / 2;
            var py = (a.y + b.y) / 2;
            do
            {
                if (((p.y > py) != (p.next.y > py)) && p.next.y != p.y &&
                        (px < (p.next.x - p.x) * (py - p.y) / (p.next.y - p.y) + p.x))
                    inside = !inside;
                p = p.next;
            } while (p != a);

            return inside;
        }

        // for collinear points p, q, r, check if point q lies on segment pr
        private static bool OnSegment(Node p, Node q, Node r)
        {
            return q.x <= Mathf.Max(p.x, r.x) && q.x >= Mathf.Min(p.x, r.x) && q.y <= Mathf.Max(p.y, r.y) && q.y >= Mathf.Min(p.y, r.y);
        }

        private static int Sign(float num)
        {
            return num > 0 ? 1 : num < 0 ? -1 : 0;
        }

        // check if a polygon diagonal intersects any polygon segments
        private static bool IntersectsPolygon(Node a, Node b)
        {
            var p = a;
            do
            {
                if (p.i != a.i && p.next.i != a.i && p.i != b.i && p.next.i != b.i && Intersects(p, p.next, a, b))
                    return true;
                p = p.next;
            } while (p != a);

            return false;
        }

        // check if a diagonal between two polygon nodes is valid (lies in polygon interior)
        private static bool IsValidDiagonal(Node a, Node b)
        {
            return a.next.i != b.i && a.prev.i != b.i && !IntersectsPolygon(a, b) && // dones't intersect other edges
                (LocallyInside(a, b) && LocallyInside(b, a) && MiddleInside(a, b) && // locally visible
                (Area(a.prev, a, b.prev) != 0 || Area(a, b.prev, b) != 0) || // does not create opposite-facing sectors
                Equals(a, b) && Area(a.prev, a, a.next) > 0 && Area(b.prev, b, b.next) > 0); // special zero-length case
        }


        // check if two segments intersect
        private static bool Intersects(Node p1, Node q1, Node p2, Node q2)
        {
            var o1 = Sign(Area(p1, q1, p2));
            var o2 = Sign(Area(p1, q1, q2));
            var o3 = Sign(Area(p2, q2, p1));
            var o4 = Sign(Area(p2, q2, q1));

            if (o1 != o2 && o3 != o4)
                return true; // general case

            if (o1 == 0 && OnSegment(p1, p2, q1)) return true; // p1, q1 and p2 are collinear and p2 lies on p1q1
            if (o2 == 0 && OnSegment(p1, q2, q1)) return true; // p1, q1 and q2 are collinear and q2 lies on p1q1
            if (o3 == 0 && OnSegment(p2, p1, q2)) return true; // p2, q2 and p1 are collinear and p1 lies on p2q2
            if (o4 == 0 && OnSegment(p2, q1, q2)) return true; // p2, q2 and q1 are collinear and q1 lies on p2q2
            return false;
        }

        // find the leftmost node of a polygon ring
        private static Node getLeftmost(Node start)
        {
            Node p = start;
            Node leftmost = start;
            do
            {
                if (p.x < leftmost.x || (p.x == leftmost.x && p.y < leftmost.y))
                {
                    leftmost = p;
                }
                p = p.next;
            } while (p != start);
            return leftmost;
        }

        // check if a point lies within a convex triangle
        private static bool PointInTriangle(float ax, float ay, float bx, float by, float cx, float cy, float px, float py)
        {
            return (cx - px) * (ay - py) - (ax - px) * (cy - py) >= 0 &&
                    (ax - px) * (by - py) - (bx - px) * (ay - py) >= 0 &&
                    (bx - px) * (cy - py) - (cx - px) * (by - py) >= 0;
        }

        // whether sector in vertex m contains sector in vertex p in the same coordinates
        private static bool SectorContainsSector(Node m, Node p)
        {
            return Area(m.prev, m, p.prev) < 0 && Area(p.next, m, m.next) < 0;
        }

        // interlink polygon nodes in z-order
        private static void IndexCurve(Node start, float minX, float minY, float invSize)
        {
            var p = start;
            do
            {
                if (p.z == null)
                {
                    p.z = zOrder(p.x, p.y, minX, minY, invSize);
                }
                p.prevZ = p.prev;
                p.nextZ = p.next;
                p = p.next;

            } while (p != start);

            p.prevZ.nextZ = null;
            p.prevZ = null;

            SortLinked(p);
        }

        // z-order of a point given coords and inverse of the longer side of data bbox
        private static int zOrder(float x, float y, float minX, float minY, float invSize)
        {
            // coords are transformed into non-negative 15-bit integer range
            var lX = 32767 * (Mathf.Floor(x) - Mathf.Floor(minX)) * Mathf.Floor(invSize);
            var lY = 32767 * (Mathf.Floor(y) - Mathf.Floor(minY)) * Mathf.Floor(invSize);

            lX = (lX | (lX << 8)) & 0x00FF00FF;
            lX = (lX | (lX << 4)) & 0x0F0F0F0F;
            lX = (lX | (lX << 2)) & 0x33333333;
            lX = (lX | (lX << 1)) & 0x55555555;

            lY = (lY | (lY << 8)) & 0x00FF00FF;
            lY = (lY | (lY << 4)) & 0x0F0F0F0F;
            lY = (lY | (lY << 2)) & 0x33333333;
            lY = (lY | (lY << 1)) & 0x55555555;

            return lX | (lY << 1);
        }

        // Simon Tatham's linked list merge sort algorithm
        // http://www.chiark.greenend.org.uk/~sgtatham/algorithms/listsort.html
        private static Node SortLinked(Node list)
        {
            //var i, p, q, e, tail, numMerges, pSize, qSize,
            int inSize = 1;
            int numMerges = 0;
            Node e = null;

            do
            {
                var p = list;
                list = null;
                Node tail = null;
                numMerges = 0;

                while (p != null)
                {
                    numMerges++;
                    var q = p;
                    var pSize = 0;
                    for (var i = 0; i < inSize; i++)
                    {
                        pSize++;
                        q = q.nextZ;
                        if (q == null)
                            break;
                    }

                    var qSize = inSize;

                    while (pSize > 0 || (qSize > 0 && q != null))
                    {
                        if (pSize != 0 && (qSize == 0 || q == null || p.z <= q.z))
                        {
                            e = p;
                            p = p.nextZ;
                            pSize--;
                        }
                        else
                        {
                            e = q;
                            q = q.nextZ;
                            qSize--;
                        }

                        if (tail != null)
                            tail.nextZ = e;
                        else
                            list = e;

                        e.prevZ = tail;
                        tail = e;
                    }

                    p = q;
                }

                tail.nextZ = null;
                inSize *= 2;
            } while (numMerges > 1);

            return list;
        }
    }

    class Node
    {
        public int i;
        public float x;
        public float y;
        public Node next;
        public Node prev;
        public float? z;
        public Node nextZ;
        public Node prevZ;
        public bool steiner;

        public Node(int i, float x, float y)
        {
            // vertex index in coordinates array
            this.i = i;

            // vertex coordinates
            this.x = x;
            this.y = y;

            // previous and next vertex nodes in a polygon ring
            this.prev = null;
            this.next = null;

            // z-order curve value
            this.z = null;

            // previous and next nodes in z-order
            this.prevZ = null;
            this.nextZ = null;

            // indicates whether this is a steiner point
            this.steiner = false;
        }
    }
}
