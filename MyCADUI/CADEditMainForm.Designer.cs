namespace MyCADUI
{
	partial class CADEditMainForm
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
			this.m_msMainMenu = new System.Windows.Forms.MenuStrip();
			this.m_tsmiFile = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsmiExport = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsmiOpen = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsmiView = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsmiX_Pos = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsmiX_Neg = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsmiY_Pos = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsmiY_Neg = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsmiZ_Pos = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsmiZ_Neg = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsmiDir_Pos = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsmiDir_Neg = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsmiISO = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsmiZoomToFit = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsmiEdit = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsmiUndo = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsmiRedo = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsmiMainTube = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsmiMainTube_Circle = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsmiMainTube_Rectangle = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsmiMainTube_Oval = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsmiMainTube_FlatOval = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsmiMainTube_DShape = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsmiCADFeature = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsmiEndCutter = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsmiBranchTube = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsmiBranchTube_Circle = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsmiBranchTube_Rectangle = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsmiBranchTube_Oval = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsmiBranchTube_FlatOval = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsmiBranchTube_DShape = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsmiBendingNotch = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsmiBendingNotch_VShape = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsmiBendingNotch_BothSide = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsmiBendingNotch_OneSide = new System.Windows.Forms.ToolStripMenuItem();
			this.m_tsmiAbout = new System.Windows.Forms.ToolStripMenuItem();
			this.m_msMainMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_msMainMenu
			// 
			this.m_msMainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_tsmiFile,
            this.m_tsmiView,
            this.m_tsmiEdit,
            this.m_tsmiMainTube,
            this.m_tsmiCADFeature,
            this.m_tsmiAbout});
			this.m_msMainMenu.Location = new System.Drawing.Point(0, 0);
			this.m_msMainMenu.Name = "m_msMainMenu";
			this.m_msMainMenu.Size = new System.Drawing.Size(801, 24);
			this.m_msMainMenu.TabIndex = 0;
			this.m_msMainMenu.Text = "menuStrip1";
			// 
			// m_tsmiFile
			// 
			this.m_tsmiFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_tsmiExport,
            this.m_tsmiOpen});
			this.m_tsmiFile.Name = "m_tsmiFile";
			this.m_tsmiFile.Size = new System.Drawing.Size(38, 20);
			this.m_tsmiFile.Text = "File";
			// 
			// m_tsmiExport
			// 
			this.m_tsmiExport.Name = "m_tsmiExport";
			this.m_tsmiExport.Size = new System.Drawing.Size(143, 22);
			this.m_tsmiExport.Text = "Export";
			this.m_tsmiExport.Click += new System.EventHandler(this.m_tsmiExport_Click);
			// 
			// m_tsmiOpen
			// 
			this.m_tsmiOpen.Name = "m_tsmiOpen";
			this.m_tsmiOpen.Size = new System.Drawing.Size(143, 22);
			this.m_tsmiOpen.Text = "Open (beta)";
			this.m_tsmiOpen.Click += new System.EventHandler(this.m_tsmiOpen_Click);
			// 
			// m_tsmiView
			// 
			this.m_tsmiView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_tsmiX_Pos,
            this.m_tsmiX_Neg,
            this.m_tsmiY_Pos,
            this.m_tsmiY_Neg,
            this.m_tsmiZ_Pos,
            this.m_tsmiZ_Neg,
            this.m_tsmiDir_Pos,
            this.m_tsmiDir_Neg,
            this.m_tsmiISO,
            this.m_tsmiZoomToFit});
			this.m_tsmiView.Name = "m_tsmiView";
			this.m_tsmiView.Size = new System.Drawing.Size(46, 20);
			this.m_tsmiView.Text = "View";
			// 
			// m_tsmiX_Pos
			// 
			this.m_tsmiX_Pos.Name = "m_tsmiX_Pos";
			this.m_tsmiX_Pos.Size = new System.Drawing.Size(139, 22);
			this.m_tsmiX_Pos.Text = "X+";
			this.m_tsmiX_Pos.Click += new System.EventHandler(this.m_tsmiX_Pos_Click);
			// 
			// m_tsmiX_Neg
			// 
			this.m_tsmiX_Neg.Name = "m_tsmiX_Neg";
			this.m_tsmiX_Neg.Size = new System.Drawing.Size(139, 22);
			this.m_tsmiX_Neg.Text = "X-";
			this.m_tsmiX_Neg.Click += new System.EventHandler(this.m_tsmiX_Neg_Click);
			// 
			// m_tsmiY_Pos
			// 
			this.m_tsmiY_Pos.Name = "m_tsmiY_Pos";
			this.m_tsmiY_Pos.Size = new System.Drawing.Size(139, 22);
			this.m_tsmiY_Pos.Text = "Y+";
			this.m_tsmiY_Pos.Click += new System.EventHandler(this.m_tsmiY_Pos_Click);
			// 
			// m_tsmiY_Neg
			// 
			this.m_tsmiY_Neg.Name = "m_tsmiY_Neg";
			this.m_tsmiY_Neg.Size = new System.Drawing.Size(139, 22);
			this.m_tsmiY_Neg.Text = "Y-";
			this.m_tsmiY_Neg.Click += new System.EventHandler(this.m_tsmiY_Neg_Click);
			// 
			// m_tsmiZ_Pos
			// 
			this.m_tsmiZ_Pos.Name = "m_tsmiZ_Pos";
			this.m_tsmiZ_Pos.Size = new System.Drawing.Size(139, 22);
			this.m_tsmiZ_Pos.Text = "Z+";
			this.m_tsmiZ_Pos.Click += new System.EventHandler(this.m_tsmiZ_Pos_Click);
			// 
			// m_tsmiZ_Neg
			// 
			this.m_tsmiZ_Neg.Name = "m_tsmiZ_Neg";
			this.m_tsmiZ_Neg.Size = new System.Drawing.Size(139, 22);
			this.m_tsmiZ_Neg.Text = "Z-";
			this.m_tsmiZ_Neg.Click += new System.EventHandler(this.m_tsmiZ_Neg_Click);
			// 
			// m_tsmiDir_Pos
			// 
			this.m_tsmiDir_Pos.Name = "m_tsmiDir_Pos";
			this.m_tsmiDir_Pos.Size = new System.Drawing.Size(139, 22);
			this.m_tsmiDir_Pos.Text = "Dir+";
			this.m_tsmiDir_Pos.Click += new System.EventHandler(this.m_tsmiDir_Pos_Click);
			// 
			// m_tsmiDir_Neg
			// 
			this.m_tsmiDir_Neg.Name = "m_tsmiDir_Neg";
			this.m_tsmiDir_Neg.Size = new System.Drawing.Size(139, 22);
			this.m_tsmiDir_Neg.Text = "Dir-";
			this.m_tsmiDir_Neg.Click += new System.EventHandler(this.m_tsmiDir_Neg_Click);
			// 
			// m_tsmiISO
			// 
			this.m_tsmiISO.Name = "m_tsmiISO";
			this.m_tsmiISO.Size = new System.Drawing.Size(139, 22);
			this.m_tsmiISO.Text = "ISO";
			this.m_tsmiISO.Click += new System.EventHandler(this.m_tsmiISO_Click);
			// 
			// m_tsmiZoomToFit
			// 
			this.m_tsmiZoomToFit.Name = "m_tsmiZoomToFit";
			this.m_tsmiZoomToFit.Size = new System.Drawing.Size(139, 22);
			this.m_tsmiZoomToFit.Text = "Zoom to Fit";
			this.m_tsmiZoomToFit.Click += new System.EventHandler(this.m_tsmiZoomToFit_Click);
			// 
			// m_tsmiEdit
			// 
			this.m_tsmiEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_tsmiUndo,
            this.m_tsmiRedo});
			this.m_tsmiEdit.Name = "m_tsmiEdit";
			this.m_tsmiEdit.Size = new System.Drawing.Size(41, 20);
			this.m_tsmiEdit.Text = "Edit";
			// 
			// m_tsmiUndo
			// 
			this.m_tsmiUndo.Name = "m_tsmiUndo";
			this.m_tsmiUndo.Size = new System.Drawing.Size(106, 22);
			this.m_tsmiUndo.Text = "Undo";
			this.m_tsmiUndo.Click += new System.EventHandler(this.m_tsmiUndo_Click);
			// 
			// m_tsmiRedo
			// 
			this.m_tsmiRedo.Name = "m_tsmiRedo";
			this.m_tsmiRedo.Size = new System.Drawing.Size(106, 22);
			this.m_tsmiRedo.Text = "Redo";
			this.m_tsmiRedo.Click += new System.EventHandler(this.m_tsmiRedo_Click);
			// 
			// m_tsmiMainTube
			// 
			this.m_tsmiMainTube.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_tsmiMainTube_Circle,
            this.m_tsmiMainTube_Rectangle,
            this.m_tsmiMainTube_Oval,
            this.m_tsmiMainTube_FlatOval,
            this.m_tsmiMainTube_DShape});
			this.m_tsmiMainTube.Name = "m_tsmiMainTube";
			this.m_tsmiMainTube.Size = new System.Drawing.Size(80, 20);
			this.m_tsmiMainTube.Text = "Main Tube";
			// 
			// m_tsmiMainTube_Circle
			// 
			this.m_tsmiMainTube_Circle.Name = "m_tsmiMainTube_Circle";
			this.m_tsmiMainTube_Circle.Size = new System.Drawing.Size(131, 22);
			this.m_tsmiMainTube_Circle.Text = "Circle";
			this.m_tsmiMainTube_Circle.Click += new System.EventHandler(this.m_tsmiMainTube_Circle_Click);
			// 
			// m_tsmiMainTube_Rectangle
			// 
			this.m_tsmiMainTube_Rectangle.Name = "m_tsmiMainTube_Rectangle";
			this.m_tsmiMainTube_Rectangle.Size = new System.Drawing.Size(131, 22);
			this.m_tsmiMainTube_Rectangle.Text = "Rectangle";
			this.m_tsmiMainTube_Rectangle.Click += new System.EventHandler(this.m_tsmiMainTube_Rectangle_Click);
			// 
			// m_tsmiMainTube_Oval
			// 
			this.m_tsmiMainTube_Oval.Name = "m_tsmiMainTube_Oval";
			this.m_tsmiMainTube_Oval.Size = new System.Drawing.Size(131, 22);
			this.m_tsmiMainTube_Oval.Text = "Oval";
			this.m_tsmiMainTube_Oval.Click += new System.EventHandler(this.m_tsmiMainTube_Oval_Click);
			// 
			// m_tsmiMainTube_FlatOval
			// 
			this.m_tsmiMainTube_FlatOval.Name = "m_tsmiMainTube_FlatOval";
			this.m_tsmiMainTube_FlatOval.Size = new System.Drawing.Size(131, 22);
			this.m_tsmiMainTube_FlatOval.Text = "Fla tOval";
			this.m_tsmiMainTube_FlatOval.Click += new System.EventHandler(this.m_tsmiMainTube_FlatOval_Click);
			// 
			// m_tsmiMainTube_DShape
			// 
			this.m_tsmiMainTube_DShape.Name = "m_tsmiMainTube_DShape";
			this.m_tsmiMainTube_DShape.Size = new System.Drawing.Size(131, 22);
			this.m_tsmiMainTube_DShape.Text = "DShape";
			this.m_tsmiMainTube_DShape.Click += new System.EventHandler(this.m_tsmiMainTube_DShape_Click);
			// 
			// m_tsmiCADFeature
			// 
			this.m_tsmiCADFeature.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_tsmiEndCutter,
            this.m_tsmiBranchTube,
            this.m_tsmiBendingNotch});
			this.m_tsmiCADFeature.Name = "m_tsmiCADFeature";
			this.m_tsmiCADFeature.Size = new System.Drawing.Size(89, 20);
			this.m_tsmiCADFeature.Text = "CAD Feature";
			// 
			// m_tsmiEndCutter
			// 
			this.m_tsmiEndCutter.Name = "m_tsmiEndCutter";
			this.m_tsmiEndCutter.Size = new System.Drawing.Size(159, 22);
			this.m_tsmiEndCutter.Text = "End Cutter";
			this.m_tsmiEndCutter.Click += new System.EventHandler(this.m_tsmiEndCutter_Click);
			// 
			// m_tsmiBranchTube
			// 
			this.m_tsmiBranchTube.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_tsmiBranchTube_Circle,
            this.m_tsmiBranchTube_Rectangle,
            this.m_tsmiBranchTube_Oval,
            this.m_tsmiBranchTube_FlatOval,
            this.m_tsmiBranchTube_DShape});
			this.m_tsmiBranchTube.Name = "m_tsmiBranchTube";
			this.m_tsmiBranchTube.Size = new System.Drawing.Size(159, 22);
			this.m_tsmiBranchTube.Text = "Branch Tube";
			// 
			// m_tsmiBranchTube_Circle
			// 
			this.m_tsmiBranchTube_Circle.Name = "m_tsmiBranchTube_Circle";
			this.m_tsmiBranchTube_Circle.Size = new System.Drawing.Size(131, 22);
			this.m_tsmiBranchTube_Circle.Text = "Circle";
			this.m_tsmiBranchTube_Circle.Click += new System.EventHandler(this.m_tsmiBranchTube_Circle_Click);
			// 
			// m_tsmiBranchTube_Rectangle
			// 
			this.m_tsmiBranchTube_Rectangle.Name = "m_tsmiBranchTube_Rectangle";
			this.m_tsmiBranchTube_Rectangle.Size = new System.Drawing.Size(131, 22);
			this.m_tsmiBranchTube_Rectangle.Text = "Rectangle";
			this.m_tsmiBranchTube_Rectangle.Click += new System.EventHandler(this.m_tsmiBranchTube_Rectangle_Click);
			// 
			// m_tsmiBranchTube_Oval
			// 
			this.m_tsmiBranchTube_Oval.Name = "m_tsmiBranchTube_Oval";
			this.m_tsmiBranchTube_Oval.Size = new System.Drawing.Size(131, 22);
			this.m_tsmiBranchTube_Oval.Text = "Oval";
			this.m_tsmiBranchTube_Oval.Click += new System.EventHandler(this.m_tsmiBranchTube_Oval_Click);
			// 
			// m_tsmiBranchTube_FlatOval
			// 
			this.m_tsmiBranchTube_FlatOval.Name = "m_tsmiBranchTube_FlatOval";
			this.m_tsmiBranchTube_FlatOval.Size = new System.Drawing.Size(131, 22);
			this.m_tsmiBranchTube_FlatOval.Text = "Flat Oval";
			this.m_tsmiBranchTube_FlatOval.Click += new System.EventHandler(this.m_tsmiBranchTube_FlatOval_Click);
			// 
			// m_tsmiBranchTube_DShape
			// 
			this.m_tsmiBranchTube_DShape.Name = "m_tsmiBranchTube_DShape";
			this.m_tsmiBranchTube_DShape.Size = new System.Drawing.Size(131, 22);
			this.m_tsmiBranchTube_DShape.Text = "DShape";
			this.m_tsmiBranchTube_DShape.Click += new System.EventHandler(this.m_tsmiBranchTube_DShape_Click);
			// 
			// m_tsmiBendingNotch
			// 
			this.m_tsmiBendingNotch.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_tsmiBendingNotch_VShape,
            this.m_tsmiBendingNotch_BothSide,
            this.m_tsmiBendingNotch_OneSide});
			this.m_tsmiBendingNotch.Name = "m_tsmiBendingNotch";
			this.m_tsmiBendingNotch.Size = new System.Drawing.Size(159, 22);
			this.m_tsmiBendingNotch.Text = "Bending Notch";
			// 
			// m_tsmiBendingNotch_VShape
			// 
			this.m_tsmiBendingNotch_VShape.Name = "m_tsmiBendingNotch_VShape";
			this.m_tsmiBendingNotch_VShape.Size = new System.Drawing.Size(128, 22);
			this.m_tsmiBendingNotch_VShape.Text = "VShape";
			this.m_tsmiBendingNotch_VShape.Click += new System.EventHandler(this.m_tsmiBendingNotch_VShape_Click);
			// 
			// m_tsmiBendingNotch_BothSide
			// 
			this.m_tsmiBendingNotch_BothSide.Name = "m_tsmiBendingNotch_BothSide";
			this.m_tsmiBendingNotch_BothSide.Size = new System.Drawing.Size(128, 22);
			this.m_tsmiBendingNotch_BothSide.Text = "Both Side";
			this.m_tsmiBendingNotch_BothSide.Click += new System.EventHandler(this.m_tsmiBendingNotch_BothSide_Click);
			// 
			// m_tsmiBendingNotch_OneSide
			// 
			this.m_tsmiBendingNotch_OneSide.Name = "m_tsmiBendingNotch_OneSide";
			this.m_tsmiBendingNotch_OneSide.Size = new System.Drawing.Size(128, 22);
			this.m_tsmiBendingNotch_OneSide.Text = "One Side";
			this.m_tsmiBendingNotch_OneSide.Click += new System.EventHandler(this.m_tsmiBendingNotch_OneSide_Click);
			// 
			// m_tsmiAbout
			// 
			this.m_tsmiAbout.Name = "m_tsmiAbout";
			this.m_tsmiAbout.Size = new System.Drawing.Size(54, 20);
			this.m_tsmiAbout.Text = "About";
			this.m_tsmiAbout.Click += new System.EventHandler(this.m_tsmiAbout_Click);
			// 
			// CADEditMainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(801, 624);
			this.Controls.Add(this.m_msMainMenu);
			this.MainMenuStrip = this.m_msMainMenu;
			this.Name = "CADEditMainForm";
			this.Text = "Form1";
			this.m_msMainMenu.ResumeLayout(false);
			this.m_msMainMenu.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.MenuStrip m_msMainMenu;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiFile;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiExport;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiView;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiX_Pos;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiX_Neg;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiY_Pos;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiY_Neg;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiZ_Pos;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiZ_Neg;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiDir_Pos;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiDir_Neg;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiISO;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiMainTube;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiCADFeature;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiEndCutter;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiBranchTube;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiBranchTube_Circle;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiBranchTube_Rectangle;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiBranchTube_Oval;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiBranchTube_FlatOval;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiBranchTube_DShape;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiBendingNotch;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiBendingNotch_VShape;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiMainTube_Circle;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiMainTube_Rectangle;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiMainTube_Oval;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiMainTube_FlatOval;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiMainTube_DShape;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiBendingNotch_BothSide;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiBendingNotch_OneSide;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiEdit;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiUndo;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiRedo;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiOpen;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiZoomToFit;
		private System.Windows.Forms.ToolStripMenuItem m_tsmiAbout;
	}
}

