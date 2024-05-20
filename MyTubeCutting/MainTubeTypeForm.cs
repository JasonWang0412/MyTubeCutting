using System;
using System.Windows.Forms;

namespace MyTubeCutting
{
	public partial class MainTubeTypeForm : Form
	{
		public MainTubeTypeForm()
		{
			InitializeComponent();
		}

		void m_btnCircle_Click( object sender, EventArgs e )
		{
			MainTubeTypeSelected?.Invoke( TypeDefine.MainTubeType.Circle );
			Close();
		}

		void m_btnRectangle_Click( object sender, EventArgs e )
		{
			MainTubeTypeSelected?.Invoke( TypeDefine.MainTubeType.Rectangle );
			Close();
		}

		internal delegate void MainTubeTypeSelectedHandler( TypeDefine.MainTubeType type );
		internal event MainTubeTypeSelectedHandler MainTubeTypeSelected;
	}
}
