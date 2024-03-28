using OCC.BRepAlgoAPI;
using OCC.BRepBuilderAPI;
using OCC.BRepPrimAPI;
using OCC.gp;
using OCC.TopoDS;
using System.Collections.Generic;

namespace MyCore.CAD
{
	public class TubeMaker
	{
		public TubeMaker( MainTubeParam mainTubeParam,
			List<EndCutterParam> endCutterParam,
			List<BranchTubeParam> branchTubeParam )
		{
			MakeMainTube( mainTubeParam );
		}

		TopoDS_Shape MakeMainTube( MainTubeParam mainTubeParam )
		{
			// Make outer wire and inner wire
			TopoDS_Wire outerWire = MakeWire( mainTubeParam.CrossSection.BasicGeom, 0 );
			TopoDS_Wire innerWire = MakeWire( mainTubeParam.CrossSection.BasicGeom, mainTubeParam.CrossSection.Thickness );

			// Get solid shape by wire
			GetMakeTubeShape( outerWire, innerWire, mainTubeParam.Length );

			return default;
		}

		TopoDS_Wire MakeWire( IBasicGeom basicGeom, double dNeckin )
		{
			if( basicGeom.Type == BG_Type.Circle ) {
				return MakeCircleWire( (BG_Circle)basicGeom, dNeckin );
			}
			else {
				throw new System.Exception( "Not supported cross section type" );
			}
		}

		TopoDS_Wire MakeCircleWire( BG_Circle circleParam, double dNeckin )
		{
			// get radius
			double dRadius = circleParam.Radius - dNeckin;

			// make circle wire
			gp_Circ gpCircle = new gp_Circ( gp.YOZ(), circleParam.Radius );
			BRepBuilderAPI_MakeEdge edgeMaker = new BRepBuilderAPI_MakeEdge( gpCircle );
			BRepBuilderAPI_MakeWire wireMaker = new BRepBuilderAPI_MakeWire( edgeMaker.Edge() );
			return wireMaker.Wire();
		}

		TopoDS_Shape GetMakeTubeShape( TopoDS_Wire outerWire, TopoDS_Wire innerWire, double tubeLength )
		{
			BRepBuilderAPI_MakeFace outerFaceMaker = new BRepBuilderAPI_MakeFace( outerWire );
			BRepBuilderAPI_MakeFace innerFaceMaker = new BRepBuilderAPI_MakeFace( innerWire );

			// cut outer face by inner face
			BRepAlgoAPI_Cut cut = new BRepAlgoAPI_Cut( outerFaceMaker.Face(), innerFaceMaker.Face() );

			// make tube
			gp_Vec vec = new gp_Vec( tubeLength, 0, 0 );
			BRepPrimAPI_MakePrism tubeMaker = new BRepPrimAPI_MakePrism( cut.Shape(), vec );

			return tubeMaker.Shape();
		}
	}
}
