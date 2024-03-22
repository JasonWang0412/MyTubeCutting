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
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.button4 = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
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
			this.panel1.Controls.Add(this.button4);
			this.panel1.Controls.Add(this.button3);
			this.panel1.Controls.Add(this.button2);
			this.panel1.Controls.Add(this.button1);
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
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(7, 13);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 0;
			this.button1.Text = "button1";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(7, 43);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 23);
			this.button2.TabIndex = 1;
			this.button2.Text = "button2";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(7, 73);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(75, 23);
			this.button3.TabIndex = 2;
			this.button3.Text = "button3";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// button4
			// 
			this.button4.Location = new System.Drawing.Point(7, 103);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(75, 23);
			this.button4.TabIndex = 3;
			this.button4.Text = "button4";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler(this.button4_Click);
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
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel m_PanelViewer;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button1;
	}
}

