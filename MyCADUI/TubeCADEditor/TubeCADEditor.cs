using MyCADCore;
using MyLanguageManager;
using OCC.AIS;
using OCC.gp;
using OCC.Graphic3d;
using OCC.STEPControl;
using OCC.TopoDS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using Utility;

namespace MyCADUI
{
	internal enum ViewDir
	{
		Top,
		Bottom,
		Left,
		Right,
		Front,
		Back,
		Isometric,
		Dir_Pos,
		Dir_Neg,
	}

	internal class TubeCADEditor
	{
		// GUI
		Panel m_panViewer = new Panel();
		Panel m_panObjBrowser = new Panel();
		Panel m_panPropertyBar = new Panel();
		OCCViewer m_Viewer = new OCCViewer();
		TreeView m_treeObjBrowser = new TreeView();
		PropertyGrid m_propgrdPropertyBar = new PropertyGrid();
		bool m_bSupressBrowserSelectEvent = false;

		// viewer
		int m_nXMousePosition = 0;
		int m_nYMousePosition = 0;

		// parameter map
		CADFeatureParamMap m_CADFeatureParamMap = new CADFeatureParamMap();

		// display shape map
		AIS_Shape m_ResultTubeAIS;
		Dictionary<string, AIS_Shape> m_CADFeatureNameAISMap = new Dictionary<string, AIS_Shape>();

		// object browser map
		TreeNode m_MainTubeNode;

		// feature name
		int m_nEndCutterCount = 1;
		int m_nBranchTubeCount = 1;
		int m_nBendingNotchCount = 1;
		string m_szMainTubeName = "MainTube";
		LanguageManager m_LanguageManager = new LanguageManager( "TubeCADEditor" );

		// command
		List<ICADEditCommand> m_CADEditUndoCommandQueue = new List<ICADEditCommand>();
		List<ICADEditCommand> m_CADEditRedoCommandQueue = new List<ICADEditCommand>();
		internal delegate void CommandStatusChangedEventHandler( bool bUndo, bool bRedo );
		internal event CommandStatusChangedEventHandler CommandStatusChanged;

		// main tube status changed event
		internal delegate void MainTubeStatusChangedEventHandler( bool bExistMainTube );
		internal event MainTubeStatusChangedEventHandler MainTubeStatusChanged;

		// cad edit error event
		internal delegate void CADEditErrorEventHandler( CADEditErrorCode errorCode );
		internal event CADEditErrorEventHandler CADEditErrorEvent;

		// cad edit sucess event
		internal delegate void CADEditSuccessEventHandler();
		internal event CADEditSuccessEventHandler CADEditSuccessEvent;

		internal TubeCADEditor()
		{
			bool isSuccess = m_Viewer.InitViewer( m_panViewer.Handle );
			if( isSuccess == false ) {
				MessageBox.Show( "init failed" );
			}
			m_Viewer.SetBackgroundColor( 0, 0, 0 );
			m_Viewer.IsometricView();
			m_panViewer.Dock = DockStyle.Fill;

			m_treeObjBrowser.Dock = DockStyle.Fill;
			m_panObjBrowser.Controls.Add( m_treeObjBrowser );
			m_panObjBrowser.Dock = DockStyle.Fill;

			m_propgrdPropertyBar.Dock = DockStyle.Fill;
			m_panPropertyBar.Controls.Add( m_propgrdPropertyBar );
			m_panPropertyBar.Dock = DockStyle.Fill;

			// action
			m_panViewer.Paint += m_panViewer_Paint;
			m_panViewer.MouseDown += m_panViewer_MouseDown;
			m_panViewer.MouseMove += m_panViewer_MouseMove;
			m_panViewer.MouseWheel += m_panViewer_MouseWheel;

			m_treeObjBrowser.KeyDown += m_treeObjBrowser_KeyDown;
			m_treeObjBrowser.AfterSelect += m_treeObjBrowser_AfterSelect;

			m_propgrdPropertyBar.PropertyValueChanged += m_propgrdPropertyBar_PropertyValueChanged;
		}

		internal void AddMainTube( CADft_MainTubeParam mainTubeParam )
		{
			// data protection
			if( mainTubeParam == null ) {
				CADEditErrorEvent?.Invoke( CADEditErrorCode.NullParam );
				return;
			}
			if( mainTubeParam.IsValid() == false ) {
				CADEditErrorEvent?.Invoke( CADEditErrorCode.InvalidParam );
				return;
			}

			m_szMainTubeName = GetMainTubeName( mainTubeParam );
			AddMainTubeCommand command = new AddMainTubeCommand( m_szMainTubeName, mainTubeParam, m_CADFeatureParamMap );
			DoCommand( command );
		}

		internal void AddCADFeature( ICADFeatureParam cadFeatureParam )
		{
			// data protection
			if( m_CADFeatureParamMap.MainTubeParam == null ) {
				CADEditErrorEvent?.Invoke( CADEditErrorCode.NoMainTube );
				return;
			}
			if( cadFeatureParam == null ) {
				CADEditErrorEvent?.Invoke( CADEditErrorCode.NullParam );
				return;
			}
			if( cadFeatureParam.IsValid() == false ) {
				CADEditErrorEvent?.Invoke( CADEditErrorCode.InvalidParam );
				return;
			}

			// check special case if feature type is bending notch
			CheckFeatureSpecialCase( cadFeatureParam );

			string szName = GetNewCADFeatureName( cadFeatureParam );
			AddCadFeatureCommand command = new AddCadFeatureCommand( szName, cadFeatureParam, m_CADFeatureParamMap );
			DoCommand( command );
		}

		internal void ModifyCADFeature()
		{
			// data protection
			if( m_treeObjBrowser.SelectedNode == null ) {
				CADEditErrorEvent?.Invoke( CADEditErrorCode.NoSelectedObject );
				return;
			}
			string szObjecName = m_treeObjBrowser.SelectedNode.Text;
			if( string.IsNullOrEmpty( szObjecName ) ) {
				CADEditErrorEvent?.Invoke( CADEditErrorCode.NoSelectedObject );
				return;
			}
			if( m_propgrdPropertyBar.SelectedObject == null ) {
				CADEditErrorEvent?.Invoke( CADEditErrorCode.NoSelectedObject );
				return;
			}
			ICADFeatureParam editingParam = m_propgrdPropertyBar.SelectedObject as ICADFeatureParam;
			if( editingParam == null ) {
				CADEditErrorEvent?.Invoke( CADEditErrorCode.NoSelectedObject );
				return;
			}
			if( editingParam.IsValid() == false ) {
				CADEditErrorEvent?.Invoke( CADEditErrorCode.InvalidParam );

				// show original property when modify failed
				ShowObjectProperty( szObjecName );
				return;
			}

			// main tube
			if( szObjecName == m_szMainTubeName ) {
				ModifyMainTubeCommand command = new ModifyMainTubeCommand( m_szMainTubeName, CloneHelper.Clone( editingParam ), m_CADFeatureParamMap );
				DoCommand( command );
			}

			// cad feature
			else {

				// check special case if feature type is bending notch
				CheckFeatureSpecialCase( editingParam );
				ModifyCadFeatureCommand command = new ModifyCadFeatureCommand( szObjecName, CloneHelper.Clone( editingParam ), m_CADFeatureParamMap );
				DoCommand( command );
			}
		}

		internal void RemoveCADFeature()
		{
			// data protection
			if( m_treeObjBrowser.SelectedNode == null ) {
				CADEditErrorEvent?.Invoke( CADEditErrorCode.NoSelectedObject );
				return;
			}
			string szObjectName = m_treeObjBrowser.SelectedNode.Text;
			if( string.IsNullOrEmpty( szObjectName ) ) {
				CADEditErrorEvent?.Invoke( CADEditErrorCode.NoSelectedObject );
				return;
			}

			// remove main tube
			if( szObjectName == m_szMainTubeName ) {
				if( m_CADFeatureParamMap.FeatureMap.Count != 0 ) {
					CADEditErrorEvent?.Invoke( CADEditErrorCode.CanNotRemoveMainTube );
					return;
				}
				RemoveMainTubeCommand command = new RemoveMainTubeCommand( m_szMainTubeName, m_CADFeatureParamMap );
				DoCommand( command );
			}

			// remove cad feature
			else {
				RemoveCadFeatureCommand command = new RemoveCadFeatureCommand( szObjectName, m_CADFeatureParamMap );
				DoCommand( command );
			}
		}

		internal gp_Dir GetEditObjectDir()
		{
			// data protection
			if( m_treeObjBrowser.SelectedNode == null ) {
				return new gp_Dir( 0, 1, 0 );
			}

			string szObjectName = m_treeObjBrowser.SelectedNode.Text;
			if( string.IsNullOrEmpty( szObjectName ) ) {
				return new gp_Dir( 0, 1, 0 );
			}

			// main tube
			if( szObjectName == m_szMainTubeName
				&& m_CADFeatureParamMap.MainTubeParam != null ) {
				return new gp_Dir( 0, 1, 0 );
			}

			// cad feature
			if( m_CADFeatureParamMap.FeatureMap.ContainsKey( szObjectName )
				&& m_CADFeatureParamMap.FeatureMap[ szObjectName ] != null ) {
				return CADFeatureMaker.GetCADFeatureDir( m_CADFeatureParamMap.FeatureMap[ szObjectName ] );
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

			// invoke the command status changed event
			CommandStatusChanged?.Invoke( m_CADEditUndoCommandQueue.Count != 0, true );
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

			// invoke the command status changed event
			CommandStatusChanged?.Invoke( true, m_CADEditRedoCommandQueue.Count != 0 );
		}

		internal void ExportStep()
		{
			// file directory
			string szFileDir = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "OutPut" );
			Directory.CreateDirectory( szFileDir );

			// save map file
			string szMapFilePath = Path.Combine( szFileDir, "map.dat" );
			using( FileStream stream = new FileStream( szMapFilePath, FileMode.Create ) ) {
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize( stream, m_CADFeatureParamMap );
			}

			// make result tube
			TopoDS_Shape resultTube = CADFeatureMaker.MakeResultTube( m_CADFeatureParamMap );
			if( resultTube == null ) {
				CADEditErrorEvent?.Invoke( CADEditErrorCode.MakeShapeFailed );
				return;
			}

			// save step file
			string szStepFilePath = Path.Combine( szFileDir, "result.stp" );
			STEPControl_Writer writer = new STEPControl_Writer();
			writer.Transfer( resultTube, STEPControl_StepModelType.STEPControl_AsIs );
			writer.Write( szStepFilePath );
		}

		// TODO: compelte the implementation
		internal void OpenMapFile()
		{
			CADFeatureParamMap loadedMap;

			try {
				// load map file
				string szFileDir = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "OutPut" );
				string szFilePath = Path.Combine( szFileDir, "map.dat" );
				using( FileStream stream = new FileStream( szFilePath, FileMode.Open ) ) {
					BinaryFormatter formatter = new BinaryFormatter();
					loadedMap = formatter.Deserialize( stream ) as CADFeatureParamMap;
				}
			}
			catch {
				return;
			}

			// data protection
			if( loadedMap == null || loadedMap.MainTubeParam == null || loadedMap.FeatureMap == null ) {
				return;
			}
			m_CADFeatureParamMap = loadedMap;
			m_szMainTubeName = GetMainTubeName( m_CADFeatureParamMap.MainTubeParam );

			// update object browser and update property bar
			m_bSupressBrowserSelectEvent = true;
			ReconstructObjectBrowser( string.Empty );
			ShowObjectProperty( string.Empty );
			m_bSupressBrowserSelectEvent = false;

			// update cad feature display
			RefreshAIS();

			// update result tube display
			UpdateAndRedrawResultTube( out bool isSucess );
			if( isSucess ) {

				// zoom all view
				m_Viewer.ZoomAllView();

				// invoke main tube status changed event and cad edit success event
				MainTubeStatusChanged?.Invoke( true );
				CADEditSuccessEvent?.Invoke();
			}
		}

		// layout property
		internal Panel ViewerPanel => m_panViewer;

		internal Panel ObjectBrowserPanel => m_panObjBrowser;

		internal Panel PropertyBarPanel => m_panPropertyBar;

		// viewer pointer
		internal OCCViewer Viewer => m_Viewer;

		// view direction
		internal void SetViewDir( ViewDir dir )
		{
			switch( dir ) {
				case ViewDir.Top:
					m_Viewer.TopView();
					break;
				case ViewDir.Bottom:
					m_Viewer.BottomView();
					break;
				case ViewDir.Left:
					m_Viewer.LeftView();
					break;
				case ViewDir.Right:
					m_Viewer.RightView();
					break;
				case ViewDir.Front:
					m_Viewer.FrontView();
					break;
				case ViewDir.Back:
					m_Viewer.BackView();
					break;
				case ViewDir.Isometric:
					m_Viewer.IsometricView();
					break;
				case ViewDir.Dir_Pos:
					m_Viewer.SetViewDir( GetEditObjectDir() );
					break;
				case ViewDir.Dir_Neg:
					m_Viewer.SetViewDir( GetEditObjectDir().Reversed() );
					break;
				default:
					break;
			}
			m_Viewer.ZoomAllView();
		}

		internal void ZoomToFit()
		{
			m_Viewer.ZoomAllView();
		}

		bool CheckFeatureSpecialCase( ICADFeatureParam cadFeatureParam )
		{
			if( cadFeatureParam.Type == CADFeatureType.BendingNotch ) {
				CADft_BendingNotchParam bendingNotchParam = cadFeatureParam as CADft_BendingNotchParam;
				double zPos = bendingNotchParam.GapFromButtom;
				double mainTubeThickness = m_CADFeatureParamMap.MainTubeParam.CrossSection.Thickness;
				if( zPos < mainTubeThickness ) {
					bendingNotchParam.GapFromButtom = mainTubeThickness;
				}
			}
			return true;
		}

		void UpdateEditorAfterCommand( EditType type, string szObjectName )
		{
			// data protection
			if( string.IsNullOrEmpty( szObjectName ) ) {
				return;
			}

			m_bSupressBrowserSelectEvent = true;
			if( type == EditType.AddMainTube ) {

				// update object browser
				// remove old node from object browser if exist, should not happen
				if( m_treeObjBrowser.Nodes.Count != 0 ) {
					m_treeObjBrowser.Nodes.Clear();
				}

				// add new main tube node
				m_MainTubeNode = m_treeObjBrowser.Nodes.Add( m_szMainTubeName, m_szMainTubeName );
				m_treeObjBrowser.Focus();
				m_treeObjBrowser.SelectedNode = m_MainTubeNode;

				// update property bar
				ShowObjectProperty( m_szMainTubeName );

				// invoke the main tube status changed event
				MainTubeStatusChanged?.Invoke( true );
			}
			else if( type == EditType.RemoveMainTube ) {

				// update object browser
				m_treeObjBrowser.Nodes.Clear();
				m_MainTubeNode = null;

				// update property bar
				ShowObjectProperty( string.Empty );

				// invoke the main tube status changed event
				MainTubeStatusChanged?.Invoke( false );
			}
			else if( type == EditType.ModifyMainTube ) {

				// no need to update object browser, property bar
				// need to refresh cad feature AIS shape when main tube size changed
				RefreshAIS();
			}
			else if( type == EditType.AddCADFeature ) {

				// update object browser
				// reconstruct object browser here to ensure the order of the node in case of undo remove
				ReconstructObjectBrowser( szObjectName );

				// update property bar
				ShowObjectProperty( szObjectName );

				// update cad feature display
				if( m_CADFeatureParamMap.FeatureMap.ContainsKey( szObjectName ) == false || m_CADFeatureParamMap.FeatureMap[ szObjectName ] == null ) {
					CADEditErrorEvent?.Invoke( CADEditErrorCode.NoSelectedObject );
					return;
				}
				AIS_Shape cadFeatureAIS = CADFeatureMaker.MakeCADFeatureAIS( m_CADFeatureParamMap.FeatureMap[ szObjectName ], m_CADFeatureParamMap.MainTubeParam );
				if( cadFeatureAIS == null ) {
					CADEditErrorEvent?.Invoke( CADEditErrorCode.MakeShapeFailed );
					return;
				}
				m_CADFeatureNameAISMap[ szObjectName ] = cadFeatureAIS;
				DisplayObjectShape( szObjectName );
			}
			else if( type == EditType.RemoveCADFeature ) {

				// update object browser
				m_MainTubeNode.Nodes.RemoveByKey( szObjectName );
				string szNextObjectName = string.Empty;
				if( m_treeObjBrowser.SelectedNode != null ) {
					szNextObjectName = m_treeObjBrowser.SelectedNode.Text;
				}

				// update property bar
				ShowObjectProperty( szNextObjectName );

				// update cad feature display
				if( m_CADFeatureNameAISMap.ContainsKey( szObjectName ) ) {
					m_Viewer.GetAISContext().Remove( m_CADFeatureNameAISMap[ szObjectName ], false );
					m_CADFeatureNameAISMap.Remove( szObjectName );
				}
				DisplayObjectShape( szNextObjectName );
			}
			else if( type == EditType.ModifyCADFeature ) {

				// no need to update object browser

				// update property bar
				// this is to ensure the dynamic property reloaded
				ShowObjectProperty( szObjectName );

				// update cad feature display
				if( m_CADFeatureParamMap.FeatureMap.ContainsKey( szObjectName ) == false || m_CADFeatureParamMap.FeatureMap[ szObjectName ] == null ) {
					CADEditErrorEvent?.Invoke( CADEditErrorCode.NoSelectedObject );
					return;
				}
				ICADFeatureParam cadFeatureParam = m_CADFeatureParamMap.FeatureMap[ szObjectName ];
				AIS_Shape newCADFeatureAIS = CADFeatureMaker.MakeCADFeatureAIS( cadFeatureParam, m_CADFeatureParamMap.MainTubeParam );
				if( newCADFeatureAIS == null ) {
					CADEditErrorEvent?.Invoke( CADEditErrorCode.MakeShapeFailed );
					return;
				}
				if( m_CADFeatureNameAISMap.ContainsKey( szObjectName ) ) {
					m_Viewer.GetAISContext().Remove( m_CADFeatureNameAISMap[ szObjectName ], false );
					m_CADFeatureNameAISMap.Remove( szObjectName );
				}
				m_CADFeatureNameAISMap[ szObjectName ] = newCADFeatureAIS;
				DisplayObjectShape( szObjectName );
			}
			m_bSupressBrowserSelectEvent = false;

			// update result tube display
			UpdateAndRedrawResultTube( out bool isSucess );
			if( isSucess ) {
				if( type == EditType.AddMainTube || type == EditType.ModifyMainTube ) {

					// zoom all view
					m_Viewer.ZoomAllView();
				}

				// invoke the cad edit success event
				CADEditSuccessEvent?.Invoke();
			}
		}

		void ReconstructObjectBrowser( string szSelectNodeName )
		{
			// remove all node
			m_treeObjBrowser.Nodes.Clear();
			m_treeObjBrowser.SelectedNode = null;
			m_MainTubeNode = null;

			// add main tube node
			if( m_CADFeatureParamMap.MainTubeParam == null ) {
				return;
			}
			m_MainTubeNode = m_treeObjBrowser.Nodes.Add( m_szMainTubeName, m_szMainTubeName );

			// add cad feature node, and select node
			foreach( var pair in m_CADFeatureParamMap.FeatureMap ) {
				if( string.IsNullOrEmpty( pair.Key ) ) {
					continue;
				}
				TreeNode newNode = m_MainTubeNode.Nodes.Add( pair.Key, pair.Key );
				if( pair.Key == szSelectNodeName ) {
					m_treeObjBrowser.SelectedNode = newNode;
				}
			}
			m_MainTubeNode.Expand();

			// select main tube node if no node selected
			if( m_treeObjBrowser.SelectedNode == null ) {
				m_treeObjBrowser.SelectedNode = m_MainTubeNode;
			}
		}

		void UpdateAndRedrawResultTube( out bool isSucess )
		{
			isSucess = false;

			// remove all shape if main tube not exist
			if( m_CADFeatureParamMap.MainTubeParam == null ) {
				m_Viewer.GetAISContext().RemoveAll( true );
				m_ResultTubeAIS = null;
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
				CADEditErrorEvent?.Invoke( CADEditErrorCode.MakeShapeFailed );
				return;
			}

			// display new tube
			m_ResultTubeAIS = new AIS_Shape( tubeShape );
			Graphic3d_MaterialAspect aspect = new Graphic3d_MaterialAspect( Graphic3d_NameOfMaterial.Graphic3d_NOM_STEEL );
			m_ResultTubeAIS.SetMaterial( aspect );
			m_ResultTubeAIS.SetDisplayMode( 1 );
			m_Viewer.GetAISContext().Display( m_ResultTubeAIS, false );
			m_Viewer.UpdateView();
			isSucess = true;
		}

		void RefreshAIS()
		{
			// data protection
			if( m_CADFeatureParamMap == null || m_CADFeatureParamMap.MainTubeParam == null || m_CADFeatureParamMap.FeatureMap == null ) {
				return;
			}

			// remove all shape from viewer
			m_Viewer.GetAISContext().RemoveAll( false );

			// remove all shape from map
			m_CADFeatureNameAISMap.Clear();

			// make new shape and add to map
			foreach( var pair in m_CADFeatureParamMap.FeatureMap ) {
				if( pair.Value == null ) {
					continue;
				}
				AIS_Shape cadFeatureAIS = CADFeatureMaker.MakeCADFeatureAIS( pair.Value, m_CADFeatureParamMap.MainTubeParam );
				if( cadFeatureAIS == null ) {
					CADEditErrorEvent?.Invoke( CADEditErrorCode.MakeShapeFailed );
					continue;
				}
				m_CADFeatureNameAISMap[ pair.Key ] = cadFeatureAIS;
			}

			// update display
			m_Viewer.UpdateView();
		}

		string GetNewCADFeatureName( ICADFeatureParam param )
		{
			CADFeatureType type = param.Type;
			switch( type ) {
				case CADFeatureType.EndCutter:
					return m_LanguageManager.GetString( "EndCutter" ) + "_" + m_nEndCutterCount++;
				case CADFeatureType.BranchTube:
					CADft_BranchTubeParam branchTubeParam = param as CADft_BranchTubeParam;
					string szBranchTubeShapeName = GetGeom2DShapeName( branchTubeParam.Shape.Type );
					return m_LanguageManager.GetString( "BranchTube" ) + "_" + szBranchTubeShapeName + "_" + m_nBranchTubeCount++;
				case CADFeatureType.BendingNotch:
					CADft_BendingNotchParam bendingNotchParam = param as CADft_BendingNotchParam;
					string szBendingNotchShapeName = string.Empty;
					switch( bendingNotchParam.Shape.Type ) {
						case BendingNotch_Type.VShape:
							szBendingNotchShapeName = m_LanguageManager.GetString( "VShape" );
							break;
						case BendingNotch_Type.BothSide:
							szBendingNotchShapeName = m_LanguageManager.GetString( "BothSide" );
							break;
						case BendingNotch_Type.OneSide:
							szBendingNotchShapeName = m_LanguageManager.GetString( "OneSide" );
							break;
						default:
							break;
					}
					return m_LanguageManager.GetString( "BendingNotch" ) + "_" + szBendingNotchShapeName + "_" + m_nBendingNotchCount++;
				default:
					return "NewFeature";
			}
		}

		string GetMainTubeName( CADft_MainTubeParam mainTUbeParam )
		{
			return m_LanguageManager.GetString( "MainTube" ) + "_" + GetGeom2DShapeName( mainTUbeParam.CrossSection.Shape.Type );
		}

		string GetGeom2DShapeName( Geom2D_Type type )
		{
			switch( type ) {
				case Geom2D_Type.Circle:
					return m_LanguageManager.GetString( "Circle" );
				case Geom2D_Type.Rectangle:
					return m_LanguageManager.GetString( "Rectangle" );
				case Geom2D_Type.Oval:
					return m_LanguageManager.GetString( "Oval" );
				case Geom2D_Type.FlatOval:
					return m_LanguageManager.GetString( "FlatOval" );
				case Geom2D_Type.DShape:
					return m_LanguageManager.GetString( "DShape" );
				default:
					return string.Empty;
			}
		}

		void DisplayObjectShape( string szObjectName )
		{
			if( string.IsNullOrEmpty( szObjectName ) ) {
				HideAllShapeExceptMainTube();
				return;
			}
			HideAllShapeExceptMainTube();

			// just hide all shape except main tube
			if( szObjectName == m_szMainTubeName ) {
				return;
			}

			// display selected object shape
			else {
				if( m_CADFeatureNameAISMap.ContainsKey( szObjectName ) == false || m_CADFeatureNameAISMap[ szObjectName ] == null ) {
					CADEditErrorEvent?.Invoke( CADEditErrorCode.NoSelectedObject );
					return;
				}
			}
			m_Viewer.GetAISContext().Display( m_CADFeatureNameAISMap[ szObjectName ], false );
			m_Viewer.UpdateView();
		}

		void ShowObjectProperty( string szObjectName )
		{
			if( string.IsNullOrEmpty( szObjectName ) ) {
				m_propgrdPropertyBar.SelectedObject = null;
				return;
			}

			// here we need to use the pointer of the object, a null check is necessary
			ICADFeatureParam editingObjParam;

			// main tube
			if( szObjectName == m_szMainTubeName ) {
				if( m_CADFeatureParamMap.MainTubeParam == null ) {
					CADEditErrorEvent?.Invoke( CADEditErrorCode.NoSelectedObject );
					return;
				}
				editingObjParam = CloneHelper.Clone( m_CADFeatureParamMap.MainTubeParam );
			}

			// cad feature
			else {
				if( m_CADFeatureParamMap.FeatureMap.ContainsKey( szObjectName ) == false || m_CADFeatureParamMap.FeatureMap[ szObjectName ] == null ) {
					CADEditErrorEvent?.Invoke( CADEditErrorCode.NoSelectedObject );
					return;
				}
				editingObjParam = CloneHelper.Clone( m_CADFeatureParamMap.FeatureMap[ szObjectName ] );
			}
			m_propgrdPropertyBar.SelectedObject = editingObjParam;
			m_propgrdPropertyBar.ExpandAllGridItems();
		}

		void HideAllShapeExceptMainTube()
		{
			foreach( var pair in m_CADFeatureNameAISMap ) {
				m_Viewer.GetAISContext().Erase( pair.Value, false );
			}
			m_Viewer.UpdateView();
		}

		void DoCommand( ICADEditCommand command )
		{
			command.CommandFinished += UpdateEditorAfterCommand;
			command.Do();
			m_CADEditUndoCommandQueue.Add( command );
			m_CADEditRedoCommandQueue.Clear();
			CommandStatusChanged?.Invoke( true, false );
		}

		// viewer action
		void m_panViewer_MouseDown( object sender, MouseEventArgs e )
		{
			ViewerMouseAction.MouseDown( e, m_Viewer, ref m_nXMousePosition, ref m_nYMousePosition );
		}

		void m_panViewer_MouseMove( object sender, MouseEventArgs e )
		{
			ViewerMouseAction.MouseMove( e, m_Viewer, ref m_nXMousePosition, ref m_nYMousePosition );
		}

		void m_panViewer_MouseWheel( object sender, MouseEventArgs e )
		{
			ViewerMouseAction.MouseWheel( e, m_Viewer );
		}

		void m_panViewer_Paint( object sender, PaintEventArgs e )
		{
			m_Viewer.UpdateView();
		}

		// object browser action
		void m_treeObjBrowser_KeyDown( object sender, KeyEventArgs e )
		{
			if( e.KeyCode == Keys.Delete ) {
				RemoveCADFeature();
			}
			else if( e.Modifiers == Keys.Control ) {
				if( e.KeyCode == Keys.Z ) {
					Undo();
				}
				else if( e.KeyCode == Keys.Y ) {
					Redo();
				}
			}
		}

		void m_treeObjBrowser_AfterSelect( object sender, TreeViewEventArgs e )
		{
			if( m_bSupressBrowserSelectEvent ) {
				return;
			}

			// data protection
			if( e.Node == null ) {
				return;
			}
			string szObjectName = e.Node.Text;

			// data protection
			if( string.IsNullOrEmpty( szObjectName ) ) {
				return;
			}

			// set edit object
			ShowObjectProperty( szObjectName );
			DisplayObjectShape( szObjectName );
		}

		// property bar action
		void m_propgrdPropertyBar_PropertyValueChanged( object s, PropertyValueChangedEventArgs e )
		{
			ModifyCADFeature();
		}
	}
}
