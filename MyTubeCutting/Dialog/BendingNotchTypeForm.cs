using System;
using System.Windows.Forms;

namespace MyCADUI
{
	public partial class BendingNotchTypeForm : Form
	{
		public BendingNotchTypeForm()
		{
			InitializeComponent();
		}

		void m_btnVShape_Click( object sender, EventArgs e )
		{
			BendingNotchTypeSelected?.Invoke( BendingNotchType.VShape );
			Close();
		}

		void m_btnBothSideFillet_Click( object sender, EventArgs e )
		{
			BendingNotchTypeSelected?.Invoke( BendingNotchType.BothSideFillet );
			Close();
		}

		void m_btnOneSideFillet_Click( object sender, EventArgs e )
		{
			BendingNotchTypeSelected?.Invoke( BendingNotchType.OneSideFillet );
			Close();
		}

		internal delegate void BendingNotchTypeSelectedHandler( BendingNotchType bendingNotchType );
		internal event BendingNotchTypeSelectedHandler BendingNotchTypeSelected;
	}
}
