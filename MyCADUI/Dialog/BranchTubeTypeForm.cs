using System;
using System.Windows.Forms;

namespace MyCADUI
{
	public partial class BranchTubeTypeForm : Form
	{
		public BranchTubeTypeForm()
		{
			InitializeComponent();
		}

		void m_btnCircle_Click( object sender, EventArgs e )
		{
			BranchTubeTypeSelected?.Invoke( BranchTubeType.Circle );
			Close();
		}

		void m_btnRectangle_Click( object sender, EventArgs e )
		{
			BranchTubeTypeSelected?.Invoke( BranchTubeType.Rectangle );
			Close();
		}

		void m_btnOval_Click( object sender, EventArgs e )
		{
			BranchTubeTypeSelected?.Invoke( BranchTubeType.Oval );
			Close();
		}

		void m_btnFlatOval_Click( object sender, EventArgs e )
		{
			BranchTubeTypeSelected?.Invoke( BranchTubeType.FlatOval );
			Close();
		}

		void m_btnDShape_Click( object sender, EventArgs e )
		{
			BranchTubeTypeSelected?.Invoke( BranchTubeType.DShape );
			Close();
		}

		internal delegate void BranchTubeTypeSelectedHandler( BranchTubeType type );
		internal event BranchTubeTypeSelectedHandler BranchTubeTypeSelected;
	}
}
