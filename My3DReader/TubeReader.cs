using MyUtility.General;
using MyUtility.MyOCC;
using OCC.BRep;
using OCC.BRepAdaptor;
using OCC.BRepBuilderAPI;
using OCC.BRepExtrema;
using OCC.BRepTools;
using OCC.Geom;
using OCC.GeomAPI;
using OCC.gp;
using OCC.ShapeAnalysis;
using OCC.TopAbs;
using OCC.TopExp;
using OCC.TopoDS;
using OCC.TopTools;
using System.Collections.Generic;
using System.Linq;

namespace TubeCuttingUI
{
	// TODO: move to someother place
	public class FeatureData
	{
		public FeatureData( List<TopoDS_Edge> outerWire, List<TopoDS_Edge> innerWire, List<TopoDS_Shape> featureShell )
		{
			OuterWire = outerWire;
			InnerWire = innerWire;
			FeatureShell = featureShell;
		}

		public List<TopoDS_Edge> OuterWire
		{
			get;
		}

		public List<TopoDS_Edge> InnerWire
		{
			get;
		}

		public List<TopoDS_Shape> FeatureShell
		{
			get;
		}
	}

	public class TubeReader
	{
		public void ReadTubeInformation( TopoDS_Shape oneShape,
			out FeatureData head, out FeatureData tail, out List<FeatureData> features )
		{
			head = null;
			tail = null;
			features = null;

			try {
				// sew original shape to avoid edge not common
				oneShape = OCCHelper.SewShape( oneShape );
				BoundingBox boundingBox = OCCHelper.GetBoundingBox( oneShape );

				// get all faces from shape
				List<TopoDS_Shape> shapeList = GetFaceListFromShape( oneShape );

				// get all outer, inner and feature faces
				FilterShape( shapeList, boundingBox,
					out List<TopoDS_Shape> faceShapeOuterWallList, out List<TopoDS_Shape> faceShapeInnerWallList, out List<TopoDS_Shape> faceShapeFeatureList );

				// get all edges on the inner and outer
				GetFeatureWireList( faceShapeFeatureList, faceShapeOuterWallList, faceShapeInnerWallList,
					out List<List<TopoDS_Edge>> wireOuterList, out List<List<TopoDS_Edge>> wireInnerList, out List<List<TopoDS_Shape>> fearureShellList );

				// find the head and tail end index
				FindIndexOfHeadAndTail( fearureShellList, out int nHeadIndex, out int nTailIndex, out List<BoundingBox> shapeBoxList );

				// get head and tail feature
				head = new FeatureData( wireOuterList[ nHeadIndex ], wireInnerList[ nHeadIndex ], fearureShellList[ nHeadIndex ] );
				tail = new FeatureData( wireOuterList[ nTailIndex ], wireInnerList[ nTailIndex ], fearureShellList[ nTailIndex ] );

				// get all feature
				features = new List<FeatureData>();
				for( int i = 0; i < fearureShellList.Count; i++ ) {
					if( i == nHeadIndex || i == nTailIndex ) {
						continue;
					}
					features.Add( new FeatureData( wireOuterList[ i ], wireInnerList[ i ], fearureShellList[ i ] ) );
				}
			}
			catch {
				return;
			}
		}

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

		void FilterShape( List<TopoDS_Shape> shapeList, BoundingBox boundingBoxParameter,
			out List<TopoDS_Shape> faceShapeOuterWallList, out List<TopoDS_Shape> faceShapeInnerWallList, out List<TopoDS_Shape> faceShapeFeatureList )
		{
			List<gp_Vec> cutPlaneNormalVectorList = CreateCutPlaneNormalVector( boundingBoxParameter );

			faceShapeOuterWallList = new List<TopoDS_Shape>();
			faceShapeInnerWallList = new List<TopoDS_Shape>();
			faceShapeFeatureList = new List<TopoDS_Shape>();

			for( int i = 0; i < cutPlaneNormalVectorList.Count; i++ ) {
				TopoDS_Face cutPlane = MakePlaneFace( cutPlaneNormalVectorList[ i ], new gp_Pnt( 0, 0, 0 ) );
				List<TopoDS_Shape> tempOuterFaceShapeList = FindOuterOrInnerFaceByCutPlane( shapeList, cutPlane, cutPlaneNormalVectorList[ i ], true );
				List<TopoDS_Shape> tempInnerFaceShapeList = FindOuterOrInnerFaceByCutPlane( shapeList, cutPlane, cutPlaneNormalVectorList[ i ], false );

				// Collect InnerFaceShapeList and OuterFaceShapeList
				faceShapeOuterWallList.AddRange( tempOuterFaceShapeList );
				faceShapeInnerWallList.AddRange( tempInnerFaceShapeList );
			}

			// remove repeat face
			faceShapeOuterWallList = faceShapeOuterWallList.Distinct().ToList();
			faceShapeInnerWallList = faceShapeInnerWallList.Distinct().ToList();

			for( int i = 0; i < shapeList.Count; i++ ) {

				if( faceShapeOuterWallList.Contains( shapeList[ i ] ) ) {
					continue;
				}

				if( faceShapeInnerWallList.Contains( shapeList[ i ] ) ) {
					continue;
				}

				faceShapeFeatureList.Add( shapeList[ i ] );
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

		// TODO: make it better
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

		void GetFeatureWireList( List<TopoDS_Shape> faceShapeFeature, List<TopoDS_Shape> faceShapeOuterWallList, List<TopoDS_Shape> faceShapeInnerWallList,
			out List<List<TopoDS_Edge>> featureWireOuterList, out List<List<TopoDS_Edge>> featureWireInnerList, out List<List<TopoDS_Shape>> featureShellList )
		{
			featureWireOuterList = new List<List<TopoDS_Edge>>();
			featureWireInnerList = new List<List<TopoDS_Edge>>();
			featureShellList = new List<List<TopoDS_Shape>>();

			// Get mapping between edge and the corresponding face in the sew shape
			// TODO: dont know shit here
			BRepBuilderAPI_Sewing bRepBuilderAPI_Sewing = SewFace( faceShapeFeature );
			TopTools_IndexedDataMapOfShapeListOfShape edgeFaceMap = new TopTools_IndexedDataMapOfShapeListOfShape();
			TopExp.MapShapesAndAncestors( bRepBuilderAPI_Sewing.SewedShape(), TopAbs_ShapeEnum.TopAbs_EDGE, TopAbs_ShapeEnum.TopAbs_FACE, ref edgeFaceMap );

			// get all outer edges
			featureWireOuterList = GetWiresFromShell( faceShapeOuterWallList );
			List<List<TopoDS_Edge>> tempFeatureWireInnerList = GetWiresFromShell( faceShapeInnerWallList );

			// find the all inner edges and inner/outer common Face
			for( int i = 0; i < featureWireOuterList.Count; i++ ) {
				bool isSuccess = false;
				for( int j = 0; j < tempFeatureWireInnerList.Count; j++ ) {
					if( featureWireInnerList.Contains( tempFeatureWireInnerList[ j ] ) ) {
						continue;
					}

					isSuccess = IsWireOnSameShell( featureWireOuterList[ i ], tempFeatureWireInnerList[ j ], edgeFaceMap, out List<TopoDS_Shape> featureShell );
					if( isSuccess == false ) {
						continue;
					}

					featureWireInnerList.Add( tempFeatureWireInnerList[ j ] );
					featureShellList.Add( featureShell );
					break;
				}
				if( isSuccess == false ) {

					// to make sure all lists have the same index
					featureWireInnerList.Add( new List<TopoDS_Edge>() );
					featureShellList.Add( new List<TopoDS_Shape>() );
				}
			}
		}

		BRepBuilderAPI_Sewing SewFace( List<TopoDS_Shape> ShapeList )
		{
			BRepBuilderAPI_Sewing sewer = new BRepBuilderAPI_Sewing( OCCHelper.ERROR_VALUE );
			for( int i = 0; i < ShapeList.Count; i++ ) {
				sewer.Add( ShapeList[ i ] );
			}
			sewer.Perform();
			return sewer;
		}

		// TODO: this is a find wire from edge algorithm
		List<List<TopoDS_Edge>> GetWiresFromShell( List<TopoDS_Shape> ShapeList )
		{
			// sew to shell
			BRepBuilderAPI_Sewing sewer = SewFace( ShapeList );
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
					List<TopoDS_Edge> wire = EdgeList[ nLastIndex ];
					TopoDS_Edge edge = EdgeList[ 0 ][ j ];
					bCanFitInWire = CheckEdgeCanFitInWire( ref wire, ref edge );
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

		bool CheckEdgeCanFitInWire( ref List<TopoDS_Edge> EdgeList, ref TopoDS_Edge TargetEdge )
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

		bool IsWireOnSameShell( List<TopoDS_Edge> wireOuter, List<TopoDS_Edge> wireInner, TopTools_IndexedDataMapOfShapeListOfShape edgeFaceMap,
			out List<TopoDS_Shape> featureShell )
		{
			featureShell = new List<TopoDS_Shape>();

			List<TopoDS_Shape> faceConnectedOuterWire = FindConnectedFaceList( wireOuter, edgeFaceMap );
			List<TopoDS_Shape> faceConnectedInnerWire = FindConnectedFaceList( wireInner, edgeFaceMap );

			bool isSuccess = faceConnectedOuterWire.Any( faceFormOuterWire => faceConnectedInnerWire.Any( faceFromInnerWire => faceFormOuterWire.IsEqual( faceFromInnerWire ) ) );
			featureShell.AddRange( faceConnectedOuterWire );
			return isSuccess;
		}

		List<TopoDS_Shape> FindConnectedFaceList( List<TopoDS_Edge> wire, TopTools_IndexedDataMapOfShapeListOfShape edgeFaceMap )
		{
			// Traverse wire
			return wire

				// Check if the map contains the current outer edge
				.Where( oneOuterEdge => edgeFaceMap.Contains( oneOuterEdge ) )

				// Get the list of faces connected to the outer edge
				.SelectMany( oneFeatureOuterEdge => edgeFaceMap.FindFromKey( oneFeatureOuterEdge ).elementsAsList )

				// remove the repeat face
				.Distinct().ToList();
		}

		void FindIndexOfHeadAndTail( List<List<TopoDS_Shape>> featureShellList, out int nHeadIndex, out int nTailIndex, out List<BoundingBox> shapeBoxList )
		{
			shapeBoxList = new List<BoundingBox>();
			for( int i = 0; i < featureShellList.Count; i++ ) {
				shapeBoxList.Add( GetBoundingBox( featureShellList[ i ] ) );
			}

			double MinY = double.MaxValue;
			double MaxY = double.MinValue;
			nHeadIndex = 0;
			nTailIndex = 0;

			for( int i = 0; i < shapeBoxList.Count; i++ ) {
				if( shapeBoxList[ i ].MinY < MinY ) {
					MinY = shapeBoxList[ i ].MinY;
					nHeadIndex = i;
				}
				if( shapeBoxList[ i ].MaxY > MaxY ) {
					MaxY = shapeBoxList[ i ].MaxY;
					nTailIndex = i;
				}
			}
		}

		BoundingBox GetBoundingBox( List<TopoDS_Shape> ShapeList )
		{
			TopoDS_Shape compound = OCCHelper.MakeCompound( ShapeList );
			return OCCHelper.GetBoundingBox( compound );
		}
	}
}
