namespace MyTubeCutting
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing && ( components != null ) ) {
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.m_panViewer = new System.Windows.Forms.Panel();
			this.m_btnBranchTube = new System.Windows.Forms.Button();
			this.m_btnEndCutter = new System.Windows.Forms.Button();
			this.m_btnMainTube = new System.Windows.Forms.Button();
			this.m_panMainTubeParam = new System.Windows.Forms.Panel();
			this.m_treeObjBrowser = new System.Windows.Forms.TreeView();
			this.m_panBranchTubeParam = new System.Windows.Forms.Panel();
			this.m_propgrdPropertyBar = new System.Windows.Forms.PropertyGrid();
			this.m_panViewer.SuspendLayout();
			this.m_panMainTubeParam.SuspendLayout();
			this.m_panBranchTubeParam.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_panViewer
			// 
			this.m_panViewer.Controls.Add(this.m_btnBranchTube);
			this.m_panViewer.Controls.Add(this.m_btnEndCutter);
			this.m_panViewer.Controls.Add(this.m_btnMainTube);
			this.m_panViewer.Location = new System.Drawing.Point(0, 0);
			this.m_panViewer.Name = "m_panViewer";
			this.m_panViewer.Size = new System.Drawing.Size(500, 600);
			this.m_panViewer.TabIndex = 0;
			this.m_panViewer.Paint += new System.Windows.Forms.PaintEventHandler(this.m_panViewer_Paint);
			this.m_panViewer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.m_panViewer_MouseDown);
			this.m_panViewer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.m_panViewer_MouseMove);
			this.m_panViewer.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.m_panViewer_MouseWheel);
			// 
			// m_btnBranchTube
			// 
			this.m_btnBranchTube.Location = new System.Drawing.Point(394, 74);
			this.m_btnBranchTube.Name = "m_btnBranchTube";
			this.m_btnBranchTube.Size = new System.Drawing.Size(100, 25);
			this.m_btnBranchTube.TabIndex = 2;
			this.m_btnBranchTube.Text = "BranchTube";
			this.m_btnBranchTube.UseVisualStyleBackColor = true;
			this.m_btnBranchTube.Click += new System.EventHandler(this.m_btnBranchTube_Click);
			// 
			// m_btnEndCutter
			// 
			this.m_btnEndCutter.Location = new System.Drawing.Point(394, 43);
			this.m_btnEndCutter.Name = "m_btnEndCutter";
			this.m_btnEndCutter.Size = new System.Drawing.Size(100, 25);
			this.m_btnEndCutter.TabIndex = 1;
			this.m_btnEndCutter.Text = "EndCutter";
			this.m_btnEndCutter.UseVisualStyleBackColor = true;
			this.m_btnEndCutter.Click += new System.EventHandler(this.m_btnEndCutter_Click);
			// 
			// m_btnMainTube
			// 
			this.m_btnMainTube.Location = new System.Drawing.Point(394, 12);
			this.m_btnMainTube.Name = "m_btnMainTube";
			this.m_btnMainTube.Size = new System.Drawing.Size(100, 25);
			this.m_btnMainTube.TabIndex = 0;
			this.m_btnMainTube.Text = "MainTube";
			this.m_btnMainTube.UseVisualStyleBackColor = true;
			this.m_btnMainTube.Click += new System.EventHandler(this.m_btnMainTube_Click);
			// 
			// m_panMainTubeParam
			// 
			this.m_panMainTubeParam.Controls.Add(this.m_treeObjBrowser);
			this.m_panMainTubeParam.Location = new System.Drawing.Point(500, 0);
			this.m_panMainTubeParam.Name = "m_panMainTubeParam";
			this.m_panMainTubeParam.Size = new System.Drawing.Size(300, 300);
			this.m_panMainTubeParam.TabIndex = 1;
			// 
			// m_treeObjBrowser
			// 
			this.m_treeObjBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_treeObjBrowser.Location = new System.Drawing.Point(0, 0);
			this.m_treeObjBrowser.Name = "m_treeObjBrowser";
			this.m_treeObjBrowser.Size = new System.Drawing.Size(300, 300);
			this.m_treeObjBrowser.TabIndex = 0;
			this.m_treeObjBrowser.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.m_treeObjBrowser_NodeMouseClick);
			this.m_treeObjBrowser.KeyUp += new System.Windows.Forms.KeyEventHandler(this.m_treeObjBrowser_KeyUp);
			// 
			// m_panBranchTubeParam
			// 
			this.m_panBranchTubeParam.Controls.Add(this.m_propgrdPropertyBar);
			this.m_panBranchTubeParam.Location = new System.Drawing.Point(500, 300);
			this.m_panBranchTubeParam.Name = "m_panBranchTubeParam";
			this.m_panBranchTubeParam.Size = new System.Drawing.Size(300, 300);
			this.m_panBranchTubeParam.TabIndex = 2;
			// 
			// m_propgrdPropertyBar
			// 
			this.m_propgrdPropertyBar.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_propgrdPropertyBar.Location = new System.Drawing.Point(0, 0);
			this.m_propgrdPropertyBar.Name = "m_propgrdPropertyBar";
			this.m_propgrdPropertyBar.Size = new System.Drawing.Size(300, 300);
			this.m_propgrdPropertyBar.TabIndex = 0;
			this.m_propgrdPropertyBar.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.m_propgrdPropertyBar_PropertyValueChanged);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(801, 601);
			this.Controls.Add(this.m_panBranchTubeParam);
			this.Controls.Add(this.m_panMainTubeParam);
			this.Controls.Add(this.m_panViewer);
			this.Name = "MainForm";
			this.Text = "Form1";
			this.m_panViewer.ResumeLayout(false);
			this.m_panMainTubeParam.ResumeLayout(false);
			this.m_panBranchTubeParam.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel m_panViewer;
		private System.Windows.Forms.Panel m_panMainTubeParam;
		private System.Windows.Forms.Panel m_panBranchTubeParam;
		private System.Windows.Forms.Button m_btnMainTube;
		private System.Windows.Forms.TreeView m_treeObjBrowser;
		private System.Windows.Forms.PropertyGrid m_propgrdPropertyBar;
		private System.Windows.Forms.Button m_btnBranchTube;
		private System.Windows.Forms.Button m_btnEndCutter;
	}
}

