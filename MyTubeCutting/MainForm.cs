using OCC.AIS;
using OCC.TopoDS;
using System.Drawing;
using System.Windows.Forms;
using Core;

namespace MyTubeCutting
{
	public partial class MainForm : Form
	{
		OCCViewer m_Viewer;
		public MainForm()
		{
			InitializeComponent();
			m_Viewer = new OCCViewer();
			bool isSuccess = m_Viewer.InitViewer( m_PanelViewer.Handle );
			if( isSuccess == false ) {
				MessageBox.Show( "init failed" );
			}
			m_Viewer.SetBackgroundColor( 0, 0, 0 );
			m_Viewer.TopView();
			MyDraw();
		}

		int m_nXMousePosition = 0;
		int m_nYMousePosition = 0;

		void MyDraw()
		{
			Rectangle rect = new Rectangle( 0, 0, 200, 200 );
			TopoDS_Shape shape = OCCTool.GetSquare( rect );
			AIS_Shape aisShape = new AIS_Shape( shape );
			m_Viewer.GetAISContext().Display( aisShape, true );
		}

		void m_PanelViewer_MouseDown( object sender, MouseEventArgs e )
		{
			ViewerMouseAction.MouseDown( e, m_Viewer, ref m_nXMousePosition, ref m_nYMousePosition );
		}

		void m_PanelViewer_MouseMove( object sender, MouseEventArgs e )
		{
			ViewerMouseAction.MouseMove( e, m_Viewer, ref m_nXMousePosition, ref m_nYMousePosition );
		}

		void m_PanelViewer_MouseWheel( object sender, MouseEventArgs e )
		{
			ViewerMouseAction.MouseWheel( e, m_Viewer );
		}

		void m_PanelViewer_MouseDoubleClick( object sender, MouseEventArgs e )
		{
			m_Viewer.ZoomAllView();
		}

		void m_PanelViewer_Paint( object sender, PaintEventArgs e )
		{
			m_Viewer.UpdateView();
		}
	}
}
