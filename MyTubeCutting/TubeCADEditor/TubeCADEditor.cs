using MyCore.CAD;
using MyCore.Tool;
using OCC.AIS;
using OCC.BRep;
using OCC.BRepAlgoAPI;
using OCC.BRepBuilderAPI;
using OCC.BRepTools;
using OCC.Geom;
using OCC.Graphic3d;
using OCC.Quantity;
using OCC.TopAbs;
using OCC.TopExp;
using OCC.TopoDS;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MyTubeCutting
{
	internal class TubeCADEditor
	{
		// GUI
		OCCViewer m_Viewer;
		TreeView m_treeObjBrowser;
		PropertyGrid m_propgrdPropertyBar;

		// parameter map
		CADft_MainTubeParam m_MainTubeParam;
		Dictionary<string, ICADFeatureParam> m_CADFeatureNameParamMap = new Dictionary<string, ICADFeatureParam>();

		// display shape map
		AIS_Shape m_ResultTubeAIS;
		Dictionary<string, AIS_Shape> m_CADFeatureNameAISMap = new Dictionary<string, AIS_Shape>();

		// object browser map
		TreeNode m_MainTubeNode;

		// interaction
		int m_nEndCutterCount = 1;
		int m_nBranchTubeCount = 1;
		const string MAIN_TUBE_NAME = "MainTube";
		string m_szEditObjName;
		ICADFeatureParam m_EdiObjParam;

		internal TubeCADEditor( OCCViewer viewer, TreeView treeObjBrowser, PropertyGrid propertyGrid )
		{
			m_Viewer = viewer;
			m_treeObjBrowser = treeObjBrowser;
			m_propgrdPropertyBar = propertyGrid;
		}

		internal void SetMainTube( CADft_MainTubeParam mainTubeParam )
		{
			if( m_MainTubeNode == null ) {
				m_MainTubeNode = m_treeObjBrowser.Nodes.Add( MAIN_TUBE_NAME, MAIN_TUBE_NAME );
			}
			m_MainTubeParam = mainTubeParam;
			SetEditObject( MAIN_TUBE_NAME );
			UpdateAndRedrawResultTube();
		}

		internal void AddEndCutter( CADft_EndCutterParam endCutterParam )
		{
			string szName = GetNewEndCutterName();
			AddCADFeature( szName, endCutterParam );
		}

		internal void AddBranchTube( CADft_BranchTubeParam branchTubeParam )
		{
			string szName = GetNewBranchTubeName();
			AddCADFeature( szName, branchTubeParam );
		}

		internal void RemoveCADFeature()
		{
			// cannot remove main tube
			if( m_szEditObjName == MAIN_TUBE_NAME ) {
				return;
			}

			// remove cad feature
			else if( m_CADFeatureNameParamMap.ContainsKey( m_szEditObjName ) ) {
				RemoveCADFeature( m_szEditObjName );
			}
		}

		internal void SetEditObject( string szObjName )
		{
			// data protection
			if( string.IsNullOrEmpty( szObjName ) ) {
				return;
			}
			if( szObjName == m_szEditObjName ) {
				return;
			}

			m_szEditObjName = szObjName;
			DisplayObjectShape();
			ShowObjectProperty();
		}

		internal void UpdateObjectProperty( object s, PropertyValueChangedEventArgs e )
		{
			// if new parameter is invalid, restore old parameter and show property
			if( m_EdiObjParam.IsValid() == false ) {
				ShowObjectProperty();
				return;
			}

			// main tube
			if( m_szEditObjName == MAIN_TUBE_NAME ) {
				m_MainTubeParam = (CADft_MainTubeParam)( CloneHelper.Clone( m_EdiObjParam ) );

				// need to refresh cut plane shape when main tube size changed
				RefreshCutPlaneShape();
				UpdateAndRedrawResultTube();
				return;
			}

			// cad feature
			else if( m_CADFeatureNameParamMap.ContainsKey( m_szEditObjName ) ) {
				m_CADFeatureNameParamMap[ m_szEditObjName ] = CloneHelper.Clone( m_EdiObjParam );

				// get ais from map
				AIS_Shape cadFeatureAIS = m_CADFeatureNameAISMap[ m_szEditObjName ];

				// remove old ais from viewer
				m_Viewer.GetAISContext().Remove( cadFeatureAIS, false );

				// create new ais
				ICADFeatureParam cadFeatureParam = m_CADFeatureNameParamMap[ m_szEditObjName ];
				cadFeatureAIS = MakeCADFeatureAIS( cadFeatureParam );

				// update ais map
				m_CADFeatureNameAISMap[ m_szEditObjName ] = cadFeatureAIS;

				// display new ais
				m_Viewer.GetAISContext().Display( cadFeatureAIS, false );
			}

			// redraw result tube
			UpdateAndRedrawResultTube();
		}

		internal bool IsExistMainTube()
		{
			return m_MainTubeParam != null;
		}

		void UpdateAndRedrawResultTube()
		{
			// remove old tube
			if( m_ResultTubeAIS != null ) {
				m_Viewer.GetAISContext().Remove( m_ResultTubeAIS, false );
			}

			// make new tube
			List<CADft_EndCutterParam> endCutterParams = m_CADFeatureNameParamMap
				.Where( pair => pair.Value.Type == CADFeatureType.EndCutter )
				.Select( endCutterPair => (CADft_EndCutterParam)endCutterPair.Value ).ToList();
			List<CADft_BranchTubeParam> branchTubeParamList = m_CADFeatureNameParamMap
				.Where( pair => pair.Value.Type == CADFeatureType.BranchTube )
				.Select( branchTubePair => (CADft_BranchTubeParam)branchTubePair.Value ).ToList();
			TopoDS_Shape tubeShape = TubeMaker.MakeResultTube( m_MainTubeParam, endCutterParams, branchTubeParamList );

			// display new tube
			m_ResultTubeAIS = new AIS_Shape( tubeShape );
			Graphic3d_MaterialAspect aspect = new Graphic3d_MaterialAspect( Graphic3d_NameOfMaterial.Graphic3d_NOM_STEEL );
			m_ResultTubeAIS.SetMaterial( aspect );
			m_ResultTubeAIS.SetDisplayMode( 1 );
			m_Viewer.GetAISContext().Display( m_ResultTubeAIS, false );
			m_Viewer.UpdateView();
		}


		void AddCADFeature( string szName, ICADFeatureParam cadFeatureParam )
		{
			// make AIS
			AIS_Shape cadFeatureAIS = MakeCADFeatureAIS( cadFeatureParam );

			// add node into object browser
			TreeNode newNode = m_MainTubeNode.Nodes.Add( szName, szName );

			// add into map
			m_CADFeatureNameAISMap.Add( szName, cadFeatureAIS );
			m_CADFeatureNameParamMap.Add( szName, cadFeatureParam );

			// these will call SetEditObject
			m_treeObjBrowser.Focus();
			m_treeObjBrowser.SelectedNode = newNode;

			// update display
			UpdateAndRedrawResultTube();
		}

		void RemoveCADFeature( string szName )
		{
			// remove from display
			m_Viewer.GetAISContext().Remove( m_CADFeatureNameAISMap[ szName ], false );

			// remove from map
			m_CADFeatureNameParamMap.Remove( szName );
			m_CADFeatureNameAISMap.Remove( szName );

			// remove from object browser
			m_MainTubeNode.Nodes.RemoveByKey( szName );

			// update display
			UpdateAndRedrawResultTube();
		}

		string GetNewEndCutterName()
		{
			return "EndCutter" + m_nEndCutterCount++;
		}

		string GetNewBranchTubeName()
		{
			return "BranchTube" + m_nBranchTubeCount++;
		}

		AIS_Shape MakeCADFeatureAIS( ICADFeatureParam cadFeatureParam )
		{
			AIS_Shape cadFeatureAIS = null;
			if( cadFeatureParam.Type == CADFeatureType.EndCutter ) {

				// make the face
				TopoDS_Face thePlane = TubeMaker.MakeEndCutterFace( (CADft_EndCutterParam)cadFeatureParam );

				// make the extend bounding box of main tube
				TopoDS_Shape extendBndBox = TubeMaker.MakeExtendBoundingBox( m_MainTubeParam );

				// find the common part of the face and the extend bounding box
				BRepAlgoAPI_Common common = new BRepAlgoAPI_Common( thePlane, extendBndBox );
				if( common.IsDone() == false ) {
					return cadFeatureAIS;
				}

				// retrive the geom surface and uv boundary from the section shape
				TopoDS_Face commonFace;
				TopExp_Explorer explorer = new TopExp_Explorer( common.Shape(), TopAbs_ShapeEnum.TopAbs_FACE );
				if( explorer.More() ) {
					commonFace = TopoDS.ToFace( explorer.Current() );
				}
				else {
					return cadFeatureAIS;
				}
				Geom_Surface commonSurface = BRep_Tool.Surface( commonFace );
				double Umin = 0;
				double Umax = 0;
				double Vmin = 0;
				double Vmax = 0;
				BRepTools.UVBounds( commonFace, ref Umin, ref Umax, ref Vmin, ref Vmax );
				Geom_RectangularTrimmedSurface refinedSurface = new Geom_RectangularTrimmedSurface( commonSurface, Umin, Umax, Vmin, Vmax );
				BRepBuilderAPI_MakeFace refinedFaceMaker = new BRepBuilderAPI_MakeFace( refinedSurface, 0.001 );

				cadFeatureAIS = new AIS_Shape( refinedFaceMaker.Face() );
			}
			else if( cadFeatureParam.Type == CADFeatureType.BranchTube ) {
				cadFeatureAIS = new AIS_Shape( TubeMaker.MakeBranchTube( (CADft_BranchTubeParam)cadFeatureParam ) );
			}
			else {
				return cadFeatureAIS;
			}
			Graphic3d_MaterialAspect aspect = new Graphic3d_MaterialAspect( Graphic3d_NameOfMaterial.Graphic3d_NOM_STONE );
			aspect.SetTransparency( 0.5f );
			aspect.SetColor( new Quantity_Color( Quantity_NameOfColor.Quantity_NOC_GREEN4 ) );
			cadFeatureAIS.SetMaterial( aspect );
			cadFeatureAIS.SetDisplayMode( 1 );
			return cadFeatureAIS;
		}

		void DisplayObjectShape()
		{
			HideAllShapeExceptMainTube();

			// just hide all shape except main tube
			if( m_szEditObjName == MAIN_TUBE_NAME ) {
				return;
			}

			// display selected object shape
			if( m_CADFeatureNameAISMap.ContainsKey( m_szEditObjName ) == false ) {
				return;
			}
			m_Viewer.GetAISContext().Display( m_CADFeatureNameAISMap[ m_szEditObjName ], false );
			m_Viewer.UpdateView();
		}

		void ShowObjectProperty()
		{
			// main tube
			if( m_szEditObjName == MAIN_TUBE_NAME ) {
				m_EdiObjParam = CloneHelper.Clone( m_MainTubeParam );
			}

			// cad feature
			else if( m_CADFeatureNameParamMap.ContainsKey( m_szEditObjName ) ) {
				m_EdiObjParam = CloneHelper.Clone( m_CADFeatureNameParamMap[ m_szEditObjName ] );
			}
			m_propgrdPropertyBar.SelectedObject = m_EdiObjParam;
		}

		void HideAllShapeExceptMainTube()
		{
			foreach( KeyValuePair<string, AIS_Shape> pair in m_CADFeatureNameAISMap ) {
				m_Viewer.GetAISContext().Erase( pair.Value, false );
			}
			m_Viewer.UpdateView();
		}

		void RefreshCutPlaneShape()
		{
			foreach( KeyValuePair<string, ICADFeatureParam> pair in m_CADFeatureNameParamMap ) {
				if( pair.Value.Type != CADFeatureType.EndCutter ) {
					continue;
				}

				// remove old ais from viewer
				m_Viewer.GetAISContext().Remove( m_CADFeatureNameAISMap[ pair.Key ], false );

				// create new ais
				ICADFeatureParam cadFeatureParam = m_CADFeatureNameParamMap[ pair.Key ];
				AIS_Shape cadFeatureAIS = MakeCADFeatureAIS( cadFeatureParam );

				// update ais map
				m_CADFeatureNameAISMap[ pair.Key ] = cadFeatureAIS;
			}

			// update display
			m_Viewer.UpdateView();
		}
	}
}
