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
				TopoDS_Shape oneBranchTube = MakeBranchTubeTopo( oneBranchTubeParam );
				if( oneBranchTube == null ) {
					continue;
				}

				// cut main tube
				// find all shapes in branchTube if it is compound
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
				// u'll meet some bug (from OCC maybe) if u use infinity prism, ref: AUTO-12540
				TopoDS_Shape oneBendingNotch = MakeBendingNotchTopo( oneBendingNotchParam, mainTubeParam );
				if( oneBendingNotch == null ) {
					continue;
				}

				// cut main tube
				BRepAlgoAPI_Cut cut = new BRepAlgoAPI_Cut( mainTube, oneBendingNotch );
				if( cut.IsDone() == false ) {
					continue;
				}
				mainTube = cut.Shape();
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
			aspect.SetTransparency( 0.8f );
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
			gp_Dir dir = new gp_Dir( 0, 1, 0 ); // y-axis

			// TODO: extend tp no-center-tunnel tube
			TopoDS_Wire outerWire = OCCTool.MakeGeom2DWire( mainTubeParam.CrossSection.Shape, 0, center, dir, 0 );
			TopoDS_Wire innerWire = OCCTool.MakeGeom2DWire( mainTubeParam.CrossSection.Shape, mainTubeParam.CrossSection.Thickness, center, dir, 0 );
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
		static TopoDS_Shape MakeBranchTubeTopo( CADft_BranchTubeParam branchTubeParam )
		{
			TopoDS_Shape branchTube;
			if( branchTubeParam.IsCutThrough ) {
				branchTube = MakeBranchTubeTopo_CutThrough( branchTubeParam );
			}
			else {
				branchTube = MakeBranchTubeTopo_ByLength( branchTubeParam );
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
			TopoDS_Shape branchTube;

			if( branchTubeParam.IsCutThrough ) {
				CADft_BranchTubeParam cloneParam = CloneHelper.Clone( branchTubeParam );

				// get display size
				GetMainTubeBoundingBox( mainTubeParam, out double dSize );

				// set property
				cloneParam.Length = dSize;
				cloneParam.IntersectDir = BranchIntersectDir.Both;

				// make branch tube
				branchTube = MakeBranchTubeTopo_ByLength( cloneParam );
			}
			else {
				branchTube = MakeBranchTubeTopo_ByLength( branchTubeParam );
			}

			if( branchTube == null ) {
				return null;
			}

			TopoDS_Shape arrayBranchTube = OCCTool.MakeArrayCompound( branchTube, branchTubeParam.ArrayParam );
			if( arrayBranchTube == null ) {
				return new AIS_Shape( branchTube );
			}
			return new AIS_Shape( arrayBranchTube );
		}

		static TopoDS_Shape MakeBranchTubeTopo_ByLength( CADft_BranchTubeParam branchTubeParam )
		{
			// calculate prism vector
			gp_Pnt center;
			GetBranchTubeDir( branchTubeParam.AAngle_deg, branchTubeParam.BAngle_deg, out gp_Dir dir );
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
			TopoDS_Wire outerWire = OCCTool.MakeGeom2DWire( branchTubeParam.Shape, 0, center, dir, branchTubeParam.SelfRotateAngle_deg );
			if( outerWire == null ) {
				return null;
			}
			return OCCTool.MakeConcretePrismByWire( outerWire, prismVec, false );
		}

		static TopoDS_Shape MakeBranchTubeTopo_CutThrough( CADft_BranchTubeParam branchTubeParam )
		{
			GetBranchTubeDir( branchTubeParam.AAngle_deg, branchTubeParam.BAngle_deg, out gp_Dir dir );
			gp_Pnt center = new gp_Pnt( branchTubeParam.Center_X, branchTubeParam.Center_Y, branchTubeParam.Center_Z );

			// make branch tube
			TopoDS_Wire baseWire = OCCTool.MakeGeom2DWire( branchTubeParam.Shape, 0, center, dir, branchTubeParam.SelfRotateAngle_deg );
			if( baseWire == null ) {
				return null;
			}
			gp_Vec vec = new gp_Vec( dir );
			return OCCTool.MakeConcretePrismByWire( baseWire, vec, true );
		}

		static void GetBranchTubeDir( double dA_deg, double dB_deg, out gp_Dir dir )
		{
			// the initail direction is (1, 0, 0)
			gp_Dir dirInit = new gp_Dir( 1, 0, 0 );

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
			TopoDS_Wire mainTubeWire = OCCTool.MakeGeom2DWire( mainTubeParam.CrossSection.Shape, 0, new gp_Pnt( 0, 0, 0 ), new gp_Dir( 0, 0, 1 ), 0 );
			if( mainTubeWire == null ) {
				return null;
			}

			// 2. rotate the wire (ON XY PLANE ALONG +Z)
			gp_Trsf trsfR = new gp_Trsf();
			trsfR.SetRotation( new gp_Ax1( new gp_Pnt( 0, 0, 0 ), new gp_Dir( 0, 0, 1 ) ), bendingNotchParam.BAngle_deg * Math.PI / 180 );
			BRepBuilderAPI_Transform transformR = new BRepBuilderAPI_Transform( mainTubeWire, trsfR );
			if( transformR.IsDone() == false ) {
				return null;
			}
			TopoDS_Shape shape = transformR.Shape();

			// 3. get the extrema, there might meet some bug, ref: AUTO-12540
			BoundingBox boundingBox = OCCTool.GetBoundingBox( shape );
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

			// make the bending notch
			// 1. get the direction and size
			GetBendingNotchDir( bendingNotchParam.BAngle_deg, out gp_Dir dir );
			GetMainTubeBoundingBox( mainTubeParam, out double dSize );

			// 2. translate the notch wire to the start of prism
			gp_Trsf trsf = new gp_Trsf();
			gp_Vec transVec = new gp_Vec( dir );
			transVec.Multiply( -dSize );
			trsf.SetTranslation( transVec );
			BRepBuilderAPI_Transform transform = new BRepBuilderAPI_Transform( notchWire, trsf );
			if( transform.IsDone() == false ) {
				return null;
			}

			// 3. make the prism
			gp_Vec prismVec = new gp_Vec( dir );
			prismVec.Multiply( dSize * 2 );
			return OCCTool.MakeConcretePrismByWire( TopoDS.ToWire( transform.Shape() ), prismVec, false );
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
