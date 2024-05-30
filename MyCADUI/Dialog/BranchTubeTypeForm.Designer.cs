namespace MyCADUI
{
	partial class BranchTubeTypeForm
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
			this.m_btnDShape = new System.Windows.Forms.Button();
			this.m_btnFlatOval = new System.Windows.Forms.Button();
			this.m_btnOval = new System.Windows.Forms.Button();
			this.m_btnRectangle = new System.Windows.Forms.Button();
			this.m_btnCircle = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// m_btnDShape
			// 
			this.m_btnDShape.Location = new System.Drawing.Point(342, 19);
			this.m_btnDShape.Name = "m_btnDShape";
			this.m_btnDShape.Size = new System.Drawing.Size(75, 23);
			this.m_btnDShape.TabIndex = 9;
			this.m_btnDShape.Text = "DShape";
			this.m_btnDShape.UseVisualStyleBackColor = true;
			this.m_btnDShape.Click += new System.EventHandler(this.m_btnDShape_Click);
			// 
			// m_btnFlatOval
			// 
			this.m_btnFlatOval.Location = new System.Drawing.Point(261, 19);
			this.m_btnFlatOval.Name = "m_btnFlatOval";
			this.m_btnFlatOval.Size = new System.Drawing.Size(75, 23);
			this.m_btnFlatOval.TabIndex = 8;
			this.m_btnFlatOval.Text = "FlatOval";
			this.m_btnFlatOval.UseVisualStyleBackColor = true;
			this.m_btnFlatOval.Click += new System.EventHandler(this.m_btnFlatOval_Click);
			// 
			// m_btnOval
			// 
			this.m_btnOval.Location = new System.Drawing.Point(180, 19);
			this.m_btnOval.Name = "m_btnOval";
			this.m_btnOval.Size = new System.Drawing.Size(75, 23);
			this.m_btnOval.TabIndex = 7;
			this.m_btnOval.Text = "Oval";
			this.m_btnOval.UseVisualStyleBackColor = true;
			this.m_btnOval.Click += new System.EventHandler(this.m_btnOval_Click);
			// 
			// m_btnRectangle
			// 
			this.m_btnRectangle.Location = new System.Drawing.Point(99, 19);
			this.m_btnRectangle.Name = "m_btnRectangle";
			this.m_btnRectangle.Size = new System.Drawing.Size(75, 23);
			this.m_btnRectangle.TabIndex = 6;
			this.m_btnRectangle.Text = "Rectangle";
			this.m_btnRectangle.UseVisualStyleBackColor = true;
			this.m_btnRectangle.Click += new System.EventHandler(this.m_btnRectangle_Click);
			// 
			// m_btnCircle
			// 
			this.m_btnCircle.Location = new System.Drawing.Point(18, 19);
			this.m_btnCircle.Name = "m_btnCircle";
			this.m_btnCircle.Size = new System.Drawing.Size(75, 23);
			this.m_btnCircle.TabIndex = 5;
			this.m_btnCircle.Text = "Circle";
			this.m_btnCircle.UseVisualStyleBackColor = true;
			this.m_btnCircle.Click += new System.EventHandler(this.m_btnCircle_Click);
			// 
			// BranchTubeTypeForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(434, 61);
			this.Controls.Add(this.m_btnDShape);
			this.Controls.Add(this.m_btnFlatOval);
			this.Controls.Add(this.m_btnOval);
			this.Controls.Add(this.m_btnRectangle);
			this.Controls.Add(this.m_btnCircle);
			this.Name = "BranchTubeTypeForm";
			this.Text = "BranchTubeTypeForm";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button m_btnDShape;
		private System.Windows.Forms.Button m_btnFlatOval;
		private System.Windows.Forms.Button m_btnOval;
		private System.Windows.Forms.Button m_btnRectangle;
		private System.Windows.Forms.Button m_btnCircle;
	}
}