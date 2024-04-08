using MyCore.CAD;
using MyCore.Tool;
using OCC.AIS;
using OCC.Graphic3d;
using OCC.Quantity;
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
		MainTubeParam m_MainTubeParam;
		Dictionary<string, ITubeMakeParam> m_CADFeatureNameParamMap = new Dictionary<string, ITubeMakeParam>();

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
		ITubeMakeParam m_EdiObjParam;

		internal TubeCADEditor( OCCViewer viewer, TreeView treeObjBrowser, PropertyGrid propertyGrid )
		{
			m_Viewer = viewer;
			m_treeObjBrowser = treeObjBrowser;
			m_propgrdPropertyBar = propertyGrid;
		}

		internal void SetMainTube( MainTubeParam mainTubeParam )
		{
			if( m_MainTubeNode == null ) {
				m_MainTubeNode = m_treeObjBrowser.Nodes.Add( MAIN_TUBE_NAME, MAIN_TUBE_NAME );
			}
			m_MainTubeParam = mainTubeParam;
			SetEditObject( MAIN_TUBE_NAME );
			UpdateAndRedrawResultTube();
		}

		internal void AddEndCutter( EndCutterParam endCutterParam )
		{
			string szName = GetNewEndCutterName();
			AddCADFeature( szName, endCutterParam );
		}

		internal void AddBranchTube( BranchTubeParam branchTubeParam )
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
				m_MainTubeParam = (MainTubeParam)( CloneHelper.Clone( m_EdiObjParam ) );
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
				ITubeMakeParam cadFeatureParam = m_CADFeatureNameParamMap[ m_szEditObjName ];
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
			List<EndCutterParam> endCutterParams = m_CADFeatureNameParamMap
				.Where( pair => pair.Value.Type == TubeMakeParamType.EndCutter )
				.Select( endCutterPair => (EndCutterParam)endCutterPair.Value ).ToList();
			List<BranchTubeParam> branchTubeParamList = m_CADFeatureNameParamMap
				.Where( pair => pair.Value.Type == TubeMakeParamType.BranchTube )
				.Select( branchTubePair => (BranchTubeParam)branchTubePair.Value ).ToList();
			TopoDS_Shape tubeShape = TubeMaker.MakeResultTube( m_MainTubeParam, endCutterParams, branchTubeParamList );

			// display new tube
			m_ResultTubeAIS = new AIS_Shape( tubeShape );
			Graphic3d_MaterialAspect aspect = new Graphic3d_MaterialAspect( Graphic3d_NameOfMaterial.Graphic3d_NOM_STEEL );
			m_ResultTubeAIS.SetMaterial( aspect );
			m_ResultTubeAIS.SetDisplayMode( 1 );
			m_Viewer.GetAISContext().Display( m_ResultTubeAIS, false );
			m_Viewer.UpdateView();
		}


		void AddCADFeature( string szName, ITubeMakeParam cadFeatureParam )
		{
			AIS_Shape cadFeatureAIS = MakeCADFeatureAIS( cadFeatureParam );

			// add into object browser
			m_MainTubeNode.Nodes.Add( szName, szName );

			// add into map
			m_CADFeatureNameAISMap.Add( szName, cadFeatureAIS );
			m_CADFeatureNameParamMap.Add( szName, cadFeatureParam );

			// set edit object (display)
			SetEditObject( szName );
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

		AIS_Shape MakeCADFeatureAIS( ITubeMakeParam cadFeatureParam )
		{
			AIS_Shape cadFeatureAIS = null;
			if( cadFeatureParam.Type == TubeMakeParamType.EndCutter ) {
				cadFeatureAIS = new AIS_Shape( TubeMaker.MakeEndCutter( (EndCutterParam)cadFeatureParam ) );
			}
			else if( cadFeatureParam.Type == TubeMakeParamType.BranchTube ) {
				cadFeatureAIS = new AIS_Shape( TubeMaker.MakeBranchTube( (BranchTubeParam)cadFeatureParam ) );
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
	}
}
