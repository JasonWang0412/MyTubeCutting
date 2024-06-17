using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace MyUIDisplayModel
{
	public class MyDockContent : DockContent
	{
		public MyDockContent()
		{
			FormClosing += MyDockContent_FormClosing;
		}

		void MyDockContent_FormClosing( object sender, FormClosingEventArgs e )
		{
			e.Cancel = true;
		}
	}
}
