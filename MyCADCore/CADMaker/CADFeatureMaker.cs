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
using Utility;

namespace MyCADCore
{
	public class CADFeatureMaker
	{
		public static TopoDS_Shape MakeResultTube( CADFeatureParamMap map )
		{
			// data protection
			if( map == null || map.FeatureMap == null ) {
				return null;
			}

			CADft_MainTubeParam mainTubeParam = map.MainTubeParam;
			List<CADft_EndCutterParam> endCutterParamList = map.FeatureMap.Values
				.Where( param => param.Type == CADFeatureType.EndCutter ).Cast<CADft_EndCutterParam>().ToList();
			List<CADft_BranchTubeParam> branchTubeParamList = map.FeatureMap.Values
				.Where( param => param.Type == CADFeatureType.BranchTube ).Cast<CADft_BranchTubeParam>().ToList();
			List<CADft_BendingNotchParam> bendingNotchParamList = map.FeatureMap.Values
				.Where( param => param.Type == CADFeatureType.BendingNotch ).Cast<CADft_BendingNotchParam>().ToList();

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
			if( bendingNotchParamList == null ) {
				bendingNotchParamList = new List<CADft_BendingNotchParam>();
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
			foreach( CADft_BranchTubeParam oneBranchTubeParam in branchTubeParamList ) {

				// data protection
				if( oneBranchTubeParam == null || oneBranchTubeParam.IsValid() == false ) {
					continue;
				}

				// make branch tube
				TopoDS_Shape oneBranchTube = MakeBranchTubeTopo( oneBranchTubeParam, mainTubeParam, true );
				if( oneBranchTube == null ) {
					continue;
				}

				// cut main tube
				// find all shapes in oneBranchTube if it is compound
				// u'll meet some bug (from OCC maybe) if u use compound shape directly
				if( oneBranchTube.ShapeType() == TopAbs_ShapeEnum.TopAbs_COMPOUND ) {
					foreach( TopoDS_Shape oneArrayElemnt in oneBranchTube.elementsAsList ) {
						BRepAlgoAPI_Cut cut = new BRepAlgoAPI_Cut( mainTube, oneArrayElemnt );
						if( cut.IsDone() == false ) {
							continue;
						}
						mainTube = cut.Shape();
					}
				}
				else {
					BRepAlgoAPI_Cut cut = new BRepAlgoAPI_Cut( mainTube, oneBranchTube );
					if( cut.IsDone() == false ) {
						continue;
					}
					mainTube = cut.Shape();
				}
			}

			// cut main tube by bending notches
			foreach( CADft_BendingNotchParam oneBendingNotchParam in bendingNotchParamList ) {

				// data protection
				if( oneBendingNotchParam == null || oneBendingNotchParam.IsValid() == false ) {
					continue;
				}

				// make bending notch
				TopoDS_Shape oneBendingNotch = MakeBendingNotchTopo( oneBendingNotchParam, mainTubeParam );
				if( oneBendingNotch == null ) {
					continue;
				}

				// cut main tube
				// find all shapes in oneBendingNotch if it is compound
				// u'll meet some bug (from OCC maybe) if u use compound shape directly
				if( oneBendingNotch.ShapeType() == TopAbs_ShapeEnum.TopAbs_COMPOUND ) {
					foreach( TopoDS_Shape oneArrayElemnt in oneBendingNotch.elementsAsList ) {
						BRepAlgoAPI_Cut cut = new BRepAlgoAPI_Cut( mainTube, oneArrayElemnt );
						if( cut.IsDone() == false ) {
							continue;
						}
						mainTube = cut.Shape();
					}
				}
				else {
					BRepAlgoAPI_Cut cut = new BRepAlgoAPI_Cut( mainTube, oneBendingNotch );
					if( cut.IsDone() == false ) {
						continue;
					}
					mainTube = cut.Shape();
				}
			}

			return mainTube;
		}

		public static AIS_Shape MakeCADFeatureAIS( ICADFeatureParam cadFeatureParam, CADft_MainTubeParam mainTubeParam )
		{
			// data protection
			if( cadFeatureParam == null || cadFeatureParam.IsValid() == false ) {
				return null;
			}
			if( mainTubeParam == null || mainTubeParam.IsValid() == false ) {
				return null;
			}

			AIS_Shape cadFeatureAIS = null;
			if( cadFeatureParam.Type == CADFeatureType.EndCutter ) {
				cadFeatureAIS = MakeEndCutterAIS( (CADft_EndCutterParam)cadFeatureParam, mainTubeParam );
			}
			else if( cadFeatureParam.Type == CADFeatureType.BranchTube ) {
				cadFeatureAIS = MakeBranchTubeAIS( (CADft_BranchTubeParam)cadFeatureParam, mainTubeParam );
			}
			else if( cadFeatureParam.Type == CADFeatureType.BendingNotch ) {
				cadFeatureAIS = MakeBendingNotchAIS( (CADft_BendingNotchParam)cadFeatureParam, mainTubeParam );
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

		public static gp_Dir GetCADFeatureDir( ICADFeatureParam cadFeatureParam )
		{
			// data protection
			if( cadFeatureParam == null || cadFeatureParam.IsValid() == false ) {
				return new gp_Dir( 0, 1, 0 );
			}

			if( cadFeatureParam.Type == CADFeatureType.EndCutter ) {
				CADft_EndCutterParam endCutterParam = (CADft_EndCutterParam)cadFeatureParam;
				GetEndCutterDir( endCutterParam.TiltAngle_deg, endCutterParam.RotateAngle_deg, out gp_Dir dir );
				return dir;
			}
			else if( cadFeatureParam.Type == CADFeatureType.BranchTube ) {
				CADft_BranchTubeParam branchTubeParam = (CADft_BranchTubeParam)cadFeatureParam;
				GetBranchTubeDir( branchTubeParam.AAngle_deg, branchTubeParam.BAngle_deg, out gp_Dir dir );
				return dir;
			}
			else if( cadFeatureParam.Type == CADFeatureType.BendingNotch ) {
				CADft_BendingNotchParam bendingNotchParam = (CADft_BendingNotchParam)cadFeatureParam;
				GetBendingNotchDir( bendingNotchParam.BAngle_deg, out gp_Dir dir );
				return dir;
			}
			else {
				return new gp_Dir( 0, 1, 0 );
			}
		}

		// make main tube
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
			gp_Dir dir = new gp_Dir( 0, -1, 0 ); // -Y axis, to flip to predicted orientation

			// TODO: extend tp no-center-tunnel tube
			TopoDS_Wire outerWire = OCCTool.MakeGeom2DWire( mainTubeParam.CrossSection.Shape, 0, 0, center, dir );
			TopoDS_Wire innerWire = OCCTool.MakeGeom2DWire( mainTubeParam.CrossSection.Shape, mainTubeParam.CrossSection.Thickness, 0, center, dir );
			if( outerWire == null || innerWire == null ) {
				return null;
			}

			// Get solid shape by wire
			return MakeCenterTunnelTube( outerWire, innerWire, mainTubeParam.Length );
		}

		static TopoDS_Shape MakeCenterTunnelTube( TopoDS_Wire outerWire, TopoDS_Wire innerWire, double tubeLength )
		{
			BRepBuilderAPI_MakeFace outerFaceMaker = new BRepBuilderAPI_MakeFace( outerWire );
			BRepBuilderAPI_MakeFace innerFaceMaker = new BRepBuilderAPI_MakeFace( innerWire );
			if( outerFaceMaker.IsDone() == false || innerFaceMaker.IsDone() == false ) {
				return null;
			}

			// cut outer face by inner face
			BRepAlgoAPI_Cut cut = new BRepAlgoAPI_Cut( outerFaceMaker.Face(), innerFaceMaker.Face() );
			if( cut.IsDone() == false ) {
				return null;
			}

			// make tube
			gp_Vec vec = new gp_Vec( 0, tubeLength, 0 );
			BRepPrimAPI_MakePrism tubeMaker = new BRepPrimAPI_MakePrism( cut.Shape(), vec );
			if( tubeMaker.IsDone() == false ) {
				return null;
			}

			return tubeMaker.Shape();
		}

		// make end cutter
		static TopoDS_Shape MakeEndCutterTopo( CADft_EndCutterParam endCutterParam )
		{
			// get plane
			TopoDS_Face thePlane = MakeEndCutterFace( endCutterParam );
			if( thePlane == null ) {
				return null;
			}

			// get point on cut side
			GetEndCutterDir( endCutterParam.TiltAngle_deg, endCutterParam.RotateAngle_deg, out gp_Dir dir );
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
			GetMainTubeBoundingBox( mainTubeParam, out double dParam );
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
			GetEndCutterDir( endCutterParam.TiltAngle_deg, endCutterParam.RotateAngle_deg, out gp_Dir dir );
			gp_Pln cutPlane = new gp_Pln( center, dir );
			BRepBuilderAPI_MakeFace makeFace = new BRepBuilderAPI_MakeFace( cutPlane );
			if( makeFace.IsDone() == false ) {
				return null;
			}
			return makeFace.Face();
		}

		static void GetEndCutterDir( double dTilt_deg, double dRotate_deg, out gp_Dir dir )
		{
			// the initail direction is (0, 1, 0)
			gp_Dir dirInit = new gp_Dir( 0, 1, 0 );

			// rotate around X axis by tilt angle in radian
			gp_Trsf transformTilt = new gp_Trsf();
			transformTilt.SetRotation( new gp_Ax1( new gp_Pnt( 0, 0, 0 ), new gp_Dir( 1, 0, 0 ) ), dTilt_deg * Math.PI / 180 );

			// rotate around -Y axis by rotate angle in radian
			gp_Trsf transformRotate = new gp_Trsf();
			transformRotate.SetRotation( new gp_Ax1( new gp_Pnt( 0, 0, 0 ), new gp_Dir( 0, -1, 0 ) ), dRotate_deg * Math.PI / 180 );

			gp_Trsf trsfFinal = transformRotate.Multiplied( transformTilt );
			dir = dirInit.Transformed( trsfFinal );
		}

		// make branch tube
		static TopoDS_Shape MakeBranchTubeTopo( CADft_BranchTubeParam branchTubeParam, CADft_MainTubeParam mainTubeParam, bool bForCut )
		{
			TopoDS_Shape branchTube;
			if( branchTubeParam.IsCutThrough ) {
				CADft_BranchTubeParam cloneParam = CloneHelper.Clone( branchTubeParam );
				double dSize;
				if( bForCut ) {
					// u'll meet some bug (from OCC maybe) if u use infinity prism
					const double MAX_VALUE = 999999;
					dSize = MAX_VALUE;
				}
				else {
					GetMainTubeBoundingBox( mainTubeParam, out dSize );
				}

				// set property
				cloneParam.Length = dSize;
				cloneParam.IntersectDir = BranchIntersectDir.Both;

				// make branch tube
				branchTube = MakeBranchTubePrism( cloneParam );
			}
			else {
				branchTube = MakeBranchTubePrism( branchTubeParam );
			}

			if( branchTube == null ) {
				return null;
			}

			// make array
			TopoDS_Shape arrayBranchTube = OCCTool.MakeArrayCompound( branchTube, branchTubeParam.ArrayParam );
			if( arrayBranchTube == null ) {
				return branchTube;
			}
			return arrayBranchTube;
		}

		static AIS_Shape MakeBranchTubeAIS( CADft_BranchTubeParam branchTubeParam, CADft_MainTubeParam mainTubeParam )
		{
			TopoDS_Shape branchTube = MakeBranchTubeTopo( branchTubeParam, mainTubeParam, false );
			if( branchTube == null ) {
				return null;
			}
			return new AIS_Shape( branchTube );
		}

		static TopoDS_Shape MakeBranchTubePrism( CADft_BranchTubeParam branchTubeParam )
		{
			// calculate prism vector
			gp_Pnt center = new gp_Pnt( branchTubeParam.Center_X, branchTubeParam.Center_Y, branchTubeParam.Center_Z );
			GetBranchTubeDir( branchTubeParam.AAngle_deg, branchTubeParam.BAngle_deg, out gp_Dir dir );

			// make branch tube
			TopoDS_Wire baseWire = OCCTool.MakeGeom2DWire( branchTubeParam.Shape, 0, branchTubeParam.SelfRotateAngle_deg, center, dir );
			if( baseWire == null ) {
				return null;
			}

			// make prism
			PrismDir prismDir;
			if( branchTubeParam.IntersectDir == BranchIntersectDir.Both ) {
				prismDir = PrismDir.Both;
			}
			else if( branchTubeParam.IntersectDir == BranchIntersectDir.Negative ) {
				prismDir = PrismDir.Negative;
			}
			else {
				prismDir = PrismDir.Positive;
			}
			return OCCTool.MakeConcretePrismByWire( baseWire, dir, branchTubeParam.Length, prismDir );
		}

		static void GetBranchTubeDir( double dA_deg, double dB_deg, out gp_Dir dir )
		{
			// the initail direction is (0, 0, 1)
			gp_Dir dirInit = new gp_Dir( 0, 0, 1 );

			// rotate around X axis by A angle in radian
			gp_Trsf transformA = new gp_Trsf();
			transformA.SetRotation( new gp_Ax1( new gp_Pnt( 0, 0, 0 ), new gp_Dir( 1, 0, 0 ) ), dA_deg * Math.PI / 180 );

			// rotate around -Y axis by B angle in radian
			gp_Trsf transformB = new gp_Trsf();
			transformB.SetRotation( new gp_Ax1( new gp_Pnt( 0, 0, 0 ), new gp_Dir( 0, -1, 0 ) ), dB_deg * Math.PI / 180 );

			gp_Trsf trsfFinal = transformB.Multiplied( transformA );
			dir = dirInit.Transformed( trsfFinal );
		}

		// make bending notch
		static TopoDS_Shape MakeBendingNotchTopo( CADft_BendingNotchParam bendingNotchParam, CADft_MainTubeParam mainTubeParam )
		{
			// get the main tube size after rotation (ON XY PLANE)
			// 1. make the wire
			TopoDS_Wire mainTubeWire = OCCTool.MakeXOYGeom2DWire( mainTubeParam.CrossSection.Shape, 0, bendingNotchParam.BAngle_deg );
			if( mainTubeWire == null ) {
				return null;
			}

			// 2. get the extrema, there might meet some bug, ref: AUTO-12540
			// solved by "copy: true" when transforming shape, dont know shit but work
			BoundingBox boundingBox = OCCTool.GetBoundingBox( mainTubeWire );
			if( boundingBox == null ) {
				return null;
			}
			double minZ = boundingBox.MinY; // taking Y here
			double maxZ = boundingBox.MaxY;

			// get the bending notch shape wire
			double posY = bendingNotchParam.YPos;
			double posZ = minZ + bendingNotchParam.GapFromButtom;
			double angleB_deg = bendingNotchParam.BAngle_deg;
			double dThickness = mainTubeParam.CrossSection.Thickness;
			TopoDS_Wire notchWire = OCCTool.MakeBendingNotchWire( bendingNotchParam.Shape, posY, posZ, minZ, maxZ, dThickness, angleB_deg );
			if( notchWire == null ) {
				return null;
			}

			// make the bending notch
			GetBendingNotchDir( bendingNotchParam.BAngle_deg, out gp_Dir dir );
			GetMainTubeBoundingBox( mainTubeParam, out double dSize );
			TopoDS_Shape bendingNotch = OCCTool.MakeConcretePrismByWire( notchWire, dir, dSize, PrismDir.Both );
			if( bendingNotch == null ) {
				return null;
			}

			// make relief hole
			TopoDS_Shape notchWithReliefHole = bendingNotch;
			TopoDS_Shape reliefHole = MakeReliefHole( bendingNotchParam, mainTubeParam );
			if( reliefHole != null ) {

				// fuse the bending notch and relief hole
				BRepAlgoAPI_Fuse fuse = new BRepAlgoAPI_Fuse( bendingNotch, reliefHole );
				if( fuse.IsDone() != false ) {
					notchWithReliefHole = fuse.Shape();
				}
			}

			// make array
			TopoDS_Shape arrayBendingNotch = OCCTool.MakeArrayCompound( notchWithReliefHole, bendingNotchParam.ArrayParam );
			if( arrayBendingNotch == null ) {
				return notchWithReliefHole;
			}
			return arrayBendingNotch;
		}

		static TopoDS_Shape MakeReliefHole( CADft_BendingNotchParam bendingNotchParam, CADft_MainTubeParam mainTubeParam )
		{
			if( bendingNotchParam.Shape.Type != BendingNotch_Type.VShape ) {
				return null;
			}

			ReliefHoleType reliefHoleType = ( (BN_VShape)bendingNotchParam.Shape ).ReliefHoleType;
			if( reliefHoleType == ReliefHoleType.No ) {
				return null;
			}

			if( reliefHoleType == ReliefHoleType.Side ) {
				return MakeSideReliefHole( bendingNotchParam, mainTubeParam );
			}
			else if( reliefHoleType == ReliefHoleType.Buttom ) {
				return MakeButtomReliefHole( bendingNotchParam, mainTubeParam );
			}
			else {
				return null;
			}
		}

		static TopoDS_Shape MakeSideReliefHole( CADft_BendingNotchParam bendingNotchParam, CADft_MainTubeParam mainTubeParam )
		{
			ReliefHole reliefHole = ( (BN_VShape)bendingNotchParam.Shape ).ReliefHole;
			if( reliefHole == null ) {
				return null;
			}

			// get the relief hole position
			TopoDS_Wire mainTubeWire = OCCTool.MakeXOYGeom2DWire( mainTubeParam.CrossSection.Shape, 0, bendingNotchParam.BAngle_deg );
			if( mainTubeWire == null ) {
				return null;
			}
			BoundingBox boundingBox = OCCTool.GetBoundingBox( mainTubeWire );
			if( boundingBox == null ) {
				return null;
			}
			double minZ = boundingBox.MinY; // taking Y here
			double centerX = 0;
			double centerY = bendingNotchParam.YPos;
			double centerZ = minZ + bendingNotchParam.GapFromButtom + reliefHole.Height / 2;
			gp_Pnt center = new gp_Pnt( centerX, centerY, centerZ );
			GetBendingNotchDir( bendingNotchParam.BAngle_deg, out gp_Dir dir );

			// make the relief hole wire
			// the width and wire should be switched here
			TopoDS_Wire reliefHoleWire = OCCTool.MakeGeom2DWire( new Geom2D_Rectangle( reliefHole.Height, reliefHole.Width, reliefHole.Fillet ), 0, 0, center, dir );
			if( reliefHoleWire == null ) {
				return null;
			}

			// make the relief hole prism
			GetMainTubeBoundingBox( mainTubeParam, out double dSize );
			return OCCTool.MakeConcretePrismByWire( reliefHoleWire, dir, dSize, PrismDir.Both );
		}

		static TopoDS_Shape MakeButtomReliefHole( CADft_BendingNotchParam bendingNotchParam, CADft_MainTubeParam mainTubeParam )
		{
			ReliefHole reliefHole = ( (BN_VShape)bendingNotchParam.Shape ).ReliefHole;
			if( reliefHole == null ) {
				return null;
			}

			// get the relief hole position
			TopoDS_Wire mainTubeWire = OCCTool.MakeXOYGeom2DWire( mainTubeParam.CrossSection.Shape, 0, bendingNotchParam.BAngle_deg );
			if( mainTubeWire == null ) {
				return null;
			}
			BoundingBox boundingBox = OCCTool.GetBoundingBox( mainTubeWire );
			if( boundingBox == null ) {
				return null;
			}
			double centerX1 = boundingBox.MinX; // taking Y here
			double centerX2 = boundingBox.MaxX;
			double centerY = bendingNotchParam.YPos;
			gp_Pnt center1 = new gp_Pnt( centerX1, centerY, 0 );
			gp_Pnt center2 = new gp_Pnt( centerX2, centerY, 0 );

			// the relief hole direction is perpendicular to the bending notch direction
			GetBendingNotchDir( bendingNotchParam.BAngle_deg + 90, out gp_Dir dir );

			// make the relief hole wire
			// the width and wire should be switched here, and the hole width should be doubled
			TopoDS_Wire reliefHoleWire1 = OCCTool.MakeGeom2DWire( new Geom2D_Rectangle( reliefHole.Height * 2, reliefHole.Width, reliefHole.Fillet ), 0, 0, center1, dir );
			TopoDS_Wire reliefHoleWire2 = OCCTool.MakeGeom2DWire( new Geom2D_Rectangle( reliefHole.Height * 2, reliefHole.Width, reliefHole.Fillet ), 0, 0, center2, dir );
			if( reliefHoleWire1 == null || reliefHoleWire2 == null ) {
				return null;
			}

			// make the relief hole prism
			GetMainTubeBoundingBox( mainTubeParam, out double dSize );
			TopoDS_Shape reliefHoleShape1 = OCCTool.MakeConcretePrismByWire( reliefHoleWire1, dir, dSize, PrismDir.Both );
			TopoDS_Shape reliefHoleShape2 = OCCTool.MakeConcretePrismByWire( reliefHoleWire2, dir, dSize, PrismDir.Both );
			if( reliefHoleShape1 == null || reliefHoleShape2 == null ) {
				return null;
			}

			// fuse the relief holes
			BRepAlgoAPI_Fuse fuse = new BRepAlgoAPI_Fuse( reliefHoleShape1, reliefHoleShape2 );
			if( fuse.IsDone() == false ) {
				return null;
			}
			return fuse.Shape();
		}

		static AIS_Shape MakeBendingNotchAIS( CADft_BendingNotchParam bendingNotchParam, CADft_MainTubeParam mainTubeParam )
		{
			TopoDS_Shape bendingNotch = MakeBendingNotchTopo( bendingNotchParam, mainTubeParam );
			if( bendingNotch == null ) {
				return null;
			}
			return new AIS_Shape( bendingNotch );
		}

		static void GetBendingNotchDir( double dAngle_deg, out gp_Dir dir )
		{
			// the initail direction is (1, 0, 0)
			gp_Dir dirInit = new gp_Dir( 1, 0, 0 );

			// rotate around -Y axis by angle in radian
			gp_Trsf transform = new gp_Trsf();
			transform.SetRotation( new gp_Ax1( new gp_Pnt( 0, 0, 0 ), new gp_Dir( 0, -1, 0 ) ), dAngle_deg * Math.PI / 180 );

			dir = dirInit.Transformed( transform );
		}

		// display size
		static BoundingBox GetMainTubeBoundingBox( CADft_MainTubeParam mainTubeParam, out double dSize )
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

			dSize = Math.Sqrt( Math.Pow( dWidth, 2 ) + Math.Pow( dHeight, 2 ) + Math.Pow( dLength, 2 ) );
			return new BoundingBox( -dWidth / 2, dWidth / 2, -dHeight / 2, dHeight / 2, 0, dLength );
		}
	}
}
