namespace MyTubeCutting
{
	partial class MainTubeShapeForm
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
			this.m_btnCircle = new System.Windows.Forms.Button();
			this.m_btnRectangle = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.button4 = new System.Windows.Forms.Button();
			this.button5 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// m_btnCircle
			// 
			this.m_btnCircle.Location = new System.Drawing.Point(12, 12);
			this.m_btnCircle.Name = "m_btnCircle";
			this.m_btnCircle.Size = new System.Drawing.Size(75, 23);
			this.m_btnCircle.TabIndex = 0;
			this.m_btnCircle.Text = "Circle";
			this.m_btnCircle.UseVisualStyleBackColor = true;
			// 
			// m_btnRectangle
			// 
			this.m_btnRectangle.Location = new System.Drawing.Point(93, 12);
			this.m_btnRectangle.Name = "m_btnRectangle";
			this.m_btnRectangle.Size = new System.Drawing.Size(75, 23);
			this.m_btnRectangle.TabIndex = 1;
			this.m_btnRectangle.Text = "Rectangle";
			this.m_btnRectangle.UseVisualStyleBackColor = true;
			// 
			// button3
			// 
			this.button3.Enabled = false;
			this.button3.Location = new System.Drawing.Point(174, 12);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(75, 23);
			this.button3.TabIndex = 2;
			this.button3.Text = "button3";
			this.button3.UseVisualStyleBackColor = true;
			// 
			// button4
			// 
			this.button4.Enabled = false;
			this.button4.Location = new System.Drawing.Point(255, 12);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(75, 23);
			this.button4.TabIndex = 3;
			this.button4.Text = "button4";
			this.button4.UseVisualStyleBackColor = true;
			// 
			// button5
			// 
			this.button5.Enabled = false;
			this.button5.Location = new System.Drawing.Point(336, 12);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(75, 23);
			this.button5.TabIndex = 4;
			this.button5.Text = "button5";
			this.button5.UseVisualStyleBackColor = true;
			// 
			// MainTubeShapeForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(427, 57);
			this.Controls.Add(this.button5);
			this.Controls.Add(this.button4);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.m_btnRectangle);
			this.Controls.Add(this.m_btnCircle);
			this.Name = "MainTubeShapeForm";
			this.Text = "MainTubeShapeForm";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button m_btnCircle;
		private System.Windows.Forms.Button m_btnRectangle;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.Button button5;
	}
}