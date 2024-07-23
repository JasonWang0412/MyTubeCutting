using MyCAMCore;
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

namespace My3DReader
{
	public class TubeReader
	{
		public bool ReadTubeInformation( TopoDS_Shape rawShape,
			out CADFeatureData head, out CADFeatureData tail, out List<CADFeatureData> features )
		{
			head = null;
			tail = null;
			features = null;

			// data protection
			if( rawShape == null ) {
				return false;
			}

			try {
				// sew original shape to avoid edge not common
				rawShape = OCCHelper.SewShape( rawShape );
				BoundingBox boundingBox = OCCHelper.GetBoundingBox( rawShape );
				if( boundingBox == null ) {
					return false;
				}

				// get all faces from shape
				List<TopoDS_Shape> allFaceList = GetFaceListFromShape( rawShape );
				if( allFaceList == null || allFaceList.Count == 0 ) {
					return false;
				}

				// get all outer, inner and feature faces
				FilterShape( allFaceList, boundingBox,
					out List<TopoDS_Shape> faceOuterWallList, out List<TopoDS_Shape> faceInnerWallList, out List<TopoDS_Shape> faceFeatureList );

				// we need at least one outer wall
				if( faceOuterWallList.Count == 0 ) {
					return false;
				}

				// get all feature wire (path) and shell
				GetFeatureWireList( faceFeatureList, faceOuterWallList, faceInnerWallList,
					out List<TopoDS_Wire> wireOuterList, out List<TopoDS_Wire> wireInnerList, out List<TopoDS_Shell> fearureShellList );

				// we need at least two outer wire
				if( wireOuterList.Count < 2 ) {
					return false;
				}

				// find the head and tail end index
				FindIndexOfHeadAndTail( fearureShellList, out int nHeadIndex, out int nTailIndex, out List<BoundingBox> shapeBoxList );

				// get head and tail feature
				head = new CADFeatureData( wireOuterList[ nHeadIndex ], wireInnerList[ nHeadIndex ], fearureShellList[ nHeadIndex ] );
				tail = new CADFeatureData( wireOuterList[ nTailIndex ], wireInnerList[ nTailIndex ], fearureShellList[ nTailIndex ] );

				// get all feature
				features = new List<CADFeatureData>();
				for( int i = 0; i < fearureShellList.Count; i++ ) {
					if( i == nHeadIndex || i == nTailIndex ) {
						continue;
					}
					features.Add( new CADFeatureData( wireOuterList[ i ], wireInnerList[ i ], fearureShellList[ i ] ) );
				}
				return true;
			}
			catch {
				return false;
			}
		}

		List<TopoDS_Shape> GetFaceListFromShape( TopoDS_Shape shape )
		{
			List<TopoDS_Shape> allFaceList = new List<TopoDS_Shape>();
			TopExp_Explorer faceExplorer = new TopExp_Explorer( shape, TopAbs_ShapeEnum.TopAbs_FACE );
			while( faceExplorer.More() ) {
				allFaceList.Add( faceExplorer.Current() );
				faceExplorer.Next();
			}
			return allFaceList;
		}

		void FilterShape( List<TopoDS_Shape> allFaceList, BoundingBox boundingBoxParameter,
			out List<TopoDS_Shape> faceOuterWallList, out List<TopoDS_Shape> faceInnerWallList, out List<TopoDS_Shape> faceFeatureList )
		{
			List<gp_Vec> cutPlaneNormalVectorList = CreateCutPlaneNormalVector( boundingBoxParameter );

			faceOuterWallList = new List<TopoDS_Shape>();
			faceInnerWallList = new List<TopoDS_Shape>();
			faceFeatureList = new List<TopoDS_Shape>();

			for( int i = 0; i < cutPlaneNormalVectorList.Count; i++ ) {
				TopoDS_Face cutPlane = MakePlaneFace( cutPlaneNormalVectorList[ i ], new gp_Pnt( 0, 0, 0 ) );
				if( cutPlane == null ) {
					continue;
				}
				List<TopoDS_Shape> tempOuterFaceList = FindWallFaceByCutPlane( allFaceList, cutPlane, cutPlaneNormalVectorList[ i ], true );
				List<TopoDS_Shape> tempInnerFaceList = FindWallFaceByCutPlane( allFaceList, cutPlane, cutPlaneNormalVectorList[ i ], false );

				// Collect InnerFaceShapeList and OuterFaceShapeList
				faceOuterWallList.AddRange( tempOuterFaceList );
				faceInnerWallList.AddRange( tempInnerFaceList );
			}

			// remove repeat face
			faceOuterWallList = faceOuterWallList.Distinct().ToList();
			faceInnerWallList = faceInnerWallList.Distinct().ToList();

			for( int i = 0; i < allFaceList.Count; i++ ) {

				if( faceOuterWallList.Contains( allFaceList[ i ] ) ) {
					continue;
				}

				if( faceInnerWallList.Contains( allFaceList[ i ] ) ) {
					continue;
				}

				faceFeatureList.Add( allFaceList[ i ] );
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
			if( FaceMaker.IsDone() == false ) {
				return null;
			}
			return FaceMaker.Face();
		}

		List<TopoDS_Shape> FindWallFaceByCutPlane( List<TopoDS_Shape> allFaceList, TopoDS_Face cutPlane, gp_Vec cutPlaneNormalVector, bool isFindOuter )
		{
			List<Geom_Curve> intersectLineList = new List<Geom_Curve>();
			List<TopoDS_Shape> possibleWallFaceList = new List<TopoDS_Shape>();
			List<TopoDS_Shape> tubeWallFaceList = new List<TopoDS_Shape>();

			for( int i = 0; i < allFaceList.Count; i++ ) {
				if( allFaceList[ i ].ShapeType() != TopAbs_ShapeEnum.TopAbs_FACE ) {
					continue;
				}
				TopoDS_Face oneFace = TopoDS.ToFace( allFaceList[ i ] );

				// create intersector list for all shapes
				GeomAPI_IntSS intSS = GetIntSS( oneFace, cutPlane );
				if( intSS == null ) {
					continue;
				}

				// check if the face is possible tube wall
				if( IsFacePossibleTubeWall( intSS, cutPlaneNormalVector, oneFace ) == false ) {
					continue;
				}
				intersectLineList.Add( intSS.Line( 1 ) );
				possibleWallFaceList.Add( allFaceList[ i ] );
			}

			if( intersectLineList.Count == 0 ) {
				return tubeWallFaceList;
			}

			// keep intersection line that is furthest/nearest from y axis
			List<int> OuterShapeIndexList = GetExtremaIntSSDisIndex( intersectLineList, isFindOuter );

			foreach( int Index in OuterShapeIndexList ) {
				tubeWallFaceList.Add( possibleWallFaceList[ Index ] );
			}

			return tubeWallFaceList;
		}

		GeomAPI_IntSS GetIntSS( TopoDS_Face theFace, TopoDS_Face cutPlane )
		{
			Geom_Surface theFaceSurface = BRep_Tool.Surface( theFace );
			Geom_Surface cutPlaneSurface = BRep_Tool.Surface( cutPlane );
			if( theFaceSurface == null || cutPlaneSurface == null ) {
				return null;
			}
			GeomAPI_IntSS intSS;

			try {
				double Umin = 0;
				double Umax = 0;
				double Vmin = 0;
				double Vmax = 0;
				BRepTools.UVBounds( theFace, ref Umin, ref Umax, ref Vmin, ref Vmax );
				Geom_RectangularTrimmedSurface trimmedFaceSurface = new Geom_RectangularTrimmedSurface( theFaceSurface, Umin, Umax, Vmin, Vmax );

				BRepTools.UVBounds( cutPlane, ref Umin, ref Umax, ref Vmin, ref Vmax );
				Geom_RectangularTrimmedSurface trimmedCutPlaneSurface = new Geom_RectangularTrimmedSurface( cutPlaneSurface, Umin, Umax, Vmin, Vmax );

				intSS = new GeomAPI_IntSS( trimmedFaceSurface, trimmedCutPlaneSurface, OCCHelper.ERROR_VALUE );
			}
			catch {
				intSS = null;
			}
			return intSS;
		}

		bool IsFacePossibleTubeWall( GeomAPI_IntSS intSS, gp_Vec cutPlaneNormalVector, TopoDS_Face theFace )
		{
			if( intSS == null ) {
				return false;
			}

			// keep shapes that have intersection with cut plane
			if( intSS.NbLines() == 0 ) {
				return false;
			}

			// check the intersection property
			Geom_Curve IntersectLine = intSS.Line( 1 );
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
				if( edgeMakerIntSS.IsDone() == false ) {
					continue;
				}
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
				if( edgeMakerIntSS.IsDone() == false ) {
					continue;
				}
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

		void GetFeatureWireList( List<TopoDS_Shape> faceFeatureList, List<TopoDS_Shape> faceOuterWallList, List<TopoDS_Shape> faceInnerWallList,
			out List<TopoDS_Wire> featureWireOuterList, out List<TopoDS_Wire> featureWireInnerList, out List<TopoDS_Shell> featureShellList )
		{
			featureWireOuterList = GetWiresFromShell( faceOuterWallList );
			featureWireInnerList = new List<TopoDS_Wire>();
			featureShellList = new List<TopoDS_Shell>();

			// make list have the same count
			for( int i = 0; i < featureWireOuterList.Count; i++ ) {
				featureWireInnerList.Add( null );
				featureShellList.Add( null );
			}

			// if there is no inner dace or feature face
			if( faceInnerWallList.Count == 0 || faceFeatureList.Count == 0 ) {
				return;
			}

			// get all inner wires
			List<TopoDS_Wire> tempFeatureWireInnerList = GetWiresFromShell( faceInnerWallList );
			if( tempFeatureWireInnerList.Count == 0 ) {
				return;
			}

			// get mapping between edge and the corresponding face in the sew shape
			TopTools_IndexedDataMapOfShapeListOfShape edgeFaceMap = new TopTools_IndexedDataMapOfShapeListOfShape();
			TopoDS_Shape shellCompound = SewFaceToShells( faceFeatureList );
			if( shellCompound == null ) {
				return;
			}
			TopExp.MapShapesAndAncestors( shellCompound, TopAbs_ShapeEnum.TopAbs_EDGE, TopAbs_ShapeEnum.TopAbs_FACE, ref edgeFaceMap );

			// find the all inner edges and inner/outer common Face
			for( int i = 0; i < featureWireOuterList.Count; i++ ) {
				for( int j = 0; j < tempFeatureWireInnerList.Count; j++ ) {
					if( featureWireInnerList.Contains( tempFeatureWireInnerList[ j ] ) ) {
						continue;
					}
					bool isSuccess = IsWireOnSameShell( featureWireOuterList[ i ], tempFeatureWireInnerList[ j ], edgeFaceMap, out TopoDS_Shell featureShell );
					if( isSuccess == false || featureShell == null ) {
						continue;
					}
					featureWireInnerList[ i ] = tempFeatureWireInnerList[ j ];
					featureShellList[ i ] = featureShell;
					break;
				}
			}
		}

		TopoDS_Shape SewFaceToShells( List<TopoDS_Shape> faceList )
		{
			try {
				BRepBuilderAPI_Sewing sewer = new BRepBuilderAPI_Sewing( OCCHelper.ERROR_VALUE );
				for( int i = 0; i < faceList.Count; i++ ) {
					sewer.Add( faceList[ i ] );
				}
				sewer.Perform();
				return sewer.SewedShape();
			}
			catch {
				return null;
			}
		}

		List<TopoDS_Wire> GetWiresFromShell( List<TopoDS_Shape> shellFaceList )
		{
			// make a compound
			TopoDS_Shape shell = OCCHelper.MakeCompound( shellFaceList );

			// get all closed wires on the shell
			ShapeAnalysis_FreeBounds freeBounds = new ShapeAnalysis_FreeBounds( shell, OCCHelper.ERROR_VALUE );
			TopoDS_Compound closedWires = freeBounds.GetClosedWires();

			// extract wires from compound
			List<TopoDS_Wire> wireList = new List<TopoDS_Wire>();
			TopExp_Explorer wireExplorer = new TopExp_Explorer( closedWires, TopAbs_ShapeEnum.TopAbs_WIRE );
			while( wireExplorer.More() ) {
				wireList.Add( TopoDS.ToWire( wireExplorer.Current() ) );
				wireExplorer.Next();
			}
			return wireList;
		}

		bool IsWireOnSameShell( TopoDS_Wire wireOuter, TopoDS_Wire wireInner, TopTools_IndexedDataMapOfShapeListOfShape edgeFaceMap,
			out TopoDS_Shell featureShell )
		{
			List<TopoDS_Shape> faceConnectedOuterWire = FindConnectedFaceList( wireOuter, edgeFaceMap );
			List<TopoDS_Shape> faceConnectedInnerWire = FindConnectedFaceList( wireInner, edgeFaceMap );
			bool isSuccess = faceConnectedOuterWire.Any( faceFormOuterWire => faceConnectedInnerWire.Any( faceFromInnerWire => faceFormOuterWire.IsEqual( faceFromInnerWire ) ) );
			if( isSuccess == false ) {
				featureShell = null;
				return false;
			}

			// build the shell if the wire is on the same shell
			featureShell = TopoDS.ToShell( OCCHelper.MakeShell( faceConnectedOuterWire ) );
			if( featureShell == null ) {
				return false;
			}
			return true;
		}

		List<TopoDS_Shape> FindConnectedFaceList( TopoDS_Wire wire, TopTools_IndexedDataMapOfShapeListOfShape edgeFaceMap )
		{
			// extract edges from wire
			return wire.elementsAsList

				// check if the map contains the current outer edge
				.Where( oneEdge => edgeFaceMap.Contains( oneEdge ) )

				// get the list of faces connected to the outer edge
				.SelectMany( oneFeatureOuterEdge => edgeFaceMap.FindFromKey( oneFeatureOuterEdge ).elementsAsList )

				// remove the repeat face
				.Distinct().ToList();
		}

		void FindIndexOfHeadAndTail( List<TopoDS_Shell> featureShellList, out int nHeadIndex, out int nTailIndex, out List<BoundingBox> shapeBoxList )
		{
			shapeBoxList = new List<BoundingBox>();
			for( int i = 0; i < featureShellList.Count; i++ ) {
				shapeBoxList.Add( OCCHelper.GetBoundingBox( featureShellList[ i ] ) );
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
	}
}
