using OCC.BRepBuilderAPI;
using OCC.BRepPrimAPI;
using OCC.Geom;
using OCC.gp;
using OCC.TopoDS;
using System;

namespace MyCore.CAD
{
	public class OCCTool
	{
		public static TopoDS_Wire MakeShapeWire( IGeom2D basicGeom, double dNeckin, gp_Pnt center, gp_Dir dir, double dRotation )
		{
			// data protection
			if( basicGeom == null || center == null || dir == null ) {
				return null;
			}

			TopoDS_Wire wireXOY;
			if( basicGeom.Type == Geom2D_Type.Circle ) {
				wireXOY = MakeXOYCircleWire( (Geom2D_Circle)basicGeom, dNeckin );
			}
			else if( basicGeom.Type == Geom2D_Type.Rectangle ) {
				wireXOY = MakeXOYRectangleWire( (Geom2D_Rectangle)basicGeom, dNeckin );
			}
			else {
				return null;
			}
			if( wireXOY == null ) {
				return null;
			}

			// transform wire
			TopoDS_Shape transformedShape = TransformXOYBaseShape( wireXOY, center, dir, dRotation );
			if( transformedShape == null ) {
				return null;
			}

			TopoDS_Wire resultWire = TopoDS.ToWire( transformedShape );
			return resultWire;
		}

		public static TopoDS_Shape TransformXOYBaseShape( TopoDS_Shape shapeToTransform, gp_Pnt targetCenter, gp_Dir targetDir, double dRotation )
		{
			// rotate shape aroud Z axis by dRotation
			gp_Trsf transformR = new gp_Trsf();
			transformR.SetRotation( new gp_Ax1( new gp_Pnt( 0, 0, 0 ), new gp_Dir( 0, 0, 1 ) ), dRotation );

			// rotate shape from Z to dir
			gp_Quaternion quaternion = new gp_Quaternion( new gp_Vec( new gp_Dir( 0, 0, 1 ) ), new gp_Vec( targetDir ) );
			gp_Trsf transformD = new gp_Trsf();
			transformD.SetRotation( quaternion );

			// translate shape from XY origin to target center
			gp_Trsf transformC = new gp_Trsf();
			transformC.SetTranslation( new gp_Vec( targetCenter.XYZ() ) );

			// combine all transformations
			gp_Trsf trsfFinal = transformC.Multiplied( transformD ).Multiplied( transformR );
			BRepBuilderAPI_Transform shapeTrsfFinal = new BRepBuilderAPI_Transform( shapeToTransform, trsfFinal );
			if( shapeTrsfFinal.IsDone() == false ) {
				return null;
			}
			return shapeTrsfFinal.Shape();
		}

		public static TopoDS_Shape MakeConcretePrismByWire( TopoDS_Wire baseWire, gp_Vec vec, bool isInf )
		{
			// data protection
			if( baseWire == null ) {
				return null;
			}

			// make face
			BRepBuilderAPI_MakeFace branchFaceMaker = new BRepBuilderAPI_MakeFace( baseWire );
			if( branchFaceMaker.IsDone() == false ) {
				return null;
			}

			// make prism
			BRepPrimAPI_MakePrism branchTubeMaker;
			if( isInf ) {
				gp_Dir dir = new gp_Dir( vec );
				branchTubeMaker = new BRepPrimAPI_MakePrism( branchFaceMaker.Shape(), dir, true );
			}
			else {
				branchTubeMaker = new BRepPrimAPI_MakePrism( branchFaceMaker.Shape(), vec );
			}
			if( branchTubeMaker.IsDone() == false ) {
				return null;
			}
			return branchTubeMaker.Shape();
		}

		static TopoDS_Wire MakeXOYCircleWire( Geom2D_Circle circleParam, double dNeckin )
		{
			// data protection
			if( circleParam == null ) {
				return null;
			}

			// get radius
			double dRadius = circleParam.Radius - dNeckin;
			if( dRadius <= 0 ) {
				return null;
			}

			// make circle wire on XY plane
			gp_Circ gpCircle = new gp_Circ( new gp_Ax2( new gp_Pnt( 0, 0, 0 ), new gp_Dir( 0, 0, 1 ) ), dRadius );
			BRepBuilderAPI_MakeEdge edgeMaker = new BRepBuilderAPI_MakeEdge( gpCircle );
			if( edgeMaker.IsDone() == false ) {
				return null;
			}
			BRepBuilderAPI_MakeWire wireMaker = new BRepBuilderAPI_MakeWire( edgeMaker.Edge() );
			if( wireMaker.IsDone() == false ) {
				return null;
			}
			return wireMaker.Wire();
		}

		static TopoDS_Wire MakeXOYRectangleWire( Geom2D_Rectangle rectParam, double dNeckin )
		{
			// data protection
			if( rectParam == null ) {
				return null;
			}

			// get width and height
			double width = rectParam.Width - 2 * dNeckin;
			double height = rectParam.Height - 2 * dNeckin;
			if( width <= 0 || height <= 0 ) {
				return null;
			}

			// get fillet
			double fillet = rectParam.Fillet - dNeckin;
			if( fillet >= width / 2 || fillet >= height / 2 ) {
				return null;
			}
			else if( fillet < 0 ) {
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

				//TODO: the "sense" argument doen't show magic during testing
				Geom_TrimmedCurve trim1 = new Geom_TrimmedCurve( circle1, 0, Math.PI / 2, true );
				Geom_TrimmedCurve trim2 = new Geom_TrimmedCurve( circle2, 3 * Math.PI / 2, Math.PI * 2, true );
				Geom_TrimmedCurve trim3 = new Geom_TrimmedCurve( circle3, Math.PI, 3 * Math.PI / 2, true );
				Geom_TrimmedCurve trim4 = new Geom_TrimmedCurve( circle4, Math.PI / 2, Math.PI, true );

				fillet1 = new BRepBuilderAPI_MakeEdge( trim1 );
				fillet2 = new BRepBuilderAPI_MakeEdge( trim2 );
				fillet3 = new BRepBuilderAPI_MakeEdge( trim3 );
				fillet4 = new BRepBuilderAPI_MakeEdge( trim4 );
				if( fillet1.IsDone() == false || fillet2.IsDone() == false || fillet3.IsDone() == false || fillet4.IsDone() == false ) {
					return null;
				}
			}

			// edge up, from left to right
			gp_Pnt pUP1 = new gp_Pnt( -width / 2 + fillet, height / 2, 0 );
			gp_Pnt pUP2 = new gp_Pnt( width / 2 - fillet, height / 2, 0 );
			BRepBuilderAPI_MakeEdge edgeUp = new BRepBuilderAPI_MakeEdge( pUP1, pUP2 );
			if( edgeUp.IsDone() == false ) {
				return null;
			}
			wireMaker.Add( edgeUp.Edge() );
			if( fillet > 0 ) {
				wireMaker.Add( fillet1.Edge() );
			}

			// edge right, from up to down
			gp_Pnt pRight1 = new gp_Pnt( width / 2, height / 2 - fillet, 0 );
			gp_Pnt pRight2 = new gp_Pnt( width / 2, -height / 2 + fillet, 0 );
			BRepBuilderAPI_MakeEdge edgeRight = new BRepBuilderAPI_MakeEdge( pRight1, pRight2 );
			if( edgeRight.IsDone() == false ) {
				return null;
			}
			wireMaker.Add( edgeRight.Edge() );
			if( fillet > 0 ) {
				wireMaker.Add( fillet2.Edge() );
			}

			// edge down, from right to left
			gp_Pnt pDown1 = new gp_Pnt( width / 2 - fillet, -height / 2, 0 );
			gp_Pnt pDown2 = new gp_Pnt( -width / 2 + fillet, -height / 2, 0 );
			BRepBuilderAPI_MakeEdge edgeDown = new BRepBuilderAPI_MakeEdge( pDown1, pDown2 );
			if( edgeDown.IsDone() == false ) {
				return null;
			}
			wireMaker.Add( edgeDown.Edge() );
			if( fillet > 0 ) {
				wireMaker.Add( fillet3.Edge() );
			}

			// edge left, from down to up
			gp_Pnt pLeft1 = new gp_Pnt( -width / 2, -height / 2 + fillet, 0 );
			gp_Pnt pLeft2 = new gp_Pnt( -width / 2, height / 2 - fillet, 0 );
			BRepBuilderAPI_MakeEdge edgeLeft = new BRepBuilderAPI_MakeEdge( pLeft1, pLeft2 );
			if( edgeLeft.IsDone() == false ) {
				return null;
			}
			wireMaker.Add( edgeLeft.Edge() );
			if( fillet > 0 ) {
				wireMaker.Add( fillet4.Edge() );
			}

			// make wire
			if( wireMaker.IsDone() == false ) {
				return null;
			}
			return wireMaker.Wire();
		}
	}
}
