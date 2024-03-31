using MyCore.CAD;
using OCC.BRepBuilderAPI;
using OCC.gp;
using OCC.TopoDS;

namespace MyCore.Tool
{
	public class OCCTool
	{
		// make wire
		public static TopoDS_Wire MakeWire( IGeom2D basicGeom, double dNeckin, gp_Pnt center, gp_Dir dir )
		{
			if( basicGeom.Type == Geom2D_Type.Circle ) {
				return MakeCircleWire( (Geom2D_Circle)basicGeom, dNeckin, center, dir );
			}
			else {
				throw new System.Exception( "Not supported cross section type" );
			}
		}

		static TopoDS_Wire MakeCircleWire( Geom2D_Circle circleParam, double dNeckin, gp_Pnt center, gp_Dir dir )
		{
			// get radius
			double dRadius = circleParam.Radius - dNeckin;

			// make circle wire
			gp_Circ gpCircle = new gp_Circ( new gp_Ax2( center, dir ), dRadius );
			BRepBuilderAPI_MakeEdge edgeMaker = new BRepBuilderAPI_MakeEdge( gpCircle );
			BRepBuilderAPI_MakeWire wireMaker = new BRepBuilderAPI_MakeWire( edgeMaker.Edge() );
			return wireMaker.Wire();
		}
	}
}
