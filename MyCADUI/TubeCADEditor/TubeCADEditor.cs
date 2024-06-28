using MyCADCore;
using MyLanguageManager;
using MyOCCViewer;
using OCC.AIS;
using OCC.gp;
using OCC.Graphic3d;
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
	public enum ViewDir
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

	public class TubeCADEditor
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
		Dictionary<string, AIS_Shape> m_CADFeatureID2AISMap = new Dictionary<string, AIS_Shape>();

		// object browser map
		TreeNode m_MainTubeNode;

		// feature ID
		int m_nEndCutterCount = 1;
		int m_nBranchTubeCount = 1;
		int m_nBendingNotchCount = 1;
		const string MAIN_TUBE_ID = "MainTube";
		LanguageManager m_LanguageManager = new LanguageManager( "TubeCADEditor" );

		// command
		List<ICADEditCommand> m_CADEditUndoCommandQueue = new List<ICADEditCommand>();
		List<ICADEditCommand> m_CADEditRedoCommandQueue = new List<ICADEditCommand>();
		public delegate void CommandStatusChangedEventHandler( bool bUndo, bool bRedo );
		public event CommandStatusChangedEventHandler CommandStatusChanged;

		// main tube status changed event
		public delegate void MainTubeStatusChangedEventHandler( bool bExistMainTube );
		public event MainTubeStatusChangedEventHandler MainTubeStatusChanged;

		// cad edit error event
		public delegate void CADEditErrorEventHandler( CADEditErrorCode errorCode );
		public event CADEditErrorEventHandler CADEditErrorEvent;

		// cad edit sucess event
		public delegate void CADEditSuccessEventHandler();
		public event CADEditSuccessEventHandler CADEditSuccessEvent;

		public TubeCADEditor()
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

		public void AddMainTube( CADft_MainTubeParam mainTubeParam )
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

			AddMainTubeCommand command = new AddMainTubeCommand( MAIN_TUBE_ID, mainTubeParam, m_CADFeatureParamMap );
			DoCommand( command );
		}

		public void AddCADFeature( ICADFeatureParam cadFeatureParam )
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

			string szID = GetCADFeatureID( cadFeatureParam );
			AddCadFeatureCommand command = new AddCadFeatureCommand( szID, cadFeatureParam, m_CADFeatureParamMap );
			DoCommand( command );
		}

		public void ModifyCADFeature()
		{
			// data protection
			if( m_treeObjBrowser.SelectedNode == null ) {
				CADEditErrorEvent?.Invoke( CADEditErrorCode.NoSelectedObject );
				return;
			}
			string szObjecID = m_treeObjBrowser.SelectedNode.Name;
			if( string.IsNullOrEmpty( szObjecID ) ) {
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
				ShowObjectProperty( szObjecID );
				return;
			}

			// main tube
			if( szObjecID == MAIN_TUBE_ID ) {
				ModifyMainTubeCommand command = new ModifyMainTubeCommand( MAIN_TUBE_ID, CloneHelper.Clone( editingParam ), m_CADFeatureParamMap );
				DoCommand( command );
			}

			// cad feature
			else {

				// check special case if feature type is bending notch
				CheckFeatureSpecialCase( editingParam );
				ModifyCadFeatureCommand command = new ModifyCadFeatureCommand( szObjecID, CloneHelper.Clone( editingParam ), m_CADFeatureParamMap );
				DoCommand( command );
			}
		}

		public void RemoveCADFeature()
		{
			// data protection
			if( m_treeObjBrowser.SelectedNode == null ) {
				CADEditErrorEvent?.Invoke( CADEditErrorCode.NoSelectedObject );
				return;
			}
			string szObjectID = m_treeObjBrowser.SelectedNode.Name;
			if( string.IsNullOrEmpty( szObjectID ) ) {
				CADEditErrorEvent?.Invoke( CADEditErrorCode.NoSelectedObject );
				return;
			}

			// remove main tube
			if( szObjectID == MAIN_TUBE_ID ) {
				if( m_CADFeatureParamMap.FeatureMap.Count != 0 ) {
					CADEditErrorEvent?.Invoke( CADEditErrorCode.CanNotRemoveMainTube );
					return;
				}
				RemoveMainTubeCommand command = new RemoveMainTubeCommand( MAIN_TUBE_ID, m_CADFeatureParamMap );
				DoCommand( command );
			}

			// remove cad feature
			else {
				RemoveCadFeatureCommand command = new RemoveCadFeatureCommand( szObjectID, m_CADFeatureParamMap );
				DoCommand( command );
			}
		}

		public gp_Dir GetEditObjectDir()
		{
			// data protection
			if( m_treeObjBrowser.SelectedNode == null ) {
				return new gp_Dir( 0, 1, 0 );
			}

			string szObjectID = m_treeObjBrowser.SelectedNode.Name;
			if( string.IsNullOrEmpty( szObjectID ) ) {
				return new gp_Dir( 0, 1, 0 );
			}

			// main tube
			if( szObjectID == MAIN_TUBE_ID
				&& m_CADFeatureParamMap.MainTubeParam != null ) {
				return new gp_Dir( 0, 1, 0 );
			}

			// cad feature
			if( m_CADFeatureParamMap.FeatureMap.ContainsKey( szObjectID )
				&& m_CADFeatureParamMap.FeatureMap[ szObjectID ] != null ) {
				return CADFeatureMaker.GetCADFeatureDir( m_CADFeatureParamMap.FeatureMap[ szObjectID ] );
			}

			// default dir
			return new gp_Dir( 0, 1, 0 );
		}

		public void Undo()
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

		public void Redo()
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

		public TopoDS_Shape GetResultTube()
		{
			// make result tube
			TopoDS_Shape resultTube = CADFeatureMaker.MakeResultTube( m_CADFeatureParamMap );
			if( resultTube == null ) {
				CADEditErrorEvent?.Invoke( CADEditErrorCode.MakeShapeFailed );
				return null;
			}
			return resultTube;
		}

		// TODO: compelte the implementation
		public void OpenMapFile()
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
		public Panel ViewerPanel => m_panViewer;

		public Panel ObjectBrowserPanel => m_panObjBrowser;

		public Panel PropertyBarPanel => m_panPropertyBar;

		// view direction
		public void SetViewDir( ViewDir dir )
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

		public void ZoomToFit()
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

		void UpdateEditorAfterCommand( EditType type, string szObjectID )
		{
			// data protection
			if( string.IsNullOrEmpty( szObjectID ) ) {
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
				m_MainTubeNode = m_treeObjBrowser.Nodes.Add( MAIN_TUBE_ID, GetMainTubeDisplayName( m_CADFeatureParamMap.MainTubeParam ) );
				m_treeObjBrowser.Focus();
				m_treeObjBrowser.SelectedNode = m_MainTubeNode;

				// update property bar
				ShowObjectProperty( MAIN_TUBE_ID );

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
				ReconstructObjectBrowser( szObjectID );

				// update property bar
				ShowObjectProperty( szObjectID );

				// update cad feature display
				if( m_CADFeatureParamMap.FeatureMap.ContainsKey( szObjectID ) == false || m_CADFeatureParamMap.FeatureMap[ szObjectID ] == null ) {
					CADEditErrorEvent?.Invoke( CADEditErrorCode.NoSelectedObject );
					return;
				}
				AIS_Shape cadFeatureAIS = CADFeatureMaker.MakeCADFeatureAIS( m_CADFeatureParamMap.FeatureMap[ szObjectID ], m_CADFeatureParamMap.MainTubeParam );
				if( cadFeatureAIS == null ) {
					CADEditErrorEvent?.Invoke( CADEditErrorCode.MakeShapeFailed );
					return;
				}
				m_CADFeatureID2AISMap[ szObjectID ] = cadFeatureAIS;
				DisplayObjectShape( szObjectID );
			}
			else if( type == EditType.RemoveCADFeature ) {

				// update object browser
				m_MainTubeNode.Nodes.RemoveByKey( szObjectID );
				string szNextObjectID = string.Empty;
				if( m_treeObjBrowser.SelectedNode != null ) {
					szNextObjectID = m_treeObjBrowser.SelectedNode.Name;
				}

				// update property bar
				ShowObjectProperty( szNextObjectID );

				// update cad feature display
				if( m_CADFeatureID2AISMap.ContainsKey( szObjectID ) ) {
					m_Viewer.GetAISContext().Remove( m_CADFeatureID2AISMap[ szObjectID ], false );
					m_CADFeatureID2AISMap.Remove( szObjectID );
				}
				DisplayObjectShape( szNextObjectID );
			}
			else if( type == EditType.ModifyCADFeature ) {

				// no need to update object browser

				// update property bar
				// this is to ensure the dynamic property reloaded
				ShowObjectProperty( szObjectID );

				// update cad feature display
				if( m_CADFeatureParamMap.FeatureMap.ContainsKey( szObjectID ) == false || m_CADFeatureParamMap.FeatureMap[ szObjectID ] == null ) {
					CADEditErrorEvent?.Invoke( CADEditErrorCode.NoSelectedObject );
					return;
				}
				ICADFeatureParam cadFeatureParam = m_CADFeatureParamMap.FeatureMap[ szObjectID ];
				AIS_Shape newCADFeatureAIS = CADFeatureMaker.MakeCADFeatureAIS( cadFeatureParam, m_CADFeatureParamMap.MainTubeParam );
				if( newCADFeatureAIS == null ) {
					CADEditErrorEvent?.Invoke( CADEditErrorCode.MakeShapeFailed );
					return;
				}
				if( m_CADFeatureID2AISMap.ContainsKey( szObjectID ) ) {
					m_Viewer.GetAISContext().Remove( m_CADFeatureID2AISMap[ szObjectID ], false );
					m_CADFeatureID2AISMap.Remove( szObjectID );
				}
				m_CADFeatureID2AISMap[ szObjectID ] = newCADFeatureAIS;
				DisplayObjectShape( szObjectID );
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
			m_MainTubeNode = m_treeObjBrowser.Nodes.Add( MAIN_TUBE_ID, GetMainTubeDisplayName( m_CADFeatureParamMap.MainTubeParam ) );

			// add cad feature node, and select node
			foreach( var pair in m_CADFeatureParamMap.FeatureMap ) {
				if( string.IsNullOrEmpty( pair.Key ) ) {
					continue;
				}
				TreeNode newNode = m_MainTubeNode.Nodes.Add( pair.Key, GetCADFeatureDisplayName( pair.Key, pair.Value ) );
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
			m_CADFeatureID2AISMap.Clear();

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
				m_CADFeatureID2AISMap[ pair.Key ] = cadFeatureAIS;
			}

			// update display
			m_Viewer.UpdateView();
		}

		string GetCADFeatureDisplayName( string szID, ICADFeatureParam param )
		{
			CADFeatureType type = param.Type;
			string szNumber = GetNumberFromID( szID );
			switch( type ) {
				case CADFeatureType.EndCutter:
					return m_LanguageManager.GetString( "EndCutter" ) + "_" + szNumber;
				case CADFeatureType.BranchTube:
					CADft_BranchTubeParam branchTubeParam = param as CADft_BranchTubeParam;
					string szBranchTubeShapeName = GetGeom2DShapeName( branchTubeParam.Shape.Type );
					return m_LanguageManager.GetString( "BranchTube" ) + "_" + szBranchTubeShapeName + "_" + szNumber;
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
					return m_LanguageManager.GetString( "BendingNotch" ) + "_" + szBendingNotchShapeName + "_" + szNumber;
				default:
					return "NewFeature";
			}
		}

		string GetNumberFromID( string szID )
		{
			// split by _
			string[] szArray = szID.Split( '_' );

			// data protection
			if( szArray.Length != 2 ) {
				return string.Empty;
			}
			return szArray[ 1 ];
		}

		string GetCADFeatureID( ICADFeatureParam param )
		{
			CADFeatureType type = param.Type;
			switch( type ) {
				case CADFeatureType.EndCutter:
					return "EndCutter" + "_" + m_nEndCutterCount++;
				case CADFeatureType.BranchTube:
					return "BranchTube" + "_" + m_nBranchTubeCount++;
				case CADFeatureType.BendingNotch:
					return "BendingNotch" + "_" + m_nBendingNotchCount++;
				default:
					return "NewFeature";
			}
		}

		string GetMainTubeDisplayName( CADft_MainTubeParam mainTUbeParam )
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

		void DisplayObjectShape( string szObjectID )
		{
			if( string.IsNullOrEmpty( szObjectID ) ) {
				HideAllShapeExceptMainTube();
				return;
			}
			HideAllShapeExceptMainTube();

			// just hide all shape except main tube
			if( szObjectID == MAIN_TUBE_ID ) {
				return;
			}

			// display selected object shape
			else {
				if( m_CADFeatureID2AISMap.ContainsKey( szObjectID ) == false || m_CADFeatureID2AISMap[ szObjectID ] == null ) {
					CADEditErrorEvent?.Invoke( CADEditErrorCode.NoSelectedObject );
					return;
				}
			}
			m_Viewer.GetAISContext().Display( m_CADFeatureID2AISMap[ szObjectID ], false );
			m_Viewer.UpdateView();
		}

		void ShowObjectProperty( string szObjectID )
		{
			if( string.IsNullOrEmpty( szObjectID ) ) {
				m_propgrdPropertyBar.SelectedObject = null;
				return;
			}

			// here we need to use the pointer of the object, a null check is necessary
			ICADFeatureParam editingObjParam;

			// main tube
			if( szObjectID == MAIN_TUBE_ID ) {
				if( m_CADFeatureParamMap.MainTubeParam == null ) {
					CADEditErrorEvent?.Invoke( CADEditErrorCode.NoSelectedObject );
					return;
				}
				editingObjParam = CloneHelper.Clone( m_CADFeatureParamMap.MainTubeParam );
			}

			// cad feature
			else {
				if( m_CADFeatureParamMap.FeatureMap.ContainsKey( szObjectID ) == false || m_CADFeatureParamMap.FeatureMap[ szObjectID ] == null ) {
					CADEditErrorEvent?.Invoke( CADEditErrorCode.NoSelectedObject );
					return;
				}
				editingObjParam = CloneHelper.Clone( m_CADFeatureParamMap.FeatureMap[ szObjectID ] );
			}
			m_propgrdPropertyBar.SelectedObject = editingObjParam;
			m_propgrdPropertyBar.ExpandAllGridItems();
		}

		void HideAllShapeExceptMainTube()
		{
			foreach( var pair in m_CADFeatureID2AISMap ) {
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
			string szObjectID = e.Node.Name;

			// data protection
			if( string.IsNullOrEmpty( szObjectID ) ) {
				return;
			}

			// set edit object
			ShowObjectProperty( szObjectID );
			DisplayObjectShape( szObjectID );
		}

		// property bar action
		void m_propgrdPropertyBar_PropertyValueChanged( object s, PropertyValueChangedEventArgs e )
		{
			ModifyCADFeature();
		}
	}
}
