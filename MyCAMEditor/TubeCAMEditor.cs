using MyLanguageManager;
using MyOCCViewer;
using OCC.gp;
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
		Panel m_panViewer = new Panel();
		Panel m_panObjBrowser = new Panel();
		Panel m_panPropertyBar = new Panel();
		Panel m_panHintLabel = new Panel();
		OCCViewer m_Viewer = new OCCViewer();
		TreeView m_treeObjBrowser = new TreeView();
		PropertyGrid m_propgrdPropertyBar = new PropertyGrid();
		Label m_lblHint = new Label();
		bool m_bSupressBrowserSelectEvent = false;

		// tube shape
		TopoDS_Shape m_RawTubeShape;

		// viewer
		int m_nXMousePosition = 0;
		int m_nYMousePosition = 0;

		// parameter map

		// display shape map

		// object browser map
		TreeNode m_RootNode;

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

			m_propgrdPropertyBar.HelpVisible = false;
			m_propgrdPropertyBar.Dock = DockStyle.Fill;
			m_panPropertyBar.Controls.Add( m_propgrdPropertyBar );
			m_panPropertyBar.Dock = DockStyle.Fill;

			m_lblHint.Dock = DockStyle.Fill;
			m_panHintLabel.Controls.Add( m_lblHint );
			m_panHintLabel.Dock = DockStyle.Fill;

			// action
			m_panViewer.Paint += m_panViewer_Paint;
			m_panViewer.MouseDown += m_panViewer_MouseDown;
			m_panViewer.MouseMove += m_panViewer_MouseMove;
			m_panViewer.MouseWheel += m_panViewer_MouseWheel;

			m_treeObjBrowser.KeyDown += m_treeObjBrowser_KeyDown;
			m_treeObjBrowser.AfterSelect += m_treeObjBrowser_AfterSelect;

			m_propgrdPropertyBar.PropertyValueChanged += m_propgrdPropertyBar_PropertyValueChanged;

			// cam edit action
			CAMEditErrorEvent += CAMEditError;
			CAMEditSuccessEvent += CAMEditSuccess;
		}

		public void SetTube( TopoDS_Shape tubeShape )
		{
			m_RawTubeShape = tubeShape;
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
		public Panel ViewerPanel
		{
			get
			{
				return m_panViewer;
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
			// TODO: complete the implementation
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
			// TODO: complete the implementation
		}

		void ShowObjectProperty( string szObjectID )
		{
			// TODO: complete the implementation
		}

		void DoCommand( ICAMEditCommand command )
		{
			command.CommandFinished += UpdateEditorAfterCommand;
			command.Do();
			m_CAMEditUndoCommandQueue.Add( command );
			m_CAMEditRedoCommandQueue.Clear();
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
			ModifyCAMFeature();
		}

		// cam edit action
		void CAMEditError( CAMEditErrorCode errorCode )
		{
			throw new NotImplementedException();
		}

		void CAMEditSuccess()
		{
			throw new NotImplementedException();
		}
	}
}
