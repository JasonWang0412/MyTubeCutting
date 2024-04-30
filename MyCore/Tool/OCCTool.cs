using MyCore.CAD;
using OCC.BRep;
using OCC.BRepAlgoAPI;
using OCC.BRepBuilderAPI;
using OCC.BRepPrimAPI;
using OCC.Geom;
using OCC.gp;
using OCC.TopoDS;
using System;
using System.Collections.Generic;

namespace MyCore.Tool
{
	public class OCCTool
	{
		// make wire
		public static TopoDS_Wire MakeBaseWire( IGeom2D basicGeom, double dNeckin, gp_Pnt center, gp_Dir dir, double dRotation )
		{
			TopoDS_Wire wireXOY = new TopoDS_Wire();
			if( basicGeom.Type == Geom2D_Type.Circle ) {
				wireXOY = MakeXOYCircleWire( (Geom2D_Circle)basicGeom, dNeckin, center, dir, dRotation );
			}
			else if( basicGeom.Type == Geom2D_Type.Rectangle ) {
				wireXOY = MakeXOYRectangleWire( (Geom2D_Rectangle)basicGeom, dNeckin, center, dir, dRotation );
			}
			else {
				throw new System.Exception( "Not supported cross section type" );
			}

			// transform wire
			TopoDS_Shape transformedShape = TransformXOYBaseShape( wireXOY, center, dir, dRotation );
			if( transformedShape == null ) {
				return null;
			}
			TopoDS_Wire resultWire = TopoDS.ToWire( transformedShape );
			return resultWire;
		}

		static TopoDS_Wire MakeXOYCircleWire( Geom2D_Circle circleParam, double dNeckin, gp_Pnt center, gp_Dir dir, double dRotation )
		{
			// get radius
			double dRadius = circleParam.Radius - dNeckin;

			// make circle wire on XY plane
			gp_Circ gpCircle = new gp_Circ( new gp_Ax2( new gp_Pnt( 0, 0, 0 ), new gp_Dir( 0, 0, 1 ) ), dRadius );
			BRepBuilderAPI_MakeEdge edgeMaker = new BRepBuilderAPI_MakeEdge( gpCircle );
			BRepBuilderAPI_MakeWire wireMaker = new BRepBuilderAPI_MakeWire( edgeMaker.Edge() );

			// data protection
			if( wireMaker.IsDone() == false ) {
				return null;
			}
			return wireMaker.Wire();
		}

		static TopoDS_Wire MakeXOYRectangleWire( Geom2D_Rectangle rectParam, double dNeckin, gp_Pnt center, gp_Dir dir, double dRotation )
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
			return wireMaker.Wire();
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

			// rotate around Y axis by B angle in radian
			gp_Trsf transformB = new gp_Trsf();
			transformB.SetRotation( new gp_Ax1( new gp_Pnt( 0, 0, 0 ), new gp_Dir( 0, 1, 0 ) ), dB_deg * Math.PI / 180 );

			gp_Trsf trsfFinal = transformB.Multiplied( transformA );
			dir = dirInit.Transformed( trsfFinal );
		}

		public static void GetEndCutterDir( double dTilt_deg, double dRotate_deg, out gp_Dir dir )
		{
			// the initail direction is (0, 1, 0)
			gp_Dir dirInit = new gp_Dir( 0, 1, 0 );

			// rotate around X axis by tilt angle in radian
			gp_Trsf transformTilt = new gp_Trsf();
			transformTilt.SetRotation( new gp_Ax1( new gp_Pnt( 0, 0, 0 ), new gp_Dir( 1, 0, 0 ) ), dTilt_deg * Math.PI / 180 );

			// rotate around Y axis by rotate angle in radian
			gp_Trsf transformRotate = new gp_Trsf();
			transformRotate.SetRotation( new gp_Ax1( new gp_Pnt( 0, 0, 0 ), new gp_Dir( 0, 1, 0 ) ), dRotate_deg * Math.PI / 180 );

			gp_Trsf trsfFinal = transformRotate.Multiplied( transformTilt );
			dir = dirInit.Transformed( trsfFinal );
		}

		// make array
		public static TopoDS_Shape MakeArrayCompound( TopoDS_Shape oneBranchTube, ArrayParam arrayParam )
		{
			// create compound
			TopoDS_Compound compound = new TopoDS_Compound();
			TopoDS_Shape compoundShape = compound;
			BRep_Builder builder = new BRep_Builder();
			builder.MakeCompound( ref compound );

			// make linear array
			List<TopoDS_Shape> linearArrayShapeList = new List<TopoDS_Shape>();
			linearArrayShapeList.Add( oneBranchTube );
			for( int i = 1; i < arrayParam.LinearCount; i++ ) {

				// caluculate the linear offset distance
				double dOffset = arrayParam.LinearDistance * i;

				// get the transformation along Y axis
				gp_Trsf trsf = new gp_Trsf();
				trsf.SetTranslation( new gp_Vec( 0, dOffset, 0 ) );
				TopoDS_Shape oneLinearCopy = oneBranchTube.Moved( new OCC.TopLoc.TopLoc_Location( trsf ) );

				linearArrayShapeList.Add( oneLinearCopy );
			}

			// make angular array
			List<List<TopoDS_Shape>> angularArrayShapeList = new List<List<TopoDS_Shape>>();
			angularArrayShapeList.Add( linearArrayShapeList );
			for( int i = 1; i < arrayParam.AngularCount; i++ ) {

				// calculate the angular offset distance
				double dAngle_Deg = arrayParam.AngularDistance_Deg * i;

				// get the transformation around Y axis
				gp_Trsf trsf = new gp_Trsf();
				trsf.SetRotation( new gp_Ax1( new gp_Pnt( 0, 0, 0 ), new gp_Dir( 0, 1, 0 ) ), dAngle_Deg * Math.PI / 180 );

				List<TopoDS_Shape> oneAngularArray = new List<TopoDS_Shape>();
				foreach( TopoDS_Shape oneLinearCopy in linearArrayShapeList ) {
					TopoDS_Shape oneAngularCopy = oneLinearCopy.Moved( new OCC.TopLoc.TopLoc_Location( trsf ) );
					oneAngularArray.Add( oneAngularCopy );
				}
				angularArrayShapeList.Add( oneAngularArray );
			}

			// add all shapes to compound
			foreach( List<TopoDS_Shape> oneAngularArray in angularArrayShapeList ) {
				foreach( TopoDS_Shape oneLinearCopy in oneAngularArray ) {
					builder.Add( ref compoundShape, oneLinearCopy );
				}
			}

			return compound;
		}

		static TopoDS_Shape TransformXOYBaseShape( TopoDS_Shape shapeToTransform, gp_Pnt targetCenter, gp_Dir targetDir, double dRotation )
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

			// data protection
			if( shapeTrsfFinal.IsDone() == false ) {
				return null;
			}
			return shapeTrsfFinal.Shape();
		}
	}
}
