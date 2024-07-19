using MyUtility.General;
using MyUtility.MyOCC;
using OCC.Bnd;
using OCC.BRep;
using OCC.BRepAdaptor;
using OCC.BRepBndLib;
using OCC.BRepBuilderAPI;
using OCC.BRepExtrema;
using OCC.BRepGProp;
using OCC.BRepOffsetAPI;
using OCC.BRepTools;
using OCC.Geom;
using OCC.GeomAPI;
using OCC.gp;
using OCC.GProp;
using OCC.IGESControl;
using OCC.ShapeAnalysis;
using OCC.TopAbs;
using OCC.TopExp;
using OCC.TopoDS;
using OCC.TopTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TubeCuttingUI
{
	// public functions and variables
	public partial class ThreeDimensionalFileReader
	{
		public void ReadTubeInformation( TopoDS_Shape oneShape, out TopoDS_Shape oneShapeAfterOffSet, out double dCrossSectionRotateAngle )
		{
			try {
				// sew original shape to avoid edge not common
				oneShape = OCCHelper.SewShape( oneShape );
				BoundingBox boundingBox = OCCHelper.GetBoundingBox( oneShape );

				// get all faces from shape
				List<TopoDS_Shape> shapeList = GetFaceListFromShape( oneShape );

				// get all outer, inner and other faces
				FilterShape( shapeList, boundingBox, out List<TopoDS_Shape> faceShapeOuterList, out List<TopoDS_Shape> faceShapeInnerList, out List<TopoDS_Shape> faceShapeOtherList );

				// get all edges on the inner and outer
				GetInnerAndOuterEdgeList( faceShapeOtherList, faceShapeOuterList, faceShapeInnerList, out List<List<TopoDS_Edge>> edgeOuterList, out List<List<TopoDS_Edge>> edgeInnerList, out _ );

				// Recording the index of not closed loop in the outer edge list
				List<int> nEdgeNotCloseList = new List<int>();
				FindEngravingLine( oneShape, ref edgeOuterList, ref nEdgeNotCloseList );

				FindIndexOfWiresOnEdge( edgeOuterList, out int nHeadCutOffEdgeListIndex, out int nTailCutOffEdgeListIndex, out List<BoundingBox> shapeBoxList );
				m_TubeData = GetMotherTubeData( edgeOuterList[ nHeadCutOffEdgeListIndex ], boundingBox.YLength, out dCrossSectionRotateAngle );

				// rotate edge
				RotateShapeByCrossection( ref edgeOuterList, dCrossSectionRotateAngle );
				RotateShapeByCrossection( ref edgeInnerList, dCrossSectionRotateAngle );

				m_TubeData.YMaxPosition = boundingBox.Ymax;
				m_TubeData.YMinPosition = boundingBox.Ymin;
				List<int> nCutOffEdgeList = FindAllCutOffWiresOnEdge( edgeOuterList, nHeadCutOffEdgeListIndex, nTailCutOffEdgeListIndex, shapeBoxList, m_TubeData.Tube );

				// Get the tube thickness by inner and outer face
				bool isGetTubeThickess = GetTubeThickess( faceShapeOuterList, faceShapeInnerList, out double dThickness );
				if( isGetTubeThickess ) {
					m_TubeData.Tube.Thickness = dThickness;
				}
			}
			catch {
				m_isReadSuccess = false;
				oneShapeAfterOffSet = oneShape;
				dCrossSectionRotateAngle = 0;
				return;
			}

			m_isReadSuccess = true;
		}

		public TopoDS_Shape RotateTopoDSShape( TopoDS_Shape topoDS_Shape, AxisDirection tubeDirection )
		{
			TopoDS_Shape newTopoDSShape = new TopoDS_Shape();

			// rotate info
			gp_Trsf transferVector = new gp_Trsf();
			gp_Pnt OriginPoint = new gp_Pnt( 0, 0, 0 );
			gp_Dir RotateDirection;
			gp_Ax1 RotateVetor;
			double RotateAngleDergee;

			switch( tubeDirection ) {
				case AxisDirection.XAxis:
					RotateDirection = new gp_Dir( 0, 0, 1 );
					RotateVetor = new gp_Ax1( OriginPoint, RotateDirection );
					RotateAngleDergee = Math_Utility.ToRadian( 90 );

					// This step only set done the transfervector, but haven't rotate path yet
					transferVector.SetRotation( RotateVetor, RotateAngleDergee );

					// rotate path
					newTopoDSShape = RotateTopoDSShape( topoDS_Shape, transferVector );
					break;
				case AxisDirection.ZAxis:
					RotateDirection = new gp_Dir( 1, 0, 0 );
					RotateVetor = new gp_Ax1( OriginPoint, RotateDirection );
					RotateAngleDergee = Math_Utility.ToRadian( -90 );

					// This step only set done the transfervector, but haven't rotate path yet
					transferVector.SetRotation( RotateVetor, RotateAngleDergee );

					// rotate path
					newTopoDSShape = RotateTopoDSShape( topoDS_Shape, transferVector );
					break;
				case AxisDirection.YAxis:
				default:
					newTopoDSShape = topoDS_Shape;
					break;
			}
			return newTopoDSShape;
		}
	}

	// private functions and variables
	public partial class ThreeDimensionalFileReader
	{
		bool m_isReadSuccess = false;

		List<TopoDS_Shape> GetFaceListFromShape( TopoDS_Shape shape )
		{
			List<TopoDS_Shape> faceList = new List<TopoDS_Shape>();
			TopExp_Explorer faceExplorer = new TopExp_Explorer( shape, TopAbs_ShapeEnum.TopAbs_FACE );
			while( faceExplorer.More() ) {
				faceList.Add( faceExplorer.Current() );
				faceExplorer.Next();
			}
			return faceList;
		}

		void FilterShape( List<TopoDS_Shape> shapeList, BoundingBox boundingBoxParameter, out List<TopoDS_Shape> faceShapeOuterList, out List<TopoDS_Shape> faceShapeInnerList, out List<TopoDS_Shape> faceShapeOtherList )
		{
			List<gp_Vec> cutPlaneNormalVectorList = CreateCutPlaneNormalVector( boundingBoxParameter );

			faceShapeOuterList = new List<TopoDS_Shape>();
			faceShapeInnerList = new List<TopoDS_Shape>();
			faceShapeOtherList = new List<TopoDS_Shape>();

			for( int i = 0; i < cutPlaneNormalVectorList.Count; i++ ) {
				TopoDS_Face cutPlane = MakePlaneFace( cutPlaneNormalVectorList[ i ], new gp_Pnt( 0, 0, 0 ) );
				List<TopoDS_Shape> tempOuterFaceShapeList = FindOuterOrInnerFaceByCutPlane( shapeList, cutPlane, cutPlaneNormalVectorList[ i ], true );
				List<TopoDS_Shape> tempInnerFaceShapeList = FindOuterOrInnerFaceByCutPlane( shapeList, cutPlane, cutPlaneNormalVectorList[ i ], false );

				// Collect InnerFaceShapeList and OuterFaceShapeList
				faceShapeOuterList.AddRange( tempOuterFaceShapeList );
				faceShapeInnerList.AddRange( tempInnerFaceShapeList );
			}

			// remove repeat face
			faceShapeOuterList = faceShapeOuterList.Distinct().ToList();
			faceShapeInnerList = faceShapeInnerList.Distinct().ToList();

			for( int i = 0; i < shapeList.Count; i++ ) {

				if( faceShapeOuterList.Contains( shapeList[ i ] ) ) {
					continue;
				}

				if( faceShapeInnerList.Contains( shapeList[ i ] ) ) {
					continue;
				}

				faceShapeOtherList.Add( shapeList[ i ] );
			}
		}

		List<gp_Vec> CreateCutPlaneNormalVector( BoundingBox boundingBox )
		{
			List<gp_Vec> ResultList = new List<gp_Vec>();

			double vecX = boundingBox.MaxX - boundingBox.MinX;
			double vecZ = boundingBox.MaxZ - boundingBox.MinZ;

			ResultList.Add( new gp_Vec( 0, 0, vecX ) );
			ResultList.Add( new gp_Vec( vecZ, 0, vecX ) );
			ResultList.Add( new gp_Vec( vecZ, 0, 0 ) );
			ResultList.Add( new gp_Vec( vecZ, 0, -vecX ) );

			return ResultList;
		}

		TopoDS_Face MakePlaneFace( gp_Vec normalVec, gp_Pnt pointOnPlane )
		{
			gp_Dir unitNormalVector = new gp_Dir( normalVec );
			gp_Pln aPlane = new gp_Pln( pointOnPlane, unitNormalVector );
			BRepBuilderAPI_MakeFace FaceMaker = new BRepBuilderAPI_MakeFace( aPlane );
			return FaceMaker.Face();
		}

		List<TopoDS_Shape> FindOuterOrInnerFaceByCutPlane( List<TopoDS_Shape> allFaceList, TopoDS_Face cutPlane, gp_Vec cutPlaneNormalVector, bool isFindOuter )
		{
			List<Geom_Curve> intersectLineList = new List<Geom_Curve>();
			List<TopoDS_Shape> possibleTubeWallFaceList = new List<TopoDS_Shape>();
			List<TopoDS_Shape> tubeWallFaceList = new List<TopoDS_Shape>();

			for( int i = 0; i < allFaceList.Count; i++ ) {
				TopoDS_Face oneFace = TopoDS.ToFace( allFaceList[ i ] );

				// create intersector list for all shapes
				GeomAPI_IntSS intersector = GetIntSS( oneFace, cutPlane );
				if( intersector == null ) {
					continue;
				}

				// check if the face is possible tube wall
				if( CheckPossibleTubeWall( intersector, cutPlaneNormalVector, oneFace ) == false ) {
					continue;
				}
				intersectLineList.Add( intersector.Line( 1 ) );
				possibleTubeWallFaceList.Add( allFaceList[ i ] );
			}

			if( intersectLineList.Count == 0 ) {
				return tubeWallFaceList;
			}

			// keep intersection line that is furthest/nearest from y axis
			List<int> OuterShapeIndexList = GetExtremaIntSSDisIndex( intersectLineList, isFindOuter );

			foreach( int Index in OuterShapeIndexList ) {
				tubeWallFaceList.Add( possibleTubeWallFaceList[ Index ] );
			}

			return tubeWallFaceList;
		}

		GeomAPI_IntSS GetIntSS( TopoDS_Face theFace, TopoDS_Face cutPlane )
		{
			Geom_Surface theFaceSurface = BRep_Tool.Surface( theFace );
			Geom_Surface cutPlaneSurface = BRep_Tool.Surface( cutPlane );
			GeomAPI_IntSS intersector;

			try {
				double Umin = 0;
				double Umax = 0;
				double Vmin = 0;
				double Vmax = 0;
				BRepTools.UVBounds( theFace, ref Umin, ref Umax, ref Vmin, ref Vmax );
				Geom_RectangularTrimmedSurface trimmedFaceSurface = new Geom_RectangularTrimmedSurface( theFaceSurface, Umin, Umax, Vmin, Vmax );

				BRepTools.UVBounds( cutPlane, ref Umin, ref Umax, ref Vmin, ref Vmax );
				Geom_RectangularTrimmedSurface trimmedCutPlaneSurface = new Geom_RectangularTrimmedSurface( cutPlaneSurface, Umin, Umax, Vmin, Vmax );

				intersector = new GeomAPI_IntSS( trimmedFaceSurface, trimmedCutPlaneSurface, OCCHelper.ERROR_VALUE );
			}
			catch {
				intersector = null;
			}
			return intersector;
		}

		bool CheckPossibleTubeWall( GeomAPI_IntSS intersector, gp_Vec cutPlaneNormalVector, TopoDS_Face theFace )
		{
			if( intersector == null ) {
				return false;
			}

			// keep shapes that have intersection with cut plane
			if( intersector.NbLines() == 0 ) {
				return false;
			}

			// check the intersection property
			Geom_Curve IntersectLine = intersector.Line( 1 );
			gp_Pnt startPoint = IntersectLine.Value( IntersectLine.FirstParameter() );
			gp_Pnt endPoint = IntersectLine.Value( IntersectLine.LastParameter() );
			gp_Vec intersectVec = new gp_Vec( startPoint, endPoint );
			if( OCCHelper.IsZeroVector( intersectVec ) ) {
				return false;
			}

			// keep intersection line with direction vector contains only y direction
			if( intersectVec.IsParallel( new gp_Vec( 0, 1, 0 ), OCCHelper.ERROR_VALUE ) == false ) {
				return false;
			}

			// if the face is intersected with the cut palne and also parallel to the cut plane, it is not what we want
			if( IsFaceParallelToPlane( theFace, cutPlaneNormalVector ) ) {
				return false;
			}
			return true;
		}

		// the method will kick out all face satisfied the criteria
		// including the face that is not parallel to the cut plane, but not general case
		bool IsFaceParallelToPlane( TopoDS_Face theFace, gp_Vec cutPlaneNormalVector )
		{
			BRepAdaptor_Surface ShapeFaceGeomFace = new BRepAdaptor_Surface( theFace );

			double FirstUParam = ShapeFaceGeomFace.FirstUParameter();
			double LastUParam = ShapeFaceGeomFace.LastUParameter();
			double FirstVParam = ShapeFaceGeomFace.FirstVParameter();
			double LastVParam = ShapeFaceGeomFace.LastVParameter();

			// make a 5x5 grid on Shape. Get normal vector of each grid point and compare to CutPlaneNormalVector.
			int nSegments = 4;

			double CurrentUParam = FirstUParam;
			double CurrentVParam = FirstVParam;
			double UParamIncrement = ( LastUParam - FirstUParam ) / nSegments;
			double VParamIncrement = ( LastVParam - FirstVParam ) / nSegments;

			for( int i = 0; i <= nSegments; i++ ) {
				for( int j = 0; j <= nSegments; j++ ) {
					gp_Pnt CurrentPoint = new gp_Pnt();
					gp_Vec CurrentUVector = new gp_Vec();
					gp_Vec CurrentVVector = new gp_Vec();

					ShapeFaceGeomFace.D1( CurrentUParam, CurrentVParam, ref CurrentPoint, ref CurrentUVector, ref CurrentVVector );
					gp_Vec NormalDirection = CurrentUVector ^ CurrentVVector;
					if( NormalDirection.IsParallel( cutPlaneNormalVector, OCCHelper.ERROR_VALUE ) == false ) {
						return false;
					}
					CurrentVParam += VParamIncrement;
				}
				CurrentUParam += UParamIncrement;
			}
			return true;
		}

		List<int> GetExtremaIntSSDisIndex( List<Geom_Curve> IntersectLineList, bool isFindOuter = true )
		{
			gp_Pnt StartPoint = new gp_Pnt( 0, 0, 0 );
			gp_Pnt EndPoint = new gp_Pnt( 0, 10000, 0 );
			BRepBuilderAPI_MakeEdge edgeMakerYAxis = new BRepBuilderAPI_MakeEdge( StartPoint, EndPoint );
			TopoDS_Shape YAxisShape = edgeMakerYAxis.Shape();

			double extremaDistance = isFindOuter ? 0 : double.MaxValue;
			int extremaIntSSDisIndex = 0;
			List<int> extremaIntSSDisIndexList = new List<int>();

			for( int i = 0; i < IntersectLineList.Count; i++ ) {
				BRepBuilderAPI_MakeEdge edgeMakerIntSS = new BRepBuilderAPI_MakeEdge( IntersectLineList[ i ] );
				TopoDS_Shape IntSSShape = edgeMakerIntSS.Shape();
				BRepExtrema_DistShapeShape DistanceCalculator = new BRepExtrema_DistShapeShape( IntSSShape, YAxisShape );
				DistanceCalculator.Perform();
				double CalculatedDistance = DistanceCalculator.Value();

				if( isFindOuter ) {
					if( CalculatedDistance > extremaDistance ) {
						extremaDistance = CalculatedDistance;
						extremaIntSSDisIndex = i;
					}
				}
				else {
					if( CalculatedDistance < extremaDistance ) {
						extremaDistance = CalculatedDistance;
						extremaIntSSDisIndex = i;
					}
				}
			}
			extremaIntSSDisIndexList.Add( extremaIntSSDisIndex );

			// find same distance IntSS
			for( int i = 0; i < IntersectLineList.Count; i++ ) {
				if( i == extremaIntSSDisIndex ) {
					continue;
				}

				BRepBuilderAPI_MakeEdge edgeMakerIntSS = new BRepBuilderAPI_MakeEdge( IntersectLineList[ i ] );
				TopoDS_Shape IntSSShape = edgeMakerIntSS.Shape();
				BRepExtrema_DistShapeShape DistanceCalculator = new BRepExtrema_DistShapeShape( IntSSShape, YAxisShape );
				DistanceCalculator.Perform();
				double CalculatedDistance = DistanceCalculator.Value();

				if( MathHelper.IsSameValue( CalculatedDistance, extremaDistance ) ) {
					extremaIntSSDisIndexList.Add( i );
				}
			}
			return extremaIntSSDisIndexList;
		}

		string CheckAndResetFilePath( string szOriginalPath )
		{
			bool isContainChinese = IsContainChineseCharacter( szOriginalPath );
			if( isContainChinese == false ) {
				return szOriginalPath;
			}

			string szTempDirectoryPath = Application.StartupPath + "\\Temp";
			if( Directory.Exists( szTempDirectoryPath ) == false ) {
				Directory.CreateDirectory( szTempDirectoryPath );
			}

			string szNewFilePath = szTempDirectoryPath + "\\temp" + Path.GetExtension( szOriginalPath );
			File.Copy( szOriginalPath, szNewFilePath, true );
			return szNewFilePath;
		}

		bool IsContainChineseCharacter( string szSource )
		{
			// chinese characters of the Unicode encoding between u4e00 and u9fa5
			Regex re = new Regex( @"[\u4e00-\u9fa5]+" );
			return re.IsMatch( szSource );
		}

		BRepBuilderAPI_Sewing SewFace( List<TopoDS_Shape> ShapeList )
		{
			int NumOfShape = ShapeList.Count;

			double SewTolerance = 0.0001;
			BRepBuilderAPI_Sewing sewer = new BRepBuilderAPI_Sewing( SewTolerance );

			for( int i = 0; i < NumOfShape; i++ ) {
				sewer.Add( ShapeList[ i ] );
			}
			sewer.Perform();

			return sewer;
		}

		BoundingBox GetBoundingBox( List<TopoDS_Shape> ShapeList )
		{
			TopoDS_Shape ShapeCompound = OCCTranslator.CombineTopoShape( ShapeList );
			return GetBoundingBox( ShapeCompound );
		}

		BoundingBox GetBoundingBox( TopoDS_Shape topoDS_Shape )
		{
			BoundingBox RoughBoundingBox = GetRoughBox( topoDS_Shape );
			BoundingBox FinishBoundingBox = GetFinishBox( topoDS_Shape, RoughBoundingBox );
			return FinishBoundingBox;
		}

		BoundingBox GetFinishBox( TopoDS_Shape ShapeCompound, BoundingBox RoughBoundingBox )
		{
			double MaxValue = GetMaxBoxValue( RoughBoundingBox );

			double Xmax = Math.Round( RefineBoxBoundary( RoughBoundingBox.Xmax, AxisDirection.XAxis, true, MaxValue, ShapeCompound ), ValueConstrain.DECIMAL_Place );
			double Xmin = Math.Round( RefineBoxBoundary( RoughBoundingBox.Xmin, AxisDirection.XAxis, false, MaxValue, ShapeCompound ), ValueConstrain.DECIMAL_Place );
			double Ymax = Math.Round( RefineBoxBoundary( RoughBoundingBox.Ymax, AxisDirection.YAxis, true, MaxValue, ShapeCompound ), ValueConstrain.DECIMAL_Place );
			double Ymin = Math.Round( RefineBoxBoundary( RoughBoundingBox.Ymin, AxisDirection.YAxis, false, MaxValue, ShapeCompound ), ValueConstrain.DECIMAL_Place );
			double Zmax = Math.Round( RefineBoxBoundary( RoughBoundingBox.Zmax, AxisDirection.ZAxis, true, MaxValue, ShapeCompound ), ValueConstrain.DECIMAL_Place );
			double Zmin = Math.Round( RefineBoxBoundary( RoughBoundingBox.Zmin, AxisDirection.ZAxis, false, MaxValue, ShapeCompound ), ValueConstrain.DECIMAL_Place );

			BoundingBox BoundingBox = new BoundingBox( Xmax, Xmin, Ymax, Ymin, Zmax, Zmin );
			return BoundingBox;
		}

		double GetMaxBoxValue( BoundingBox BoundingBox )
		{
			double MaxValue = new List<double>() {
					Math.Abs( BoundingBox.Xmax ),
					Math.Abs( BoundingBox.Xmin ),
					Math.Abs( BoundingBox.Ymax ),
					Math.Abs( BoundingBox.Ymin ),
					Math.Abs( BoundingBox.Zmax ),
					Math.Abs(BoundingBox.Zmin ) }.Max();

			return MaxValue;
		}

		double RefineBoxBoundary( double BoxBoundary, AxisDirection NormalDirection, bool isMaxBound, double MaxBoxValue, TopoDS_Shape Shape )
		{
			TopoDS_Shape BoxBoundaryShape = GetXYZFaceWithOffset( BoxBoundary, NormalDirection, MaxBoxValue );
			BRepExtrema_DistShapeShape DistCalculator = new BRepExtrema_DistShapeShape( BoxBoundaryShape, Shape );
			DistCalculator.Perform();
			double Difference = DistCalculator.Value();

			if( isMaxBound ) {
				return BoxBoundary - Difference;
			}
			else {
				return BoxBoundary + Difference;
			}
		}

		// create face with boundary. Only used in refining bounding box
		TopoDS_Face GetXYZFaceWithOffset( double Offset, AxisDirection NormalDirection, double length )
		{
			List<TopoDS_Edge> ResultList = new List<TopoDS_Edge>();
			gp_Pnt BoundPoint1, BoundPoint2, BoundPoint3, BoundPoint4;

			switch( NormalDirection ) {
				case AxisDirection.XAxis:
					BoundPoint1 = new gp_Pnt( Offset, length, length );
					BoundPoint2 = new gp_Pnt( Offset, -length, length );
					BoundPoint3 = new gp_Pnt( Offset, -length, -length );
					BoundPoint4 = new gp_Pnt( Offset, length, -length );
					break;
				case AxisDirection.ZAxis:
					BoundPoint1 = new gp_Pnt( length, length, Offset );
					BoundPoint2 = new gp_Pnt( -length, length, Offset );
					BoundPoint3 = new gp_Pnt( -length, -length, Offset );
					BoundPoint4 = new gp_Pnt( length, -length, Offset );
					break;
				case AxisDirection.YAxis:
				default:
					BoundPoint1 = new gp_Pnt( length, Offset, length );
					BoundPoint2 = new gp_Pnt( -length, Offset, length );
					BoundPoint3 = new gp_Pnt( -length, Offset, -length );
					BoundPoint4 = new gp_Pnt( length, Offset, -length );
					break;
			}

			TopoDS_Edge BoundEdge1 = TopoDS.ToEdge( OCCTranslator.MakeEdge( BoundPoint1, BoundPoint2 ) );
			TopoDS_Edge BoundEdge2 = TopoDS.ToEdge( OCCTranslator.MakeEdge( BoundPoint2, BoundPoint3 ) );
			TopoDS_Edge BoundEdge3 = TopoDS.ToEdge( OCCTranslator.MakeEdge( BoundPoint3, BoundPoint4 ) );
			TopoDS_Edge BoundEdge4 = TopoDS.ToEdge( OCCTranslator.MakeEdge( BoundPoint4, BoundPoint1 ) );

			BRepBuilderAPI_MakeWire WireMaker = new BRepBuilderAPI_MakeWire();

			WireMaker.Add( BoundEdge1 );
			WireMaker.Add( BoundEdge2 );
			WireMaker.Add( BoundEdge3 );
			WireMaker.Add( BoundEdge4 );

			TopoDS_Wire BoundWire = WireMaker.Wire();

			BRepBuilderAPI_MakeFace FaceMaker = new BRepBuilderAPI_MakeFace( BoundWire );

			return FaceMaker.Face();
		}

		BoundingBox GetRoughBox( TopoDS_Shape Shape )
		{
			double Xmin = 0, Ymin = 0, Zmin = 0, Xmax = 0, Ymax = 0, Zmax = 0;
			Bnd_Box box = new Bnd_Box();
			BRepBndLib.Add( Shape, ref box );

			box.Get( ref Xmin, ref Ymin, ref Zmin, ref Xmax, ref Ymax, ref Zmax );

			BoundingBox BoundingBox = new BoundingBox( Xmax, Xmin, Ymax, Ymin, Zmax, Zmin );
			return BoundingBox;
		}

		List<TopoDS_Shape> GetShape( IGESControl_Reader Reader )
		{
			Reader.TransferRoots();

			int NumOfShape = Reader.NbShapes();
			List<TopoDS_Shape> ShapeList = new List<TopoDS_Shape>();
			for( int i = 1; i <= NumOfShape; i++ ) {
				ShapeList.Add( Reader.Shape( i ) );
			}
			return ShapeList;
		}



		List<List<TopoDS_Edge>> SeperateEdgesOfWires( BRepBuilderAPI_Sewing sewer )
		{
			List<List<TopoDS_Edge>> EdgeList = new List<List<TopoDS_Edge>>();
			int nNumOfSegments = sewer.NbFreeEdges();

			EdgeList.Add( new List<TopoDS_Edge>() );

			// get total edges
			for( int i = 1; i <= nNumOfSegments; i++ ) {
				TopoDS_Shape oneTopoShape = sewer.FreeEdge( i );
				TopoDS_Edge oneTopoEdge = TopoDS.ToEdge( oneTopoShape );
				EdgeList[ 0 ].Add( oneTopoEdge );
			}

			bool bCanFitInWire = false;
			int nLastIndex = 0;

			// add edges in same wire to other list
			for( int i = 0; i < nNumOfSegments; i++ ) {

				if( bCanFitInWire == false ) {
					EdgeList.Add( new List<TopoDS_Edge>() );
					EdgeList[ nLastIndex + 1 ].Add( EdgeList[ 0 ][ 0 ] );
					EdgeList[ 0 ].RemoveAt( 0 );
					nLastIndex = EdgeList.Count - 1;
				}

				for( int j = 0; j < EdgeList[ 0 ].Count; j++ ) {
					bCanFitInWire = CheckEdgeCanFitInWire( EdgeList[ nLastIndex ], EdgeList[ 0 ][ j ] );

					if( bCanFitInWire ) {
						EdgeList[ 0 ].RemoveAt( j );
						j--;
						break;
					}
				}
				if( EdgeList[ 0 ].Count == 0 ) {
					break;
				}
			}
			EdgeList.RemoveAt( 0 );

			return EdgeList;
		}

		bool CheckEdgeCanFitInWire( List<TopoDS_Edge> EdgeList, TopoDS_Edge TargetEdge )
		{
			int nLastIndex = EdgeList.Count - 1;

			// get start end vertex of target edge
			TopoDS_Vertex TargetEdgeStartVertex = new TopoDS_Vertex();
			TopoDS_Vertex TargetEdgeEndVertex = new TopoDS_Vertex();
			ShapeAnalysis.FindBounds( TargetEdge, ref TargetEdgeStartVertex, ref TargetEdgeEndVertex );

			// get start end vertex of start edge
			TopoDS_Edge FirstEdge = EdgeList[ 0 ];
			TopoDS_Vertex FirstEdgeStartVertex = new TopoDS_Vertex();
			TopoDS_Vertex FirstEdgeEndVertex = new TopoDS_Vertex();
			ShapeAnalysis.FindBounds( FirstEdge, ref FirstEdgeStartVertex, ref FirstEdgeEndVertex );

			// get start end vertex of end edge
			TopoDS_Edge LastEdge = EdgeList[ nLastIndex ];
			TopoDS_Vertex LastEdgeStartVertex = new TopoDS_Vertex();
			TopoDS_Vertex LastEdgeEndVertex = new TopoDS_Vertex();
			ShapeAnalysis.FindBounds( LastEdge, ref LastEdgeStartVertex, ref LastEdgeEndVertex );

			if( FirstEdgeStartVertex.IsSame( TargetEdgeEndVertex ) ) {
				EdgeList.Insert( 0, TargetEdge );
				return true;
			}

			if( LastEdgeEndVertex.IsSame( TargetEdgeStartVertex ) ) {
				EdgeList.Add( TargetEdge );
				return true;
			}

			// change the direction of edge for wrong direction
			if( FirstEdgeStartVertex.IsSame( TargetEdgeStartVertex ) ) {
				TargetEdge.Reverse();
				EdgeList.Insert( 0, TargetEdge );
				return true;
			}

			if( LastEdgeEndVertex.IsSame( TargetEdgeEndVertex ) ) {
				TargetEdge.Reverse();
				EdgeList.Add( TargetEdge );
				return true;
			}

			return false;
		}

		void ArrangeTubeDirection( ref List<TopoDS_Shape> ShapeList, ref BoundingBox boundingBox )
		{
			if( m_Direction == AxisDirection.YAxis ) {
				return;
			}

			ShapeList = RotateTopoDSShapeList( ShapeList, m_Direction );

			// rotate 90 along Z axis if the longest axis is X
			if( m_Direction == AxisDirection.XAxis ) {
				boundingBox.RotateBoxBy90Degree( AxisDirection.ZAxis, true );
			}

			// rotate -90 along X axis if the longest axis is Z
			if( m_Direction == AxisDirection.ZAxis ) {
				boundingBox.RotateBoxBy90Degree( AxisDirection.XAxis, false );
			}
		}

		void ArrangeTubeDirection( ref TopoDS_Shape topoDS_Shape, ref BoundingBox boundingBox )
		{
			if( m_Direction == AxisDirection.YAxis ) {
				return;
			}

			topoDS_Shape = RotateTopoDSShape( topoDS_Shape, m_Direction );

			// rotate 90 along Z axis if the longest axis is X
			if( m_Direction == AxisDirection.XAxis ) {
				boundingBox.RotateBoxBy90Degree( AxisDirection.ZAxis, true );
			}

			// rotate -90 along X axis if the longest axis is Z
			if( m_Direction == AxisDirection.ZAxis ) {
				boundingBox.RotateBoxBy90Degree( AxisDirection.XAxis, false );
			}
		}

		AxisDirection GetBoxLongestDirection( BoundingBox BoundingBox )
		{
			AxisDirection BoxLongestDirection;
			double DeltaX = BoundingBox.XLength;
			double DeltaY = BoundingBox.YLength;
			double DeltaZ = BoundingBox.ZLength;

			if( DeltaX > DeltaY && DeltaX > DeltaZ ) {
				BoxLongestDirection = AxisDirection.XAxis;
			}
			else if( DeltaZ > DeltaX && DeltaZ > DeltaY ) {
				BoxLongestDirection = AxisDirection.ZAxis;
			}
			else {
				BoxLongestDirection = AxisDirection.YAxis;
			}
			return BoxLongestDirection;
		}

		List<List<TopoDS_Edge>> GetEdgeList( List<TopoDS_Shape> ShapeList )
		{
			// sew to shell
			BRepBuilderAPI_Sewing sewer = SewFace( ShapeList );

			return SeperateEdgesOfWires( sewer );
		}

		// CAUTION : need rearrange
		List<List<Point3D>> GetPathPoint3D( List<List<TopoDS_Edge>> EdgeList, List<int> nEdgeNotCloseList )
		{
			List<List<Point3D>> PathPoint3D = new List<List<Point3D>>();
			for( int i = 0; i < EdgeList.Count; i++ ) {

				List<Point3D> tempPointList = new List<Point3D>();

				for( int j = 0; j < EdgeList[ i ].Count; j++ ) {

					// get target edge length
					GProp_GProps System = new GProp_GProps();
					TopoDS_Edge CurrentEdge = EdgeList[ i ][ j ];
					BRepGProp.LinearProperties( CurrentEdge, ref System );
					double EdgeLength = System.Mass();
					int nSegments = (int)Math.Ceiling( EdgeLength / TubeCuttingParameter.IGS_Definition );

					double StartU = 0;
					double EndU = 0;
					List<Point3D> ResultList = new List<Point3D>();
					Geom_Curve oneGeomCurve = BRep_Tool.Curve( EdgeList[ i ][ j ], ref StartU, ref EndU );

					// if the direction of topo is different from geom,
					// swap StartU and EndU to change the direction of points
					if( CurrentEdge.Orientation() == TopAbs_Orientation.TopAbs_REVERSED ) {
						double TempU = StartU;
						StartU = EndU;
						EndU = TempU;
					}

					double ValueIncrement = ( EndU - StartU ) / nSegments;

					for( int k = 0; k < nSegments; k++ ) {
						double U = StartU + ValueIncrement * k;
						ResultList.Add( OCCTranslator.ConvertGPPointToPoint3D( oneGeomCurve.Value( U ) ) );
					}

					// add to last point list
					tempPointList.AddRange( ResultList );
				}

				// Following issue "AUTO-9645", the edge may not be closed, and there is no requirement for it to be closed.
				if( nEdgeNotCloseList.Contains( i ) == false ) {

					// make closed path
					tempPointList.Add( tempPointList[ 0 ] );
				}

				PathPoint3D.Add( tempPointList );
			}
			return PathPoint3D;
		}

		List<int> FindAllCutOffWiresOnEdge( List<List<TopoDS_Edge>> EdgeList, int nHeadCutOffEdgeListIndex, int nTrialCutOffEdgeListIndex, List<BoundingBox> shapeBoxList, ITube Tube )
		{
			List<int> CutOffPathList = new List<int>()
			{
				nHeadCutOffEdgeListIndex,
				nTrialCutOffEdgeListIndex
			};

			// the cutoff line has same boundbox and the distance of the x coord is equal to the tube perimeter
			for( int i = 0; i < EdgeList.Count; i++ ) {

				if( i == nHeadCutOffEdgeListIndex || i == nTrialCutOffEdgeListIndex ) {
					continue;
				}

				// get point3D list from EdgeList by IGS definition
				List<List<Point3D>> PathPoint3DList = GetPathPoint3D( new List<List<TopoDS_Edge>>() { EdgeList[ i ] }, new List<int>() );

				// convert to polyline vertex
				List<List<PolyLineVertex>> PathVertexPointList = CuttingPathUtility.GetPathVertexPoint( PathPoint3DList, Tube );

				double dis = Math.Abs( PathVertexPointList[ 0 ][ 0 ].VertexPoint.X - PathVertexPointList[ 0 ][ PathVertexPointList[ 0 ].Count - 1 ].VertexPoint.X );

				if( Math.Abs( dis - Tube.CrossSection.Perimeter ) > ValueConstrain.ACCURACY ) {
					continue;
				}

				CutOffPathList.Add( i );
			}
			return CutOffPathList;
		}

		void FindIndexOfWiresOnEdge( List<List<TopoDS_Edge>> EdgeList, out int nHeadCutOffEdgeListIndex, out int nTailCutOffEdgeListIndex, out List<BoundingBox> shapeBoxList )
		{
			shapeBoxList = new List<BoundingBox>();
			for( int i = 0; i < EdgeList.Count; i++ ) {

				List<TopoDS_Shape> Wire = new List<TopoDS_Shape>();
				for( int j = 0; j < EdgeList[ i ].Count; j++ ) {
					Wire.Add( EdgeList[ i ][ j ] );
				}
				if( EnvParameter.IsRoughImport ) {
					shapeBoxList.Add( GetRoughBox( OCCTranslator.CombineTopoShape( Wire ) ) );
				}
				else {
					shapeBoxList.Add( GetBoundingBox( Wire ) );
				}
			}
			FindMaxAndMinYPoint( shapeBoxList, out nHeadCutOffEdgeListIndex, out nTailCutOffEdgeListIndex );
		}

		void FindMaxAndMinYPoint( List<BoundingBox> BoxList, out int nMinYPointIndex, out int nMaxYPointIndex )
		{
			double MinY = double.MaxValue;
			double MaxY = double.MinValue;

			nMinYPointIndex = 0;
			nMaxYPointIndex = 0;

			for( int i = 0; i < BoxList.Count; i++ ) {
				if( BoxList[ i ].Ymin < MinY ) {
					MinY = BoxList[ i ].Ymin;
					nMinYPointIndex = i;
				}
				if( BoxList[ i ].Ymax > MaxY ) {
					MaxY = BoxList[ i ].Ymax;
					nMaxYPointIndex = i;
				}
			}
		}

		TubeData GetMotherTubeData( List<TopoDS_Edge> EdgeList, double tubeLength, out double crossSectionRotateAngle )
		{
			List<TopoDS_Edge> FirstEdgeListOnXZPlane = ProjectEdgeListOnXZPlane( EdgeList );

			// identify tube type
			CrossSectionType TubeType = GetTubeType( FirstEdgeListOnXZPlane, out crossSectionRotateAngle );

			// rotate edge
			RotateShapeByCrossection( ref FirstEdgeListOnXZPlane, crossSectionRotateAngle );

			List<Point3D> UsefulFirstEdgePointList = GetUsefulPoint( FirstEdgeListOnXZPlane, TubeType );

			// identify circle and oval
			TubeType = GetTubeType( TubeType, UsefulFirstEdgePointList );

			ITube Tube = GetTube( TubeType, UsefulFirstEdgePointList, tubeLength );

			return new TubeData( Tube );
		}

		void RotateShapeByCrossection( ref List<TopoDS_Edge> oneShapeList, double crossSectionRotateAngle )
		{
			if( crossSectionRotateAngle == 0 ) {
				return;
			}
			for( int i = 0; i < oneShapeList.Count; i++ ) {
				oneShapeList[ i ] = TopoDS.ToEdge( OCCTranslator.RotateShapeByAngle( oneShapeList[ i ], crossSectionRotateAngle ) );
			}
		}

		void RotateShapeByCrossection( ref List<List<TopoDS_Edge>> EdgeList, double crossSectionRotateAngle )
		{
			if( crossSectionRotateAngle == 0 ) {
				return;
			}
			for( int i = 0; i < EdgeList.Count; i++ ) {
				List<TopoDS_Edge> newEdge = EdgeList[ i ];
				RotateShapeByCrossection( ref newEdge, crossSectionRotateAngle );
				EdgeList[ i ] = newEdge;
			}
		}

		List<TopoDS_Edge> ProjectEdgeListOnXZPlane( List<TopoDS_Edge> EdgeList )
		{
			// ( 0, -0.1, 0 ):projection base face should cannot be exactly at the origin, AUTO-8323
			TopoDS_Face XZFace = CreateFace( new gp_Vec( 0, 1, 0 ), new gp_Pnt( 0, -0.1, 0 ), 10000 );

			List<TopoDS_Edge> ResultList = new List<TopoDS_Edge>();
			for( int i = 0; i < EdgeList.Count; i++ ) {
				BRepOffsetAPI_NormalProjection Projector = new BRepOffsetAPI_NormalProjection( XZFace );
				Projector.Add( EdgeList[ i ] );
				Projector.Build();

				TopExp_Explorer CompoundExplorer = new TopExp_Explorer( Projector.Shape(), TopAbs_ShapeEnum.TopAbs_EDGE );

				// add original edge if can not get projection result
				if( CompoundExplorer.More() == false ) {
					ResultList.Add( EdgeList[ i ] );
					continue;
				}

				for( ; CompoundExplorer.More(); CompoundExplorer.Next() ) {
					ResultList.Add( TopoDS.ToEdge( CompoundExplorer.Current() ) );
				}
			}

			OrderEdge( ref ResultList );
			return ResultList;
		}

		void OrderEdge( ref List<TopoDS_Edge> anEdges )
		{
			List<TopoDS_Edge> orderedEdges = new List<TopoDS_Edge>();

			// get edges order for the wire.
			ShapeAnalysis_Edge edgeAnalyser = new ShapeAnalysis_Edge();
			ShapeAnalysis_WireOrder wireOrder = new ShapeAnalysis_WireOrder();
			for( int i = 0; i < anEdges.Count; i++ ) {
				TopoDS_Vertex aVf = edgeAnalyser.FirstVertex( anEdges[ i ] );
				TopoDS_Vertex aVl = edgeAnalyser.LastVertex( anEdges[ i ] );

				gp_Pnt aPf = BRep_Tool.Pnt( aVf );
				gp_Pnt aPl = BRep_Tool.Pnt( aVl );
				wireOrder.Add( aPf.XYZ(), aPl.XYZ() );
			}
			wireOrder.Perform();

			for( int i = 1; i <= wireOrder.NbEdges(); i++ ) {
				int nIdex = wireOrder.Ordered( i );
				TopoDS_Edge anEdge = anEdges[ Math.Abs( nIdex ) - 1 ];
				if( nIdex < 0 ) {

					// build a Reverse edge
					double StartValue = 0;
					double EndValue = 0;
					Geom_Curve curve = BRep_Tool.Curve( anEdge, ref StartValue, ref EndValue );
					curve.Reverse();
					TopoDS_Edge reverseEdge = ( new BRepBuilderAPI_MakeEdge( curve ) ).Edge();
					orderedEdges.Add( reverseEdge );
				}
				else {
					orderedEdges.Add( anEdge );
				}
			}
			anEdges = orderedEdges;
		}

		List<Geom_Curve> GetCurveList( List<TopoDS_Edge> EdgeList )
		{
			List<gp_Pnt> NoUseList;
			return GetCurveList( EdgeList, out NoUseList, out NoUseList );
		}

		List<Geom_Curve> GetCurveList( List<TopoDS_Edge> EdgeList, out List<gp_Pnt> StartPointList, out List<gp_Pnt> EndPointList )
		{
			List<Geom_Curve> CurveList = new List<Geom_Curve>();
			StartPointList = new List<gp_Pnt>();
			EndPointList = new List<gp_Pnt>();

			double StartValue = 0;
			double EndValue = 0;

			for( int i = 0; i < EdgeList.Count; i++ ) {
				Geom_Curve CurrentCurve = BRep_Tool.Curve( EdgeList[ i ], ref StartValue, ref EndValue );
				CurveList.Add( CurrentCurve );
				StartPointList.Add( CurrentCurve.Value( StartValue ) );
				EndPointList.Add( CurrentCurve.Value( EndValue ) );
			}
			return CurveList;
		}

		// create a face parallel to the Y-axis with boundary
		TopoDS_Face CreateHalfFaceParallelYAxis( gp_Vec vecNormal, gp_Pnt ptOnFace, BoundingBox boundingBoxParameter, double dTolerance )
		{
			try {
				gp_Dir dirUnitNormal = new gp_Dir( vecNormal );
				gp_Dir dirFace = gp.DY().Crossed( dirUnitNormal );

				double dDisXZ = boundingBoxParameter.XLength > boundingBoxParameter.ZLength ? boundingBoxParameter.XLength : boundingBoxParameter.ZLength;
				double dDisY = boundingBoxParameter.YLength;

				gp_Pnt pt1 = new gp_Pnt( ptOnFace.XYZ() - ( gp.DY().XYZ() ) * dTolerance );
				gp_Pnt pt2 = new gp_Pnt( ptOnFace.XYZ() + ( gp.DY().XYZ() ) * dTolerance + ( gp.DY().XYZ() ) * dDisY );
				gp_Pnt pt3 = new gp_Pnt( pt2.XYZ() + ( dirFace.XYZ() ) * dTolerance + ( dirFace.XYZ() ) * dDisXZ );
				gp_Pnt pt4 = new gp_Pnt( pt1.XYZ() + ( dirFace.XYZ() ) * dTolerance + ( dirFace.XYZ() ) * dDisXZ );

				List<gp_Pnt> ptList = new List<gp_Pnt> { pt1, pt2, pt3, pt4, };
				return OCCTranslator.MakeFace( ptList );
			}
			catch {
				return new TopoDS_Face();
			}
		}

		// create face with boundary.
		TopoDS_Face CreateFace( gp_Vec NormalVector, gp_Pnt PointOnFace, double Scale )
		{
			NormalVector.Normalize();

			// generate a vector is not parallel to Normal Vector
			gp_Vec AnyOtherVector = new gp_Vec( 1, 0, 0 );
			if( AnyOtherVector.IsParallel( NormalVector, ValueConstrain.ACCURACY ) ) {
				AnyOtherVector = new gp_Vec( 0, 1, 0 );
			}
			gp_Vec U_Vector = AnyOtherVector ^ NormalVector;
			gp_Vec V_Vector = U_Vector ^ NormalVector;

			gp_Pnt BoundPoint1 = PointOnFace.Translated( U_Vector.Normalized() * Scale );
			gp_Pnt BoundPoint2 = PointOnFace.Translated( V_Vector.Normalized() * Scale );
			gp_Pnt BoundPoint3 = PointOnFace.Translated( U_Vector.Normalized() * -Scale );
			gp_Pnt BoundPoint4 = PointOnFace.Translated( V_Vector.Normalized() * -Scale );

			TopoDS_Edge BoundEdge1 = TopoDS.ToEdge( OCCTranslator.MakeEdge( BoundPoint1, BoundPoint2 ) );
			TopoDS_Edge BoundEdge2 = TopoDS.ToEdge( OCCTranslator.MakeEdge( BoundPoint2, BoundPoint3 ) );
			TopoDS_Edge BoundEdge3 = TopoDS.ToEdge( OCCTranslator.MakeEdge( BoundPoint3, BoundPoint4 ) );
			TopoDS_Edge BoundEdge4 = TopoDS.ToEdge( OCCTranslator.MakeEdge( BoundPoint4, BoundPoint1 ) );

			BRepBuilderAPI_MakeWire WireMaker = new BRepBuilderAPI_MakeWire();

			WireMaker.Add( BoundEdge1 );
			WireMaker.Add( BoundEdge2 );
			WireMaker.Add( BoundEdge3 );
			WireMaker.Add( BoundEdge4 );

			TopoDS_Wire BoundWire = WireMaker.Wire();

			BRepBuilderAPI_MakeFace FaceMaker = new BRepBuilderAPI_MakeFace( BoundWire );

			TopoDS_Face Face = FaceMaker.Face();

			return Face;
		}

		// CAUTION : 可能會有超出面範圍的交點
		List<Point3D> GetPlaneIntersectionPoint( List<Geom_Curve> CurveList, gp_Vec NormalVector )
		{
			TopoDS_Face Face = MakePlaneFace( NormalVector, new gp_Pnt( 0, 0, 0 ) );

			Geom_Surface Surface = BRep_Tool.Surface( Face );

			List<Point3D> IntersectPointList = new List<Point3D>();

			for( int i = 0; i < CurveList.Count; i++ ) {
				GeomAPI_IntCS Intersector = new GeomAPI_IntCS( CurveList[ i ], Surface );
				for( int j = 1; j <= Intersector.NbPoints(); j++ ) {
					IntersectPointList.Add( OCCTranslator.ConvertGPPointToPoint3D( Intersector.Point( j ) ) );
				}
			}
			return IntersectPointList;
		}

		// tell which one is rectangular
		CrossSectionType GetTubeType( List<TopoDS_Edge> OneEdgeList, out double crossSectionRotateAngle )
		{
			crossSectionRotateAngle = 0;
			List<TopoDS_Shape> ShapeList = new List<TopoDS_Shape>();
			ShapeList.AddRange( OneEdgeList );
			BoundingBox boundBox = GetBoundingBox( ShapeList );
			bool isOvalTube = checkAllEdgeInOval_XOZ( OneEdgeList, boundBox );
			if( isOvalTube ) {
				return CrossSectionType.Oval;
			}

			// convert product resut to 2d polyline
			Geom2d_PolyLine tubeCrossSection = GetProductCrossSection( OneEdgeList );

			// filter DShape
			bool isDShape = CheckIsDShape( tubeCrossSection, out crossSectionRotateAngle );
			if( isDShape ) {
				return CrossSectionType.DShape;
			}

			// filter FlatOval
			bool isFlatOvalType = CheckIsFlatOvalType( tubeCrossSection, out crossSectionRotateAngle );
			if( isFlatOvalType ) {
				return CrossSectionType.FlatOval;
			}

			return CrossSectionType.Rectangle;
		}

		bool CheckIsFlatOvalType( Geom2d_PolyLine polyline, out double crossSectionRotateAngle )
		{
			crossSectionRotateAngle = 0;
			Extrema extrema = polyline.ExtremaPoint;
			if( extrema.Width > extrema.Height ) {
				PointF leftCenterX = new PointF( extrema.MinX + extrema.Height / 2, 0 );
				Geom2d_Line intersectedLine = new Geom2d_Line( leftCenterX, new PointF( extrema.MinX, extrema.MaxY ) );
				List<PointF> IntersectionPointList = new List<PointF>();
				IntersectionPointList.AddRange( Geom_Utility.GetIntersectPts( intersectedLine, polyline ) );
				double leftCenterXDistance = Math_Utility.GetDistance( leftCenterX, IntersectionPointList[ 0 ] );

				if( Math_Utility.IsSameValue( leftCenterXDistance, extrema.Height / 2, ValueConstrain.PoorACCURACY ) ) {
					crossSectionRotateAngle = 0;
					return true;
				}
				return false;
			}
			else {
				PointF upCenterX = new PointF( 0, extrema.MaxY - extrema.Width / 2 );
				Geom2d_Line intersectedLine = new Geom2d_Line( upCenterX, new PointF( extrema.MaxX, extrema.MaxY ) );
				List<PointF> IntersectionPointList = new List<PointF>();
				IntersectionPointList.AddRange( Geom_Utility.GetIntersectPts( intersectedLine, polyline ) );
				double upCenterXDistance = Math_Utility.GetDistance( upCenterX, IntersectionPointList[ 0 ] );

				if( Math_Utility.IsSameValue( upCenterXDistance, extrema.Width / 2, ValueConstrain.PoorACCURACY ) ) {
					crossSectionRotateAngle = 90;
					return true;
				}
				return false;
			}
		}

		bool CheckIsDShape( Geom2d_PolyLine polyline, out double crossSectionRotateAngle )
		{
			// 1.get four corner line intersect length
			Extrema extrema = polyline.ExtremaPoint;
			PointF leftUpPoint = new PointF( extrema.MinX, extrema.MaxY );
			PointF rightUpPoint = new PointF( extrema.MaxX, extrema.MaxY );
			PointF leftDownPoint = new PointF( extrema.MinX, extrema.MinY );
			PointF rightDownPoint = new PointF( extrema.MaxX, extrema.MinY );
			PointF zeroPoint = new PointF( 0, 0 );

			List<PointF> IntersectionPointList = new List<PointF>();
			Geom2d_Line intersectedLine = new Geom2d_Line( zeroPoint, leftUpPoint );
			IntersectionPointList.AddRange( Geom_Utility.GetIntersectPts( intersectedLine, polyline ) );
			double leftUpDistance = Math_Utility.GetDistance( zeroPoint, IntersectionPointList[ 0 ] );

			IntersectionPointList.Clear();
			intersectedLine = new Geom2d_Line( zeroPoint, rightUpPoint );
			IntersectionPointList.AddRange( Geom_Utility.GetIntersectPts( intersectedLine, polyline ) );
			double rightUpDistance = Math_Utility.GetDistance( zeroPoint, IntersectionPointList[ 0 ] );

			IntersectionPointList.Clear();
			intersectedLine = new Geom2d_Line( zeroPoint, leftDownPoint );
			IntersectionPointList.AddRange( Geom_Utility.GetIntersectPts( intersectedLine, polyline ) );
			double leftDownDistance = Math_Utility.GetDistance( zeroPoint, IntersectionPointList[ 0 ] );

			IntersectionPointList.Clear();
			intersectedLine = new Geom2d_Line( zeroPoint, rightDownPoint );
			IntersectionPointList.AddRange( Geom_Utility.GetIntersectPts( intersectedLine, polyline ) );
			double rightDownDistance = Math_Utility.GetDistance( zeroPoint, IntersectionPointList[ 0 ] );

			// 2.check four length equal
			if( Math_Utility.IsSameValue( leftUpDistance, rightUpDistance, ValueConstrain.PoorACCURACY ) &&
				Math_Utility.IsSameValue( leftUpDistance, leftDownDistance, ValueConstrain.PoorACCURACY ) &&
				Math_Utility.IsSameValue( leftUpDistance, rightDownDistance, ValueConstrain.PoorACCURACY ) ) {
				crossSectionRotateAngle = 0;
				return false;
			}

			if( Math_Utility.IsSameValue( leftDownDistance, rightDownDistance, ValueConstrain.PoorACCURACY ) ) {

				// up DShape
				if( leftDownDistance < leftUpDistance ) {
					crossSectionRotateAngle = 0;
				}

				// down DShape
				else {
					crossSectionRotateAngle = 180;
				}
			}
			else {
				// right DShape
				if( leftDownDistance < rightDownDistance ) {
					crossSectionRotateAngle = 270;
				}

				// left DShape
				else {
					crossSectionRotateAngle = 90;
				}
			}
			return true;
		}

		Geom2d_PolyLine GetProductCrossSection( List<TopoDS_Edge> oneEdgeList )
		{
			// 1.convert edge to point list
			List<PointF> pointlist = ConvertEdgeToPointList( oneEdgeList );
			List<PointF[]> listSourcePolygon = new List<PointF[]>();
			listSourcePolygon.Add( pointlist.ToArray() );

			// 2.convert PointF to IntPoint paths for calculate robustness in clipperlib
			List<List<IntPoint>> listPath = Convert2ListIntPointPath( listSourcePolygon, 1000000 );

			// 3.get union result
			Clipper dataArranger = new Clipper();
			dataArranger.AddPath( listPath[ 0 ], PolyType.ptSubject, true );
			List<List<IntPoint>> IntersectionPathList = new List<List<IntPoint>>();
			dataArranger.Execute( ClipType.ctUnion, ref IntersectionPathList, PolyFillType.pftNonZero );

			// 4.simplify polypath in order to prevent path1 and path2 boundaries coincide
			IntersectionPathList = Clipper.SimplifyPolygons( IntersectionPathList, PolyFillType.pftNonZero );

			// 5.convert from IntPoint to PointF path, and make a polygon
			List<PointF[]> resultPolygons = Geom_Utility.Convert2ListPointFPath( IntersectionPathList, (float)1 / 1000000, true );
			if( resultPolygons.Count == 0 ) {
				return new Geom2d_PolyLine();
			}

			// get max length polygon to remove interfer polygon
			var countOrderPolygon = from polygon in resultPolygons
									orderby polygon.Length descending
									select polygon;
			Geom2d_PolyLine resultGeometry = Geom_Utility.GetPolylineGeometry( countOrderPolygon.ToList()[ 0 ] );
			return resultGeometry;
		}

		List<PointF> ConvertEdgeToPointList( List<TopoDS_Edge> oneEdgeList )
		{
			List<PointF> pointlist = new List<PointF>();
			for( int i = 0; i < oneEdgeList.Count; i++ ) {
				double Start = 0;
				double End = 0;
				Geom_Curve oneGeomCurve = BRep_Tool.Curve( oneEdgeList[ i ], ref Start, ref End );
				string szEdgeName = oneGeomCurve.DynamicType().Name();
				if( szEdgeName == "Geom_Circle" ) {
					Geom_Circle circle = Geom_Circle.DownCast( oneGeomCurve );
					pointlist.AddRange( GetArcList( circle, Start, End ) );
					continue;
				}
				else if( szEdgeName == "Geom_BSplineCurve" ) {
					Geom_BSplineCurve SplineCurve = Geom_BSplineCurve.DownCast( oneGeomCurve );
					pointlist.AddRange( GetSplineList( SplineCurve, Start, End ) );
					continue;
				}

				TopExp_Explorer VertexExplorer = new TopExp_Explorer();
				VertexExplorer.Init( oneEdgeList[ i ], TopAbs_ShapeEnum.TopAbs_VERTEX );
				while( VertexExplorer.More() ) {
					TopoDS_Vertex vtx = TopoDS.ToVertex( VertexExplorer.Current() );
					PointF point = OCCTranslator.ConvertVertexXZToPointF( vtx );
					pointlist.Add( point );
					VertexExplorer.Next();
				}
			}
			return pointlist;
		}

		List<PointF> GetSplineList( Geom_BSplineCurve spline, double start, double end )
		{
			double arcSplitStep = 0.1;
			List<PointF> pointList = new List<PointF>();
			for( double i = start; i <= end; i += arcSplitStep ) {
				gp_Pnt point = new gp_Pnt();
				spline.D0( i, ref point );
				pointList.Add( new PointF( (float)point.X(), (float)point.Z() ) );
			}

			// add end point
			gp_Pnt endPoint = new gp_Pnt();
			spline.D0( end, ref endPoint );
			pointList.Add( new PointF( (float)endPoint.X(), (float)endPoint.Z() ) );
			return pointList;
		}

		List<PointF> GetArcList( Geom_Circle circle, double start, double end )
		{
			double arcSplitStep = 0.1;
			List<PointF> pointList = new List<PointF>();
			for( double i = start; i <= end; i += arcSplitStep ) {
				gp_Pnt point = new gp_Pnt();
				circle.D0( i, ref point );
				pointList.Add( new PointF( (float)point.X(), (float)point.Z() ) );
			}

			gp_Pnt endpoint = new gp_Pnt();
			circle.D0( end, ref endpoint );
			pointList.Add( new PointF( (float)endpoint.X(), (float)endpoint.Z() ) );
			return pointList;
		}

		List<List<IntPoint>> Convert2ListIntPointPath( List<PointF[]> pointFPaths, int scale )
		{
			int nCurvesCount = pointFPaths.Count;
			List<List<IntPoint>> Paths = new List<List<IntPoint>>();

			for( int i = 0; i < nCurvesCount; i++ ) {
				int nPointsCount = pointFPaths[ i ].Length;
				List<IntPoint> Path = new List<IntPoint>();

				// add originCurve to Path
				for( int j = 0; j < nPointsCount; j++ ) {
					IntPoint scaledPoint = new IntPoint();
					scaledPoint.X = (long)( pointFPaths[ i ][ j ].X * scale );
					scaledPoint.Y = (long)( pointFPaths[ i ][ j ].Y * scale );
					Path.Add( scaledPoint );
				}
				Paths.Add( Path );
			}
			return Paths;
		}

		// tell which one is circular or oval
		CrossSectionType GetTubeType( CrossSectionType TubeType, List<Point3D> UsefulEdgePointList )
		{
			if( TubeType == CrossSectionType.Rectangle ) {
				return CrossSectionType.Rectangle;
			}
			else if( TubeType == CrossSectionType.DShape ) {
				return CrossSectionType.DShape;
			}
			else if( TubeType == CrossSectionType.FlatOval ) {
				return CrossSectionType.FlatOval;
			}

			if( TubeType == CrossSectionType.Oval ) {

				var XOrderedPoint = from P in UsefulEdgePointList
									orderby P.X descending
									select P;

				var ZOrderedPoint = from P in UsefulEdgePointList
									orderby P.Z descending
									select P;

				double HalfHorizontalAxis = Math.Round( Math.Abs( XOrderedPoint.ToList()[ 0 ].X ), 2 );
				double HalfVerticalAxis = Math.Round( Math.Abs( ZOrderedPoint.ToList()[ 0 ].Z ), 2 );

				if( HalfHorizontalAxis == HalfVerticalAxis ) {
					return CrossSectionType.Circle;
				}
			}
			return CrossSectionType.Oval;
		}

		ITube GetTube( CrossSectionType TubeType, List<Point3D> UsefulFirstEdgePointList, double tubeLength )
		{
			ITube tube;

			var FirstXOrderedPoint = from P in UsefulFirstEdgePointList
									 orderby P.X descending
									 select P;

			var FirstZOrderedPoint = from P in UsefulFirstEdgePointList
									 orderby P.Z descending
									 select P;

			switch( TubeType ) {
				case CrossSectionType.Rectangle:
					RectangularCrossSection CrossSection = GetRectangularCrossSection( UsefulFirstEdgePointList );

					double RoundedWidth = Math.Round( CrossSection.Width, ValueConstrain.DECIMAL_Place );
					double RoundedHeight = Math.Round( CrossSection.Height, ValueConstrain.DECIMAL_Place );
					double RoundedFillet = Math.Round( CrossSection.Fillet, ValueConstrain.DECIMAL_Place );

					tube = new RectangularTube( tubeLength, RoundedWidth, RoundedHeight, RoundedFillet );
					break;

				case CrossSectionType.FlatOval:
					FlatOvalCrossSection flatOvalCrossSection = GetFlatOvalCrossSection( UsefulFirstEdgePointList );
					RoundedWidth = Math.Round( flatOvalCrossSection.Width, ValueConstrain.DECIMAL_Place );
					RoundedHeight = Math.Round( flatOvalCrossSection.Height, ValueConstrain.DECIMAL_Place );
					tube = new FlatOvalTube( tubeLength, RoundedWidth, RoundedHeight );
					break;

				case CrossSectionType.DShape:
					DShapeCrossSection dShapeCrossSection = GetDShapeCrossSection( UsefulFirstEdgePointList );
					RoundedWidth = Math.Round( dShapeCrossSection.Width, ValueConstrain.DECIMAL_Place );
					RoundedHeight = Math.Round( dShapeCrossSection.Height, ValueConstrain.DECIMAL_Place );
					RoundedFillet = Math.Round( dShapeCrossSection.Fillet, ValueConstrain.DECIMAL_Place );
					tube = new DShapeTube( tubeLength, RoundedWidth, RoundedHeight, RoundedFillet );
					break;

				case CrossSectionType.Circle:
					double RoughRadius = Math.Abs( FirstXOrderedPoint.ToList()[ 0 ].X );

					double RoundedRadius = Math.Round( ( RoughRadius ), ValueConstrain.DECIMAL_Place );

					tube = new CircularTube( tubeLength, RoundedRadius );
					break;

				case CrossSectionType.Oval:
				default:
					double RoughHalfHorizontalAxis = Math.Abs( FirstXOrderedPoint.ToList()[ 0 ].X );
					double RoughHalfVerticalAxis = Math.Abs( FirstZOrderedPoint.ToList()[ 0 ].Z );

					double RoundedHalfHorizontalAxis = Math.Round( RoughHalfHorizontalAxis, ValueConstrain.DECIMAL_Place );
					double RoundedHalfVerticalAxis = Math.Round( RoughHalfVerticalAxis, ValueConstrain.DECIMAL_Place );

					tube = new OvalTube( tubeLength, RoundedHalfHorizontalAxis, RoundedHalfVerticalAxis );
					break;
			}
			return tube;
		}

		FlatOvalCrossSection GetFlatOvalCrossSection( List<Point3D> UsefulOneEdgePointList )
		{
			int nLastIndex = UsefulOneEdgePointList.Count - 1;

			var ZOrderedPointList = from P in UsefulOneEdgePointList
									orderby P.Z descending
									select P;

			var XOrderedPointList = from P in UsefulOneEdgePointList
									orderby P.X descending
									select P;

			Point3D MaxZPoint = ZOrderedPointList.ToList()[ 0 ];
			Point3D MaxXPoint = XOrderedPointList.ToList()[ 0 ];

			double Height = MaxZPoint.Z * 2;
			double Width = MaxXPoint.X * 2;
			return new FlatOvalCrossSection( Width, Height );
		}

		DShapeCrossSection GetDShapeCrossSection( List<Point3D> UsefulOneEdgePointList )
		{
			int nLastIndex = UsefulOneEdgePointList.Count - 1;

			var ZOrderedPointList = from P in UsefulOneEdgePointList
									orderby P.Z descending
									select P;

			var XOrderedPointList = from P in UsefulOneEdgePointList
									orderby P.X descending
									select P;

			Point3D MaxZPoint = ZOrderedPointList.ToList()[ 0 ];
			Point3D MaxXPoint = XOrderedPointList.ToList()[ 0 ];

			double Height = MaxZPoint.Z * 2;
			double Width = MaxXPoint.X * 2;
			double Radius = MaxZPoint.Z - MaxXPoint.Z;
			return new DShapeCrossSection( Width, Height, Radius );
		}

		// the definition of useful points is defined in LASER-1655
		List<Point3D> GetUsefulPoint( List<TopoDS_Edge> OneEdgeList, CrossSectionType crossSectionType )
		{
			List<Point3D> UsefulPointList = new List<Point3D>();
			List<Geom_Curve> CurveListForFindPoint;

			switch( crossSectionType ) {

				case CrossSectionType.Rectangle:
					List<Point3D> PointList = new List<Point3D>();

					List<gp_Pnt> StartPointList, EndPointList;
					CurveListForFindPoint = GetCurveList( OneEdgeList, out StartPointList, out EndPointList );

					for( int i = 0; i < OneEdgeList.Count; i++ ) {
						PointList.Add( OCCTranslator.ConvertGPPointToPoint3D( StartPointList[ i ] ) );
						PointList.Add( OCCTranslator.ConvertGPPointToPoint3D( EndPointList[ i ] ) );
					}

					PointList.AddRange( GetPlaneIntersectionPoint( CurveListForFindPoint, new gp_Vec( 1, 0, 0 ) ) );
					PointList.AddRange( GetPlaneIntersectionPoint( CurveListForFindPoint, new gp_Vec( 0, 0, 1 ) ) );

					UsefulPointList.AddRange( GetRectangularEdgeUsefulPoint( PointList ) );
					break;

				case CrossSectionType.DShape:
					PointList = new List<Point3D>();
					CurveListForFindPoint = GetCurveList( OneEdgeList, out StartPointList, out EndPointList );

					for( int i = 0; i < OneEdgeList.Count; i++ ) {
						PointList.Add( OCCTranslator.ConvertGPPointToPoint3D( StartPointList[ i ] ) );
						PointList.Add( OCCTranslator.ConvertGPPointToPoint3D( EndPointList[ i ] ) );
					}

					PointList.AddRange( GetPlaneIntersectionPoint( CurveListForFindPoint, new gp_Vec( 1, 0, 0 ) ) );
					PointList.AddRange( GetPlaneIntersectionPoint( CurveListForFindPoint, new gp_Vec( 0, 0, 1 ) ) );

					UsefulPointList.AddRange( GetRectangularEdgeUsefulPoint( PointList ) );
					break;

				case CrossSectionType.FlatOval:
					PointList = new List<Point3D>();
					CurveListForFindPoint = GetCurveList( OneEdgeList, out StartPointList, out EndPointList );

					for( int i = 0; i < OneEdgeList.Count; i++ ) {
						PointList.Add( OCCTranslator.ConvertGPPointToPoint3D( StartPointList[ i ] ) );
						PointList.Add( OCCTranslator.ConvertGPPointToPoint3D( EndPointList[ i ] ) );
					}

					PointList.AddRange( GetPlaneIntersectionPoint( CurveListForFindPoint, new gp_Vec( 1, 0, 0 ) ) );
					PointList.AddRange( GetPlaneIntersectionPoint( CurveListForFindPoint, new gp_Vec( 0, 0, 1 ) ) );

					UsefulPointList.AddRange( GetRectangularEdgeUsefulPoint( PointList ) );
					break;

				case CrossSectionType.Oval:
				case CrossSectionType.Circle:
				default:
					CurveListForFindPoint = GetCurveList( OneEdgeList );
					UsefulPointList.AddRange( GetPlaneIntersectionPoint( CurveListForFindPoint, new gp_Vec( 1, 0, 0 ) ) );
					UsefulPointList.AddRange( GetPlaneIntersectionPoint( CurveListForFindPoint, new gp_Vec( 0, 0, 1 ) ) );
					break;
			}
			return UsefulPointList;
		}

		List<Point3D> GetRectangularEdgeUsefulPoint( List<Point3D> PointList )
		{
			List<Point3D> UsefulPointList = new List<Point3D>();

			int nLastIndex = PointList.Count - 1;
			var ZOrderedPointList = from P in PointList
									orderby P.Z descending
									select P;

			var XOrderedPointList = from P in PointList
									orderby P.X descending
									select P;

			double MaxZValue = ZOrderedPointList.ToList()[ 0 ].Z;
			double MaxXValue = XOrderedPointList.ToList()[ 0 ].X;

			var MaxZOrderedPointList = from P in PointList
									   where Comparer.IsSameValue( P.Z, MaxZValue )
									   orderby P.X descending
									   select P;

			var MaxXOrderedPointList = from P in PointList
									   where Comparer.IsSameValue( P.X, MaxXValue )
									   orderby P.Z descending
									   select P;

			// get points on plane - fillet intersection
			UsefulPointList.Add( MaxZOrderedPointList.ToList()[ 0 ] );
			UsefulPointList.Add( MaxXOrderedPointList.ToList()[ 0 ] );

			return UsefulPointList;
		}

		List<TopoDS_Shape> RotateTopoDSShapeList( List<TopoDS_Shape> topoDSShapeList, AxisDirection tubeDirection )
		{
			List<TopoDS_Shape> newTopoDSShapeList = new List<TopoDS_Shape>();

			for( int i = 0; i < topoDSShapeList.Count; i++ ) {
				newTopoDSShapeList.Add( RotateTopoDSShape( topoDSShapeList[ i ], tubeDirection ) );
			}

			return newTopoDSShapeList;
		}

		TopoDS_Shape RotateTopoDSShape( TopoDS_Shape originalTopoDSShape, gp_Trsf transferVector )
		{
			BRepBuilderAPI_Transform transForm = new BRepBuilderAPI_Transform( originalTopoDSShape, transferVector );
			return transForm.Shape();
		}

		RectangularCrossSection GetRectangularCrossSection( List<Point3D> UsefulOneEdgePointList )
		{
			int nLastIndex = UsefulOneEdgePointList.Count - 1;

			var ZOrderedPointList = from P in UsefulOneEdgePointList
									orderby P.Z descending
									select P;

			var XOrderedPointList = from P in UsefulOneEdgePointList
									orderby P.X descending
									select P;

			Point3D MaxZPoint = ZOrderedPointList.ToList()[ 0 ];
			Point3D MaxXPoint = XOrderedPointList.ToList()[ 0 ];

			double Height = MaxZPoint.Z * 2;
			double Width = MaxXPoint.X * 2;
			double Radius = MaxZPoint.Z - MaxXPoint.Z;

			return new RectangularCrossSection( Width, Height, Radius );
		}

		bool checkAllEdgeInOval_XOZ( List<TopoDS_Edge> EdgeList, BoundingBox boundBox )
		{
			double a = boundBox.XLength / 2;
			double b = boundBox.ZLength / 2;
			for( int i = 0; i < EdgeList.Count; i++ ) {
				TopoDS_Edge Edge = EdgeList[ i ];
				TopoDS_Vertex StartVertex = new TopoDS_Vertex();
				TopoDS_Vertex EndVertex = new TopoDS_Vertex();
				ShapeAnalysis.FindBounds( Edge, ref StartVertex, ref EndVertex );

				gp_Pnt gpStrPoint = BRep_Tool.Pnt( StartVertex );
				gp_Pnt gpEndPoint = BRep_Tool.Pnt( EndVertex );

				double pStrX = gpStrPoint.X() - boundBox.XCenter;
				double pStrY = gpStrPoint.Z() - boundBox.ZCenter;
				double pEndX = gpEndPoint.X() - boundBox.XCenter;
				double pEndY = gpEndPoint.Z() - boundBox.ZCenter;

				double eStr = ( pStrX * pStrX ) / ( a * a ) + ( pStrY * pStrY ) / ( b * b );
				double eEnd = ( pEndX * pEndX ) / ( a * a ) + ( pEndY * pEndY ) / ( b * b );

				if( eStr - 1 > ValueConstrain.PoorACCURACY || eEnd - 1 > ValueConstrain.PoorACCURACY ) {
					return false;
				}
			}
			return true;
		}

		bool checkAllEdgeInOval_ZOY( List<TopoDS_Edge> EdgeList, BoundingBox boundBox )
		{
			double a = boundBox.ZLength / 2;
			double b = boundBox.YLength / 2;
			for( int i = 0; i < EdgeList.Count; i++ ) {
				TopoDS_Edge Edge = EdgeList[ i ];
				TopoDS_Vertex StartVertex = new TopoDS_Vertex();
				TopoDS_Vertex EndVertex = new TopoDS_Vertex();
				ShapeAnalysis.FindBounds( Edge, ref StartVertex, ref EndVertex );

				gp_Pnt gpStrPoint = BRep_Tool.Pnt( StartVertex );
				gp_Pnt gpEndPoint = BRep_Tool.Pnt( EndVertex );

				double pStrX = gpStrPoint.Z() - boundBox.ZCenter;
				double pStrY = gpStrPoint.Y() - boundBox.YCenter;
				double pEndX = gpEndPoint.Z() - boundBox.ZCenter;
				double pEndY = gpEndPoint.Y() - boundBox.YCenter;

				double eStr = ( pStrX * pStrX ) / ( a * a ) + ( pStrY * pStrY ) / ( b * b );
				double eEnd = ( pEndX * pEndX ) / ( a * a ) + ( pEndY * pEndY ) / ( b * b );

				if( eStr - 1 > ValueConstrain.PoorACCURACY || eEnd - 1 > ValueConstrain.PoorACCURACY ) {
					return false;
				}
			}
			return true;
		}

		bool checkAllEdgeInOval_YOX( List<TopoDS_Edge> EdgeList, BoundingBox boundBox )
		{
			double a = boundBox.YLength / 2;
			double b = boundBox.XLength / 2;
			for( int i = 0; i < EdgeList.Count; i++ ) {
				TopoDS_Edge Edge = EdgeList[ i ];
				TopoDS_Vertex StartVertex = new TopoDS_Vertex();
				TopoDS_Vertex EndVertex = new TopoDS_Vertex();
				ShapeAnalysis.FindBounds( Edge, ref StartVertex, ref EndVertex );

				gp_Pnt gpStrPoint = BRep_Tool.Pnt( StartVertex );
				gp_Pnt gpEndPoint = BRep_Tool.Pnt( EndVertex );

				double pStrX = gpStrPoint.Y() - boundBox.YCenter;
				double pStrY = gpStrPoint.X() - boundBox.XCenter;
				double pEndX = gpEndPoint.Y() - boundBox.YCenter;
				double pEndY = gpEndPoint.X() - boundBox.XCenter;

				double eStr = ( pStrX * pStrX ) / ( a * a ) + ( pStrY * pStrY ) / ( b * b );
				double eEnd = ( pEndX * pEndX ) / ( a * a ) + ( pEndY * pEndY ) / ( b * b );

				if( eStr - 1 > ValueConstrain.PoorACCURACY || eEnd - 1 > ValueConstrain.PoorACCURACY ) {
					return false;
				}
			}
			return true;
		}

		List<TopoDS_Edge> GetEdgeShapeList( TopoDS_Shape Shape )
		{
			List<TopoDS_Edge> edgeList = new List<TopoDS_Edge>();
			TopExp_Explorer FaceExplorer = new TopExp_Explorer();
			FaceExplorer.Init( Shape, TopAbs_ShapeEnum.TopAbs_FACE );

			while( FaceExplorer.More() ) {
				TopoDS_Face face = TopoDS.ToFace( FaceExplorer.Current() );
				TopExp_Explorer EdgeExplorer = new TopExp_Explorer();
				EdgeExplorer.Init( face, TopAbs_ShapeEnum.TopAbs_EDGE );
				while( EdgeExplorer.More() ) {
					edgeList.Add( TopoDS.ToEdge( EdgeExplorer.Current() ) );
					EdgeExplorer.Next();
				}

				FaceExplorer.Next();
			}
			return edgeList;
		}

		List<TopoDS_Shape> GetCompound( TopoDS_Shape shape )
		{
			List<TopoDS_Shape> comList = new List<TopoDS_Shape>();
			TopoDS_Iterator iter = new TopoDS_Iterator( shape );

			while( iter.More() ) {
				TopoDS_Shape shapeIter = iter.Value();
				if( shapeIter.ShapeType() == TopAbs_ShapeEnum.TopAbs_COMPOUND ) {
					comList.Add( shapeIter );
				}
				iter.Next();
			}
			return comList;
		}

		List<TopoDS_Edge> GetEdgeFromShape( List<TopoDS_Shape> shapeList )
		{
			List<TopoDS_Edge> edgeList = new List<TopoDS_Edge>();
			for( int i = 0; i < shapeList.Count; i++ ) {
				for( TopExp_Explorer explorer = new TopExp_Explorer( shapeList[ i ], TopAbs_ShapeEnum.TopAbs_EDGE ); explorer.More(); explorer.Next() ) {
					edgeList.Add( TopoDS.ToEdge( explorer.Current() ) );
				}
			}
			return edgeList;
		}

		void FindEngravingLine( TopoDS_Shape oneShape, ref List<List<TopoDS_Edge>> edgeList, ref List<int> nEdgeNotCloseList )
		{
			// It is able to recognize sketch information in IGS and STEP files.
			// It currently supports the identification of the following file types:
			// - "IGS" files exported from "SolidWorks"
			// - "STEP" files exported from "Autodesk Inventor"
			// The subsequent functionality will be enhanced by the Issue AUTO-10196.
			List<TopoDS_Shape> compoundList = GetCompound( oneShape );
			List<TopoDS_Edge> edgeEngravinList = GetEdgeFromShape( compoundList );
			List<List<TopoDS_Edge>> edgeEngravinListList = OCCTranslator.SortEdgeList( edgeEngravinList );
			for( int i = 0; i < edgeEngravinListList.Count; i++ ) {
				edgeList.Add( edgeEngravinListList[ i ] );
				if( OCCTranslator.isEdgeListClosed( edgeEngravinListList[ i ] ) ) {
					continue;
				}
				nEdgeNotCloseList.Add( edgeList.Count - 1 );
			}
		}

		void AdjustCutOffPathOri( List<int> nCutOffEdgeList, ref List<List<Point3D>> pathPoint3DList )
		{
			for( int i = 0; i < nCutOffEdgeList.Count; i++ ) {
				int nIndex = nCutOffEdgeList[ i ];

				List<IntPoint> ptList = new List<IntPoint>();
				for( int j = 0; j < pathPoint3DList[ nIndex ].Count; j++ ) {
					ptList.Add( new IntPoint( pathPoint3DList[ nIndex ][ j ].X, pathPoint3DList[ nIndex ][ j ].Z ) );
				}

				if( Clipper.Orientation( ptList ) ) {
					pathPoint3DList[ nIndex ].Reverse();
				}
			}
		}

		void GetInnerAndOuterEdgeList( List<TopoDS_Shape> faceShapeOtherList, List<TopoDS_Shape> faceShapeOuterList, List<TopoDS_Shape> faceShapeInnerList, out List<List<TopoDS_Edge>> edgeOuterList, out List<List<TopoDS_Edge>> edgeInnerList, out List<List<TopoDS_Shape>> faceList )
		{
			edgeOuterList = new List<List<TopoDS_Edge>>();
			edgeInnerList = new List<List<TopoDS_Edge>>();
			faceList = new List<List<TopoDS_Shape>>();

			// Get mapping between edge and the corresponding face in the sew shape.
			BRepBuilderAPI_Sewing bRepBuilderAPI_Sewing = SewFace( faceShapeOtherList );
			TopTools_IndexedDataMapOfShapeListOfShape topTools_IndexedDataMapOfShapeListOfShape = new TopTools_IndexedDataMapOfShapeListOfShape();
			TopExp.MapShapesAndAncestors( bRepBuilderAPI_Sewing.SewedShape(), TopAbs_ShapeEnum.TopAbs_EDGE, TopAbs_ShapeEnum.TopAbs_FACE, ref topTools_IndexedDataMapOfShapeListOfShape );

			// get all outer edges
			edgeOuterList = GetEdgeList( faceShapeOuterList );
			List<List<TopoDS_Edge>> tempEdgeInnerList = GetEdgeList( faceShapeInnerList );

			// find the all inner edges and inner/outer common Face
			for( int i = 0; i < edgeOuterList.Count; i++ ) {
				bool isSuccess = false;
				for( int j = 0; j < tempEdgeInnerList.Count; j++ ) {
					if( edgeInnerList.Contains( tempEdgeInnerList[ j ] ) ) {
						continue;
					}

					isSuccess = IsEdgeListCommonFace( edgeOuterList[ i ], tempEdgeInnerList[ j ], topTools_IndexedDataMapOfShapeListOfShape, out List<TopoDS_Shape> FaceShapeList );
					if( isSuccess == false ) {
						continue;
					}

					edgeInnerList.Add( tempEdgeInnerList[ j ] );
					faceList.Add( FaceShapeList );
					break;
				}
				if( isSuccess == false ) {
					edgeInnerList.Add( new List<TopoDS_Edge>() );
					faceList.Add( new List<TopoDS_Shape>() );
				}
			}
		}

		bool IsEdgeListCommonFace( List<TopoDS_Edge> edgeOuterList, List<TopoDS_Edge> edgeInnerList, TopTools_IndexedDataMapOfShapeListOfShape topTools_IndexedDataMapOfShapeListOfShape, out List<TopoDS_Shape> faceShapeList )
		{
			faceShapeList = new List<TopoDS_Shape>();

			List<TopoDS_Shape> faceShapeOuterList = new List<TopoDS_Shape>();
			List<TopoDS_Shape> faceShapeInnerList = new List<TopoDS_Shape>();

			// Traverse outer edge list
			// Check if the map contains the current outer edge
			// Get the list of faces connected to the outer edge
			// If the face doesn't exist, add it to FaceShapeOuterList
			faceShapeOuterList.AddRange(
				edgeOuterList
				.Where( outerEdge => topTools_IndexedDataMapOfShapeListOfShape.Contains( outerEdge ) )
				.SelectMany( outerEdge => topTools_IndexedDataMapOfShapeListOfShape.FindFromKey( outerEdge ).elementsAsList )
				.Where( outerFace => !faceShapeOuterList.Any( existingFace => existingFace.IsEqual( outerFace ) ) ) );

			// Traverse inner edge list
			// Check if the map contains the current inner edge
			// Get the list of faces connected to the inner edge
			// If the face doesn't exist, add it to FaceShapeInnerList
			faceShapeInnerList.AddRange(
				edgeInnerList
				.Where( innerEdge => topTools_IndexedDataMapOfShapeListOfShape.Contains( innerEdge ) )
				.SelectMany( innerEdge => topTools_IndexedDataMapOfShapeListOfShape.FindFromKey( innerEdge ).elementsAsList )
				.Where( innerFace => !faceShapeInnerList.Any( existingFace => existingFace.IsEqual( innerFace ) ) ) );

			bool isSuccess = faceShapeOuterList.Any( outerShape => faceShapeInnerList.Any( innerShape => outerShape.IsEqual( innerShape ) ) );
			faceShapeList.AddRange( faceShapeOuterList );

			return isSuccess;
		}

		List<Geom2d_Geometry> GetCutOffPathInnerGoemetry( List<Geom2d_PolyLine> polyLineList, List<int> nCutOffEdgeList, List<List<TopoDS_Edge>> edgeInnerList, ITube Tube, List<double> YPositionList )
		{
			List<Geom2d_Geometry> geometryInnerList = Enumerable.Repeat<Geom2d_Geometry>( null, polyLineList.Count ).ToList();
			for( int i = 0; i < nCutOffEdgeList.Count; i++ ) {
				int nIndex = nCutOffEdgeList[ i ];

				if( nIndex >= edgeInnerList.Count ) {
					continue;
				}
				Geom2d_Geometry innerGeom = OCCTranslator.GetPolyFromTopoEdge( edgeInnerList[ nIndex ], Tube );
				if( innerGeom == null ) {
					continue;
				}
				innerGeom.OffSet( 0, -YPositionList[ nIndex ] );
				geometryInnerList[ nIndex ] = innerGeom;
			}
			return geometryInnerList;
		}

		bool GetTubeThickess( List<TopoDS_Shape> faceShapeOuterList, List<TopoDS_Shape> faceShapeInnerList, out double dThickness )
		{
			dThickness = 0;
			if( faceShapeOuterList == null || faceShapeInnerList == null || faceShapeOuterList.Count == 0 || faceShapeInnerList.Count == 0 ) {
				return false;
			}

			// Compound shape
			TopoDS_Shape shapeOuterCompound = OCCTranslator.CombineTopoShape( faceShapeOuterList );
			TopoDS_Shape shapeInnerCompound = OCCTranslator.CombineTopoShape( faceShapeInnerList );

			if( shapeOuterCompound.IsNull() || shapeInnerCompound.IsNull() ) {
				return false;
			}

			BoundingBox boxOuterCompound = GetRoughBox( shapeOuterCompound );
			BoundingBox boxInnerCompound = GetRoughBox( shapeInnerCompound );

			dThickness = Math.Round( boxOuterCompound.Zmax - boxInnerCompound.Zmax, ValueConstrain.DECIMAL_Place );
			bool isSame = Comparer.IsSameValue( dThickness, 0 );
			if( isSame ) {
				return false;
			}

			return true;
		}
	}
}
