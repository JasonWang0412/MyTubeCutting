using OCC.BRepBuilderAPI;
using OCC.BRepExtrema;
using OCC.BRepPrimAPI;
using OCC.Geom;
using OCC.gp;
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
			double y, double z, double minZ, double maxZ, double angleB_deg )
		{
			// data protection
			if( shape == null || shape.IsValid() == false ) {
				return null;
			}

			TopoDS_Wire baseWire;
			if( shape.Type == BendingNotch_Type.VShape ) {
				baseWire = MakeYOZVShapeWire( (BN_VShape)shape, y, z, minZ, maxZ );
			}
			else {
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

		static TopoDS_Wire MakeYOZVShapeWire( BN_VShape shape, double y, double z, double minZ, double maxZ )
		{
			/*
			    3_______4
			   _|      _|
			  1\ 2   6/ 5
			    \    /
			     \  /
			      \/
			       0
			*/

			// calculate points
			// height from 0 to 1, joint gap is the distance between 1 and 2
			double dVHeight = maxZ - Math.Abs( shape.JointGapLength ) - z;

			// width from 1 to 6
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
			double y4 = y3 + 2 * dVHalfWidth + Math.Abs( shape.JointGapLength );
			double z4 = z3;
			double y5 = y4;
			double z5 = z4 - Math.Abs( shape.JointGapLength ) - safeHeight;
			double y6 = y5 - Math.Abs( Math.Max( shape.JointGapLength, 0 ) );
			double z6 = z5;

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
