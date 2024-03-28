using MyCore.CAD;
using MyCore.Tool;
using OCC.AIS;
using OCC.BRepAlgoAPI;
using OCC.BRepBuilderAPI;
using OCC.BRepPrimAPI;
using OCC.Geom;
using OCC.gp;
using OCC.TopoDS;
using System.Collections.Generic;
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
			m_Viewer.IsometricView();
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
			// make main tube
			double dRadius = 25;
			double dThickness = 2;
			double dTubeLength = 100;
			CS_Circle crossSectionParam = new CS_Circle( new BG_Circle() { Radius = dRadius } )
			{
				Thickness = dThickness,
			};
			TopoDS_Shape tubeShape = MakeCircleMainTube( crossSectionParam, dTubeLength );

			// make end cutters
			TopoDS_Shape cutterL = MakeEndCutter( new gp_Pnt( dRadius, 0, 0 ), new gp_Dir( -1, 0, 1 ) );
			TopoDS_Shape cutterR = MakeEndCutter( new gp_Pnt( dTubeLength - dRadius, 0, 0 ), new gp_Dir( 1, 0, -1 ) );

			// make branch tube
			TopoDS_Shape branch = MakeCircleBranchTube( new gp_Pnt( 60, 0, 0 ), new gp_Dir( 0, 0, 1 ), new BG_Circle() { Radius = 5 } );

			// cut main tube by end cutters
			BRepAlgoAPI_Cut cutL = new BRepAlgoAPI_Cut( tubeShape, cutterL );
			BRepAlgoAPI_Cut cutR = new BRepAlgoAPI_Cut( cutL.Shape(), cutterR );

			// cut main tube by branch tube
			BRepAlgoAPI_Cut cutBranch = new BRepAlgoAPI_Cut( cutR.Shape(), branch );

			AIS_Shape tubeAIS = new AIS_Shape( cutBranch.Shape() );
			tubeAIS.Attributes().SetDisplayMode( 1 );
			m_Viewer.GetAISContext().Display( tubeAIS, true );
		}

		TopoDS_Shape MakeCircleMainTube( CS_Circle param, double tubeLength )
		{
			gp_Circ outGP = new gp_Circ( gp.YOZ(), ( (BG_Circle)param.BasicGeom ).Radius );
			BRepBuilderAPI_MakeEdge outEdgeMaker = new BRepBuilderAPI_MakeEdge( outGP );
			BRepBuilderAPI_MakeWire outWireMaker = new BRepBuilderAPI_MakeWire( outEdgeMaker.Edge() );
			BRepBuilderAPI_MakeFace outFaceMaker = new BRepBuilderAPI_MakeFace( outWireMaker.Wire() );

			gp_Circ inGP = new gp_Circ( gp.YOZ(), ( (BG_Circle)param.BasicGeom ).Radius - param.Thickness );
			BRepBuilderAPI_MakeEdge inEdgeMaker = new BRepBuilderAPI_MakeEdge( inGP );
			BRepBuilderAPI_MakeWire inWireMaker = new BRepBuilderAPI_MakeWire( inEdgeMaker.Edge() );
			BRepBuilderAPI_MakeFace inFaceMaker = new BRepBuilderAPI_MakeFace( inWireMaker.Wire() );

			BRepAlgoAPI_Cut cut = new BRepAlgoAPI_Cut( outFaceMaker.Face(), inFaceMaker.Face() );
			gp_Vec vec = new gp_Vec( tubeLength, 0, 0 );
			BRepPrimAPI_MakePrism tubeMaker = new BRepPrimAPI_MakePrism( cut.Shape(), vec );

			return tubeMaker.Shape();
		}

		TopoDS_Shape MakeEndCutter( gp_Pnt pntCenter, gp_Dir cutPlaneDir )
		{
			gp_Pln cutPlane = new gp_Pln( pntCenter, cutPlaneDir );
			TopoDS_Face cutFace = new BRepBuilderAPI_MakeFace( cutPlane ).Face();
			gp_Pnt pointOnCutSide = new gp_Pnt( pntCenter.XYZ() + cutPlaneDir.XYZ() );
			BRepPrimAPI_MakeHalfSpace halfSpace = new BRepPrimAPI_MakeHalfSpace( cutFace, pointOnCutSide );
			return halfSpace.Shape();
		}

		TopoDS_Shape MakeCircleBranchTube( gp_Pnt pntCenter, gp_Dir Dir, BG_Circle param )
		{
			gp_Circ outGP = new gp_Circ( new gp_Ax2( pntCenter, Dir ), param.Radius );
			BRepBuilderAPI_MakeEdge outEdgeMaker = new BRepBuilderAPI_MakeEdge( outGP );
			BRepBuilderAPI_MakeWire outWireMaker = new BRepBuilderAPI_MakeWire( outEdgeMaker.Edge() );
			BRepBuilderAPI_MakeFace outFaceMaker = new BRepBuilderAPI_MakeFace( outWireMaker.Wire() );
			gp_Vec vec = new gp_Vec( Dir.x * 100, Dir.y * 100, Dir.z * 100 );
			BRepPrimAPI_MakePrism tubeMaker = new BRepPrimAPI_MakePrism( outFaceMaker.Shape(), vec );

			return tubeMaker.Shape();
		}
	}
}
