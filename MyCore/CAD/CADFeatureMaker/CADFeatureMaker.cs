using MyCore.Tool;
using OCC.AIS;
using OCC.BRep;
using OCC.BRepAlgoAPI;
using OCC.BRepBuilderAPI;
using OCC.BRepPrimAPI;
using OCC.Geom;
using OCC.gp;
using OCC.Graphic3d;
using OCC.Quantity;
using OCC.TopAbs;
using OCC.TopoDS;
using System;
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
				.Where( param => param.Type == CADFeatureType.EndCutter ).Cast<CADft_EndCutterParam>().ToList();
			List<CADft_BranchTubeParam> branchTubeParamList = map.FeatureMap.Values
				.Where( param => param.Type == CADFeatureType.BranchTube ).Cast<CADft_BranchTubeParam>().ToList();

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
			foreach( CADft_EndCutterParam oneEndCutterParam in endCutterParamList ) {

				// data protection
				if( oneEndCutterParam == null || oneEndCutterParam.IsValid() == false ) {
					continue;
				}

				// make end cutter
				TopoDS_Shape oneEndCutter = MakeEndCutterTopo( oneEndCutterParam );
				if( oneEndCutter == null ) {
					continue;
				}

				// cut main tube
				BRepAlgoAPI_Cut cut = new BRepAlgoAPI_Cut( mainTube, oneEndCutter );
				if( cut.IsDone() == false ) {
					continue;
				}
				mainTube = cut.Shape();
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
			// data protection
			if( cadFeatureParam == null || mainTubeParam == null ) {
				return null;
			}
			if( cadFeatureParam.IsValid() == false || mainTubeParam.IsValid() == false ) {
				return null;
			}

			AIS_Shape cadFeatureAIS = null;
			if( cadFeatureParam.Type == CADFeatureType.EndCutter ) {
				cadFeatureAIS = MakeEndCutterAIS( (CADft_EndCutterParam)cadFeatureParam, mainTubeParam );
			}
			else if( cadFeatureParam.Type == CADFeatureType.BranchTube ) {
				cadFeatureAIS = new AIS_Shape( MakeBranchTube( (CADft_BranchTubeParam)cadFeatureParam, mainTubeParam ) );
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
			gp_Pnt center = new gp_Pnt( 0, 0, 0 ); // origin
			gp_Dir dir = new gp_Dir( 0, 1, 0 ); // y-axis

			// TODO: extend tp no-center-tunnel tube
			TopoDS_Wire outerWire = OCCTool.MakeShapeWire( mainTubeParam.CrossSection.Shape, 0, center, dir, 0 );
			TopoDS_Wire innerWire = OCCTool.MakeShapeWire( mainTubeParam.CrossSection.Shape, mainTubeParam.CrossSection.Thickness, center, dir, 0 );
			if( outerWire == null || innerWire == null ) {
				return null;
			}

			// Get solid shape by wire
			return OCCTool.MakeCenterTunnelTube( outerWire, innerWire, mainTubeParam.Length );
		}

		static TopoDS_Shape MakeEndCutterTopo( CADft_EndCutterParam endCutterParam )
		{
			// get plane
			TopoDS_Face thePlane = MakeEndCutterFace( endCutterParam );
			if( thePlane == null ) {
				return null;
			}

			// get point on cut side
			OCCTool.GetEndCutterDir( endCutterParam.TiltAngle_deg, endCutterParam.RotateAngle_deg, out gp_Dir dir );
			if( endCutterParam.Side == EEndSide.Left ) {
				dir.Reverse();
			}

			// make cutter half space
			// Discuss: it can work with Y only, even when tilt is 90
			gp_Pnt pointOnCutSide = new gp_Pnt( dir.X(), endCutterParam.Center_Y + dir.Y(), dir.Z() );
			BRepPrimAPI_MakeHalfSpace halfSpace = new BRepPrimAPI_MakeHalfSpace( thePlane, pointOnCutSide );
			if( halfSpace.IsDone() == false ) {
				return null;
			}
			return halfSpace.Shape();
		}

		static AIS_Shape MakeEndCutterAIS( CADft_EndCutterParam endCutterParam, CADft_MainTubeParam mainTubeParam )
		{
			// make the face
			TopoDS_Face thePlane = MakeEndCutterFace( endCutterParam );
			if( thePlane == null ) {
				return null;
			}

			// make a large face
			// the face is for display only, the size is not important
			Geom_Surface originalSurface = BRep_Tool.Surface( thePlane );
			GetMainTubeBoundingBox( mainTubeParam, out BoundingBox boundingBox );
			double dWidth = boundingBox.MaxX - boundingBox.MinX;
			double dHeight = boundingBox.MaxZ - boundingBox.MinZ;
			double dLength = boundingBox.MaxY - boundingBox.MinY;
			double dParam = Math.Sqrt( Math.Pow( dWidth, 2 ) + Math.Pow( dHeight, 2 ) + Math.Pow( dLength, 2 ) );
			Geom_RectangularTrimmedSurface refinedSurface = new Geom_RectangularTrimmedSurface( originalSurface, -dParam, dParam, -dParam, dParam );

			// TODO: magic number 0.001
			BRepBuilderAPI_MakeFace refinedFaceMaker = new BRepBuilderAPI_MakeFace( refinedSurface, 0.001 );
			if( refinedFaceMaker.IsDone() == false ) {
				return null;
			}

			return new AIS_Shape( refinedFaceMaker.Face() );
		}

		static TopoDS_Face MakeEndCutterFace( CADft_EndCutterParam endCutterParam )
		{
			gp_Pnt center = new gp_Pnt( 0, endCutterParam.Center_Y, 0 );
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
			TopoDS_Wire outerWire = OCCTool.MakeShapeWire( branchTubeParam.Shape, 0, center, dir, branchTubeParam.SelfRotateAngle_deg );
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

		static void GetMainTubeBoundingBox( CADft_MainTubeParam mainTubeParam, out BoundingBox boundingBox )
		{
			double dWidth = 0;
			double dHeight = 0;
			if( mainTubeParam.CrossSection.Shape.Type == Geom2D_Type.Circle ) {
				dWidth = ( (Geom2D_Circle)( mainTubeParam.CrossSection.Shape ) ).Radius * 2;
				dHeight = dWidth;
			}
			else if( mainTubeParam.CrossSection.Shape.Type == Geom2D_Type.Rectangle ) {
				dWidth = ( (Geom2D_Rectangle)( mainTubeParam.CrossSection.Shape ) ).Width;
				dHeight = ( (Geom2D_Rectangle)( mainTubeParam.CrossSection.Shape ) ).Height;
			}
			double dLength = mainTubeParam.Length * 2;

			boundingBox = new BoundingBox( -dWidth / 2, dWidth / 2, -dHeight / 2, dHeight / 2, 0, dLength );
		}
	}
}
