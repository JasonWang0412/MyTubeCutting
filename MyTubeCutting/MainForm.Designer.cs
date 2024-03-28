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
			this.m_panMainTubeParam = new System.Windows.Forms.Panel();
			this.m_btnRectangleTest = new System.Windows.Forms.Button();
			this.m_btcCircleTest = new System.Windows.Forms.Button();
			this.m_panBranchTubeParam = new System.Windows.Forms.Panel();
			this.m_panMainTubeParam.SuspendLayout();
			this.SuspendLayout();
			// 
			// m_panViewer
			// 
			this.m_panViewer.Location = new System.Drawing.Point(0, 0);
			this.m_panViewer.Name = "m_panViewer";
			this.m_panViewer.Size = new System.Drawing.Size(500, 600);
			this.m_panViewer.TabIndex = 0;
			this.m_panViewer.Paint += new System.Windows.Forms.PaintEventHandler(this.m_panViewer_Paint);
			this.m_panViewer.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.m_panViewer_MouseDoubleClick);
			this.m_panViewer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.m_panViewer_MouseDown);
			this.m_panViewer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.m_panViewer_MouseMove);
			this.m_panViewer.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.m_panViewer_MouseWheel);
			// 
			// m_panMainTubeParam
			// 
			this.m_panMainTubeParam.Controls.Add(this.m_btnRectangleTest);
			this.m_panMainTubeParam.Controls.Add(this.m_btcCircleTest);
			this.m_panMainTubeParam.Location = new System.Drawing.Point(500, 0);
			this.m_panMainTubeParam.Name = "m_panMainTubeParam";
			this.m_panMainTubeParam.Size = new System.Drawing.Size(300, 300);
			this.m_panMainTubeParam.TabIndex = 1;
			// 
			// m_btnRectangleTest
			// 
			this.m_btnRectangleTest.Location = new System.Drawing.Point(7, 43);
			this.m_btnRectangleTest.Name = "m_btnRectangleTest";
			this.m_btnRectangleTest.Size = new System.Drawing.Size(75, 23);
			this.m_btnRectangleTest.TabIndex = 1;
			this.m_btnRectangleTest.Text = "Rectangle";
			this.m_btnRectangleTest.UseVisualStyleBackColor = true;
			// 
			// m_btcCircleTest
			// 
			this.m_btcCircleTest.Location = new System.Drawing.Point(7, 13);
			this.m_btcCircleTest.Name = "m_btcCircleTest";
			this.m_btcCircleTest.Size = new System.Drawing.Size(75, 23);
			this.m_btcCircleTest.TabIndex = 0;
			this.m_btcCircleTest.Text = "Circle";
			this.m_btcCircleTest.UseVisualStyleBackColor = true;
			this.m_btcCircleTest.Click += new System.EventHandler(this.m_btcCircleTest_Click);
			// 
			// m_panBranchTubeParam
			// 
			this.m_panBranchTubeParam.Location = new System.Drawing.Point(500, 300);
			this.m_panBranchTubeParam.Name = "m_panBranchTubeParam";
			this.m_panBranchTubeParam.Size = new System.Drawing.Size(300, 300);
			this.m_panBranchTubeParam.TabIndex = 2;
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
			this.m_panMainTubeParam.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel m_panViewer;
		private System.Windows.Forms.Panel m_panMainTubeParam;
		private System.Windows.Forms.Panel m_panBranchTubeParam;
		private System.Windows.Forms.Button m_btnRectangleTest;
		private System.Windows.Forms.Button m_btcCircleTest;
	}
}

