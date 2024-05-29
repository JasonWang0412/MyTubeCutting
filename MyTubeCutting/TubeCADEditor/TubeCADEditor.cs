using MyCore.CAD;
using MyCore.Tool;
using OCC.AIS;
using OCC.gp;
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

		// the result shape
		TopoDS_Shape m_ResultTubeTopo;

		// object browser map
		TreeNode m_MainTubeNode;

		// action
		int m_nEndCutterCount = 1;
		int m_nBranchTubeCount = 1;
		int m_nBendingNotchCount = 1;
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

		internal void AddBendingNotch( CADft_BendingNotchParam bendingNotchParam )
		{
			if( m_CADFeatureParamMap.MainTubeParam == null ) {
				MessageBox.Show( "Please add main tube first." );
				return;
			}
			string szName = GetNewBendingNotchName();
			AddCADFeature( szName, bendingNotchParam );
		}

		internal void RemoveCADFeature()
		{
			// remove main tube
			if( m_szEditObjName == MAIN_TUBE_NAME ) {
				RemoveMainTubeCommand command = new RemoveMainTubeCommand( MAIN_TUBE_NAME, m_CADFeatureParamMap );
				DoCommand( command );
			}

			// remove cad feature
			else {
				RemoveCadFeatureCommand command = new RemoveCadFeatureCommand( m_szEditObjName, m_CADFeatureParamMap );
				DoCommand( command );
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

		internal void ModifyObjectProperty( object s, PropertyValueChangedEventArgs e )
		{
			CommandErrorCode error;
			// main tube
			if( m_szEditObjName == MAIN_TUBE_NAME ) {
				ModifyMainTubeCommand command = new ModifyMainTubeCommand( MAIN_TUBE_NAME, CloneHelper.Clone( m_EdiObjParam ), m_CADFeatureParamMap );
				error = DoCommand( command );
			}

			// cad feature
			else {
				ModifyCadFeatureCommand command = new ModifyCadFeatureCommand( m_szEditObjName, CloneHelper.Clone( m_EdiObjParam ), m_CADFeatureParamMap );
				error = DoCommand( command );
			}

			if( error != CommandErrorCode.OK ) {

				// show original property when modify failed
				ShowObjectProperty();
			}
		}

		internal gp_Dir GetEditObjectDir()
		{
			// main tube
			if( m_szEditObjName == MAIN_TUBE_NAME
				&& m_CADFeatureParamMap.MainTubeParam != null ) {
				return new gp_Dir( 0, 1, 0 );
			}

			// cad feature
			if( m_CADFeatureParamMap.FeatureMap.ContainsKey( m_szEditObjName )
				&& m_CADFeatureParamMap.FeatureMap[ m_szEditObjName ] != null ) {
				return CADFeatureMaker.GetCADFeatureDir( m_CADFeatureParamMap.FeatureMap[ m_szEditObjName ] );
			}

			// default dir
			return new gp_Dir( 0, 1, 0 );
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

		internal void ExportStep()
		{
			ExportHelper.ExportStep( m_ResultTubeTopo, "ResultTube" );
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
				m_ResultTubeAIS = null;
			}

			// make new tube
			TopoDS_Shape tubeShape = CADFeatureMaker.MakeResultTube( m_CADFeatureParamMap );
			if( tubeShape == null ) {
				return;
			}

			// display new tube
			m_ResultTubeTopo = tubeShape;
			m_ResultTubeAIS = new AIS_Shape( tubeShape );
			Graphic3d_MaterialAspect aspect = new Graphic3d_MaterialAspect( Graphic3d_NameOfMaterial.Graphic3d_NOM_STEEL );
			m_ResultTubeAIS.SetMaterial( aspect );
			m_ResultTubeAIS.SetDisplayMode( 1 );
			m_Viewer.GetAISContext().Display( m_ResultTubeAIS, false );
			m_Viewer.UpdateView();
		}

		void AddCADFeature( string szName, ICADFeatureParam cadFeatureParam )
		{
			// TODO: let command check validility
			AddCadFeatureCommand command = new AddCadFeatureCommand( szName, cadFeatureParam, m_CADFeatureParamMap );
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

		string GetNewBendingNotchName()
		{
			return "BendingNotch" + m_nBendingNotchCount++;
		}

		void DisplayObjectShape()
		{
			HideAllShapeExceptMainTube();

			// just hide all shape except main tube
			if( m_szEditObjName == MAIN_TUBE_NAME ) {
				return;
			}

			// display selected object shape
			else {
				if( m_CADFeatureNameAISMap.ContainsKey( m_szEditObjName ) == false || m_CADFeatureNameAISMap[ m_szEditObjName ] == null ) {
					MessageBox.Show( "Error: CAD Feature AIS not found in map." );
					return;
				}
			}
			m_Viewer.GetAISContext().Display( m_CADFeatureNameAISMap[ m_szEditObjName ], false );
			m_Viewer.UpdateView();
		}

		void ShowObjectProperty()
		{
			// here we need to use the pointer of the object, a null check is necessary
			// main tube
			if( m_szEditObjName == MAIN_TUBE_NAME ) {
				if( m_CADFeatureParamMap.MainTubeParam == null ) {
					MessageBox.Show( "Error: Main tube param not found in map." );
					return;
				}
				m_EdiObjParam = CloneHelper.Clone( m_CADFeatureParamMap.MainTubeParam );
			}

			// cad feature
			else {
				if( m_CADFeatureParamMap.FeatureMap.ContainsKey( m_szEditObjName ) == false || m_CADFeatureParamMap.FeatureMap[ m_szEditObjName ] == null ) {
					MessageBox.Show( "Error: CAD Feature param not found in map." );
					return;
				}
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

		void RefreshAIS()
		{
			foreach( KeyValuePair<string, ICADFeatureParam> pair in m_CADFeatureParamMap.FeatureMap ) {

				// create new ais
				ICADFeatureParam cadFeatureParam = pair.Value;
				if( cadFeatureParam == null ) {
					MessageBox.Show( "Error: CAD Feature param not found in map." );
					return;
				}
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

				// remove old node from object browser if exist, should not happen
				if( m_treeObjBrowser.Nodes.Count != 0 ) {
					m_treeObjBrowser.Nodes.Clear();
				}

				// add new main tube node
				m_MainTubeNode = m_treeObjBrowser.Nodes.Add( MAIN_TUBE_NAME, MAIN_TUBE_NAME );
				m_treeObjBrowser.Focus();
				m_treeObjBrowser.SelectedNode = m_MainTubeNode;

				// update property bar
				SetEditObject( MAIN_TUBE_NAME );

				// invoke the main tube status changed event
				MainTubeStatusChanged?.Invoke( true );
			}
			else if( type == EditType.RemoveMainTube ) {

				// remove main tube node from object browser
				m_treeObjBrowser.Nodes.Clear();
				m_MainTubeNode = null;

				// update property bar, need to set null to clear the property bar
				m_propgrdPropertyBar.SelectedObject = null;

				// invoke the main tube status changed event
				MainTubeStatusChanged?.Invoke( false );
			}
			else if( type == EditType.ModifyMainTube ) {

				// need to refresh cad feature AIS shape when main tube size changed
				RefreshAIS();
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
				if( m_CADFeatureNameAISMap.ContainsKey( szObjectName ) ) {
					m_Viewer.GetAISContext().Remove( m_CADFeatureNameAISMap[ szObjectName ], false );
					m_CADFeatureNameAISMap.Remove( szObjectName );
				}

				// TODO: reconstruct object browser, check main tube node first
				m_MainTubeNode.Nodes.RemoveByKey( szObjectName );

				// the object browser will find a node to select, so do not need to set edit object
			}
			else if( type == EditType.ModifyCADFeature ) {

				// data protection
				if( m_CADFeatureParamMap.FeatureMap.ContainsKey( szObjectName ) == false || m_CADFeatureParamMap.FeatureMap[ szObjectName ] == null ) {
					MessageBox.Show( "Error: CAD Feature param not found in map." );
					return;
				}

				// make new ais
				ICADFeatureParam cadFeatureParam = m_CADFeatureParamMap.FeatureMap[ m_szEditObjName ];
				AIS_Shape newCADFeatureAIS = CADFeatureMaker.MakeCADFeatureAIS( cadFeatureParam, m_CADFeatureParamMap.MainTubeParam );
				if( newCADFeatureAIS == null ) {
					MessageBox.Show( "Error: CAD Feature AIS generated failed." );
					return;
				}

				// remove old ais from display and map
				if( m_CADFeatureNameAISMap.ContainsKey( szObjectName ) ) {
					m_Viewer.GetAISContext().Remove( m_CADFeatureNameAISMap[ szObjectName ], false );
					m_CADFeatureNameAISMap.Remove( szObjectName );
				}

				// display new ais and update ais map
				m_Viewer.GetAISContext().Display( newCADFeatureAIS, false );
				m_CADFeatureNameAISMap[ m_szEditObjName ] = newCADFeatureAIS;
			}

			// update display
			UpdateAndRedrawResultTube();
		}

		CommandErrorCode DoCommand( ICADEditCommand command )
		{
			command.EditFinished += UpdateEditorAfterCommand;
			CommandErrorCode error = command.Do();
			if( error != CommandErrorCode.OK ) {

				// TODO: show error message
				MessageBox.Show( "Error: Command failed." );
				return error;
			}
			m_CADEditUndoCommandQueue.Add( command );
			m_CADEditRedoCommandQueue.Clear();
			return error;
		}
	}
}
