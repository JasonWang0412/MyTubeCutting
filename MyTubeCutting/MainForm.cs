using MyCore.CAD;
using MyCore.Tool;
using OCC.AIS;
using OCC.BRepAlgoAPI;
using OCC.BRepBuilderAPI;
using OCC.BRepPrimAPI;
using OCC.gp;
using OCC.TopoDS;
using System.Drawing;
using System.Windows.Forms;

namespace MyTubeCutting
{
	public partial class MainForm : Form
	{
		OCCViewer m_Viewer;
		public MainForm()
		{
			InitializeComponent();
			m_Viewer = new OCCViewer();
			bool isSuccess = m_Viewer.InitViewer( m_panViewer.Handle );
			if( isSuccess == false ) {
				MessageBox.Show( "init failed" );
			}
			m_Viewer.SetBackgroundColor( 0, 0, 0 );
			m_Viewer.TopView();
			//MyDraw();
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

		void m_panViewer_MouseDown( object sender, MouseEventArgs e )
		{
			ViewerMouseAction.MouseDown( e, m_Viewer, ref m_nXMousePosition, ref m_nYMousePosition );
		}

		void m_panViewer_MouseMove( object sender, MouseEventArgs e )
		{
			ViewerMouseAction.MouseMove( e, m_Viewer, ref m_nXMousePosition, ref m_nYMousePosition );
		}

		void m_panViewer_MouseWheel( object sender, MouseEventArgs e )
		{
			ViewerMouseAction.MouseWheel( e, m_Viewer );
		}

		void m_panViewer_MouseDoubleClick( object sender, MouseEventArgs e )
		{
			if( e.Button == MouseButtons.Left ) {
				m_Viewer.ZoomAllView();
			}
			else {
				m_Viewer.TopView();
			}
		}

		void m_panViewer_Paint( object sender, PaintEventArgs e )
		{
			m_Viewer.UpdateView();
		}

		void m_btcCircleTest_Click( object sender, System.EventArgs e )
		{
			CircleCrossSectionParam param = new CircleCrossSectionParam()
			{
				Radius = 25,
				Thickness = 2,
			};

			gp_Circ outGP = new gp_Circ( gp.YOZ(), param.Radius );
			BRepBuilderAPI_MakeEdge outEdgeMaker = new BRepBuilderAPI_MakeEdge( outGP );
			BRepBuilderAPI_MakeWire outWireMaker = new BRepBuilderAPI_MakeWire( outEdgeMaker.Edge() );
			BRepBuilderAPI_MakeFace outFaceMaker = new BRepBuilderAPI_MakeFace( outWireMaker.Wire() );

			gp_Circ inGP = new gp_Circ( gp.YOZ(), param.Radius - param.Thickness );
			BRepBuilderAPI_MakeEdge inEdgeMaker = new BRepBuilderAPI_MakeEdge( inGP );
			BRepBuilderAPI_MakeWire inWireMaker = new BRepBuilderAPI_MakeWire( inEdgeMaker.Edge() );
			BRepBuilderAPI_MakeFace inFaceMaker = new BRepBuilderAPI_MakeFace( inWireMaker.Wire() );

			BRepAlgoAPI_Cut cut = new BRepAlgoAPI_Cut( outFaceMaker.Face(), inFaceMaker.Face() );
			gp_Vec vec = new gp_Vec( 100, 0, 0 );
			BRepPrimAPI_MakePrism tubeMaker = new BRepPrimAPI_MakePrism( cut.Shape(), vec );

			AIS_Shape tubeAIS = new AIS_Shape( tubeMaker.Shape() );
			tubeAIS.Attributes().SetDisplayMode( 1 );
			AIS_Shape outAIS = new AIS_Shape( outWireMaker.Shape() );
			AIS_Shape inAIS = new AIS_Shape( inWireMaker.Shape() );
			m_Viewer.GetAISContext().Display( tubeAIS, true );
			//m_Viewer.GetAISContext().Display( outAIS, true );
			//m_Viewer.GetAISContext().Display( inAIS, true );
		}
	}
}
