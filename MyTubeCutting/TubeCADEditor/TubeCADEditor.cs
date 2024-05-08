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
				if( m_CADFeatureParamMap.ParamMap.Count != 0 ) {
					MessageBox.Show( "Cannot remove main tube when there exist CAD Features." );
					return;
				}
				RemoveMainTubeCommand command = new RemoveMainTubeCommand( MAIN_TUBE_NAME, m_CADFeatureParamMap );
				DoCommand( command );
			}

			// remove cad feature
			else if( m_CADFeatureParamMap.ParamMap.ContainsKey( m_szEditObjName ) ) {
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
			else if( m_CADFeatureParamMap.ParamMap.ContainsKey( m_szEditObjName ) ) {
				ModifyCadFeatureCommand command = new ModifyCadFeatureCommand( m_szEditObjName, CloneHelper.Clone( m_EdiObjParam ), m_CADFeatureParamMap.ParamMap );
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

		// TODO: refine from here
		void UpdateAndRedrawResultTube()
		{
			// return when there exist no main tube
			if( m_CADFeatureParamMap.MainTubeParam == null ) {
				return;
			}

			// remove old tube
			if( m_ResultTubeAIS != null ) {
				m_Viewer.GetAISContext().Remove( m_ResultTubeAIS, false );
			}

			// make new tube
			List<CADft_EndCutterParam> endCutterParams = m_CADFeatureParamMap.ParamMap
				.Where( pair => pair.Value.Type == CADFeatureType.EndCutter )
				.Select( endCutterPair => (CADft_EndCutterParam)endCutterPair.Value ).ToList();
			List<CADft_BranchTubeParam> branchTubeParamList = m_CADFeatureParamMap.ParamMap
				.Where( pair => pair.Value.Type == CADFeatureType.BranchTube )
				.Select( branchTubePair => (CADft_BranchTubeParam)branchTubePair.Value ).ToList();
			TopoDS_Shape tubeShape = CADFeatureMaker.MakeResultTube( m_CADFeatureParamMap.MainTubeParam, endCutterParams, branchTubeParamList );

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
			AddCadFeatureCommand command = new AddCadFeatureCommand( szName, cadFeatureParam, m_CADFeatureParamMap.ParamMap );
			DoCommand( command );
		}

		void RemoveCADFeature( string szName )
		{
			// set command
			RemoveCadFeatureCommand command = new RemoveCadFeatureCommand( szName, m_CADFeatureParamMap.ParamMap );
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

		AIS_Shape MakeCADFeatureAIS( ICADFeatureParam cadFeatureParam )
		{
			// TODO: let CADFeatureMaker to do this
			AIS_Shape cadFeatureAIS = null;
			if( cadFeatureParam.Type == CADFeatureType.EndCutter ) {

				// make the face
				TopoDS_Face thePlane = CADFeatureMaker.MakeEndCutterFace( (CADft_EndCutterParam)cadFeatureParam );

				// make the extend bounding box of main tube
				TopoDS_Shape extendBndBox = CADFeatureMaker.MakeExtendBoundingBox( m_CADFeatureParamMap.MainTubeParam );

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
				cadFeatureAIS = new AIS_Shape( CADFeatureMaker.MakeBranchTube( (CADft_BranchTubeParam)cadFeatureParam ) );
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
			// here we need to use the pointer of the object, a null check is necessary
			// main tube
			if( m_szEditObjName == MAIN_TUBE_NAME && m_CADFeatureParamMap.MainTubeParam != null ) {
				m_EdiObjParam = CloneHelper.Clone( m_CADFeatureParamMap.MainTubeParam );
			}

			// cad feature
			else if( m_CADFeatureParamMap.ParamMap.ContainsKey( m_szEditObjName ) && m_CADFeatureParamMap.ParamMap[ m_szEditObjName ] != null ) {
				m_EdiObjParam = CloneHelper.Clone( m_CADFeatureParamMap.ParamMap[ m_szEditObjName ] );
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
			foreach( KeyValuePair<string, ICADFeatureParam> pair in m_CADFeatureParamMap.ParamMap ) {
				if( pair.Value.Type != CADFeatureType.EndCutter ) {
					continue;
				}

				// remove old ais from viewer
				m_Viewer.GetAISContext().Remove( m_CADFeatureNameAISMap[ pair.Key ], false );

				// create new ais
				ICADFeatureParam cadFeatureParam = m_CADFeatureParamMap.ParamMap[ pair.Key ];
				AIS_Shape cadFeatureAIS = MakeCADFeatureAIS( cadFeatureParam );

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
				if( m_CADFeatureParamMap.ParamMap.ContainsKey( szObjectName ) == false || m_CADFeatureParamMap.ParamMap[ szObjectName ] == null ) {
					MessageBox.Show( "Error: CAD Feature param not found in map." );
					return;
				}

				// make AIS and add into map
				AIS_Shape cadFeatureAIS = MakeCADFeatureAIS( m_CADFeatureParamMap.ParamMap[ szObjectName ] );
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
				if( m_CADFeatureParamMap.ParamMap.ContainsKey( szObjectName ) == false || m_CADFeatureParamMap.ParamMap[ szObjectName ] == null ) {
					MessageBox.Show( "Error: CAD Feature param not found in map." );
					return;
				}

				// create new ais
				ICADFeatureParam cadFeatureParam = m_CADFeatureParamMap.ParamMap[ m_szEditObjName ];
				cadFeatureAIS = MakeCADFeatureAIS( cadFeatureParam );
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
