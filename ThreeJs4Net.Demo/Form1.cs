using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;

namespace ThreeJs4Net.Demo
{
    public partial class Form1 : Form
    {
        private Example example;

        private bool _exampleLoaded;

        /// <summary>
        /// 
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glControl_Load(object sender, EventArgs e)
        {
            LoadSamplesFromAssembly(Assembly.GetExecutingAssembly());
            
            Text =
                GL.GetString(StringName.Vendor) + " " +
                GL.GetString(StringName.Renderer) + " " +
                GL.GetString(StringName.Version);

            toolStripStatusLabel.Text = string.Empty;

            //stats.begin();
        }


        /// <summary>
        /// 
        /// </summary>
        private void Render()
        {
            if (null != example)
                example.Render();

            //stats.update();

            this.glControl.SwapBuffers();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Application_Idle(object sender, EventArgs e)
        {
            while (glControl.IsIdle)
            {
                Render();
            }
        }

        private void glControl_Paint(object sender, PaintEventArgs e)
        {
            this.Render();
        }

        private int windowHalfX;
        private int windowHalfY;

        private void glControl_Resize(object sender, EventArgs e)
        {
            var control = sender as Control;

            if (control.ClientSize.Height == 0)
                control.ClientSize = new Size(control.ClientSize.Width, 1);

            //windowHalfX = control.ClientSize.Width / 2;
            //windowHalfY = control.ClientSize.Height / 2;

            if (null != example)
                example.Resize(control.ClientSize);
        }

        private int mouseX;
        private int mouseY;

        private void glControl_MouseMove(object sender, MouseEventArgs e)
        {
            var control = sender as Control;
            
            mouseX = (e.X - windowHalfX);
            mouseY = (e.Y - windowHalfY);

            if (null != example)
                example.MouseMove(control.ClientSize,   new Point(mouseX, mouseY));
        }

        private void glControl_MouseDown(object sender, MouseEventArgs e)
        {
            var control = sender as Control;

            mouseX = (e.X - windowHalfX);
            mouseY = (e.Y - windowHalfY);

            if (null != example)
                example.MouseDown(control.ClientSize, new Point(mouseX, mouseY));
        }

        private void glControl_MouseUp(object sender, MouseEventArgs e)
        {
            var control = sender as Control;

            mouseX = (e.X - windowHalfX);
            mouseY = (e.Y - windowHalfY);

            if (null != example)
                example.MouseUp(control.ClientSize, new Point(mouseX, mouseY));
        }

        private void glControl_MouseWheel(object sender, MouseEventArgs e)
        {
            var control = sender as Control;

            mouseX = (e.X - windowHalfX);
            mouseY = (e.Y - windowHalfY);

            if (null != example)
                example.MouseWheel(control.ClientSize, new Point(mouseX, mouseY), e.Delta);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        void LoadSamplesFromAssembly(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            var loC = new List<Color>(10);
            loC.Add(Color.Red); // 0
            loC.Add(Color.Orange); // 0
            loC.Add(Color.RosyBrown); // 0
            loC.Add(Color.Teal); // 0
            loC.Add(Color.Blue); // 0
            loC.Add(Color.Black); // 0

            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                var attributes = type.GetCustomAttributes(false);
                foreach (var example in attributes.OfType<ExampleAttribute>())
                {
                    // Add this example to the sample TreeView.
                    // First check whether the ExampleCategory exists in the tree (and add it if it doesn't).
                    // Then add the example as A child node on this category.

                    if (!this.treeViewSamples.Nodes.ContainsKey(example.Category.ToString()))
                    {
                        //          int category_index = GetImageIndexForSample(imageListSampleCategories, example.Category.ToString(), String.Empty);
                        this.treeViewSamples.Nodes.Add(example.Category.ToString(), String.Format("{0} samples", example.Category), -1, -1);
                    }

                    //       int image_index = GetImageIndexForSample(imageListSampleCategories, example.Category.ToString(), example.Subcategory);
                    var node = new TreeNode(example.Title, -1, -1);
                    node.Name = example.Title;

                    int u = (int)(example.LevelComplete * 5);

                    node.ForeColor = loC[u];
                    node.Tag = new ExampleInfo(type, example);

                    this.treeViewSamples.Nodes[example.Category.ToString()].Nodes.Add(node);
                }
            }

            treeViewSamples.Sort();

            // 
            treeViewSamples.ExpandAll();
        }

        private void treeViewSamples_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag != null)
            {
                ActivateNode(e.Node);
            }
        }

        void ActivateNode(TreeNode node)
        {
            if (node == null)
                return;

            if (node.Tag == null)
            {
                if (node.IsExpanded)
                    node.Collapse();
                else
                    node.Expand();
            }
            else
            {
                RunSample((ExampleInfo)node.Tag);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        void RunSample(ExampleInfo e)
        {
            if (null != example)
            {
                example.Unload();
                //example.Dispose();
                //example = null;

                toolStripStatusLabel.Tag = toolStripStatusLabel.Text = string.Empty;
                toolStripStatusLabel.LinkVisited = false;
            }

            Application.Idle -= Application_Idle;

            example = (Example)Activator.CreateInstance(e.Example);
            if (null != example)
            {
                example.Load(this.glControl);

                toolStripStatusLabel.Text = e.Example.Name.Replace("_", " - ");
                toolStripStatusLabel.Tag = e.Example.Name;

                // Ensure that the viewport and projection matrix are Set correctly.
                example.Resize(glControl.ClientSize);

                Application.Idle += Application_Idle;
            }
        }

        private void toolStripStatusLabelTheeCs_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://threejs.org");
            toolStripStatusLabelTheeCs.LinkVisited = true;
        }

        private void toolStripStatusLabel_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://threejs.org/examples/#" + toolStripStatusLabel.Tag);
            toolStripStatusLabel.LinkVisited = true;
        }

        private void toolStripStatusLabelOpenTK_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("www.opentk.com");
            toolStripStatusLabelOpenTK.LinkVisited = true;
        }

        private void toolStripStatusLabelThreeCs_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("www.threeCs.org");
            toolStripStatusLabelThreeCs.LinkVisited = true;
        }

    }
}
