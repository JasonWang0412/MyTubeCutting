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

		AIS_Shape m_ais1;
		AIS_Shape m_ais2;
		AIS_Shape m_ais3;

		void MyDraw()
		{
			Rectangle rect1 = new Rectangle( 0, 200, 200, 200 );
			Rectangle rect2 = new Rectangle( 0, 0, 200, 200 );
			Rectangle rect3 = new Rectangle( 200, 0, 200, 200 );

			TopoDS_Shape shape1 = OCCTool.GetSquare( rect1 );
			TopoDS_Shape shape2 = OCCTool.GetSquare( rect2 );
			TopoDS_Shape shape3 = OCCTool.GetSquare( rect3 );

			m_ais1 = new AIS_Shape( shape1 );
			m_ais2 = new AIS_Shape( shape2 );
			m_ais3 = new AIS_Shape( shape3 );

			m_Viewer.GetAISContext().Display( m_ais1, false );
			m_Viewer.GetAISContext().Display( m_ais2, false );
			m_Viewer.GetAISContext().Display( m_ais3, false );
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

		private void button1_Click( object sender, System.EventArgs e )
		{
			m_Viewer.GetAISContext().Remove( m_ais1, false );
			m_Viewer.GetAISContext().Remove( m_ais3, false );
		}

		private void button2_Click( object sender, System.EventArgs e )
		{
			m_Viewer.GetAISContext().UpdateCurrentViewer();
			m_Viewer.GetAISContext().Display( m_ais1, false );
		}

		private void button3_Click( object sender, System.EventArgs e )
		{
			m_Viewer.GetAISContext().UpdateCurrentViewer();
		}

		private void button4_Click( object sender, System.EventArgs e )
		{

		}
	}
}
