using System;
using System.Windows.Forms;

namespace MyCADUI
{
	public partial class MainTubeTypeForm : Form
	{
		public MainTubeTypeForm()
		{
			InitializeComponent();
		}

		void m_btnCircle_Click( object sender, EventArgs e )
		{
			MainTubeTypeSelected?.Invoke( MainTubeType.Circle );
			Close();
		}

		void m_btnRectangle_Click( object sender, EventArgs e )
		{
			MainTubeTypeSelected?.Invoke( MainTubeType.Rectangle );
			Close();
		}

		void m_btnOval_Click( object sender, EventArgs e )
		{
			MainTubeTypeSelected?.Invoke( MainTubeType.Oval );
			Close();
		}

		void m_btnFlatOval_Click( object sender, EventArgs e )
		{
			MainTubeTypeSelected?.Invoke( MainTubeType.FlatOval );
			Close();
		}

		void m_btnDShape_Click( object sender, EventArgs e )
		{
			MainTubeTypeSelected?.Invoke( MainTubeType.DShape );
			Close();
		}

		internal delegate void MainTubeTypeSelectedHandler( MainTubeType type );
		internal event MainTubeTypeSelectedHandler MainTubeTypeSelected;
	}
}
