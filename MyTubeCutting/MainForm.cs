using OCC.AIS;
using OCC.TopoDS;
using System.Drawing;
using System.Windows.Forms;
using MyCore.Tool;
using OCC.BRepPrimAPI;
using OCC.gp;

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
			gp_Vec vec = new gp_Vec( 0, 0, 200 );
			BRepPrimAPI_MakePrism mp = new BRepPrimAPI_MakePrism( shape, vec );

			AIS_Shape aisShape = new AIS_Shape( mp.Shape() );
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
			if( e.Button == MouseButtons.Left ) {
				m_Viewer.ZoomAllView();
			}
			else {
				m_Viewer.TopView();
			}
		}

		void m_PanelViewer_Paint( object sender, PaintEventArgs e )
		{
			m_Viewer.UpdateView();
		}
	}
}
