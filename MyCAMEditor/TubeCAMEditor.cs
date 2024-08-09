using My3DReader;
using MyCAMCore;
using MyLanguageManager;
using MyOCCViewer;
using OCC.AIS;
using OCC.BRepBuilderAPI;
using OCC.BRepGProp;
using OCC.BRepProj;
using OCC.gp;
using OCC.GProp;
using OCC.Graphic3d;
using OCC.Quantity;
using OCC.TopoDS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MyCAMEditor
{
	delegate void CAMEditFinishEventHandler( EditType type, string szObjectName );

	internal enum EditType
	{
	}

	internal interface ICAMEditCommand
	{
		void Do();

		void Undo();

		event CAMEditFinishEventHandler CommandFinished;
	}

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

	public enum CAMEditErrorCode
	{
	}

	public class TubeCAMEditor
	{
		// GUI
		Panel m_pan3DViewer = new Panel();
		Panel m_pan2DViewer = new Panel();
		Panel m_panObjBrowser = new Panel();
		Panel m_panPropertyBar = new Panel();
		Panel m_panHintLabel = new Panel();
		OCCViewer m_3DViewer = new OCCViewer();
		OCCViewer m_2DViewer = new OCCViewer();
		TreeView m_treeObjBrowser = new TreeView();
		PropertyGrid m_propgrdPropertyBar = new PropertyGrid();
		Label m_lblHint = new Label();
		bool m_bSupressBrowserSelectEvent = false;

		// tube shape
		AIS_Shape m_RawTubeAISShape;

		// cad feature map
		Dictionary<string, CADFeatureData> m_CADFeatureDataMap = new Dictionary<string, CADFeatureData>();
		Dictionary<string, AIS_Shape> m_CADFeatureRawAISMap = new Dictionary<string, AIS_Shape>();

		// viewer
		int m_nXMousePosition = 0;
		int m_nYMousePosition = 0;

		// object browser map
		TreeNode m_RootNode;
		const string ROOT_NODE_NAME = "ROOT";

		// feature ID
		LanguageManager m_LanguageManager = new LanguageManager( "TubeCAMEditor" );

		// command
		List<ICAMEditCommand> m_CAMEditUndoCommandQueue = new List<ICAMEditCommand>();
		List<ICAMEditCommand> m_CAMEditRedoCommandQueue = new List<ICAMEditCommand>();
		public delegate void CommandStatusChangedEventHandler( bool bUndo, bool bRedo );
		public event CommandStatusChangedEventHandler CommandStatusChanged;

		// cam edit error event
		public delegate void CAMEditErrorEventHandler( CAMEditErrorCode errorCode );
		public event CAMEditErrorEventHandler CAMEditErrorEvent;

		// cam edit sucess event
		public delegate void CAMEditSuccessEventHandler();
		public event CAMEditSuccessEventHandler CAMEditSuccessEvent;

		public TubeCAMEditor()
		{
			bool is3DSuccess = m_3DViewer.InitViewer( m_pan3DViewer.Handle );
			if( is3DSuccess == false ) {
				MessageBox.Show( "init 3D failed" );
			}
			m_3DViewer.SetBackgroundColor( 0, 0, 0 );
			m_3DViewer.IsometricView();
			m_pan3DViewer.Dock = DockStyle.Fill;

			bool is2DSuccess = m_2DViewer.InitViewer( m_pan2DViewer.Handle );
			if( is2DSuccess == false ) {
				MessageBox.Show( "init 2D failed" );
			}
			m_2DViewer.SetBackgroundColor( 0, 0, 0 );
			m_2DViewer.TopView();
			m_pan2DViewer.Dock = DockStyle.Fill;

			m_treeObjBrowser.Dock = DockStyle.Fill;
			m_panObjBrowser.Controls.Add( m_treeObjBrowser );
			m_panObjBrowser.Dock = DockStyle.Fill;

			m_propgrdPropertyBar.HelpVisible = false;
			m_propgrdPropertyBar.Dock = DockStyle.Fill;
			m_panPropertyBar.Controls.Add( m_propgrdPropertyBar );
			m_panPropertyBar.Dock = DockStyle.Fill;

			m_lblHint.Dock = DockStyle.Fill;
			m_panHintLabel.Controls.Add( m_lblHint );
			m_panHintLabel.Dock = DockStyle.Fill;

			// action
			m_pan3DViewer.Paint += m_pan3DViewer_Paint;
			m_pan3DViewer.MouseDown += m_pan3DViewer_MouseDown;
			m_pan3DViewer.MouseMove += m_pan3DViewer_MouseMove;
			m_pan3DViewer.MouseWheel += m_pan3DViewer_MouseWheel;

			m_pan2DViewer.Paint += m_pan2DViewer_Paint;

			m_treeObjBrowser.KeyDown += m_treeObjBrowser_KeyDown;
			m_treeObjBrowser.AfterSelect += m_treeObjBrowser_AfterSelect;

			m_propgrdPropertyBar.PropertyValueChanged += m_propgrdPropertyBar_PropertyValueChanged;

			// cam edit action
			CAMEditErrorEvent += CAMEditError;
			CAMEditSuccessEvent += CAMEditSuccess;

			// init object browser
			m_RootNode = m_treeObjBrowser.Nodes.Add( "ROOT", "ROOT_DISLAY" );
		}

		public void SetTube( TopoDS_Shape tubeShape )
		{
			// read tube cad feature data
			TubeReader tubeReader = new TubeReader();
			bool bRead = tubeReader.ReadTubeInformation( tubeShape,
				out CADFeatureData head, out CADFeatureData tail, out List<CADFeatureData> features );
			if( bRead == false ) {
				return;
			}

			// update cad feature data map
			m_CADFeatureDataMap.Clear();
			m_CADFeatureDataMap.Add( "HEAD", head );
			m_CADFeatureDataMap.Add( "TAIL", tail );
			for( int i = 0; i < features.Count; i++ ) {
				m_CADFeatureDataMap.Add( "FEATURE" + i.ToString( "00" ), features[ i ] );
			}

			// update object browser
			ReconstructObjectBrowser( "HEAD" );

			// create tube AIS shape
			UpdateCADFeatureRawAISMap();

			// display tube
			m_RawTubeAISShape = new AIS_Shape( tubeShape );
			Graphic3d_MaterialAspect aspect = new Graphic3d_MaterialAspect( Graphic3d_NameOfMaterial.Graphic3d_NOM_STEEL );
			m_RawTubeAISShape.SetMaterial( aspect );
			m_RawTubeAISShape.SetDisplayMode( 1 );
			m_3DViewer.GetAISContext().Display( m_RawTubeAISShape, false );
			DisplayObjectShape( "HEAD" );
			m_3DViewer.ZoomAllView();
		}

		public gp_Dir GetEditObjectDir()
		{
			// TODO: complete the implementation

			// default dir
			return new gp_Dir( 0, 1, 0 );
		}

		public void Undo()
		{
			// no command to undo
			if( m_CAMEditUndoCommandQueue.Count == 0 ) {
				return;
			}

			// undo command
			m_CAMEditUndoCommandQueue.Last().Undo();

			// move command to redo queue
			m_CAMEditRedoCommandQueue.Add( m_CAMEditUndoCommandQueue.Last() );

			// remove command from undo queue
			m_CAMEditUndoCommandQueue.RemoveAt( m_CAMEditUndoCommandQueue.Count - 1 );

			// invoke the command status changed event
			CommandStatusChanged?.Invoke( m_CAMEditUndoCommandQueue.Count != 0, true );
		}

		public void Redo()
		{
			// no command to redo
			if( m_CAMEditRedoCommandQueue.Count == 0 ) {
				return;
			}

			// redo command
			m_CAMEditRedoCommandQueue.Last().Do();

			// move command to undo queue
			m_CAMEditUndoCommandQueue.Add( m_CAMEditRedoCommandQueue.Last() );

			// remove command from redo queue
			m_CAMEditRedoCommandQueue.RemoveAt( m_CAMEditRedoCommandQueue.Count - 1 );

			// invoke the command status changed event
			CommandStatusChanged?.Invoke( true, m_CAMEditRedoCommandQueue.Count != 0 );
		}

		// layout property
		public Panel Viewer3DPanel
		{
			get
			{
				return m_pan3DViewer;
			}
		}

		public Panel Viewer2DPanel
		{
			get
			{
				return m_pan2DViewer;
			}
		}

		public Panel ObjectBrowserPanel
		{
			get
			{
				return m_panObjBrowser;
			}
		}

		public Panel PropertyBarPanel
		{
			get
			{
				return m_panPropertyBar;
			}
		}

		public Panel HintLabelPanel
		{
			get
			{
				return m_panHintLabel;
			}
		}

		// view direction
		public void SetViewDir( ViewDir dir )
		{
			switch( dir ) {
				case ViewDir.Top:
					m_3DViewer.TopView();
					break;
				case ViewDir.Bottom:
					m_3DViewer.BottomView();
					break;
				case ViewDir.Left:
					m_3DViewer.LeftView();
					break;
				case ViewDir.Right:
					m_3DViewer.RightView();
					break;
				case ViewDir.Front:
					m_3DViewer.FrontView();
					break;
				case ViewDir.Back:
					m_3DViewer.BackView();
					break;
				case ViewDir.Isometric:
					m_3DViewer.IsometricView();
					break;
				case ViewDir.Dir_Pos:
					m_3DViewer.SetViewDir( GetEditObjectDir() );
					break;
				case ViewDir.Dir_Neg:
					m_3DViewer.SetViewDir( GetEditObjectDir().Reversed() );
					break;
				default:
					break;
			}
			m_3DViewer.ZoomAllView();
		}

		public void ZoomToFit()
		{
			m_3DViewer.ZoomAllView();
		}

		void UpdateCADFeatureRawAISMap()
		{
			m_CADFeatureRawAISMap.Clear();
			foreach( var pair in m_CADFeatureDataMap ) {
				TopoDS_Shape oneFeatureWire = pair.Value.OuterWire;
				if( oneFeatureWire == null ) {
					continue;
				}
				AIS_Shape oneAIS = new AIS_Shape( oneFeatureWire );
				oneAIS.SetColor( new Quantity_Color( Quantity_NameOfColor.Quantity_NOC_RED ) );
				oneAIS.SetWidth( 2 );
				m_CADFeatureRawAISMap.Add( pair.Key, oneAIS );
			}
		}

		void Make2DView()
		{
			// get axial projection of head wire
			TopoDS_Shape headOutWire = m_CADFeatureDataMap[ "HEAD" ].OuterWire;
			TopoDS_Shape headOutWire2D = GetAxialProjectionOfWire( headOutWire );

			// get 2D wire length
			double dPerimeter = Get2DWireLength( headOutWire2D );

			foreach( var pair in m_CADFeatureDataMap ) {

				// get radial projection of wire
			}
		}

		TopoDS_Shape GetAxialProjectionOfWire( TopoDS_Shape wire )
		{
			// make a face represent XZ plane
			gp_Pln pln_XZ = new gp_Pln( new gp_Pnt( 0, 0, 0 ), new gp_Dir( 0, 1, 0 ) );
			BRepBuilderAPI_MakeFace mf = new BRepBuilderAPI_MakeFace( pln_XZ );
			if( mf.IsDone() == false ) {
				return null;
			}
			TopoDS_Face face_XZ = mf.Face();

			// project the wire onto the XZ plane
			BRepProj_Projection proj = new BRepProj_Projection( wire, face_XZ, new gp_Dir( 0, 1, 0 ) );
			if( proj.IsDone() == false ) {
				return null;
			}
			return proj.Shape();
		}

		double Get2DWireLength( TopoDS_Shape wire )
		{
			GProp_GProps props = new GProp_GProps();
			BRepGProp.LinearProperties( wire, ref props );
			return props.Mass();
		}

		TopoDS_Shape GetRa

		void ModifyCAMFeature()
		{
			// TODO: complete the implementation
		}

		void RemoveCAMFeature()
		{
			// TODO: complete the implementation
		}

		void UpdateEditorAfterCommand( EditType type, string szObjectID )
		{
			// TODO: complete the implementation
		}

		void ReconstructObjectBrowser( string szSelectNodeName )
		{
			m_RootNode.Nodes.Clear();
			m_bSupressBrowserSelectEvent = true;

			// add head
			m_RootNode.Nodes.Add( "HEAD", "HEAD_DISPLAY" );

			// add tail
			m_RootNode.Nodes.Add( "TAIL", "TAIL_DISPLAY" );

			// add features
			for( int i = 0; i < m_CADFeatureDataMap.Count - 2; i++ ) {
				m_RootNode.Nodes.Add( "FEATURE" + i.ToString( "00" ), "FEATURE" + i.ToString( "00" ) + "_DISPLAY" );
			}

			// select node
			m_treeObjBrowser.SelectedNode = m_RootNode.Nodes[ szSelectNodeName ];
			m_bSupressBrowserSelectEvent = false;
		}

		void UpdateAndRedrawResultTube( out bool isSucess )
		{
			// TODO: complete the implementation

			isSucess = false;
		}

		void RefreshAIS()
		{
			// TODO: complete the implementation
		}

		void DisplayObjectShape( string szObjectID )
		{
			HideAllShapeExceptMainTube();
			m_3DViewer.GetAISContext().Display( m_CADFeatureRawAISMap[ szObjectID ], true );
		}

		void ShowObjectProperty( string szObjectID )
		{
			// TODO: complete the implementation
		}

		void HideAllShapeExceptMainTube()
		{
			foreach( var pair in m_CADFeatureRawAISMap ) {
				m_3DViewer.GetAISContext().Erase( pair.Value, false );
			}
			m_3DViewer.UpdateView();
		}

		void DoCommand( ICAMEditCommand command )
		{
			command.CommandFinished += UpdateEditorAfterCommand;
			command.Do();
			m_CAMEditUndoCommandQueue.Add( command );
			m_CAMEditRedoCommandQueue.Clear();
			CommandStatusChanged?.Invoke( true, false );
		}

		// viewer 3D action
		void m_pan3DViewer_MouseDown( object sender, MouseEventArgs e )
		{
			ViewerMouseAction.MouseDown( e, m_3DViewer, ref m_nXMousePosition, ref m_nYMousePosition );
		}

		void m_pan3DViewer_MouseMove( object sender, MouseEventArgs e )
		{
			ViewerMouseAction.MouseMove( e, m_3DViewer, ref m_nXMousePosition, ref m_nYMousePosition );
		}

		void m_pan3DViewer_MouseWheel( object sender, MouseEventArgs e )
		{
			ViewerMouseAction.MouseWheel( e, m_3DViewer );
		}

		void m_pan3DViewer_Paint( object sender, PaintEventArgs e )
		{
			m_3DViewer.UpdateView();
		}

		// viewer 2D action
		void m_pan2DViewer_Paint( object sender, PaintEventArgs e )
		{
			m_2DViewer.UpdateView();
		}

		// object browser action
		void m_treeObjBrowser_KeyDown( object sender, KeyEventArgs e )
		{
			if( e.KeyCode == Keys.Delete ) {
				RemoveCAMFeature();
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
			if( string.IsNullOrEmpty( szObjectID ) || szObjectID == ROOT_NODE_NAME ) {
				return;
			}

			// set edit object
			ShowObjectProperty( szObjectID );
			DisplayObjectShape( szObjectID );
		}

		// property bar action
		void m_propgrdPropertyBar_PropertyValueChanged( object s, PropertyValueChangedEventArgs e )
		{
			ModifyCAMFeature();
		}

		// cam edit action
		void CAMEditError( CAMEditErrorCode errorCode )
		{
			// TODO: complete the implementation
			throw new NotImplementedException();
		}

		void CAMEditSuccess()
		{
			// TODO: complete the implementation
			throw new NotImplementedException();
		}
	}
}
