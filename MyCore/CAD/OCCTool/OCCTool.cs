using OCC.BRep;
using OCC.BRepBuilderAPI;
using OCC.BRepExtrema;
using OCC.BRepPrimAPI;
using OCC.Geom;
using OCC.gp;
using OCC.TopLoc;
using OCC.TopoDS;
using System;
using System.Collections.Generic;

namespace MyCore.CAD
{
	public class OCCTool
	{
		// make wire
		public static TopoDS_Wire MakeGeom2DWire( IGeom2D basicGeom, double dNeckin, gp_Pnt center, gp_Dir dir, double dRotation_deg )
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
			TopoDS_Shape transformedShape = TransformXOYBaseShape( wireXOY, center, dir, dRotation_deg );
			if( transformedShape == null ) {
				return null;
			}

			TopoDS_Wire resultWire = TopoDS.ToWire( transformedShape );
			return resultWire;
		}

		public static TopoDS_Wire MakeBendingNotchWire( IBendingNotchShape shape,
			double y, double z, double minZ, double maxZ, double dThickness, double angleB_deg )
		{
			// data protection
			if( shape == null || shape.IsValid() == false ) {
				return null;
			}

			TopoDS_Wire baseWire;
			if( shape.Type == BendingNotch_Type.VShape ) {
				baseWire = MakeYOZVShapeBNWire( (BN_VShape)shape, y, z, maxZ );
			}
			else if( shape.Type == BendingNotch_Type.BothSideFillet ) {
				baseWire = MakeYOZBothSideBNWire( (BN_BothSideFillet)shape, y, z, minZ, maxZ, dThickness );
			}
			else if( shape.Type == BendingNotch_Type.OneSideFillet ) {
				baseWire = MakeYOZOneSideBNWire( (BN_OneSideFillet)shape, y, z, minZ, maxZ, dThickness );
			}
			else {
				return null;
			}
			if( baseWire == null ) {
				return null;
			}

			// rotate wire around -Y axis by angleB
			gp_Trsf trsf = new gp_Trsf();
			trsf.SetRotation( new gp_Ax1( new gp_Pnt( 0, 0, 0 ), new gp_Dir( 0, -1, 0 ) ), angleB_deg * Math.PI / 180 );
			BRepBuilderAPI_Transform wireTrsf = new BRepBuilderAPI_Transform( baseWire, trsf );
			if( wireTrsf.IsDone() == false ) {
				return null;
			}

			return TopoDS.ToWire( wireTrsf.Shape() );
		}

		// get bounding box
		public static BoundingBox GetBoundingBox( TopoDS_Shape shape )
		{
			// data protection
			if( shape == null ) {
				return null;
			}

			// get bounding box
			double minX = GetBoundaryValue( shape, BoundaryType.MinX );
			double maxX = GetBoundaryValue( shape, BoundaryType.MaxX );
			double minY = GetBoundaryValue( shape, BoundaryType.MinY );
			double maxY = GetBoundaryValue( shape, BoundaryType.MaxY );
			double minZ = GetBoundaryValue( shape, BoundaryType.MinZ );
			double maxZ = GetBoundaryValue( shape, BoundaryType.MaxZ );

			BoundingBox BoundingBox = new BoundingBox( minX, maxX, minY, maxY, minZ, maxZ );
			return BoundingBox;
		}

		// transform XOY base shape
		public static TopoDS_Shape TransformXOYBaseShape( TopoDS_Shape shapeToTransform, gp_Pnt targetCenter, gp_Dir targetDir, double dRotation_deg )
		{
			// rotate shape aroud Z axis by dRotation
			gp_Trsf transformR = new gp_Trsf();
			transformR.SetRotation( new gp_Ax1( new gp_Pnt( 0, 0, 0 ), new gp_Dir( 0, 0, 1 ) ), dRotation_deg * Math.PI / 180 );

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

		// make prism
		// a lot of bug happens when using Inf, not recommended, ref: AUTO-12540
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

		// make array
		public static TopoDS_Shape MakeArrayCompound( TopoDS_Shape oneFeature, ArrayParam arrayParam )
		{
			// data protection
			if( oneFeature == null || arrayParam == null || arrayParam.IsValid() == false ) {
				return null;
			}
			if( arrayParam.LinearCount == 1 && arrayParam.AngularCount == 1 ) {
				return oneFeature;
			}

			try {
				// create compound
				TopoDS_Compound compound = new TopoDS_Compound();
				TopoDS_Shape compoundShape = compound;
				BRep_Builder builder = new BRep_Builder();
				builder.MakeCompound( ref compound );

				// make linear array
				List<TopoDS_Shape> linearArrayShapeList = new List<TopoDS_Shape>();
				linearArrayShapeList.Add( oneFeature );
				for( int i = 1; i < arrayParam.LinearCount; i++ ) {

					// caluculate the linear offset distance
					double dOffset = arrayParam.LinearDistance * i * ( arrayParam.LinearDirection == ArrayDirection.Positive ? 1 : -1 );

					// get the transformation along Y axis
					gp_Trsf trsf = new gp_Trsf();
					trsf.SetTranslation( new gp_Vec( 0, dOffset, 0 ) );
					TopoDS_Shape oneLinearCopy = oneFeature.Moved( new TopLoc_Location( trsf ) );

					linearArrayShapeList.Add( oneLinearCopy );
				}

				// make angular array
				List<List<TopoDS_Shape>> angularArrayShapeList = new List<List<TopoDS_Shape>>();
				angularArrayShapeList.Add( linearArrayShapeList );
				for( int i = 1; i < arrayParam.AngularCount; i++ ) {

					// calculate the angular offset distance
					double dAngle_Deg = arrayParam.AngularDistance_Deg * i * ( arrayParam.AngularDirection == ArrayDirection.Positive ? 1 : -1 );

					// get the transformation around Y axis
					gp_Trsf trsf = new gp_Trsf();
					trsf.SetRotation( new gp_Ax1( new gp_Pnt( 0, 0, 0 ), new gp_Dir( 0, -1, 0 ) ), dAngle_Deg * Math.PI / 180 );

					List<TopoDS_Shape> oneAngularArray = new List<TopoDS_Shape>();
					foreach( TopoDS_Shape oneLinearCopy in linearArrayShapeList ) {
						TopoDS_Shape oneAngularCopy = oneLinearCopy.Moved( new TopLoc_Location( trsf ) );
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

			// if any exception occurs, return the original shape
			catch( Exception ex ) {
				return oneFeature;
			}
		}

		// make wire
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

		static TopoDS_Wire MakeYOZVShapeBNWire( BN_VShape shape, double y, double z, double maxZ )
		{
			// calculate points
			double dVHeight = maxZ - Math.Abs( shape.JointGapLength ) - z;
			double dVHalfWidth = dVHeight * Math.Tan( shape.BendingAngle_deg / 2 * Math.PI / 180 );

			// this is ensure the size is big enough to cut the main tube
			double safeHeight = 1;

			double y0 = y;
			double z0 = z;
			double y1 = y0 - dVHalfWidth;
			double z1 = z0 + dVHeight;
			double y2 = y1 - Math.Abs( Math.Min( shape.JointGapLength, 0 ) );
			double z2 = z1;
			double y3 = y2;
			double z3 = z2 + Math.Abs( shape.JointGapLength ) + safeHeight;
			double y6 = y0 + dVHalfWidth;
			double z6 = z0 + dVHeight;
			double y5 = y6 + Math.Abs( Math.Max( shape.JointGapLength, 0 ) );
			double z5 = z6;
			double y4 = y5;
			double z4 = z5 + Math.Abs( shape.JointGapLength ) + safeHeight;

			// make points
			gp_Pnt p0 = new gp_Pnt( 0, y0, z0 );
			gp_Pnt p1 = new gp_Pnt( 0, y1, z1 );
			gp_Pnt p2 = new gp_Pnt( 0, y2, z2 );
			gp_Pnt p3 = new gp_Pnt( 0, y3, z3 );
			gp_Pnt p4 = new gp_Pnt( 0, y4, z4 );
			gp_Pnt p5 = new gp_Pnt( 0, y5, z5 );
			gp_Pnt p6 = new gp_Pnt( 0, y6, z6 );
			List<gp_Pnt> points = new List<gp_Pnt> { p0, p1, p2, p3, p4, p5, p6, p0 };

			// make wire
			BRepBuilderAPI_MakeWire wireMaker = new BRepBuilderAPI_MakeWire();
			for( int i = 0; i < points.Count - 1; i++ ) {
				BRepBuilderAPI_MakeEdge edgeMaker = new BRepBuilderAPI_MakeEdge( points[ i ], points[ i + 1 ] );

				// some sgement would not exist depends on the joint gap
				if( edgeMaker.IsDone() == false ) {
					continue;
				}
				wireMaker.Add( edgeMaker.Edge() );
			}
			if( wireMaker.IsDone() == false ) {
				return null;
			}

			return wireMaker.Wire();
		}

		static TopoDS_Wire MakeYOZBothSideBNWire( BN_BothSideFillet shape, double y, double z, double minZ, double maxZ, double dThickness )
		{
			// calculate points
			double dHalfAngle_Rad = shape.BendingAngle_deg * Math.PI / 180 / 2;
			double dHalfArcLength = shape.FilletRadius * dHalfAngle_Rad;
			double dOverCut = 0;
			if( shape.IsOverCut && z - minZ > dThickness ) {
				dOverCut = z - minZ - dThickness;
			}

			double y0 = y;
			double z0 = z - dOverCut;
			double y1 = y0 - dHalfArcLength;
			double z1 = z0;
			double y2 = y1;
			double z2 = z1 + dOverCut; // equal to z
			double y12 = y0 + dHalfArcLength;
			double z12 = z0;
			double y11 = y12;
			double z11 = z12 + dOverCut; // equal to z

			Geom_Circle circleL = new Geom_Circle( new gp_Ax2( new gp_Pnt( 0, y2, z2 + shape.FilletRadius ), new gp_Dir( 1, 0, 0 ) ), shape.FilletRadius );
			Geom_Circle circleR = new Geom_Circle( new gp_Ax2( new gp_Pnt( 0, y11, z11 + shape.FilletRadius ), new gp_Dir( 1, 0, 0 ) ), shape.FilletRadius );
			Geom_TrimmedCurve trimL = new Geom_TrimmedCurve( circleL, Math.PI, Math.PI + dHalfAngle_Rad, true );
			Geom_TrimmedCurve trimR = new Geom_TrimmedCurve( circleR, Math.PI - dHalfAngle_Rad, Math.PI, true );
			BRepBuilderAPI_MakeEdge edgeCircleL = new BRepBuilderAPI_MakeEdge( trimL );
			BRepBuilderAPI_MakeEdge edgeCircleR = new BRepBuilderAPI_MakeEdge( trimR );
			if( edgeCircleL.IsDone() == false || edgeCircleR.IsDone() == false ) {
				return null;
			}
			gp_Pnt pL = trimL.Value( trimL.LastParameter() );
			gp_Pnt pR = trimR.Value( trimR.FirstParameter() );
			double y3 = pL.Y();
			double z3 = pL.Z();
			double y10 = pR.Y();
			double z10 = pR.Z();

			double centerZ = z3; // should be equal to z9
			double dVHeight = maxZ - Math.Abs( shape.JointGapLength ) - centerZ;
			double dVHalfWidth = dVHeight * Math.Tan( dHalfAngle_Rad );
			double safeHeight = 1;
			double y4 = y3 - dVHalfWidth;
			double z4 = z3 + dVHeight;
			double y5 = y4 - Math.Abs( Math.Min( shape.JointGapLength, 0 ) );
			double z5 = z4;
			double y6 = y5;
			double z6 = z5 + Math.Abs( shape.JointGapLength ) + safeHeight;
			double y9 = y10 + dVHalfWidth;
			double z9 = z10 + dVHeight;
			double y8 = y9 + Math.Abs( Math.Max( shape.JointGapLength, 0 ) );
			double z8 = z9;
			double y7 = y8;
			double z7 = z8 + Math.Abs( shape.JointGapLength ) + safeHeight;

			// make points
			gp_Pnt p0 = new gp_Pnt( 0, y0, z0 );
			gp_Pnt p1 = new gp_Pnt( 0, y1, z1 );
			gp_Pnt p2 = new gp_Pnt( 0, y2, z2 );
			gp_Pnt p3 = new gp_Pnt( 0, y3, z3 );
			gp_Pnt p4 = new gp_Pnt( 0, y4, z4 );
			gp_Pnt p5 = new gp_Pnt( 0, y5, z5 );
			gp_Pnt p6 = new gp_Pnt( 0, y6, z6 );
			gp_Pnt p7 = new gp_Pnt( 0, y7, z7 );
			gp_Pnt p8 = new gp_Pnt( 0, y8, z8 );
			gp_Pnt p9 = new gp_Pnt( 0, y9, z9 );
			gp_Pnt p10 = new gp_Pnt( 0, y10, z10 );
			gp_Pnt p11 = new gp_Pnt( 0, y11, z11 );
			gp_Pnt p12 = new gp_Pnt( 0, y12, z12 );

			// make wire
			BRepBuilderAPI_MakeWire wireMaker = new BRepBuilderAPI_MakeWire();
			BRepBuilderAPI_MakeEdge edge01 = new BRepBuilderAPI_MakeEdge( p0, p1 );
			if( edge01.IsDone() ) {
				wireMaker.Add( edge01.Edge() );
			}
			BRepBuilderAPI_MakeEdge edge12 = new BRepBuilderAPI_MakeEdge( p1, p2 );
			if( edge12.IsDone() ) {
				wireMaker.Add( edge12.Edge() );
			}
			wireMaker.Add( edgeCircleL.Edge() );
			BRepBuilderAPI_MakeEdge edge34 = new BRepBuilderAPI_MakeEdge( p3, p4 );
			if( edge34.IsDone() ) {
				wireMaker.Add( edge34.Edge() );
			}
			BRepBuilderAPI_MakeEdge edge45 = new BRepBuilderAPI_MakeEdge( p4, p5 );
			if( edge45.IsDone() ) {
				wireMaker.Add( edge45.Edge() );
			}
			BRepBuilderAPI_MakeEdge edge56 = new BRepBuilderAPI_MakeEdge( p5, p6 );
			if( edge56.IsDone() ) {
				wireMaker.Add( edge56.Edge() );
			}
			BRepBuilderAPI_MakeEdge edge67 = new BRepBuilderAPI_MakeEdge( p6, p7 );
			if( edge67.IsDone() ) {
				wireMaker.Add( edge67.Edge() );
			}
			BRepBuilderAPI_MakeEdge edge78 = new BRepBuilderAPI_MakeEdge( p7, p8 );
			if( edge78.IsDone() ) {
				wireMaker.Add( edge78.Edge() );
			}
			BRepBuilderAPI_MakeEdge edge89 = new BRepBuilderAPI_MakeEdge( p8, p9 );
			if( edge89.IsDone() ) {
				wireMaker.Add( edge89.Edge() );
			}
			BRepBuilderAPI_MakeEdge edge910 = new BRepBuilderAPI_MakeEdge( p9, p10 );
			if( edge910.IsDone() ) {
				wireMaker.Add( edge910.Edge() );
			}
			wireMaker.Add( edgeCircleR.Edge() );
			BRepBuilderAPI_MakeEdge edge1112 = new BRepBuilderAPI_MakeEdge( p11, p12 );
			if( edge1112.IsDone() ) {
				wireMaker.Add( edge1112.Edge() );
			}
			BRepBuilderAPI_MakeEdge edge120 = new BRepBuilderAPI_MakeEdge( p12, p0 );
			if( edge120.IsDone() ) {
				wireMaker.Add( edge120.Edge() );
			}
			if( wireMaker.IsDone() == false ) {
				return null;
			}
			return wireMaker.Wire();
		}

		static TopoDS_Wire MakeYOZOneSideBNWire( BN_OneSideFillet shape, double y, double z, double minZ, double maxZ, double dThickness )
		{
			// calculate points
			double dArcRadius = maxZ - z;
			double dArcLength = dArcRadius * Math.PI / 2;
			double dOverCut = 0;
			if( shape.IsOverCut && z - minZ > dThickness ) {
				dOverCut = z - minZ - dThickness;
			}
			double y0 = y;
			double z0 = z - dOverCut;
			double y1 = y0;
			double z1 = z0 + dOverCut; // equal to z
			double y2 = y1;
			double z2 = z1 + dArcRadius; // equal to maxZ

			double dTopLength = dArcLength - dArcRadius;
			double y3 = y2 + dTopLength;
			double z3 = z2;

			Geom_Circle circle = new Geom_Circle( new gp_Ax2( new gp_Pnt( 0, y + dArcLength, z + dArcRadius ), new gp_Dir( 1, 0, 0 ) ), dArcRadius );
			Geom_TrimmedCurve trim = new Geom_TrimmedCurve( circle, Math.PI / 2, Math.PI, true );
			BRepBuilderAPI_MakeEdge edgeCircle = new BRepBuilderAPI_MakeEdge( trim );
			if( edgeCircle.IsDone() == false ) {
				return null;
			}

			double y5 = y0 + dArcLength;
			double z5 = z0;
			double y4 = y5;
			double z4 = z5 + dOverCut; // equal to z

			// make points
			gp_Pnt p0 = new gp_Pnt( 0, y0, z0 );
			gp_Pnt p1 = new gp_Pnt( 0, y1, z1 );
			gp_Pnt p2 = new gp_Pnt( 0, y2, z2 );
			gp_Pnt p3 = new gp_Pnt( 0, y3, z3 );
			gp_Pnt p4 = new gp_Pnt( 0, y4, z4 );
			gp_Pnt p5 = new gp_Pnt( 0, y5, z5 );

			// make wire
			BRepBuilderAPI_MakeWire wireMaker = new BRepBuilderAPI_MakeWire();
			BRepBuilderAPI_MakeEdge edge01 = new BRepBuilderAPI_MakeEdge( p0, p1 );
			if( edge01.IsDone() ) {
				wireMaker.Add( edge01.Edge() );
			}
			BRepBuilderAPI_MakeEdge edge12 = new BRepBuilderAPI_MakeEdge( p1, p2 );
			if( edge12.IsDone() ) {
				wireMaker.Add( edge12.Edge() );
			}
			BRepBuilderAPI_MakeEdge edge23 = new BRepBuilderAPI_MakeEdge( p2, p3 );
			if( edge23.IsDone() ) {
				wireMaker.Add( edge23.Edge() );
			}
			wireMaker.Add( edgeCircle.Edge() );
			BRepBuilderAPI_MakeEdge edge45 = new BRepBuilderAPI_MakeEdge( p4, p5 );
			if( edge45.IsDone() ) {
				wireMaker.Add( edge45.Edge() );
			}
			BRepBuilderAPI_MakeEdge edge50 = new BRepBuilderAPI_MakeEdge( p5, p0 );
			if( edge50.IsDone() ) {
				wireMaker.Add( edge50.Edge() );
			}
			if( wireMaker.IsDone() == false ) {
				return null;
			}
			if( shape.Side == NotchSide.Left ) {
				return wireMaker.Wire();
			}

			// flip the wire by rotate 180 degree around Z axis and center <0, y, 0>
			gp_Trsf trsf = new gp_Trsf();
			trsf.SetRotation( new gp_Ax1( new gp_Pnt( 0, y, 0 ), new gp_Dir( 0, 0, 1 ) ), Math.PI );
			BRepBuilderAPI_Transform wireTrsf = new BRepBuilderAPI_Transform( wireMaker.Wire(), trsf );
			if( wireTrsf.IsDone() == false ) {
				return null;
			}
			return TopoDS.ToWire( wireTrsf.Shape() );
		}

		// get bounding box
		// u'll meet some bug if u use double.MaxValue or double.MinValue directly
		const double MAX_VALUE = 999999;

		static double GetBoundaryValue( TopoDS_Shape Shape, BoundaryType type )
		{
			TopoDS_Face boundaryTestFace = GetBoundingTestFace( type );
			if( boundaryTestFace == null ) {
				return 0;
			}

			// TODO: the distance API is not stable, this might from OCC, ref: AUTO-12540
			BRepExtrema_DistShapeShape dss = new BRepExtrema_DistShapeShape( boundaryTestFace, Shape );
			dss.Perform();
			double dis = dss.Value();

			if( type == BoundaryType.MinX || type == BoundaryType.MinY || type == BoundaryType.MinZ ) {
				return -MAX_VALUE + dis;
			}
			else {
				return MAX_VALUE - dis;
			}
		}

		static TopoDS_Face GetBoundingTestFace( BoundaryType type )
		{
			gp_Pnt center;
			gp_Dir dir;
			if( type == BoundaryType.MinX ) {
				center = new gp_Pnt( -MAX_VALUE, 0, 0 );
				dir = new gp_Dir( 1, 0, 0 );
			}
			else if( type == BoundaryType.MaxX ) {
				center = new gp_Pnt( MAX_VALUE, 0, 0 );
				dir = new gp_Dir( -1, 0, 0 );
			}
			else if( type == BoundaryType.MinY ) {
				center = new gp_Pnt( 0, -MAX_VALUE, 0 );
				dir = new gp_Dir( 0, 1, 0 );
			}
			else if( type == BoundaryType.MaxY ) {
				center = new gp_Pnt( 0, MAX_VALUE, 0 );
				dir = new gp_Dir( 0, -1, 0 );
			}
			else if( type == BoundaryType.MinZ ) {
				center = new gp_Pnt( 0, 0, -MAX_VALUE );
				dir = new gp_Dir( 0, 0, 1 );
			}
			else if( type == BoundaryType.MaxZ ) {
				center = new gp_Pnt( 0, 0, MAX_VALUE );
				dir = new gp_Dir( 0, 0, -1 );
			}
			else {
				return null;
			}
			gp_Pln plane = new gp_Pln( center, dir );
			BRepBuilderAPI_MakeFace makeFace = new BRepBuilderAPI_MakeFace( plane );
			if( makeFace.IsDone() == false ) {
				return null;
			}
			return makeFace.Face();
		}
	}
}
