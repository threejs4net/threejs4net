using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ThreeJs4Net.Math;
using ThreeJs4Net.Textures;

namespace ThreeJs4Net.Objects
{
    public class Skeleton : IDisposable
    {
        private Matrix4 _offsetMatrix = new Matrix4();
        private Matrix4 _identityMatrix = new Matrix4();
        private float[] boneMatrices;
        private int Frame;
        private List<Matrix4> boneInverses;
        private DataTexture boneTexture;

        public List<Bone> Bones { get; } = new List<Bone>();

        public Skeleton(IEnumerable<Bone> bones = null, IEnumerable<Matrix4> boneInverses = null)
        {
            // copy the bone array
            bones ??= new List<Bone>();

            this.Bones.Clear();
            this.Bones.AddRange(bones);
            this.boneMatrices = new float[this.Bones.Count * 16];

            this.Frame = -1;

            // use the supplied bone inverses or calculate the inverses
            if (boneInverses == null)
            {
                this.CalculateInverses();
            }
            else
            {
                if (this.Bones.Count == boneInverses.Count())
                {
                    this.boneInverses.Clear();
                    this.boneInverses.AddRange(boneInverses);
                }
                else
                {
                    //console.warn('THREE.Skeleton boneInverses is the wrong length.');

                    this.boneInverses = new List<Matrix4>();
                    for (var i = 0; i < this.Bones.Count; i++)
                    {
                        this.boneInverses.Add(new Matrix4());
                    }
                }
            }
        }

        public void CalculateInverses()
        {
            this.boneInverses = new List<Matrix4>();

            foreach (var t in this.Bones)
            {
                var inverse = new Matrix4();
                if (t != null)
                {
                    inverse.GetInverse(t.MatrixWorld);
                }
                this.boneInverses.Add(inverse);
            }
        }

        public void Pose()
        {
            Bone bone;
            int i;

            // recover the bind-time world matrices
            for (i = 0; i < this.Bones.Count; i++)
            {
                bone = this.Bones[i];
                bone?.MatrixWorld.GetInverse(this.boneInverses[i]);
            }

            // compute the local matrices, positions, rotations and scales
            for (i = 0; i < this.Bones.Count; i++)
            {
                bone = this.Bones[i];
                if (bone != null)
                {
                    if (bone.Parent is Bone)
                    {
                        bone.Matrix.GetInverse(bone.Parent.MatrixWorld);
                        bone.Matrix.Multiply(bone.MatrixWorld);
                    }
                    else
                    {
                        bone.Matrix.Copy(bone.MatrixWorld);
                    }
                    bone.Matrix.Decompose(bone.Position, bone.Quaternion, bone.Scale);
                }
            }
        }

        public void Update()
        {
            var bones = this.Bones;
            var boneInverses = this.boneInverses;
            var boneMatrices = this.boneMatrices;
            var boneTexture = this.boneTexture;

            // flatten bone matrices to array

            for (var i = 0; i < bones.Count; i++)
            {
                // compute the offset between the current and the original transform
                var matrix = bones[i] != null ? bones[i].MatrixWorld : _identityMatrix;
                _offsetMatrix.MultiplyMatrices(matrix, boneInverses[i]);
                _offsetMatrix.ToArray(ref boneMatrices, i * 16);
            }

            if (boneTexture != null)
            {
                boneTexture.NeedsUpdate = true;
            }
        }

        public Skeleton Clone()
        {
            return new Skeleton(this.Bones, this.boneInverses);
        }

        public Bone GetBoneByName(string name)
        {
            //TODO: Confirm is is unique. Otherwise we will need to use FirstOrDefault
            return this.Bones.SingleOrDefault(h => h.Name == name);
        }

        public void Dispose()
        {
            boneTexture?.Dispose();
        }
    }
}
