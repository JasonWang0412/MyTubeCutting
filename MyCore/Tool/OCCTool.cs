using MyCore.CAD;
using OCC.BRepBuilderAPI;
using OCC.Geom;
using OCC.gp;
using OCC.TopoDS;
using System;

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
			else if( basicGeom.Type == Geom2D_Type.Rectangle ) {
				return MakeRectangleWire( (Geom2D_Rectangle)basicGeom, dNeckin, center, dir );
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

		static TopoDS_Wire MakeRectangleWire( Geom2D_Rectangle rectParam, double dNeckin, gp_Pnt center, gp_Dir dir )
		{
			// get width and height
			double width = rectParam.Width - 2 * dNeckin;
			double height = rectParam.Height - 2 * dNeckin;

			// get fillet
			double fillet = rectParam.Fillet - dNeckin;
			if( fillet < 0 ) {
				fillet = 0;
			}

			BRepBuilderAPI_MakeWire wireMaker = new BRepBuilderAPI_MakeWire();

			// the fillets
			BRepBuilderAPI_MakeEdge fillet1 = null;
			BRepBuilderAPI_MakeEdge fillet2 = null;
			BRepBuilderAPI_MakeEdge fillet3 = null;
			BRepBuilderAPI_MakeEdge fillet4 = null;

			// the order is clockwise and start from the first quadrant
			if( fillet > 0 ) {
				gp_Dir normalDir = new gp_Dir( 0, 0, 1 );
				Geom_Circle circle1 = new Geom_Circle( new gp_Ax2( new gp_Pnt( width / 2 - fillet, height / 2 - fillet, 0 ), normalDir ), fillet );
				Geom_Circle circle2 = new Geom_Circle( new gp_Ax2( new gp_Pnt( width / 2 - fillet, -height / 2 + fillet, 0 ), normalDir ), fillet );
				Geom_Circle circle3 = new Geom_Circle( new gp_Ax2( new gp_Pnt( -width / 2 + fillet, -height / 2 + fillet, 0 ), normalDir ), fillet );
				Geom_Circle circle4 = new Geom_Circle( new gp_Ax2( new gp_Pnt( -width / 2 + fillet, height / 2 - fillet, 0 ), normalDir ), fillet );

				Geom_TrimmedCurve trim1 = new Geom_TrimmedCurve( circle1, 0, Math.PI / 2, true );
				Geom_TrimmedCurve trim2 = new Geom_TrimmedCurve( circle2, 3 * Math.PI / 2, Math.PI * 2, true );
				Geom_TrimmedCurve trim3 = new Geom_TrimmedCurve( circle3, Math.PI, 3 * Math.PI / 2, true );
				Geom_TrimmedCurve trim4 = new Geom_TrimmedCurve( circle4, Math.PI / 2, Math.PI, true );

				fillet1 = new BRepBuilderAPI_MakeEdge( trim1 );
				fillet2 = new BRepBuilderAPI_MakeEdge( trim2 );
				fillet3 = new BRepBuilderAPI_MakeEdge( trim3 );
				fillet4 = new BRepBuilderAPI_MakeEdge( trim4 );
			}

			// edge up, from left to right
			gp_Pnt pUP1 = new gp_Pnt( -width / 2 + fillet, height / 2, 0 );
			gp_Pnt pUP2 = new gp_Pnt( width / 2 - fillet, height / 2, 0 );
			BRepBuilderAPI_MakeEdge edgeUp = new BRepBuilderAPI_MakeEdge( pUP1, pUP2 );
			wireMaker.Add( edgeUp.Edge() );
			if( fillet > 0 ) {
				wireMaker.Add( fillet1.Edge() );
			}

			// edge right, from up to down
			gp_Pnt pRight1 = new gp_Pnt( width / 2, height / 2 - fillet, 0 );
			gp_Pnt pRight2 = new gp_Pnt( width / 2, -height / 2 + fillet, 0 );
			BRepBuilderAPI_MakeEdge edgeRight = new BRepBuilderAPI_MakeEdge( pRight1, pRight2 );
			wireMaker.Add( edgeRight.Edge() );
			if( fillet > 0 ) {
				wireMaker.Add( fillet2.Edge() );
			}

			// edge down, from right to left
			gp_Pnt pDown1 = new gp_Pnt( width / 2 - fillet, -height / 2, 0 );
			gp_Pnt pDown2 = new gp_Pnt( -width / 2 + fillet, -height / 2, 0 );
			BRepBuilderAPI_MakeEdge edgeDown = new BRepBuilderAPI_MakeEdge( pDown1, pDown2 );
			wireMaker.Add( edgeDown.Edge() );
			if( fillet > 0 ) {
				wireMaker.Add( fillet3.Edge() );
			}

			// edge left, from down to up
			gp_Pnt pLeft1 = new gp_Pnt( -width / 2, -height / 2 + fillet, 0 );
			gp_Pnt pLeft2 = new gp_Pnt( -width / 2, height / 2 - fillet, 0 );
			BRepBuilderAPI_MakeEdge edgeLeft = new BRepBuilderAPI_MakeEdge( pLeft1, pLeft2 );
			wireMaker.Add( edgeLeft.Edge() );
			if( fillet > 0 ) {
				wireMaker.Add( fillet4.Edge() );
			}

			// data protection
			if( wireMaker.IsDone() == false ) {
				return null;
			}

			// TODO: dont know shit about this
			// rotate wire from Z to dir
			gp_Trsf transformR = new gp_Trsf();
			gp_Quaternion quaternion = new gp_Quaternion( new gp_Vec( 0, 0, 1 ), new gp_Vec( dir ) );
			transformR.SetRotation( quaternion );
			BRepBuilderAPI_Transform wireTransformR = new BRepBuilderAPI_Transform( wireMaker.Wire(), transformR );
			if( wireTransformR.IsDone() == false ) {
				return null;
			}

			// translate wire from XY origin to center
			gp_Trsf transformT = new gp_Trsf();
			transformT.SetTranslation( new gp_Vec( center.XYZ() ) );
			BRepBuilderAPI_Transform wireTransformT = new BRepBuilderAPI_Transform( wireTransformR.Shape(), transformT );
			if( wireTransformT.IsDone() == false ) {
				return null;
			}

			TopoDS_Wire wire = TopoDS.ToWire( wireTransformT.Shape() );
			return wire;
		}
	}
}
