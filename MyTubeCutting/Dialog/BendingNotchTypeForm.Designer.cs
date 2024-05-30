namespace MyCADUI
{
	partial class BendingNotchTypeForm
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
			this.m_btnVShape = new System.Windows.Forms.Button();
			this.m_btnBothSideFillet = new System.Windows.Forms.Button();
			this.m_btnOneSideFillet = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// m_btnVShape
			// 
			this.m_btnVShape.Location = new System.Drawing.Point(13, 13);
			this.m_btnVShape.Name = "m_btnVShape";
			this.m_btnVShape.Size = new System.Drawing.Size(75, 23);
			this.m_btnVShape.TabIndex = 0;
			this.m_btnVShape.Text = "VShape";
			this.m_btnVShape.UseVisualStyleBackColor = true;
			this.m_btnVShape.Click += new System.EventHandler(this.m_btnVShape_Click);
			// 
			// m_btnBothSideFillet
			// 
			this.m_btnBothSideFillet.Location = new System.Drawing.Point(95, 13);
			this.m_btnBothSideFillet.Name = "m_btnBothSideFillet";
			this.m_btnBothSideFillet.Size = new System.Drawing.Size(75, 23);
			this.m_btnBothSideFillet.TabIndex = 1;
			this.m_btnBothSideFillet.Text = "BothSideFillet";
			this.m_btnBothSideFillet.UseVisualStyleBackColor = true;
			this.m_btnBothSideFillet.Click += new System.EventHandler(this.m_btnBothSideFillet_Click);
			// 
			// m_btnOneSideFillet
			// 
			this.m_btnOneSideFillet.Location = new System.Drawing.Point(177, 13);
			this.m_btnOneSideFillet.Name = "m_btnOneSideFillet";
			this.m_btnOneSideFillet.Size = new System.Drawing.Size(75, 23);
			this.m_btnOneSideFillet.TabIndex = 2;
			this.m_btnOneSideFillet.Text = "OneSideFillet";
			this.m_btnOneSideFillet.UseVisualStyleBackColor = true;
			this.m_btnOneSideFillet.Click += new System.EventHandler(this.m_btnOneSideFillet_Click);
			// 
			// BendingNotchTypeForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(434, 61);
			this.Controls.Add(this.m_btnOneSideFillet);
			this.Controls.Add(this.m_btnBothSideFillet);
			this.Controls.Add(this.m_btnVShape);
			this.Name = "BendingNotchTypeForm";
			this.Text = "BendingNotchTypeForm";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button m_btnVShape;
		private System.Windows.Forms.Button m_btnBothSideFillet;
		private System.Windows.Forms.Button m_btnOneSideFillet;
	}
}