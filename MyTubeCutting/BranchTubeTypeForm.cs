using System;
using System.Windows.Forms;

namespace MyTubeCutting
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

		internal delegate void BranchTubeTypeSelectedHandler( BranchTubeType type );
		internal event BranchTubeTypeSelectedHandler BranchTubeTypeSelected;
	}
}
