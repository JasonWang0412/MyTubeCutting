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
			this.m_btnBendingNotch = new System.Windows.Forms.Button();
			this.m_btnISO = new System.Windows.Forms.Button();
			this.m_btnDir_Neg = new System.Windows.Forms.Button();
			this.m_btnDir_Pos = new System.Windows.Forms.Button();
			this.m_btnZ_Neg = new System.Windows.Forms.Button();
			this.m_btnZ_Pos = new System.Windows.Forms.Button();
			this.m_btnY_Neg = new System.Windows.Forms.Button();
			this.m_btnY_Pos = new System.Windows.Forms.Button();
			this.m_btnX_Neg = new System.Windows.Forms.Button();
			this.m_btnX_Pos = new System.Windows.Forms.Button();
			this.m_btnBranchTube = new System.Windows.Forms.Button();
			this.m_btnEndCutter = new System.Windows.Forms.Button();
			this.m_btnMainTube = new System.Windows.Forms.Button();
			this.m_panObjBrowser = new System.Windows.Forms.Panel();
			this.m_treeObjBrowser = new System.Windows.Forms.TreeView();
			this.m_panPropertyBar = new System.Windows.Forms.Panel();
			this.m_propgrdPropertyBar = new System.Windows.Forms.PropertyGrid();
			this.m_btnExport = new System.Windows.Forms.Button();
			this.m_panViewer.SuspendLayout();
			this.m_panObjBrowser.SuspendLayout();
			this.m_panPropertyBar.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_panViewer
			// 
			this.m_panViewer.Controls.Add(this.m_btnExport);
			this.m_panViewer.Controls.Add(this.m_btnBendingNotch);
			this.m_panViewer.Controls.Add(this.m_btnISO);
			this.m_panViewer.Controls.Add(this.m_btnDir_Neg);
			this.m_panViewer.Controls.Add(this.m_btnDir_Pos);
			this.m_panViewer.Controls.Add(this.m_btnZ_Neg);
			this.m_panViewer.Controls.Add(this.m_btnZ_Pos);
			this.m_panViewer.Controls.Add(this.m_btnY_Neg);
			this.m_panViewer.Controls.Add(this.m_btnY_Pos);
			this.m_panViewer.Controls.Add(this.m_btnX_Neg);
			this.m_panViewer.Controls.Add(this.m_btnX_Pos);
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
			// m_btnBendingNotch
			// 
			this.m_btnBendingNotch.Location = new System.Drawing.Point(394, 105);
			this.m_btnBendingNotch.Name = "m_btnBendingNotch";
			this.m_btnBendingNotch.Size = new System.Drawing.Size(100, 25);
			this.m_btnBendingNotch.TabIndex = 12;
			this.m_btnBendingNotch.Text = "BendingNotch";
			this.m_btnBendingNotch.UseVisualStyleBackColor = true;
			this.m_btnBendingNotch.Click += new System.EventHandler(this.m_btnBendingNotch_Click);
			// 
			// m_btnISO
			// 
			this.m_btnISO.Location = new System.Drawing.Point(13, 133);
			this.m_btnISO.Name = "m_btnISO";
			this.m_btnISO.Size = new System.Drawing.Size(40, 23);
			this.m_btnISO.TabIndex = 11;
			this.m_btnISO.Text = "ISO";
			this.m_btnISO.UseVisualStyleBackColor = true;
			this.m_btnISO.Click += new System.EventHandler(this.m_btnISO_Click);
			// 
			// m_btnDir_Neg
			// 
			this.m_btnDir_Neg.Location = new System.Drawing.Point(60, 103);
			this.m_btnDir_Neg.Name = "m_btnDir_Neg";
			this.m_btnDir_Neg.Size = new System.Drawing.Size(39, 23);
			this.m_btnDir_Neg.TabIndex = 10;
			this.m_btnDir_Neg.Text = "Dir-";
			this.m_btnDir_Neg.UseVisualStyleBackColor = true;
			this.m_btnDir_Neg.Click += new System.EventHandler(this.m_btnDir_Neg_Click);
			// 
			// m_btnDir_Pos
			// 
			this.m_btnDir_Pos.Location = new System.Drawing.Point(13, 103);
			this.m_btnDir_Pos.Name = "m_btnDir_Pos";
			this.m_btnDir_Pos.Size = new System.Drawing.Size(40, 23);
			this.m_btnDir_Pos.TabIndex = 9;
			this.m_btnDir_Pos.Text = "Dir+";
			this.m_btnDir_Pos.UseVisualStyleBackColor = true;
			this.m_btnDir_Pos.Click += new System.EventHandler(this.m_btnDir_Pos_Click);
			// 
			// m_btnZ_Neg
			// 
			this.m_btnZ_Neg.Location = new System.Drawing.Point(60, 73);
			this.m_btnZ_Neg.Name = "m_btnZ_Neg";
			this.m_btnZ_Neg.Size = new System.Drawing.Size(39, 23);
			this.m_btnZ_Neg.TabIndex = 8;
			this.m_btnZ_Neg.Text = "Z-";
			this.m_btnZ_Neg.UseVisualStyleBackColor = true;
			this.m_btnZ_Neg.Click += new System.EventHandler(this.m_btnZ_Neg_Click);
			// 
			// m_btnZ_Pos
			// 
			this.m_btnZ_Pos.Location = new System.Drawing.Point(13, 73);
			this.m_btnZ_Pos.Name = "m_btnZ_Pos";
			this.m_btnZ_Pos.Size = new System.Drawing.Size(40, 23);
			this.m_btnZ_Pos.TabIndex = 7;
			this.m_btnZ_Pos.Text = "Z+";
			this.m_btnZ_Pos.UseVisualStyleBackColor = true;
			this.m_btnZ_Pos.Click += new System.EventHandler(this.m_btnZ_Pos_Click);
			// 
			// m_btnY_Neg
			// 
			this.m_btnY_Neg.Location = new System.Drawing.Point(60, 43);
			this.m_btnY_Neg.Name = "m_btnY_Neg";
			this.m_btnY_Neg.Size = new System.Drawing.Size(39, 23);
			this.m_btnY_Neg.TabIndex = 6;
			this.m_btnY_Neg.Text = "Y-";
			this.m_btnY_Neg.UseVisualStyleBackColor = true;
			this.m_btnY_Neg.Click += new System.EventHandler(this.m_btnY_Neg_Click);
			// 
			// m_btnY_Pos
			// 
			this.m_btnY_Pos.Location = new System.Drawing.Point(13, 43);
			this.m_btnY_Pos.Name = "m_btnY_Pos";
			this.m_btnY_Pos.Size = new System.Drawing.Size(40, 23);
			this.m_btnY_Pos.TabIndex = 5;
			this.m_btnY_Pos.Text = "Y+";
			this.m_btnY_Pos.UseVisualStyleBackColor = true;
			this.m_btnY_Pos.Click += new System.EventHandler(this.m_btnY_Pos_Click);
			// 
			// m_btnX_Neg
			// 
			this.m_btnX_Neg.Location = new System.Drawing.Point(59, 13);
			this.m_btnX_Neg.Name = "m_btnX_Neg";
			this.m_btnX_Neg.Size = new System.Drawing.Size(40, 23);
			this.m_btnX_Neg.TabIndex = 4;
			this.m_btnX_Neg.Text = "X-";
			this.m_btnX_Neg.UseVisualStyleBackColor = true;
			this.m_btnX_Neg.Click += new System.EventHandler(this.m_btnX_Neg_Click);
			// 
			// m_btnX_Pos
			// 
			this.m_btnX_Pos.Location = new System.Drawing.Point(13, 13);
			this.m_btnX_Pos.Name = "m_btnX_Pos";
			this.m_btnX_Pos.Size = new System.Drawing.Size(40, 23);
			this.m_btnX_Pos.TabIndex = 3;
			this.m_btnX_Pos.Text = "X+";
			this.m_btnX_Pos.UseVisualStyleBackColor = true;
			this.m_btnX_Pos.Click += new System.EventHandler(this.m_btnX_Pos_Click);
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
			// m_panObjBrowser
			// 
			this.m_panObjBrowser.Controls.Add(this.m_treeObjBrowser);
			this.m_panObjBrowser.Location = new System.Drawing.Point(500, 0);
			this.m_panObjBrowser.Name = "m_panObjBrowser";
			this.m_panObjBrowser.Size = new System.Drawing.Size(300, 300);
			this.m_panObjBrowser.TabIndex = 1;
			// 
			// m_treeObjBrowser
			// 
			this.m_treeObjBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
			this.m_treeObjBrowser.Location = new System.Drawing.Point(0, 0);
			this.m_treeObjBrowser.Name = "m_treeObjBrowser";
			this.m_treeObjBrowser.Size = new System.Drawing.Size(300, 300);
			this.m_treeObjBrowser.TabIndex = 0;
			this.m_treeObjBrowser.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.m_treeObjBrowser_AfterSelect);
			// 
			// m_panPropertyBar
			// 
			this.m_panPropertyBar.Controls.Add(this.m_propgrdPropertyBar);
			this.m_panPropertyBar.Location = new System.Drawing.Point(500, 300);
			this.m_panPropertyBar.Name = "m_panPropertyBar";
			this.m_panPropertyBar.Size = new System.Drawing.Size(300, 300);
			this.m_panPropertyBar.TabIndex = 2;
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
			// m_btnExport
			// 
			this.m_btnExport.Location = new System.Drawing.Point(13, 163);
			this.m_btnExport.Name = "m_btnExport";
			this.m_btnExport.Size = new System.Drawing.Size(86, 23);
			this.m_btnExport.TabIndex = 13;
			this.m_btnExport.Text = "Export";
			this.m_btnExport.UseVisualStyleBackColor = true;
			this.m_btnExport.Click += new System.EventHandler(this.m_btnExport_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(801, 601);
			this.Controls.Add(this.m_panPropertyBar);
			this.Controls.Add(this.m_panObjBrowser);
			this.Controls.Add(this.m_panViewer);
			this.Name = "MainForm";
			this.Text = "Form1";
			this.m_panViewer.ResumeLayout(false);
			this.m_panObjBrowser.ResumeLayout(false);
			this.m_panPropertyBar.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel m_panViewer;
		private System.Windows.Forms.Panel m_panObjBrowser;
		private System.Windows.Forms.Panel m_panPropertyBar;
		private System.Windows.Forms.Button m_btnMainTube;
		private System.Windows.Forms.TreeView m_treeObjBrowser;
		private System.Windows.Forms.PropertyGrid m_propgrdPropertyBar;
		private System.Windows.Forms.Button m_btnBranchTube;
		private System.Windows.Forms.Button m_btnEndCutter;
		private System.Windows.Forms.Button m_btnISO;
		private System.Windows.Forms.Button m_btnDir_Neg;
		private System.Windows.Forms.Button m_btnDir_Pos;
		private System.Windows.Forms.Button m_btnZ_Neg;
		private System.Windows.Forms.Button m_btnZ_Pos;
		private System.Windows.Forms.Button m_btnY_Neg;
		private System.Windows.Forms.Button m_btnY_Pos;
		private System.Windows.Forms.Button m_btnX_Neg;
		private System.Windows.Forms.Button m_btnX_Pos;
		private System.Windows.Forms.Button m_btnBendingNotch;
		private System.Windows.Forms.Button m_btnExport;
	}
}

