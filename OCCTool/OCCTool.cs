using OCC.BRepBuilderAPI;
using OCC.gp;
using OCC.TopoDS;
using System.Drawing;

namespace Core
{
	public class OCCTool
	{
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
	}
}
