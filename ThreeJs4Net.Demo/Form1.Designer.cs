namespace ThreeJs4Net.Demo
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.glControl = new OpenTK.GLControl();
            this.treeViewSamples = new System.Windows.Forms.TreeView();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.statusStrip2 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelThreeCs = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelTheeCs = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelOpenTK = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.statusStrip2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // glControl
            // 
            this.glControl.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.glControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glControl.Location = new System.Drawing.Point(0, 0);
            this.glControl.Name = "glControl";
            this.glControl.Size = new System.Drawing.Size(714, 729);
            this.glControl.TabIndex = 2;
            this.glControl.VSync = true;
            this.glControl.Load += new System.EventHandler(this.glControl_Load);
            this.glControl.Paint += new System.Windows.Forms.PaintEventHandler(this.glControl_Paint);
            this.glControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.glControl_MouseDown);
            this.glControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.glControl_MouseMove);
            this.glControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.glControl_MouseUp);
            this.glControl.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.glControl_MouseWheel);
            this.glControl.Resize += new System.EventHandler(this.glControl_Resize);
            // 
            // treeViewSamples
            // 
            this.treeViewSamples.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewSamples.Location = new System.Drawing.Point(0, 0);
            this.treeViewSamples.Name = "treeViewSamples";
            this.treeViewSamples.Size = new System.Drawing.Size(357, 729);
            this.treeViewSamples.TabIndex = 3;
            this.treeViewSamples.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewSamples_AfterSelect);
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.statusStrip2);
            this.splitContainer.Panel1.Controls.Add(this.treeViewSamples);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.statusStrip1);
            this.splitContainer.Panel2.Controls.Add(this.glControl);
            this.splitContainer.Size = new System.Drawing.Size(1075, 729);
            this.splitContainer.SplitterDistance = 357;
            this.splitContainer.TabIndex = 4;
            // 
            // statusStrip2
            // 
            this.statusStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelThreeCs});
            this.statusStrip2.Location = new System.Drawing.Point(0, 707);
            this.statusStrip2.Name = "statusStrip2";
            this.statusStrip2.Size = new System.Drawing.Size(357, 22);
            this.statusStrip2.SizingGrip = false;
            this.statusStrip2.TabIndex = 4;
            this.statusStrip2.Text = "statusStrip2";
            // 
            // toolStripStatusLabelThreeCs
            // 
            this.toolStripStatusLabelThreeCs.IsLink = true;
            this.toolStripStatusLabelThreeCs.Name = "toolStripStatusLabelThreeCs";
            this.toolStripStatusLabelThreeCs.Size = new System.Drawing.Size(51, 17);
            this.toolStripStatusLabelThreeCs.Text = "Three.cs";
            this.toolStripStatusLabelThreeCs.Click += new System.EventHandler(this.toolStripStatusLabelThreeCs_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelTheeCs,
            this.toolStripStatusLabel,
            this.toolStripStatusLabel2,
            this.toolStripStatusLabelOpenTK});
            this.statusStrip1.Location = new System.Drawing.Point(0, 707);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(714, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelTheeCs
            // 
            this.toolStripStatusLabelTheeCs.IsLink = true;
            this.toolStripStatusLabelTheeCs.Name = "toolStripStatusLabelTheeCs";
            this.toolStripStatusLabelTheeCs.Size = new System.Drawing.Size(48, 17);
            this.toolStripStatusLabelTheeCs.Text = "Three.js";
            this.toolStripStatusLabelTheeCs.Click += new System.EventHandler(this.toolStripStatusLabelTheeCs_Click);
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.IsLink = true;
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel.Text = "toolStripStatusLabel1";
            this.toolStripStatusLabel.Click += new System.EventHandler(this.toolStripStatusLabel_Click);
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(483, 17);
            this.toolStripStatusLabel2.Spring = true;
            // 
            // toolStripStatusLabelOpenTK
            // 
            this.toolStripStatusLabelOpenTK.IsLink = true;
            this.toolStripStatusLabelOpenTK.Name = "toolStripStatusLabelOpenTK";
            this.toolStripStatusLabelOpenTK.Size = new System.Drawing.Size(50, 17);
            this.toolStripStatusLabelOpenTK.Text = "OpenTK";
            this.toolStripStatusLabelOpenTK.Click += new System.EventHandler(this.toolStripStatusLabelOpenTK_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1075, 729);
            this.Controls.Add(this.splitContainer);
            this.Name = "Form1";
            this.Text = "Form1";
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.statusStrip2.ResumeLayout(false);
            this.statusStrip2.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private OpenTK.GLControl glControl;
        private System.Windows.Forms.TreeView treeViewSamples;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelTheeCs;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelOpenTK;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.StatusStrip statusStrip2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelThreeCs;
    }
}

