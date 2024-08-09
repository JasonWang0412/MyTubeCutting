using MyCAMEditor;
using MyLanguageManager;
using OCC.TopoDS;
using System.Collections.Generic;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace MyCAMUI
{
	public partial class CAMEditMainForm : Form
	{
		public CAMEditMainForm()
		{
			// set layout back ground
			m_dockPanel.DocumentStyle = DocumentStyle.DockingWindow;
			m_dockPanel.Dock = DockStyle.Fill;
			Controls.Add( m_dockPanel );
			m_dockPanel.DockLeftPortion = 0.3;

			// initalize component
			InitializeComponent();

			m_tsbtnUndo.Enabled = false;
			m_tsbtnRedo.Enabled = false;
			m_tsbtnExport.Enabled = false;

			// editor
			m_TubeCAMEditor = new TubeCAMEditor();

			// editor layout
			m_pan3DViewer.Controls.Add( m_TubeCAMEditor.Viewer3DPanel );
			m_pan3DViewer.Show( m_dockPanel, DockState.Document );

			m_pan2DViewer.Controls.Add( m_TubeCAMEditor.Viewer2DPanel );
			m_pan2DViewer.Show( m_pan3DViewer.Pane, DockAlignment.Bottom, 0.5 );

			m_panObjBrowser.Controls.Add( m_TubeCAMEditor.ObjectBrowserPanel );
			m_panObjBrowser.Show( m_dockPanel, DockState.DockLeft );

			m_panPropertyBar.Controls.Add( m_TubeCAMEditor.PropertyBarPanel );
			m_panPropertyBar.Show( m_panObjBrowser.Pane, DockAlignment.Bottom, 0.5 );

			m_panHint.Controls.Add( m_TubeCAMEditor.HintLabelPanel );
			m_panHint.Show( m_panPropertyBar.Pane, DockAlignment.Bottom, 0.2 );

			// initialize tube editor
			m_TubeCAMEditor.CommandStatusChanged += ( bUndo, bRedo ) =>
			{
				m_tsbtnUndo.Enabled = bUndo;
				m_tsbtnRedo.Enabled = bRedo;
			};

			// set language zh-TW
			// TODO: any language
			SetLanguage();
		}

		public void SetTube( TopoDS_Shape tubeShape )
		{
			m_TubeCAMEditor.SetTube( tubeShape );
		}

		// tube editor action
		void m_tsbtnUndo_Click( object sender, System.EventArgs e )
		{
			m_TubeCAMEditor.Undo();
		}

		void m_tsbtnRedo_Click( object sender, System.EventArgs e )
		{
			m_TubeCAMEditor.Redo();
		}

		void m_tsbtnExport_Click( object sender, System.EventArgs e )
		{
			// TODO: complete code implementation
		}

		// view action
		void m_tsbtnX_Pos_Click( object sender, System.EventArgs e )
		{
			m_TubeCAMEditor.SetViewDir( ViewDir.Right );
		}

		void m_tsbtnX_Neg_Click( object sender, System.EventArgs e )
		{
			m_TubeCAMEditor.SetViewDir( ViewDir.Left );
		}

		void m_tsbtnY_Pos_Click( object sender, System.EventArgs e )
		{
			m_TubeCAMEditor.SetViewDir( ViewDir.Front );
		}

		void m_tsbtnY_Neg_Click( object sender, System.EventArgs e )
		{
			m_TubeCAMEditor.SetViewDir( ViewDir.Back );
		}

		void m_tsbtnZ_Pos_Click( object sender, System.EventArgs e )
		{
			m_TubeCAMEditor.SetViewDir( ViewDir.Top );
		}

		void m_tsbtnZ_Neg_Click( object sender, System.EventArgs e )
		{
			m_TubeCAMEditor.SetViewDir( ViewDir.Bottom );
		}

		void m_tsbtnDir_Pos_Click( object sender, System.EventArgs e )
		{
			m_TubeCAMEditor.SetViewDir( ViewDir.Dir_Pos );
		}

		void m_tsbtnDir_Neg_Click( object sender, System.EventArgs e )
		{
			m_TubeCAMEditor.SetViewDir( ViewDir.Dir_Neg );
		}

		void m_tsbtnISO_Click( object sender, System.EventArgs e )
		{
			m_TubeCAMEditor.SetViewDir( ViewDir.Isometric );
		}

		void m_tsbtnZoomToFit_Click( object sender, System.EventArgs e )
		{
			m_TubeCAMEditor.ZoomToFit();
		}

		// DockContent closing
		void DockContentClosing( object sender, FormClosingEventArgs e )
		{
			// cancel closing
			e.Cancel = true;
		}

		// language action
		// this is temporary function
		void SetLanguage()
		{
			LanguageManager.CurrentUILanguage = "zh-TW";
			ApplyComponentResource();
		}

		void ApplyComponentResource()
		{
			List<ToolStripButton> menuItems = new List<ToolStripButton>();
			menuItems.AddRange( FindAllToolStripItems( m_tsView.Items ) );
			menuItems.AddRange( FindAllToolStripItems( m_tsEdit.Items ) );
			foreach( ToolStripButton button in menuItems ) {
				string szText = m_LanguageManager.GetString( button.Name );
				if( string.IsNullOrEmpty( szText ) == false ) {
					button.Text = szText;
				}
			}
		}

		List<ToolStripButton> FindAllToolStripItems( ToolStripItemCollection items )
		{
			List<ToolStripButton> list = new List<ToolStripButton>();
			foreach( ToolStripItem item in items ) {
				if( item is ToolStripButton button ) {
					list.Add( button );
				}
			}
			return list;
		}

		// layout
		DockPanel m_dockPanel = new DockPanel();
		DockContent m_pan3DViewer = new DockContent();
		DockContent m_pan2DViewer = new DockContent();
		DockContent m_panObjBrowser = new DockContent();
		DockContent m_panPropertyBar = new DockContent();
		DockContent m_panHint = new DockContent();

		// tube editor property
		TubeCAMEditor m_TubeCAMEditor;

		// language manager
		LanguageManager m_LanguageManager = new LanguageManager( "CAMEditMainForm" );
	}
}
