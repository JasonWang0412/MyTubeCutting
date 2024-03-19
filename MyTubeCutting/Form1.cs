using OCC.AIS;
using OCC.BRepBuilderAPI;
using OCC.gp;
using OCC.TopoDS;
using System.Drawing;
using System.Windows.Forms;

namespace MyTubeCutting
{
	public partial class Form1 : Form
	{
		OCCViewer m_Viewer;
		public Form1()
		{
			InitializeComponent();
			m_Viewer = new OCCViewer();
			bool isSuccess = m_Viewer.InitViewer( this.Handle );
			if( isSuccess == false ) {
				MessageBox.Show( "init failed" );
			}
			m_Viewer.SetBackgroundColor( 0, 0, 0 );
			m_Viewer.TopView();

			Rectangle rect = new Rectangle( 0, 0, 200, 200 );
			TopoDS_Shape shape = GetSquare( rect );
			AIS_Shape aisShape = new AIS_Shape( shape );
			m_Viewer.GetAISContext().Display( aisShape, true );
			m_Viewer.ZoomAllView();

			Paint += new PaintEventHandler( Form1_Paint );
		}

		public static TopoDS_Shape GetSquare( Rectangle rect )
		{
			BRepBuilderAPI_MakeWire makeWire = new BRepBuilderAPI_MakeWire();
			gp_Pnt2d pnt1 = new gp_Pnt2d( rect.Left, rect.Bottom );
			gp_Pnt2d pnt2 = new gp_Pnt2d( rect.Right, rect.Bottom );
			gp_Pnt2d pnt3 = new gp_Pnt2d( rect.Right, rect.Top );
			gp_Pnt2d pnt4 = new gp_Pnt2d( rect.Left, rect.Top );
			BRepBuilderAPI_MakeEdge2d makeEdge = new BRepBuilderAPI_MakeEdge2d( pnt1, pnt2 );
			TopoDS_Edge tEdge = makeEdge.Edge();
			makeWire.Add( tEdge );
			makeEdge = new BRepBuilderAPI_MakeEdge2d( pnt2, pnt3 );
			tEdge = makeEdge.Edge();
			makeWire.Add( tEdge );
			makeEdge = new BRepBuilderAPI_MakeEdge2d( pnt3, pnt4 );
			tEdge = makeEdge.Edge();
			makeWire.Add( tEdge );
			makeEdge = new BRepBuilderAPI_MakeEdge2d( pnt4, pnt1 );
			tEdge = makeEdge.Edge();
			makeWire.Add( tEdge );
			return makeWire.Wire();
		}
		void Form1_Paint( object sender, PaintEventArgs e )
		{
			m_Viewer.UpdateView();
		}
	}
}
