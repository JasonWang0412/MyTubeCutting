﻿using MyCore.Tool;
using OCC.BRepAlgoAPI;
using OCC.BRepBuilderAPI;
using OCC.BRepPrimAPI;
using OCC.gp;
using OCC.TopoDS;
using System.Collections.Generic;

namespace MyCore.CAD
{
	public class TubeMaker
	{
		public static TopoDS_Shape MakeResultTube( CADft_MainTubeParam mainTubeParam,
			List<CADft_EndCutterParam> endCutterParamList,
			List<CADft_BranchTubeParam> branchTubeParamList )
		{
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
			List<TopoDS_Shape> branchTubes = MakeBranchTubes( branchTubeParamList );
			if( branchTubes != null && branchTubes.Count != 0 ) {
				foreach( TopoDS_Shape branchTube in branchTubes ) {
					BRepAlgoAPI_Cut cut = new BRepAlgoAPI_Cut( mainTube, branchTube );
					if( cut.IsDone() == false ) {
						continue;
					}
					mainTube = cut.Shape();
				}
			}

			return mainTube;
		}

		// make main tube
		public static TopoDS_Shape MakeMainTube( CADft_MainTubeParam mainTubeParam )
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
			TopoDS_Wire outerWire = OCCTool.MakeWire( mainTubeParam.CrossSection.Shape, 0, center, dir, 0 );
			TopoDS_Wire innerWire = OCCTool.MakeWire( mainTubeParam.CrossSection.Shape, mainTubeParam.CrossSection.Thickness, center, dir, 0 );

			// Get solid shape by wire
			return OCCTool.MakeRawTubeShape( outerWire, innerWire, mainTubeParam.Length );
		}

		// make end cutters
		public static List<TopoDS_Shape> MakeEndCutters( List<CADft_EndCutterParam> endCutterParamList )
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

		public static TopoDS_Shape MakeEndCutter( CADft_EndCutterParam endCutterParam )
		{
			// data protection
			if( endCutterParam == null ) {
				return null;
			}
			if( endCutterParam.IsValid() == false ) {
				return null;
			}

			// get plane
			gp_Pnt center = new gp_Pnt( endCutterParam.Center_X, endCutterParam.Center_Y, endCutterParam.Center_Z );
			OCCTool.GetEndCutterDir( endCutterParam.TiltAngle_deg, endCutterParam.RotateAngle_deg, out gp_Dir dir );
			gp_Pln cutPlane = new gp_Pln( center, dir );
			BRepBuilderAPI_MakeFace makeFace = new BRepBuilderAPI_MakeFace( cutPlane );
			if( makeFace.IsDone() == false ) {
				return null;
			}

			// get point on cut side
			double dYpos = endCutterParam.Side == EEndSide.Left ? endCutterParam.Center_Y - 1 : endCutterParam.Center_Y + 1;

			// make cutter half space
			gp_Pnt pointOnCutSide = new gp_Pnt( 0, dYpos, 0 );
			BRepPrimAPI_MakeHalfSpace halfSpace = new BRepPrimAPI_MakeHalfSpace( makeFace.Face(), pointOnCutSide );
			if( halfSpace.IsDone() == false ) {
				return null;
			}
			return halfSpace.Shape();
		}

		// make branch tubes
		public static List<TopoDS_Shape> MakeBranchTubes( List<CADft_BranchTubeParam> branchTubeParamList )
		{
			// data protection
			if( branchTubeParamList == null ) {
				return null;
			}

			List<TopoDS_Shape> branchTubes = new List<TopoDS_Shape>();
			foreach( CADft_BranchTubeParam branchTubeParam in branchTubeParamList ) {
				TopoDS_Shape oneBranchTube = MakeBranchTube( branchTubeParam );
				if( oneBranchTube == null ) {
					continue;
				}
				branchTubes.Add( oneBranchTube );
			}
			return branchTubes;
		}

		public static TopoDS_Shape MakeBranchTube( CADft_BranchTubeParam branchTubeParam )
		{
			// data protection
			if( branchTubeParam == null ) {
				return null;
			}
			if( branchTubeParam.IsValid() == false ) {
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
			TopoDS_Wire outerWire = OCCTool.MakeWire( branchTubeParam.Shape, 0, center, dir, branchTubeParam.SelfRotateAngle_deg );
			BRepBuilderAPI_MakeFace branchFaceMaker = new BRepBuilderAPI_MakeFace( outerWire );
			if( branchFaceMaker.IsDone() == false ) {
				return null;
			}
			BRepPrimAPI_MakePrism branchTubeMaker = new BRepPrimAPI_MakePrism( branchFaceMaker.Shape(), prismVec );
			if( branchTubeMaker.IsDone() == false ) {
				return null;
			}

			return branchTubeMaker.Shape();
		}
	}
}
