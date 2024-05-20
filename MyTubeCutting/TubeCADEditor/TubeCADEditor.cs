using MyCore.CAD;
using MyCore.Tool;
using OCC.AIS;
using OCC.Graphic3d;
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
		CADFeatureParamMap m_CADFeatureParamMap = new CADFeatureParamMap();

		// display shape map
		AIS_Shape m_ResultTubeAIS;
		Dictionary<string, AIS_Shape> m_CADFeatureNameAISMap = new Dictionary<string, AIS_Shape>();

		// object browser map
		TreeNode m_MainTubeNode;

		// action
		int m_nEndCutterCount = 1;
		int m_nBranchTubeCount = 1;
		const string MAIN_TUBE_NAME = "MainTube";
		string m_szEditObjName;
		ICADFeatureParam m_EdiObjParam;

		// command
		List<ICADEditCommand> m_CADEditUndoCommandQueue = new List<ICADEditCommand>();
		List<ICADEditCommand> m_CADEditRedoCommandQueue = new List<ICADEditCommand>();

		// main tube status changed event
		internal delegate void MainTubeStatusChangedEventHandler( bool bExistMainTube );
		internal event MainTubeStatusChangedEventHandler MainTubeStatusChanged;

		internal TubeCADEditor( OCCViewer viewer, TreeView treeObjBrowser, PropertyGrid propertyGrid )
		{
			m_Viewer = viewer;
			m_treeObjBrowser = treeObjBrowser;
			m_propgrdPropertyBar = propertyGrid;
		}

		internal void AddMainTube( CADft_MainTubeParam mainTubeParam )
		{
			AddMainTubeCommand command = new AddMainTubeCommand( MAIN_TUBE_NAME, mainTubeParam, m_CADFeatureParamMap );
			DoCommand( command );
		}

		internal void AddEndCutter( CADft_EndCutterParam endCutterParam )
		{
			if( m_CADFeatureParamMap.MainTubeParam == null ) {
				MessageBox.Show( "Please add main tube first." );
				return;
			}
			string szName = GetNewEndCutterName();
			AddCADFeature( szName, endCutterParam );
		}

		internal void AddBranchTube( CADft_BranchTubeParam branchTubeParam )
		{
			if( m_CADFeatureParamMap.MainTubeParam == null ) {
				MessageBox.Show( "Please add main tube first." );
				return;
			}
			string szName = GetNewBranchTubeName();
			AddCADFeature( szName, branchTubeParam );
		}

		internal void RemoveCADFeature()
		{
			// remove main tube
			if( m_szEditObjName == MAIN_TUBE_NAME ) {

				// can not remove main tube when there exist CAD Features
				if( m_CADFeatureParamMap.FeatureMap.Count != 0 ) {
					MessageBox.Show( "Cannot remove main tube when there exist CAD Features." );
					return;
				}
				RemoveMainTubeCommand command = new RemoveMainTubeCommand( MAIN_TUBE_NAME, m_CADFeatureParamMap );
				DoCommand( command );
			}

			// remove cad feature
			else if( m_CADFeatureParamMap.FeatureMap.ContainsKey( m_szEditObjName ) ) {
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

				ModifyMainTubeCommand command = new ModifyMainTubeCommand( MAIN_TUBE_NAME, CloneHelper.Clone( m_EdiObjParam ), m_CADFeatureParamMap );
				DoCommand( command );
			}

			// cad feature
			else if( m_CADFeatureParamMap.FeatureMap.ContainsKey( m_szEditObjName ) ) {
				ModifyCadFeatureCommand command = new ModifyCadFeatureCommand( m_szEditObjName, CloneHelper.Clone( m_EdiObjParam ), m_CADFeatureParamMap.FeatureMap );
				DoCommand( command );
			}
		}

		internal void Undo()
		{
			// no command to undo
			if( m_CADEditUndoCommandQueue.Count == 0 ) {
				return;
			}

			// undo command
			m_CADEditUndoCommandQueue.Last().Undo();

			// move command to redo queue
			m_CADEditRedoCommandQueue.Add( m_CADEditUndoCommandQueue.Last() );

			// remove command from undo queue
			m_CADEditUndoCommandQueue.RemoveAt( m_CADEditUndoCommandQueue.Count - 1 );
		}

		internal void Redo()
		{
			// no command to redo
			if( m_CADEditRedoCommandQueue.Count == 0 ) {
				return;
			}

			// redo command
			m_CADEditRedoCommandQueue.Last().Do();

			// move command to undo queue
			m_CADEditUndoCommandQueue.Add( m_CADEditRedoCommandQueue.Last() );

			// remove command from redo queue
			m_CADEditRedoCommandQueue.RemoveAt( m_CADEditRedoCommandQueue.Count - 1 );
		}

		void UpdateAndRedrawResultTube()
		{
			// remove all shape if main tube not exist
			if( m_CADFeatureParamMap.MainTubeParam == null ) {
				m_Viewer.GetAISContext().RemoveAll( true );
				return;
			}

			// remove old tube
			if( m_ResultTubeAIS != null ) {
				m_Viewer.GetAISContext().Remove( m_ResultTubeAIS, false );
			}

			// make new tube
			TopoDS_Shape tubeShape = CADFeatureMaker.MakeResultTube( m_CADFeatureParamMap );

			// data protection
			if( tubeShape == null ) {
				return;
			}

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
			// set command
			AddCadFeatureCommand command = new AddCadFeatureCommand( szName, cadFeatureParam, m_CADFeatureParamMap.FeatureMap );
			DoCommand( command );
		}

		void RemoveCADFeature( string szName )
		{
			// set command
			RemoveCadFeatureCommand command = new RemoveCadFeatureCommand( szName, m_CADFeatureParamMap.FeatureMap );
			DoCommand( command );
		}

		string GetNewEndCutterName()
		{
			return "EndCutter" + m_nEndCutterCount++;
		}

		string GetNewBranchTubeName()
		{
			return "BranchTube" + m_nBranchTubeCount++;
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
			// here we need to use the pointer of the object, a null check is necessary
			// main tube
			if( m_szEditObjName == MAIN_TUBE_NAME && m_CADFeatureParamMap.MainTubeParam != null ) {
				m_EdiObjParam = CloneHelper.Clone( m_CADFeatureParamMap.MainTubeParam );
			}

			// cad feature
			else if( m_CADFeatureParamMap.FeatureMap.ContainsKey( m_szEditObjName ) && m_CADFeatureParamMap.FeatureMap[ m_szEditObjName ] != null ) {
				m_EdiObjParam = CloneHelper.Clone( m_CADFeatureParamMap.FeatureMap[ m_szEditObjName ] );
			}
			m_propgrdPropertyBar.SelectedObject = m_EdiObjParam;
			m_propgrdPropertyBar.ExpandAllGridItems();
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
			foreach( KeyValuePair<string, ICADFeatureParam> pair in m_CADFeatureParamMap.FeatureMap ) {
				if( pair.Value.Type != CADFeatureType.EndCutter ) {
					continue;
				}

				// create new ais
				ICADFeatureParam cadFeatureParam = m_CADFeatureParamMap.FeatureMap[ pair.Key ];
				AIS_Shape cadFeatureAIS = CADFeatureMaker.MakeCADFeatureAIS( cadFeatureParam, m_CADFeatureParamMap.MainTubeParam );
				if( cadFeatureAIS == null ) {
					MessageBox.Show( "Error: CAD Feature AIS generated failed." );
					return;
				}

				// remove old ais from viewer
				m_Viewer.GetAISContext().Remove( m_CADFeatureNameAISMap[ pair.Key ], false );

				// update ais map
				m_CADFeatureNameAISMap[ pair.Key ] = cadFeatureAIS;
			}

			// update display
			m_Viewer.UpdateView();
		}

		void UpdateEditorAfterCommand( EditType type, string szObjectName )
		{
			if( type == EditType.AddMainTube ) {

				// remove old main tube node from object browser if exist, should not happen
				if( m_MainTubeNode != null ) {
					m_treeObjBrowser.Nodes.Remove( m_MainTubeNode );
				}

				// add new main tube node
				m_MainTubeNode = m_treeObjBrowser.Nodes.Add( MAIN_TUBE_NAME, MAIN_TUBE_NAME );
				m_treeObjBrowser.Focus();
				m_treeObjBrowser.SelectedNode = m_MainTubeNode;

				// update property bar
				SetEditObject( MAIN_TUBE_NAME );

				// invoke the main tube status changed event
				MainTubeStatusChanged( true );
			}
			else if( type == EditType.RemoveMainTube ) {

				// remove main tube node from object browser
				m_treeObjBrowser.Nodes.Remove( m_MainTubeNode );

				// set null to main tube node
				m_MainTubeNode = null;

				// update property bar, need to set null to clear the property bar
				m_propgrdPropertyBar.SelectedObject = null;

				// invoke the main tube status changed event
				MainTubeStatusChanged( false );
			}
			else if( type == EditType.ModifyMainTube ) {

				// need to refresh cut plane shape when main tube size changed
				RefreshCutPlaneShape();
			}
			else if( type == EditType.AddCADFeature ) {

				// data protection
				if( m_CADFeatureParamMap.FeatureMap.ContainsKey( szObjectName ) == false || m_CADFeatureParamMap.FeatureMap[ szObjectName ] == null ) {
					MessageBox.Show( "Error: CAD Feature param not found in map." );
					return;
				}

				// make AIS and add into map
				AIS_Shape cadFeatureAIS = CADFeatureMaker.MakeCADFeatureAIS( m_CADFeatureParamMap.FeatureMap[ szObjectName ], m_CADFeatureParamMap.MainTubeParam );
				if( cadFeatureAIS == null ) {
					MessageBox.Show( "Error: CAD Feature AIS generated failed." );
					return;
				}
				m_CADFeatureNameAISMap.Add( szObjectName, cadFeatureAIS );

				// TODO: reconstruct object browser, check main tube node first
				TreeNode newNode = m_MainTubeNode.Nodes.Add( szObjectName, szObjectName );

				// these will call SetEditObject, and expand the object browser
				m_treeObjBrowser.Focus();
				m_treeObjBrowser.SelectedNode = newNode;
			}
			else if( type == EditType.RemoveCADFeature ) {

				// remove from display and map
				if( m_CADFeatureNameAISMap.ContainsKey( szObjectName ) == false ) {
					MessageBox.Show( "Error: CAD Feature AIS not found in map." );
					return;
				}
				m_Viewer.GetAISContext().Remove( m_CADFeatureNameAISMap[ szObjectName ], false );
				m_CADFeatureNameAISMap.Remove( szObjectName );

				// TODO: reconstruct object browser, check main tube node first
				m_MainTubeNode.Nodes.RemoveByKey( szObjectName );

				// the object browser will find a node to select, so do not need to set edit object
			}
			else if( type == EditType.ModifyCADFeature ) {

				// get old ais from map
				if( m_CADFeatureNameAISMap.ContainsKey( szObjectName ) == false ) {
					MessageBox.Show( "Error: CAD Feature AIS not found in map." );
					return;
				}
				AIS_Shape cadFeatureAIS = m_CADFeatureNameAISMap[ m_szEditObjName ];

				// remove old ais from viewer
				m_Viewer.GetAISContext().Remove( cadFeatureAIS, false );

				// data protection
				if( m_CADFeatureParamMap.FeatureMap.ContainsKey( szObjectName ) == false || m_CADFeatureParamMap.FeatureMap[ szObjectName ] == null ) {
					MessageBox.Show( "Error: CAD Feature param not found in map." );
					return;
				}

				// create new ais
				ICADFeatureParam cadFeatureParam = m_CADFeatureParamMap.FeatureMap[ m_szEditObjName ];
				cadFeatureAIS = CADFeatureMaker.MakeCADFeatureAIS( cadFeatureParam, m_CADFeatureParamMap.MainTubeParam );
				if( cadFeatureAIS == null ) {
					MessageBox.Show( "Error: CAD Feature AIS generated failed." );
					return;
				}

				// display new ais and update ais map
				m_Viewer.GetAISContext().Display( cadFeatureAIS, false );
				m_CADFeatureNameAISMap[ m_szEditObjName ] = cadFeatureAIS;
			}

			// update display
			UpdateAndRedrawResultTube();
		}

		void DoCommand( ICADEditCommand command )
		{
			command.EditFinished += UpdateEditorAfterCommand;
			command.Do();
			m_CADEditUndoCommandQueue.Add( command );
			m_CADEditRedoCommandQueue.Clear();
		}
	}
}
