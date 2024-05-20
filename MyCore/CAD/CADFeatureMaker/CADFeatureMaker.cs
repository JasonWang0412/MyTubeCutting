using MyCore.Tool;
using OCC.AIS;
using OCC.BRep;
using OCC.BRepAlgoAPI;
using OCC.BRepBuilderAPI;
using OCC.BRepPrimAPI;
using OCC.BRepTools;
using OCC.Geom;
using OCC.gp;
using OCC.Graphic3d;
using OCC.Quantity;
using OCC.TopAbs;
using OCC.TopExp;
using OCC.TopoDS;
using System.Collections.Generic;
using System.Linq;

namespace MyCore.CAD
{
	public class CADFeatureMaker
	{
		public static TopoDS_Shape MakeResultTube( CADFeatureParamMap map )
		{
			// data protection
			if( map == null ) {
				return null;
			}

			CADft_MainTubeParam mainTubeParam = map.MainTubeParam;
			List<CADft_EndCutterParam> endCutterParamList = map.FeatureMap.Values
				.Where( param => param.Type == CADFeatureType.EndCutter )
				.Select( endCutterParam => endCutterParam as CADft_EndCutterParam ).ToList();
			List<CADft_BranchTubeParam> branchTubeParamList = map.FeatureMap.Values
				.Where( param => param.Type == CADFeatureType.BranchTube )
				.Select( branchTubeParam => branchTubeParam as CADft_BranchTubeParam ).ToList();

			// data protection
			if( mainTubeParam == null ) {
				return null;
			}
			if( endCutterParamList == null ) {
				endCutterParamList = new List<CADft_EndCutterParam>();
			}
			if( branchTubeParamList == null ) {
				branchTubeParamList = new List<CADft_BranchTubeParam>();
			}

			// make main tube
			TopoDS_Shape mainTube = MakeMainTube( mainTubeParam );
			if( mainTube == null ) {
				return null;
			}

			// cut main tube by end cutters
			List<TopoDS_Shape> endCutters = MakeEndCutters( endCutterParamList );
			if( endCutters != null && endCutters.Count != 0 ) {
				foreach( TopoDS_Shape endCutter in endCutters ) {
					BRepAlgoAPI_Cut cut = new BRepAlgoAPI_Cut( mainTube, endCutter );
					if( cut.IsDone() == false ) {
						continue;
					}
					mainTube = cut.Shape();
				}
			}

			// cut main tube by branch tubes
			List<TopoDS_Shape> branchTubes = MakeBranchTubes( branchTubeParamList, mainTubeParam );
			if( branchTubes != null && branchTubes.Count != 0 ) {
				foreach( TopoDS_Shape branchTube in branchTubes ) {

					// find all shapes in branchTube if it is compound
					// u'll meet some bug (from OCC maybe) if u use compound shape directly
					if( branchTube.ShapeType() == TopAbs_ShapeEnum.TopAbs_COMPOUND ) {
						foreach( TopoDS_Shape oneBranchTube in branchTube.elementsAsList ) {
							BRepAlgoAPI_Cut cut = new BRepAlgoAPI_Cut( mainTube, oneBranchTube );
							if( cut.IsDone() == false ) {
								continue;
							}
							mainTube = cut.Shape();
						}
					}
					else {
						BRepAlgoAPI_Cut cut = new BRepAlgoAPI_Cut( mainTube, branchTube );
						if( cut.IsDone() == false ) {
							continue;
						}
						mainTube = cut.Shape();
					}
				}
			}

			return mainTube;
		}

		public static AIS_Shape MakeCADFeatureAIS( ICADFeatureParam cadFeatureParam, CADft_MainTubeParam mainTubeParam )
		{
			AIS_Shape cadFeatureAIS = null;
			if( cadFeatureParam.Type == CADFeatureType.EndCutter ) {
				cadFeatureAIS = MakeEndCutterAIS( (CADft_EndCutterParam)cadFeatureParam, mainTubeParam );
			}
			else if( cadFeatureParam.Type == CADFeatureType.BranchTube ) {
				cadFeatureAIS = new AIS_Shape( MakeBranchTube( (CADft_BranchTubeParam)cadFeatureParam, mainTubeParam ) );
			}
			else {
				return cadFeatureAIS;
			}

			if( cadFeatureAIS == null ) {
				return null;
			}

			Graphic3d_MaterialAspect aspect = new Graphic3d_MaterialAspect( Graphic3d_NameOfMaterial.Graphic3d_NOM_STONE );
			aspect.SetTransparency( 0.5f );
			aspect.SetColor( new Quantity_Color( Quantity_NameOfColor.Quantity_NOC_GREEN4 ) );
			cadFeatureAIS.SetMaterial( aspect );
			cadFeatureAIS.SetDisplayMode( 1 );
			return cadFeatureAIS;
		}

		static TopoDS_Shape MakeMainTube( CADft_MainTubeParam mainTubeParam )
		{
			// data protection
			if( mainTubeParam == null ) {
				return null;
			}
			if( mainTubeParam.IsValid() == false ) {
				return null;
			}

			// Make outer wire and inner wire
			gp_Pnt center = new gp_Pnt( 0, 0, 0 );
			gp_Dir dir = new gp_Dir( 0, 1, 0 );
			TopoDS_Wire outerWire = OCCTool.MakeBaseWire( mainTubeParam.CrossSection.Shape, 0, center, dir, 0 );
			TopoDS_Wire innerWire = OCCTool.MakeBaseWire( mainTubeParam.CrossSection.Shape, mainTubeParam.CrossSection.Thickness, center, dir, 0 );
			if( outerWire == null || innerWire == null ) {
				return null;
			}

			// Get solid shape by wire
			return OCCTool.MakeRawTubeShape( outerWire, innerWire, mainTubeParam.Length );
		}

		static List<TopoDS_Shape> MakeEndCutters( List<CADft_EndCutterParam> endCutterParamList )
		{
			// data protection
			if( endCutterParamList == null ) {
				return null;
			}

			List<TopoDS_Shape> cutters = new List<TopoDS_Shape>();
			foreach( CADft_EndCutterParam endCutterParam in endCutterParamList ) {
				TopoDS_Shape oneEndCutter = MakeEndCutter( endCutterParam );
				if( oneEndCutter == null ) {
					continue;
				}
				cutters.Add( oneEndCutter );
			}
			return cutters;
		}

		static TopoDS_Shape MakeEndCutter( CADft_EndCutterParam endCutterParam )
		{
			// data protection
			if( endCutterParam == null ) {
				return null;
			}
			if( endCutterParam.IsValid() == false ) {
				return null;
			}

			// get plane
			TopoDS_Face thePlane = MakeEndCutterFace( endCutterParam );
			if( thePlane == null ) {
				return null;
			}

			// get point on cut side
			double dYpos = endCutterParam.Side == EEndSide.Left ? endCutterParam.Center_Y - 1 : endCutterParam.Center_Y + 1;

			// make cutter half space
			gp_Pnt pointOnCutSide = new gp_Pnt( 0, dYpos, 0 );
			BRepPrimAPI_MakeHalfSpace halfSpace = new BRepPrimAPI_MakeHalfSpace( thePlane, pointOnCutSide );
			if( halfSpace.IsDone() == false ) {
				return null;
			}
			return halfSpace.Shape();
		}

		static AIS_Shape MakeEndCutterAIS( CADft_EndCutterParam endCutterParam, CADft_MainTubeParam mainTubeParam )
		{
			// data protection
			if( endCutterParam == null || mainTubeParam == null ) {
				return null;
			}
			if( endCutterParam.IsValid() == false || mainTubeParam.IsValid() == false ) {
				return null;
			}

			// make the face
			TopoDS_Face thePlane = MakeEndCutterFace( endCutterParam );
			if( thePlane == null ) {
				return null;
			}

			// make the extend bounding box of main tube
			TopoDS_Shape extendBndBox = MakeExtendBoundingBox( mainTubeParam );
			if( extendBndBox == null ) {
				return null;
			}

			// find the common part of the face and the extend bounding box
			BRepAlgoAPI_Common common = new BRepAlgoAPI_Common( thePlane, extendBndBox );
			if( common.IsDone() == false ) {
				return null;
			}

			// find the face inside the extend bounding box
			TopoDS_Face commonFace;
			TopExp_Explorer explorer = new TopExp_Explorer( common.Shape(), TopAbs_ShapeEnum.TopAbs_FACE );
			if( explorer.More() ) {
				commonFace = TopoDS.ToFace( explorer.Current() );
			}
			else {
				return null;
			}

			// retrive the geom surface and uv boundary from the face
			// this is to make the face a complete face (ex: tiled -45, rotate 45, retangular tube)
			Geom_Surface commonSurface = BRep_Tool.Surface( commonFace );
			double Umin = 0;
			double Umax = 0;
			double Vmin = 0;
			double Vmax = 0;
			BRepTools.UVBounds( commonFace, ref Umin, ref Umax, ref Vmin, ref Vmax );
			Geom_RectangularTrimmedSurface refinedSurface = new Geom_RectangularTrimmedSurface( commonSurface, Umin, Umax, Vmin, Vmax );
			BRepBuilderAPI_MakeFace refinedFaceMaker = new BRepBuilderAPI_MakeFace( refinedSurface, 0.001 );

			return new AIS_Shape( refinedFaceMaker.Face() );
		}

		static TopoDS_Face MakeEndCutterFace( CADft_EndCutterParam endCutterParam )
		{
			// data protection
			if( endCutterParam == null ) {
				return null;
			}
			if( endCutterParam.IsValid() == false ) {
				return null;
			}

			gp_Pnt center = new gp_Pnt( endCutterParam.Center_X, endCutterParam.Center_Y, endCutterParam.Center_Z );
			OCCTool.GetEndCutterDir( endCutterParam.TiltAngle_deg, endCutterParam.RotateAngle_deg, out gp_Dir dir );
			gp_Pln cutPlane = new gp_Pln( center, dir );
			BRepBuilderAPI_MakeFace makeFace = new BRepBuilderAPI_MakeFace( cutPlane );
			if( makeFace.IsDone() == false ) {
				return null;
			}
			return makeFace.Face();
		}

		static List<TopoDS_Shape> MakeBranchTubes( List<CADft_BranchTubeParam> branchTubeParamList, CADft_MainTubeParam mainTubeParam )
		{
			// data protection
			if( branchTubeParamList == null ) {
				return null;
			}

			List<TopoDS_Shape> branchTubes = new List<TopoDS_Shape>();
			foreach( CADft_BranchTubeParam branchTubeParam in branchTubeParamList ) {
				TopoDS_Shape oneBranchTube = MakeBranchTube( branchTubeParam, mainTubeParam );
				if( oneBranchTube == null ) {
					continue;
				}
				branchTubes.Add( oneBranchTube );
			}
			return branchTubes;
		}

		static TopoDS_Shape MakeBranchTube( CADft_BranchTubeParam branchTubeParam, CADft_MainTubeParam mainTubeParam )
		{
			// data protection
			if( branchTubeParam == null || mainTubeParam == null ) {
				return null;
			}
			if( branchTubeParam.IsValid() == false || mainTubeParam.IsValid() == false ) {
				return null;
			}

			// calculate prism vector
			gp_Pnt center;
			OCCTool.GetBranchTubeDir( branchTubeParam.AAngle_deg, branchTubeParam.BAngle_deg, out gp_Dir dir );
			gp_Vec prismVec = new gp_Vec( dir );
			if( branchTubeParam.IntersectDir == BranchIntersectDir.Positive
				|| branchTubeParam.IntersectDir == BranchIntersectDir.Negative ) {
				center = new gp_Pnt( branchTubeParam.Center_X, branchTubeParam.Center_Y, branchTubeParam.Center_Z );
				prismVec.Multiply( branchTubeParam.IntersectDir == BranchIntersectDir.Positive ? branchTubeParam.Length : -branchTubeParam.Length );
			}
			else {
				center = new gp_Pnt(
					branchTubeParam.Center_X - dir.x * branchTubeParam.Length,
					branchTubeParam.Center_Y - dir.y * branchTubeParam.Length,
					branchTubeParam.Center_Z - dir.z * branchTubeParam.Length );
				prismVec.Multiply( branchTubeParam.Length * 2 );
			}

			// make branch tube
			TopoDS_Wire outerWire = OCCTool.MakeBaseWire( branchTubeParam.Shape, 0, center, dir, branchTubeParam.SelfRotateAngle_deg );
			BRepBuilderAPI_MakeFace branchFaceMaker = new BRepBuilderAPI_MakeFace( outerWire );
			if( branchFaceMaker.IsDone() == false ) {
				return null;
			}
			BRepPrimAPI_MakePrism branchTubeMaker = new BRepPrimAPI_MakePrism( branchFaceMaker.Shape(), prismVec );
			if( branchTubeMaker.IsDone() == false ) {
				return null;
			}

			if( branchTubeParam.ArrayParam.LinearCount <= 1 && branchTubeParam.ArrayParam.AngularCount <= 1 ) {
				return branchTubeMaker.Shape();
			}

			// make array
			TopoDS_Shape oneBranchTube = branchTubeMaker.Shape();
			TopoDS_Shape arrayBranchTube = OCCTool.MakeArrayCompound( oneBranchTube, branchTubeParam.ArrayParam );
			return arrayBranchTube;
		}

		static TopoDS_Shape MakeExtendBoundingBox( CADft_MainTubeParam mainTubeParam )
		{
			// data protection
			if( mainTubeParam == null ) {
				return null;
			}
			if( mainTubeParam.IsValid() == false ) {
				return null;
			}

			// calculate bounding box size
			double dWidth = 0;
			double dHeight = 0;
			if( mainTubeParam.CrossSection.Shape.Type == Geom2D_Type.Circle ) {
				dWidth = ( (Geom2D_Circle)( mainTubeParam.CrossSection.Shape ) ).Radius * 4;
				dHeight = dWidth;
			}
			else if( mainTubeParam.CrossSection.Shape.Type == Geom2D_Type.Rectangle ) {
				dWidth = ( (Geom2D_Rectangle)( mainTubeParam.CrossSection.Shape ) ).Width * 2;
				dHeight = ( (Geom2D_Rectangle)( mainTubeParam.CrossSection.Shape ) ).Height * 2;
			}
			double dLength = mainTubeParam.Length * 2;

			// make XZ plane wire
			gp_Pnt center = new gp_Pnt( 0, -mainTubeParam.Length / 2, 0 );
			gp_Dir dir = new gp_Dir( 0, 1, 0 );
			Geom2D_Rectangle rect = new Geom2D_Rectangle( dWidth, dHeight, 0 );
			TopoDS_Wire baseWire = OCCTool.MakeBaseWire( rect, 0, center, dir, 0 );

			// make the face
			BRepBuilderAPI_MakeFace faceMaker = new BRepBuilderAPI_MakeFace( baseWire );
			if( faceMaker.IsDone() == false ) {
				return null;
			}

			// make prism
			gp_Vec vec = new gp_Vec( 0, dLength, 0 );
			BRepPrimAPI_MakePrism prismMaker = new BRepPrimAPI_MakePrism( faceMaker.Face(), vec );
			if( prismMaker.IsDone() == false ) {
				return null;
			}
			return prismMaker.Shape();
		}
	}
}
