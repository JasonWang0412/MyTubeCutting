using OCC.BRep;
using OCC.BRepBuilderAPI;
using OCC.BRepExtrema;
using OCC.BRepPrimAPI;
using OCC.gp;
using OCC.TopoDS;
using System;
using System.Collections.Generic;

namespace MyUtility.MyOCC
{
	public enum PrismDir
	{
		Positive,
		Negative,
		Both,
	}

	public class OCCHelper
	{
		// make prism
		// a lot of bug happens when using Inf prism, not recommended, ref: AUTO-12540
		public static TopoDS_Shape MakeConcretePrismByWire( TopoDS_Wire baseWire, gp_Dir dir, double dSize, PrismDir prismDir )
		{
			// data protection
			if( baseWire == null || dir == null || dSize <= 0 ) {
				return null;
			}

			// make face
			BRepBuilderAPI_MakeFace branchFaceMaker = new BRepBuilderAPI_MakeFace( baseWire );
			if( branchFaceMaker.IsDone() == false ) {
				return null;
			}
			TopoDS_Face branchFace = branchFaceMaker.Face();

			// translate and scale direction
			gp_Vec prismVec = new gp_Vec( dir );
			if( prismDir == PrismDir.Both ) {

				// translate center
				gp_Trsf trsf = new gp_Trsf();
				gp_Vec transVec = new gp_Vec( dir );
				transVec.Multiply( -dSize );
				trsf.SetTranslation( transVec );
				BRepBuilderAPI_Transform transform = new BRepBuilderAPI_Transform( branchFace, trsf, true );
				if( transform.IsDone() == false ) {
					return null;
				}
				branchFace = TopoDS.ToFace( transform.Shape() );
				prismVec.Multiply( dSize * 2 );
			}
			else if( prismDir == PrismDir.Negative ) {
				prismVec.Multiply( -dSize );
			}
			else {
				prismVec.Multiply( dSize );
			}

			// make prism
			BRepPrimAPI_MakePrism branchTubeMaker = new BRepPrimAPI_MakePrism( branchFace, prismVec );
			if( branchTubeMaker.IsDone() == false ) {
				return null;
			}
			return branchTubeMaker.Shape();
		}

		// make compound
		public static TopoDS_Shape MakeCompound( List<TopoDS_Shape> shapeList )
		{
			// data protection
			if( shapeList == null || shapeList.Count == 0 ) {
				return null;
			}

			try {
				// create compound
				TopoDS_Compound compound = new TopoDS_Compound();
				TopoDS_Shape compoundShape = compound;
				BRep_Builder builder = new BRep_Builder();
				builder.MakeCompound( ref compound );

				// add all shapes to compound
				foreach( TopoDS_Shape oneShape in shapeList ) {
					if( oneShape == null ) {
						continue;
					}
					builder.Add( ref compoundShape, oneShape );
				}
				if( compound.elementsAsList.Count == 0 ) {
					return null;
				}
				return compound;
			}
			catch( Exception e ) {
				return null;
			}
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

		// get bounding box
		// u'll meet some bug if u use double.MaxValue or double.MinValue directly
		const double MAX_VALUE = 999999;

		static double GetBoundaryValue( TopoDS_Shape Shape, BoundaryType type )
		{
			TopoDS_Face boundaryTestFace = GetBoundingTestFace( type );
			if( boundaryTestFace == null ) {
				return 0;
			}

			// the distance API is not stable, this might from OCC, ref: AUTO-12540
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
