using MyCore.CAD;
using OCC.BRepAlgoAPI;
using OCC.BRepBuilderAPI;
using OCC.BRepPrimAPI;
using OCC.Geom;
using OCC.gp;
using OCC.TopoDS;
using System;

namespace MyCore.Tool
{
	public class OCCTool
	{
		// make wire
		public static TopoDS_Wire MakeWire( IGeom2D basicGeom, double dNeckin, gp_Pnt center, gp_Dir dir, double dRotation )
		{
			if( basicGeom.Type == Geom2D_Type.Circle ) {
				return MakeCircleWire( (Geom2D_Circle)basicGeom, dNeckin, center, dir, dRotation );
			}
			else if( basicGeom.Type == Geom2D_Type.Rectangle ) {
				return MakeRectangleWire( (Geom2D_Rectangle)basicGeom, dNeckin, center, dir, dRotation );
			}
			else {
				throw new System.Exception( "Not supported cross section type" );
			}
		}

		static TopoDS_Wire MakeCircleWire( Geom2D_Circle circleParam, double dNeckin, gp_Pnt center, gp_Dir dir, double dRotation )
		{
			// get radius
			double dRadius = circleParam.Radius - dNeckin;

			// make circle wire on XY plane
			gp_Circ gpCircle = new gp_Circ( new gp_Ax2( new gp_Pnt( 0, 0, 0 ), new gp_Dir( 0, 0, 1 ) ), dRadius );
			BRepBuilderAPI_MakeEdge edgeMaker = new BRepBuilderAPI_MakeEdge( gpCircle );
			BRepBuilderAPI_MakeWire wireMaker = new BRepBuilderAPI_MakeWire( edgeMaker.Edge() );

			// transform wire
			TopoDS_Shape transformedShape = TransformBasicShape( wireMaker.Wire(), center, dir, dRotation );
			if( transformedShape == null ) {
				return null;
			}
			TopoDS_Wire wire = TopoDS.ToWire( transformedShape );
			return wire;
		}

		static TopoDS_Wire MakeRectangleWire( Geom2D_Rectangle rectParam, double dNeckin, gp_Pnt center, gp_Dir dir, double dRotation )
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

				//TODO: how it works?
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

			// transform wire
			TopoDS_Shape transformedShape = TransformBasicShape( wireMaker.Wire(), center, dir, dRotation );
			if( transformedShape == null ) {
				return null;
			}
			TopoDS_Wire wire = TopoDS.ToWire( transformedShape );
			return wire;
		}

		// make tube
		public static TopoDS_Shape MakeRawTubeShape( TopoDS_Wire outerWire, TopoDS_Wire innerWire, double tubeLength )
		{
			BRepBuilderAPI_MakeFace outerFaceMaker = new BRepBuilderAPI_MakeFace( outerWire );
			BRepBuilderAPI_MakeFace innerFaceMaker = new BRepBuilderAPI_MakeFace( innerWire );

			// cut outer face by inner face
			BRepAlgoAPI_Cut cut = new BRepAlgoAPI_Cut( outerFaceMaker.Face(), innerFaceMaker.Face() );

			// make tube
			gp_Vec vec = new gp_Vec( 0, tubeLength, 0 );
			BRepPrimAPI_MakePrism tubeMaker = new BRepPrimAPI_MakePrism( cut.Shape(), vec );
			if( tubeMaker.IsDone() == false ) {
				return null;
			}

			return tubeMaker.Shape();
		}

		// convert parameter
		public static void GetBranchTubeDir( double dA_deg, double dB_deg, out gp_Dir dir )
		{
			// the initail direction is (0, 0, 1)
			gp_Dir dirInit = new gp_Dir( 0, 0, 1 );

			// rotate around X axis by A angle in radian
			gp_Trsf transformA = new gp_Trsf();
			transformA.SetRotation( new gp_Ax1( new gp_Pnt( 0, 0, 0 ), new gp_Dir( 1, 0, 0 ) ), dA_deg * Math.PI / 180 );
			gp_Dir dirA = dirInit.Transformed( transformA );

			// rotate around Y axis by B angle in radian
			gp_Trsf transformB = new gp_Trsf();
			transformB.SetRotation( new gp_Ax1( new gp_Pnt( 0, 0, 0 ), new gp_Dir( 0, 1, 0 ) ), dB_deg * Math.PI / 180 );
			dir = dirA.Transformed( transformB );
		}

		public static void GetEndCutterDir( double dTilt_deg, double dRotate_deg, out gp_Dir dir )
		{
			// the initail direction is (0, 1, 0)
			gp_Dir dirInit = new gp_Dir( 0, 1, 0 );

			// rotate around X axis by tilt angle in radian
			gp_Trsf transformTilt = new gp_Trsf();
			transformTilt.SetRotation( new gp_Ax1( new gp_Pnt( 0, 0, 0 ), new gp_Dir( 1, 0, 0 ) ), dTilt_deg * Math.PI / 180 );
			gp_Dir dirTilt = dirInit.Transformed( transformTilt );

			// rotate around Y axis by rotate angle in radian
			gp_Trsf transformRotate = new gp_Trsf();
			transformRotate.SetRotation( new gp_Ax1( new gp_Pnt( 0, 0, 0 ), new gp_Dir( 0, 1, 0 ) ), dRotate_deg * Math.PI / 180 );
			dir = dirTilt.Transformed( transformRotate );
		}

		static TopoDS_Shape TransformBasicShape( TopoDS_Shape shapeToTransform, gp_Pnt targetCenter, gp_Dir targetDir, double dRotation )
		{
			// TODO: dont know shit about this
			// rotate shape aroud Z axis by dRotation
			gp_Trsf transformR = new gp_Trsf();
			transformR.SetRotation( new gp_Ax1( new gp_Pnt( 0, 0, 0 ), new gp_Dir( 0, 0, 1 ) ), dRotation );
			BRepBuilderAPI_Transform shapeTransformR = new BRepBuilderAPI_Transform( shapeToTransform, transformR );
			if( shapeTransformR.IsDone() == false ) {
				return null;
			}

			// rotate shape from Z to dir
			gp_Quaternion quaternion = new gp_Quaternion( new gp_Vec( new gp_Dir( 0, 0, 1 ) ), new gp_Vec( targetDir ) );
			gp_Trsf transformD = new gp_Trsf();
			transformD.SetRotation( quaternion );
			BRepBuilderAPI_Transform shapeTransformD = new BRepBuilderAPI_Transform( shapeTransformR.Shape(), transformD );
			if( shapeTransformD.IsDone() == false ) {
				return null;
			}

			// translate shape from XY origin to target center
			gp_Trsf transformC = new gp_Trsf();
			transformC.SetTranslation( new gp_Vec( targetCenter.XYZ() ) );
			BRepBuilderAPI_Transform shapeTransformC = new BRepBuilderAPI_Transform( shapeTransformD.Shape(), transformC );
			if( shapeTransformC.IsDone() == false ) {
				return null;
			}

			return shapeTransformC.Shape();
		}
	}
}
