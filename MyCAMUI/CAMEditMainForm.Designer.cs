namespace MyCAMUI
{
	partial class CAMEditMainForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CAMEditMainForm));
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
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
			this.m_tsView.SuspendLayout();
			this.m_tsEdit.SuspendLayout();
			this.SuspendLayout();
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
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
			this.m_tsView.Location = new System.Drawing.Point(0, 0);
			this.m_tsView.Name = "m_tsView";
			this.m_tsView.Size = new System.Drawing.Size(1184, 25);
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
			this.m_tsEdit.Location = new System.Drawing.Point(0, 25);
			this.m_tsEdit.Name = "m_tsEdit";
			this.m_tsEdit.Size = new System.Drawing.Size(1184, 25);
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
			// CAMEditMainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(1184, 861);
			this.Controls.Add(this.m_tsEdit);
			this.Controls.Add(this.m_tsView);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "CAMEditMainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.m_tsView.ResumeLayout(false);
			this.m_tsView.PerformLayout();
			this.m_tsEdit.ResumeLayout(false);
			this.m_tsEdit.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStrip m_tsView;
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

