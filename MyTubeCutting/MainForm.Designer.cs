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
			this.m_PanelViewer = new System.Windows.Forms.Panel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// m_PanelViewer
			// 
			this.m_PanelViewer.Location = new System.Drawing.Point(0, 0);
			this.m_PanelViewer.Name = "m_PanelViewer";
			this.m_PanelViewer.Size = new System.Drawing.Size(600, 600);
			this.m_PanelViewer.TabIndex = 0;
			this.m_PanelViewer.Paint += new System.Windows.Forms.PaintEventHandler(this.m_PanelViewer_Paint);
			this.m_PanelViewer.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.m_PanelViewer_MouseDoubleClick);
			this.m_PanelViewer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.m_PanelViewer_MouseDown);
			this.m_PanelViewer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.m_PanelViewer_MouseMove);
			this.m_PanelViewer.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.m_PanelViewer_MouseWheel);
			// 
			// panel1
			// 
			this.panel1.Location = new System.Drawing.Point(600, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(200, 300);
			this.panel1.TabIndex = 1;
			// 
			// panel2
			// 
			this.panel2.Location = new System.Drawing.Point(600, 300);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(200, 300);
			this.panel2.TabIndex = 2;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(801, 601);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.m_PanelViewer);
			this.Name = "MainForm";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel m_PanelViewer;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
	}
}

