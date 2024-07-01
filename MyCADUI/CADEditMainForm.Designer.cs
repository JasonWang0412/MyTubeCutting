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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CADEditMainForm));
			this.m_tsMainTube = new System.Windows.Forms.ToolStrip();
			this.m_tsbtnMainTube_Circle = new System.Windows.Forms.ToolStripButton();
			this.m_tsbtnMainTube_Rectangle = new System.Windows.Forms.ToolStripButton();
			this.m_tsbtnMainTube_Oval = new System.Windows.Forms.ToolStripButton();
			this.m_tsbtnMainTube_FlatOval = new System.Windows.Forms.ToolStripButton();
			this.m_tsbtnMainTube_DShape = new System.Windows.Forms.ToolStripButton();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.m_tsCADFeature = new System.Windows.Forms.ToolStrip();
			this.m_tsbtnEndCutter = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.m_tsbtnBranchTube_Circle = new System.Windows.Forms.ToolStripButton();
			this.m_tsbtnBranchTube_Rectangle = new System.Windows.Forms.ToolStripButton();
			this.m_tsbtnBranchTube_Oval = new System.Windows.Forms.ToolStripButton();
			this.m_tsbtnBranchTube_FlatOval = new System.Windows.Forms.ToolStripButton();
			this.m_tsbtnBranchTube_DShape = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.m_tsbtnBendingNotch_VShape = new System.Windows.Forms.ToolStripButton();
			this.m_tsbtnBendingNotch_BothSide = new System.Windows.Forms.ToolStripButton();
			this.m_tsbtnBendingNotch_OneSide = new System.Windows.Forms.ToolStripButton();
			this.m_tsView = new System.Windows.Forms.ToolStrip();
			this.m_tsbtnX_Pos = new System.Windows.Forms.ToolStripButton();
			this.m_tsbtnX_Neg = new System.Windows.Forms.ToolStripButton();
			this.m_tsbtnY_Pos = new System.Windows.Forms.ToolStripButton();
			this.m_tsbtnY_Neg = new System.Windows.Forms.ToolStripButton();
			this.m_tsbtnZ_Pos = new System.Windows.Forms.ToolStripButton();
			this.m_tsbtnZ_Neg = new System.Windows.Forms.ToolStripButton();
			this.m_tsbtnDir_Pos = new System.Windows.Forms.ToolStripButton();
			this.m_tsbtnDir_Neg = new System.Windows.Forms.ToolStripButton();
			this.m_tsbtnISO = new System.Windows.Forms.ToolStripButton();
			this.m_tsbtnZoomToFit = new System.Windows.Forms.ToolStripButton();
			this.m_tsEdit = new System.Windows.Forms.ToolStrip();
			this.m_tsbtnUndo = new System.Windows.Forms.ToolStripButton();
			this.m_tsbtnRedo = new System.Windows.Forms.ToolStripButton();
			this.m_tsbtnExport = new System.Windows.Forms.ToolStripButton();
			this.m_tsMainTube.SuspendLayout();
			this.m_tsCADFeature.SuspendLayout();
			this.m_tsView.SuspendLayout();
			this.m_tsEdit.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_tsMainTube
			// 
			this.m_tsMainTube.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_tsbtnMainTube_Circle,
            this.m_tsbtnMainTube_Rectangle,
            this.m_tsbtnMainTube_Oval,
            this.m_tsbtnMainTube_FlatOval,
            this.m_tsbtnMainTube_DShape});
			this.m_tsMainTube.Location = new System.Drawing.Point(0, 0);
			this.m_tsMainTube.Name = "m_tsMainTube";
			this.m_tsMainTube.Size = new System.Drawing.Size(801, 25);
			this.m_tsMainTube.TabIndex = 1;
			this.m_tsMainTube.Text = "toolStrip1";
			// 
			// m_tsbtnMainTube_Circle
			// 
			this.m_tsbtnMainTube_Circle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.m_tsbtnMainTube_Circle.Image = ((System.Drawing.Image)(resources.GetObject("m_tsbtnMainTube_Circle.Image")));
			this.m_tsbtnMainTube_Circle.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tsbtnMainTube_Circle.Name = "m_tsbtnMainTube_Circle";
			this.m_tsbtnMainTube_Circle.Size = new System.Drawing.Size(42, 22);
			this.m_tsbtnMainTube_Circle.Text = "Circle";
			this.m_tsbtnMainTube_Circle.Click += new System.EventHandler(this.m_tsbtnMainTube_Circle_Click);
			// 
			// m_tsbtnMainTube_Rectangle
			// 
			this.m_tsbtnMainTube_Rectangle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.m_tsbtnMainTube_Rectangle.Image = ((System.Drawing.Image)(resources.GetObject("m_tsbtnMainTube_Rectangle.Image")));
			this.m_tsbtnMainTube_Rectangle.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tsbtnMainTube_Rectangle.Name = "m_tsbtnMainTube_Rectangle";
			this.m_tsbtnMainTube_Rectangle.Size = new System.Drawing.Size(68, 22);
			this.m_tsbtnMainTube_Rectangle.Text = "Rectangle";
			this.m_tsbtnMainTube_Rectangle.Click += new System.EventHandler(this.m_tsbtnMainTube_Rectangle_Click);
			// 
			// m_tsbtnMainTube_Oval
			// 
			this.m_tsbtnMainTube_Oval.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.m_tsbtnMainTube_Oval.Image = ((System.Drawing.Image)(resources.GetObject("m_tsbtnMainTube_Oval.Image")));
			this.m_tsbtnMainTube_Oval.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tsbtnMainTube_Oval.Name = "m_tsbtnMainTube_Oval";
			this.m_tsbtnMainTube_Oval.Size = new System.Drawing.Size(37, 22);
			this.m_tsbtnMainTube_Oval.Text = "Oval";
			this.m_tsbtnMainTube_Oval.Click += new System.EventHandler(this.m_tsbtnMainTube_Oval_Click);
			// 
			// m_tsbtnMainTube_FlatOval
			// 
			this.m_tsbtnMainTube_FlatOval.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.m_tsbtnMainTube_FlatOval.Image = ((System.Drawing.Image)(resources.GetObject("m_tsbtnMainTube_FlatOval.Image")));
			this.m_tsbtnMainTube_FlatOval.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tsbtnMainTube_FlatOval.Name = "m_tsbtnMainTube_FlatOval";
			this.m_tsbtnMainTube_FlatOval.Size = new System.Drawing.Size(60, 22);
			this.m_tsbtnMainTube_FlatOval.Text = "Flat Oval";
			this.m_tsbtnMainTube_FlatOval.ToolTipText = "Flat Oval";
			this.m_tsbtnMainTube_FlatOval.Click += new System.EventHandler(this.m_tsbtnMainTube_FlatOval_Click);
			// 
			// m_tsbtnMainTube_DShape
			// 
			this.m_tsbtnMainTube_DShape.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.m_tsbtnMainTube_DShape.Image = ((System.Drawing.Image)(resources.GetObject("m_tsbtnMainTube_DShape.Image")));
			this.m_tsbtnMainTube_DShape.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tsbtnMainTube_DShape.Name = "m_tsbtnMainTube_DShape";
			this.m_tsbtnMainTube_DShape.Size = new System.Drawing.Size(56, 22);
			this.m_tsbtnMainTube_DShape.Text = "DShape";
			this.m_tsbtnMainTube_DShape.Click += new System.EventHandler(this.m_tsbtnMainTube_DShape_Click);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
			// 
			// m_tsCADFeature
			// 
			this.m_tsCADFeature.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_tsbtnEndCutter,
            this.toolStripSeparator1,
            this.m_tsbtnBranchTube_Circle,
            this.m_tsbtnBranchTube_Rectangle,
            this.m_tsbtnBranchTube_Oval,
            this.m_tsbtnBranchTube_FlatOval,
            this.m_tsbtnBranchTube_DShape,
            this.toolStripSeparator2,
            this.m_tsbtnBendingNotch_VShape,
            this.m_tsbtnBendingNotch_BothSide,
            this.m_tsbtnBendingNotch_OneSide});
			this.m_tsCADFeature.Location = new System.Drawing.Point(0, 25);
			this.m_tsCADFeature.Name = "m_tsCADFeature";
			this.m_tsCADFeature.Size = new System.Drawing.Size(801, 25);
			this.m_tsCADFeature.TabIndex = 3;
			this.m_tsCADFeature.Text = "toolStrip1";
			// 
			// m_tsbtnEndCutter
			// 
			this.m_tsbtnEndCutter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.m_tsbtnEndCutter.Image = ((System.Drawing.Image)(resources.GetObject("m_tsbtnEndCutter.Image")));
			this.m_tsbtnEndCutter.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tsbtnEndCutter.Name = "m_tsbtnEndCutter";
			this.m_tsbtnEndCutter.Size = new System.Drawing.Size(67, 22);
			this.m_tsbtnEndCutter.Text = "EndCutter";
			this.m_tsbtnEndCutter.Click += new System.EventHandler(this.m_tsbtnEndCutter_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// m_tsbtnBranchTube_Circle
			// 
			this.m_tsbtnBranchTube_Circle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.m_tsbtnBranchTube_Circle.Image = ((System.Drawing.Image)(resources.GetObject("m_tsbtnBranchTube_Circle.Image")));
			this.m_tsbtnBranchTube_Circle.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tsbtnBranchTube_Circle.Name = "m_tsbtnBranchTube_Circle";
			this.m_tsbtnBranchTube_Circle.Size = new System.Drawing.Size(42, 22);
			this.m_tsbtnBranchTube_Circle.Text = "Circle";
			this.m_tsbtnBranchTube_Circle.Click += new System.EventHandler(this.m_tsbtnBranchTube_Circle_Click);
			// 
			// m_tsbtnBranchTube_Rectangle
			// 
			this.m_tsbtnBranchTube_Rectangle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.m_tsbtnBranchTube_Rectangle.Image = ((System.Drawing.Image)(resources.GetObject("m_tsbtnBranchTube_Rectangle.Image")));
			this.m_tsbtnBranchTube_Rectangle.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tsbtnBranchTube_Rectangle.Name = "m_tsbtnBranchTube_Rectangle";
			this.m_tsbtnBranchTube_Rectangle.Size = new System.Drawing.Size(68, 22);
			this.m_tsbtnBranchTube_Rectangle.Text = "Rectangle";
			this.m_tsbtnBranchTube_Rectangle.Click += new System.EventHandler(this.m_tsbtnBranchTube_Rectangle_Click);
			// 
			// m_tsbtnBranchTube_Oval
			// 
			this.m_tsbtnBranchTube_Oval.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.m_tsbtnBranchTube_Oval.Image = ((System.Drawing.Image)(resources.GetObject("m_tsbtnBranchTube_Oval.Image")));
			this.m_tsbtnBranchTube_Oval.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tsbtnBranchTube_Oval.Name = "m_tsbtnBranchTube_Oval";
			this.m_tsbtnBranchTube_Oval.Size = new System.Drawing.Size(37, 22);
			this.m_tsbtnBranchTube_Oval.Text = "Oval";
			this.m_tsbtnBranchTube_Oval.Click += new System.EventHandler(this.m_tsbtnBranchTube_Oval_Click);
			// 
			// m_tsbtnBranchTube_FlatOval
			// 
			this.m_tsbtnBranchTube_FlatOval.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.m_tsbtnBranchTube_FlatOval.Image = ((System.Drawing.Image)(resources.GetObject("m_tsbtnBranchTube_FlatOval.Image")));
			this.m_tsbtnBranchTube_FlatOval.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tsbtnBranchTube_FlatOval.Name = "m_tsbtnBranchTube_FlatOval";
			this.m_tsbtnBranchTube_FlatOval.Size = new System.Drawing.Size(60, 22);
			this.m_tsbtnBranchTube_FlatOval.Text = "Flat Oval";
			this.m_tsbtnBranchTube_FlatOval.Click += new System.EventHandler(this.m_tsbtnBranchTube_FlatOval_Click);
			// 
			// m_tsbtnBranchTube_DShape
			// 
			this.m_tsbtnBranchTube_DShape.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.m_tsbtnBranchTube_DShape.Image = ((System.Drawing.Image)(resources.GetObject("m_tsbtnBranchTube_DShape.Image")));
			this.m_tsbtnBranchTube_DShape.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tsbtnBranchTube_DShape.Name = "m_tsbtnBranchTube_DShape";
			this.m_tsbtnBranchTube_DShape.Size = new System.Drawing.Size(56, 22);
			this.m_tsbtnBranchTube_DShape.Text = "DShape";
			this.m_tsbtnBranchTube_DShape.Click += new System.EventHandler(this.m_tsbtnBranchTube_DShape_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			// 
			// m_tsbtnBendingNotch_VShape
			// 
			this.m_tsbtnBendingNotch_VShape.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.m_tsbtnBendingNotch_VShape.Image = ((System.Drawing.Image)(resources.GetObject("m_tsbtnBendingNotch_VShape.Image")));
			this.m_tsbtnBendingNotch_VShape.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tsbtnBendingNotch_VShape.Name = "m_tsbtnBendingNotch_VShape";
			this.m_tsbtnBendingNotch_VShape.Size = new System.Drawing.Size(55, 22);
			this.m_tsbtnBendingNotch_VShape.Text = "VShape";
			this.m_tsbtnBendingNotch_VShape.Click += new System.EventHandler(this.m_tsbtnBendingNotch_VShape_Click);
			// 
			// m_tsbtnBendingNotch_BothSide
			// 
			this.m_tsbtnBendingNotch_BothSide.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.m_tsbtnBendingNotch_BothSide.Image = ((System.Drawing.Image)(resources.GetObject("m_tsbtnBendingNotch_BothSide.Image")));
			this.m_tsbtnBendingNotch_BothSide.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tsbtnBendingNotch_BothSide.Name = "m_tsbtnBendingNotch_BothSide";
			this.m_tsbtnBendingNotch_BothSide.Size = new System.Drawing.Size(65, 22);
			this.m_tsbtnBendingNotch_BothSide.Text = "Both Side";
			this.m_tsbtnBendingNotch_BothSide.Click += new System.EventHandler(this.m_tsbtnBendingNotch_BothSide_Click);
			// 
			// m_tsbtnBendingNotch_OneSide
			// 
			this.m_tsbtnBendingNotch_OneSide.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.m_tsbtnBendingNotch_OneSide.Image = ((System.Drawing.Image)(resources.GetObject("m_tsbtnBendingNotch_OneSide.Image")));
			this.m_tsbtnBendingNotch_OneSide.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tsbtnBendingNotch_OneSide.Name = "m_tsbtnBendingNotch_OneSide";
			this.m_tsbtnBendingNotch_OneSide.Size = new System.Drawing.Size(63, 22);
			this.m_tsbtnBendingNotch_OneSide.Text = "One Side";
			this.m_tsbtnBendingNotch_OneSide.Click += new System.EventHandler(this.m_tsbtnBendingNotch_OneSide_Click);
			// 
			// m_tsView
			// 
			this.m_tsView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_tsbtnX_Pos,
            this.m_tsbtnX_Neg,
            this.m_tsbtnY_Pos,
            this.m_tsbtnY_Neg,
            this.m_tsbtnZ_Pos,
            this.m_tsbtnZ_Neg,
            this.m_tsbtnDir_Pos,
            this.m_tsbtnDir_Neg,
            this.m_tsbtnISO,
            this.m_tsbtnZoomToFit});
			this.m_tsView.Location = new System.Drawing.Point(0, 50);
			this.m_tsView.Name = "m_tsView";
			this.m_tsView.Size = new System.Drawing.Size(801, 25);
			this.m_tsView.TabIndex = 4;
			this.m_tsView.Text = "toolStrip2";
			// 
			// m_tsbtnX_Pos
			// 
			this.m_tsbtnX_Pos.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.m_tsbtnX_Pos.Image = ((System.Drawing.Image)(resources.GetObject("m_tsbtnX_Pos.Image")));
			this.m_tsbtnX_Pos.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tsbtnX_Pos.Name = "m_tsbtnX_Pos";
			this.m_tsbtnX_Pos.Size = new System.Drawing.Size(28, 22);
			this.m_tsbtnX_Pos.Text = "X+";
			this.m_tsbtnX_Pos.Click += new System.EventHandler(this.m_tsbtnX_Pos_Click);
			// 
			// m_tsbtnX_Neg
			// 
			this.m_tsbtnX_Neg.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.m_tsbtnX_Neg.Image = ((System.Drawing.Image)(resources.GetObject("m_tsbtnX_Neg.Image")));
			this.m_tsbtnX_Neg.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tsbtnX_Neg.Name = "m_tsbtnX_Neg";
			this.m_tsbtnX_Neg.Size = new System.Drawing.Size(24, 22);
			this.m_tsbtnX_Neg.Text = "X-";
			this.m_tsbtnX_Neg.Click += new System.EventHandler(this.m_tsbtnX_Neg_Click);
			// 
			// m_tsbtnY_Pos
			// 
			this.m_tsbtnY_Pos.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.m_tsbtnY_Pos.Image = ((System.Drawing.Image)(resources.GetObject("m_tsbtnY_Pos.Image")));
			this.m_tsbtnY_Pos.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tsbtnY_Pos.Name = "m_tsbtnY_Pos";
			this.m_tsbtnY_Pos.Size = new System.Drawing.Size(27, 22);
			this.m_tsbtnY_Pos.Text = "Y+";
			this.m_tsbtnY_Pos.Click += new System.EventHandler(this.m_tsbtnY_Pos_Click);
			// 
			// m_tsbtnY_Neg
			// 
			this.m_tsbtnY_Neg.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.m_tsbtnY_Neg.Image = ((System.Drawing.Image)(resources.GetObject("m_tsbtnY_Neg.Image")));
			this.m_tsbtnY_Neg.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tsbtnY_Neg.Name = "m_tsbtnY_Neg";
			this.m_tsbtnY_Neg.Size = new System.Drawing.Size(23, 22);
			this.m_tsbtnY_Neg.Text = "Y-";
			this.m_tsbtnY_Neg.Click += new System.EventHandler(this.m_tsbtnY_Neg_Click);
			// 
			// m_tsbtnZ_Pos
			// 
			this.m_tsbtnZ_Pos.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.m_tsbtnZ_Pos.Image = ((System.Drawing.Image)(resources.GetObject("m_tsbtnZ_Pos.Image")));
			this.m_tsbtnZ_Pos.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tsbtnZ_Pos.Name = "m_tsbtnZ_Pos";
			this.m_tsbtnZ_Pos.Size = new System.Drawing.Size(27, 22);
			this.m_tsbtnZ_Pos.Text = "Z+";
			this.m_tsbtnZ_Pos.Click += new System.EventHandler(this.m_tsbtnZ_Pos_Click);
			// 
			// m_tsbtnZ_Neg
			// 
			this.m_tsbtnZ_Neg.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.m_tsbtnZ_Neg.Image = ((System.Drawing.Image)(resources.GetObject("m_tsbtnZ_Neg.Image")));
			this.m_tsbtnZ_Neg.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tsbtnZ_Neg.Name = "m_tsbtnZ_Neg";
			this.m_tsbtnZ_Neg.Size = new System.Drawing.Size(23, 22);
			this.m_tsbtnZ_Neg.Text = "Z-";
			this.m_tsbtnZ_Neg.Click += new System.EventHandler(this.m_tsbtnZ_Neg_Click);
			// 
			// m_tsbtnDir_Pos
			// 
			this.m_tsbtnDir_Pos.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.m_tsbtnDir_Pos.Image = ((System.Drawing.Image)(resources.GetObject("m_tsbtnDir_Pos.Image")));
			this.m_tsbtnDir_Pos.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tsbtnDir_Pos.Name = "m_tsbtnDir_Pos";
			this.m_tsbtnDir_Pos.Size = new System.Drawing.Size(36, 22);
			this.m_tsbtnDir_Pos.Text = "Dir+";
			this.m_tsbtnDir_Pos.Click += new System.EventHandler(this.m_tsbtnDir_Pos_Click);
			// 
			// m_tsbtnDir_Neg
			// 
			this.m_tsbtnDir_Neg.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.m_tsbtnDir_Neg.Image = ((System.Drawing.Image)(resources.GetObject("m_tsbtnDir_Neg.Image")));
			this.m_tsbtnDir_Neg.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tsbtnDir_Neg.Name = "m_tsbtnDir_Neg";
			this.m_tsbtnDir_Neg.Size = new System.Drawing.Size(32, 22);
			this.m_tsbtnDir_Neg.Text = "Dir-";
			this.m_tsbtnDir_Neg.Click += new System.EventHandler(this.m_tsbtnDir_Neg_Click);
			// 
			// m_tsbtnISO
			// 
			this.m_tsbtnISO.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.m_tsbtnISO.Image = ((System.Drawing.Image)(resources.GetObject("m_tsbtnISO.Image")));
			this.m_tsbtnISO.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tsbtnISO.Name = "m_tsbtnISO";
			this.m_tsbtnISO.Size = new System.Drawing.Size(31, 22);
			this.m_tsbtnISO.Text = "ISO";
			this.m_tsbtnISO.Click += new System.EventHandler(this.m_tsbtnISO_Click);
			// 
			// m_tsbtnZoomToFit
			// 
			this.m_tsbtnZoomToFit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.m_tsbtnZoomToFit.Image = ((System.Drawing.Image)(resources.GetObject("m_tsbtnZoomToFit.Image")));
			this.m_tsbtnZoomToFit.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tsbtnZoomToFit.Name = "m_tsbtnZoomToFit";
			this.m_tsbtnZoomToFit.Size = new System.Drawing.Size(79, 22);
			this.m_tsbtnZoomToFit.Text = "Zoom To Fit";
			this.m_tsbtnZoomToFit.Click += new System.EventHandler(this.m_tsbtnZoomToFit_Click);
			// 
			// m_tsEdit
			// 
			this.m_tsEdit.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_tsbtnUndo,
            this.m_tsbtnRedo,
            this.m_tsbtnExport});
			this.m_tsEdit.Location = new System.Drawing.Point(0, 75);
			this.m_tsEdit.Name = "m_tsEdit";
			this.m_tsEdit.Size = new System.Drawing.Size(801, 25);
			this.m_tsEdit.TabIndex = 5;
			this.m_tsEdit.Text = "toolStrip3";
			// 
			// m_tsbtnUndo
			// 
			this.m_tsbtnUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.m_tsbtnUndo.Image = ((System.Drawing.Image)(resources.GetObject("m_tsbtnUndo.Image")));
			this.m_tsbtnUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tsbtnUndo.Name = "m_tsbtnUndo";
			this.m_tsbtnUndo.Size = new System.Drawing.Size(43, 22);
			this.m_tsbtnUndo.Text = "Undo";
			this.m_tsbtnUndo.Click += new System.EventHandler(this.m_tsbtnUndo_Click);
			// 
			// m_tsbtnRedo
			// 
			this.m_tsbtnRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.m_tsbtnRedo.Image = ((System.Drawing.Image)(resources.GetObject("m_tsbtnRedo.Image")));
			this.m_tsbtnRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tsbtnRedo.Name = "m_tsbtnRedo";
			this.m_tsbtnRedo.Size = new System.Drawing.Size(42, 22);
			this.m_tsbtnRedo.Text = "Redo";
			this.m_tsbtnRedo.Click += new System.EventHandler(this.m_tsbtnRedo_Click);
			// 
			// m_tsbtnExport
			// 
			this.m_tsbtnExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.m_tsbtnExport.Image = ((System.Drawing.Image)(resources.GetObject("m_tsbtnExport.Image")));
			this.m_tsbtnExport.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.m_tsbtnExport.Name = "m_tsbtnExport";
			this.m_tsbtnExport.Size = new System.Drawing.Size(48, 22);
			this.m_tsbtnExport.Text = "Export";
			this.m_tsbtnExport.Click += new System.EventHandler(this.m_tsbtnExport_Click);
			// 
			// CADEditMainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(801, 624);
			this.Controls.Add(this.m_tsEdit);
			this.Controls.Add(this.m_tsView);
			this.Controls.Add(this.m_tsCADFeature);
			this.Controls.Add(this.m_tsMainTube);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "CADEditMainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.m_tsMainTube.ResumeLayout(false);
			this.m_tsMainTube.PerformLayout();
			this.m_tsCADFeature.ResumeLayout(false);
			this.m_tsCADFeature.PerformLayout();
			this.m_tsView.ResumeLayout(false);
			this.m_tsView.PerformLayout();
			this.m_tsEdit.ResumeLayout(false);
			this.m_tsEdit.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.ToolStrip m_tsMainTube;
		private System.Windows.Forms.ToolStripButton m_tsbtnMainTube_Circle;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStrip m_tsCADFeature;
		private System.Windows.Forms.ToolStrip m_tsView;
		private System.Windows.Forms.ToolStripButton m_tsbtnMainTube_Rectangle;
		private System.Windows.Forms.ToolStripButton m_tsbtnMainTube_Oval;
		private System.Windows.Forms.ToolStripButton m_tsbtnMainTube_FlatOval;
		private System.Windows.Forms.ToolStripButton m_tsbtnMainTube_DShape;
		private System.Windows.Forms.ToolStripButton m_tsbtnBranchTube_Circle;
		private System.Windows.Forms.ToolStripButton m_tsbtnBranchTube_Rectangle;
		private System.Windows.Forms.ToolStripButton m_tsbtnBranchTube_Oval;
		private System.Windows.Forms.ToolStripButton m_tsbtnBranchTube_FlatOval;
		private System.Windows.Forms.ToolStripButton m_tsbtnBranchTube_DShape;
		private System.Windows.Forms.ToolStripButton m_tsbtnEndCutter;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripButton m_tsbtnBendingNotch_VShape;
		private System.Windows.Forms.ToolStripButton m_tsbtnBendingNotch_BothSide;
		private System.Windows.Forms.ToolStripButton m_tsbtnBendingNotch_OneSide;
		private System.Windows.Forms.ToolStripButton m_tsbtnX_Pos;
		private System.Windows.Forms.ToolStripButton m_tsbtnX_Neg;
		private System.Windows.Forms.ToolStripButton m_tsbtnY_Pos;
		private System.Windows.Forms.ToolStripButton m_tsbtnY_Neg;
		private System.Windows.Forms.ToolStripButton m_tsbtnZ_Pos;
		private System.Windows.Forms.ToolStripButton m_tsbtnZ_Neg;
		private System.Windows.Forms.ToolStripButton m_tsbtnDir_Pos;
		private System.Windows.Forms.ToolStripButton m_tsbtnDir_Neg;
		private System.Windows.Forms.ToolStripButton m_tsbtnISO;
		private System.Windows.Forms.ToolStripButton m_tsbtnZoomToFit;
		private System.Windows.Forms.ToolStrip m_tsEdit;
		private System.Windows.Forms.ToolStripButton m_tsbtnUndo;
		private System.Windows.Forms.ToolStripButton m_tsbtnRedo;
		private System.Windows.Forms.ToolStripButton m_tsbtnExport;
	}
}

