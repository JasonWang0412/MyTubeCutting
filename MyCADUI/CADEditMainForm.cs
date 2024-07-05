using MyCADCore;
using MyCADEditor;
using MyLanguageManager;
using OCC.STEPControl;
using OCC.TopoDS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace MyCADUI
{
	public partial class CADEditMainForm : Form
	{
		public CADEditMainForm()
		{
			// set layout back ground
			m_dockPanel.DocumentStyle = DocumentStyle.DockingWindow;
			m_dockPanel.Dock = DockStyle.Fill;
			Controls.Add( m_dockPanel );
			m_dockPanel.DockLeftPortion = 0.3;

			// initalize component
			InitializeComponent();

			m_tsCADFeature.Enabled = false;
			m_tsbtnUndo.Enabled = false;
			m_tsbtnRedo.Enabled = false;
			m_tsbtnExport.Enabled = false;

			// editor
			m_TubeCADEditor = new TubeCADEditor();

			// editor layout
			m_panViewer.Controls.Add( m_TubeCADEditor.ViewerPanel );
			m_panViewer.Show( m_dockPanel, DockState.Document );

			m_panObjBrowser.Controls.Add( m_TubeCADEditor.ObjectBrowserPanel );
			m_panObjBrowser.Show( m_dockPanel, DockState.DockLeft );

			m_panPropertyBar.Controls.Add( m_TubeCADEditor.PropertyBarPanel );
			m_panPropertyBar.Show( m_panObjBrowser.Pane, DockAlignment.Bottom, 0.5 );

			m_panHint.Controls.Add( m_TubeCADEditor.HintLabelPanel );
			m_panHint.Show( m_panPropertyBar.Pane, DockAlignment.Bottom, 0.2 );

			// initialize tube editor
			m_TubeCADEditor.MainTubeStatusChanged += MainTubeStatusChanged;
			m_TubeCADEditor.CommandStatusChanged += ( bUndo, bRedo ) =>
			{
				m_tsbtnUndo.Enabled = bUndo;
				m_tsbtnRedo.Enabled = bRedo;
			};

			// set language zh-TW
			// TODO: any language
			SetLanguage();
		}

		// main tube
		void m_tsbtnMainTube_Circle_Click( object sender, System.EventArgs e )
		{
			MainTubeTypeSelected( MainTubeType.Circle );
		}

		void m_tsbtnMainTube_Rectangle_Click( object sender, System.EventArgs e )
		{
			MainTubeTypeSelected( MainTubeType.Rectangle );
		}

		void m_tsbtnMainTube_Oval_Click( object sender, System.EventArgs e )
		{
			MainTubeTypeSelected( MainTubeType.Oval );
		}

		void m_tsbtnMainTube_FlatOval_Click( object sender, System.EventArgs e )
		{
			MainTubeTypeSelected( MainTubeType.FlatOval );
		}

		void m_tsbtnMainTube_DShape_Click( object sender, System.EventArgs e )
		{
			MainTubeTypeSelected( MainTubeType.DShape );
		}

		void MainTubeTypeSelected( MainTubeType type )
		{
			// set main tube parameter
			CADft_MainTubeParam mainTubeParam;
			double dThickness = 2;
			double dTubeLength = 100;
			CrossSection crossSection;
			if( type == MainTubeType.Circle ) {
				double dRadius = 25;
				Geom2D_Circle shape = new Geom2D_Circle( dRadius );
				crossSection = new CrossSection( shape, dThickness );
			}
			else if( type == MainTubeType.Rectangle ) {
				double dWidth = 50;
				double dHeight = 50;
				double dFillet = 5;
				Geom2D_Rectangle shape = new Geom2D_Rectangle( dWidth, dHeight, dFillet );
				crossSection = new CrossSection( shape, dThickness );
			}
			else if( type == MainTubeType.Oval ) {
				double dWidth = 50;
				double dHeight = 40;
				Geom2D_Oval shape = new Geom2D_Oval( dWidth, dHeight );
				crossSection = new CrossSection( shape, dThickness );
			}
			else if( type == MainTubeType.FlatOval ) {
				double dWidth = 50;
				double dRadius = 20;
				Geom2D_FlatOval shape = new Geom2D_FlatOval( dWidth, dRadius );
				crossSection = new CrossSection( shape, dThickness );
			}
			else if( type == MainTubeType.DShape ) {
				double dWidth = 50;
				double dHeight = 50;
				double dFillet = 5;
				Geom2D_DShape shape = new Geom2D_DShape( dWidth, dHeight, dFillet );
				crossSection = new CrossSection( shape, dThickness );
			}
			else {
				MessageBox.Show( "The type is currenttly not supported." );
				return;
			}
			mainTubeParam = new CADft_MainTubeParam( crossSection, dTubeLength );

			// set main tube to tube editor
			m_TubeCADEditor.AddMainTube( mainTubeParam );
		}

		// end cutter
		void m_tsbtnEndCutter_Click( object sender, System.EventArgs e )
		{
			// set end cutter parameter
			CADft_EndCutterParam endCutterParam = new CADft_EndCutterParam( 0, 0, 0, EEndSide.Left );
			m_TubeCADEditor.AddCADFeature( endCutterParam );
		}

		// branch tube
		void m_tsbtnBranchTube_Circle_Click( object sender, System.EventArgs e )
		{
			BranchTubeTypeSelected( BranchTubeType.Circle );
		}

		void m_tsbtnBranchTube_Rectangle_Click( object sender, System.EventArgs e )
		{
			BranchTubeTypeSelected( BranchTubeType.Rectangle );
		}

		void m_tsbtnBranchTube_Oval_Click( object sender, System.EventArgs e )
		{
			BranchTubeTypeSelected( BranchTubeType.Oval );
		}

		void m_tsbtnBranchTube_FlatOval_Click( object sender, System.EventArgs e )
		{
			BranchTubeTypeSelected( BranchTubeType.FlatOval );
		}

		void m_tsbtnBranchTube_DShape_Click( object sender, System.EventArgs e )
		{
			BranchTubeTypeSelected( BranchTubeType.DShape );
		}

		void BranchTubeTypeSelected( BranchTubeType type )
		{
			// set branch tube parameter
			IGeom2D shape;
			double length = 50;
			if( type == BranchTubeType.Circle ) {
				double dRadius = 10;
				shape = new Geom2D_Circle( dRadius );
			}
			else if( type == BranchTubeType.Rectangle ) {
				double dWidth = 20;
				double dHeight = 20;
				double dFillet = 2;
				shape = new Geom2D_Rectangle( dWidth, dHeight, dFillet );
			}
			else if( type == BranchTubeType.Oval ) {
				double dWidth = 20;
				double dHeight = 15;
				shape = new Geom2D_Oval( dWidth, dHeight );
			}
			else if( type == BranchTubeType.FlatOval ) {
				double dWidth = 20;
				double dRadius = 7.5;
				shape = new Geom2D_FlatOval( dWidth, dRadius );
			}
			else if( type == BranchTubeType.DShape ) {
				double dWidth = 20;
				double dHeight = 20;
				double dFillet = 2;
				shape = new Geom2D_DShape( dWidth, dHeight, dFillet );
			}
			else {
				MessageBox.Show( "The type is currenttly not supported." );
				return;
			}

			double x = 0;
			double y = 50;
			double z = 0;
			double selfRotateAngle_deg = 0;
			double angleA_deg = 0;
			double angleB_deg = 0;
			BranchIntersectDir intersectDir = BranchIntersectDir.Positive;
			CADft_BranchTubeParam branchTubeParam = new CADft_BranchTubeParam( x, y, z, selfRotateAngle_deg, angleA_deg, angleB_deg, shape, intersectDir, false, length );

			// set branch tube to tube editor
			m_TubeCADEditor.AddCADFeature( branchTubeParam );
		}

		// bending notch
		void m_tsbtnBendingNotch_VShape_Click( object sender, System.EventArgs e )
		{
			BendingNotchTypeSelected( BendingNotchType.VShape );
		}

		void m_tsbtnBendingNotch_BothSide_Click( object sender, System.EventArgs e )
		{
			BendingNotchTypeSelected( BendingNotchType.BothSide );
		}

		void m_tsbtnBendingNotch_OneSide_Click( object sender, System.EventArgs e )
		{
			BendingNotchTypeSelected( BendingNotchType.OneSide );
		}

		void BendingNotchTypeSelected( BendingNotchType type )
		{
			// set bending notch parameter
			IBendingNotchShape shape;
			if( type == BendingNotchType.VShape ) {
				double bendingAngle_deg = 90;
				double jointGapLength = 0;
				shape = new BN_VShape( bendingAngle_deg, jointGapLength );
			}
			else if( type == BendingNotchType.BothSide ) {
				double filletRadius = 25;
				double bendingAngle_deg = 90;
				bool isOverCut = false;
				double jointGapLength = 0;
				shape = new BN_BothSide( filletRadius, bendingAngle_deg, isOverCut, jointGapLength );
			}
			else if( type == BendingNotchType.OneSide ) {
				bool isOverCut = false;
				NotchSide side = NotchSide.Left;
				shape = new BN_OneSide( isOverCut, side );
			}
			else {
				MessageBox.Show( "The type is currenttly not supported." );
				return;
			}

			double y = 50;
			double gap = 0.5;
			double angleB_deg = 0;
			CADft_BendingNotchParam bendingNotchParam = new CADft_BendingNotchParam( shape, y, gap, angleB_deg );
			m_TubeCADEditor.AddCADFeature( bendingNotchParam );
		}

		// tube editor action
		void m_tsbtnUndo_Click( object sender, System.EventArgs e )
		{
			m_TubeCADEditor.Undo();
		}

		void m_tsbtnRedo_Click( object sender, System.EventArgs e )
		{
			m_TubeCADEditor.Redo();
		}

		void m_tsbtnExport_Click( object sender, System.EventArgs e )
		{
			TopoDS_Shape resultTube = m_TubeCADEditor.GetResultTube();
			if( resultTube == null ) {
				return;
			}

			// file directory
			string szFileDir = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "OutPut" );
			Directory.CreateDirectory( szFileDir );

			// save step file
			string szStepFilePath = Path.Combine( szFileDir, "result.stp" );
			STEPControl_Writer writer = new STEPControl_Writer();
			writer.Transfer( resultTube, STEPControl_StepModelType.STEPControl_AsIs );
			writer.Write( szStepFilePath );
		}

		void MainTubeStatusChanged( bool bExistMainTube )
		{
			// set button status
			if( bExistMainTube == true ) {
				m_tsCADFeature.Enabled = true;
				m_tsMainTube.Enabled = false;
				m_tsbtnExport.Enabled = true;
			}
			else {
				m_tsCADFeature.Enabled = false;
				m_tsMainTube.Enabled = true;
				m_tsbtnExport.Enabled = false;
			}
		}

		// view action
		void m_tsbtnX_Pos_Click( object sender, System.EventArgs e )
		{
			m_TubeCADEditor.SetViewDir( ViewDir.Right );
		}

		void m_tsbtnX_Neg_Click( object sender, System.EventArgs e )
		{
			m_TubeCADEditor.SetViewDir( ViewDir.Left );
		}

		void m_tsbtnY_Pos_Click( object sender, System.EventArgs e )
		{
			m_TubeCADEditor.SetViewDir( ViewDir.Front );
		}

		void m_tsbtnY_Neg_Click( object sender, System.EventArgs e )
		{
			m_TubeCADEditor.SetViewDir( ViewDir.Back );
		}

		void m_tsbtnZ_Pos_Click( object sender, System.EventArgs e )
		{
			m_TubeCADEditor.SetViewDir( ViewDir.Top );
		}

		void m_tsbtnZ_Neg_Click( object sender, System.EventArgs e )
		{
			m_TubeCADEditor.SetViewDir( ViewDir.Bottom );
		}

		void m_tsbtnDir_Pos_Click( object sender, System.EventArgs e )
		{
			m_TubeCADEditor.SetViewDir( ViewDir.Dir_Pos );
		}

		void m_tsbtnDir_Neg_Click( object sender, System.EventArgs e )
		{
			m_TubeCADEditor.SetViewDir( ViewDir.Dir_Neg );
		}

		void m_tsbtnISO_Click( object sender, System.EventArgs e )
		{
			m_TubeCADEditor.SetViewDir( ViewDir.Isometric );
		}

		void m_tsbtnZoomToFit_Click( object sender, System.EventArgs e )
		{
			m_TubeCADEditor.ZoomToFit();
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
			menuItems.AddRange( FindAllToolStripItems( m_tsMainTube.Items ) );
			menuItems.AddRange( FindAllToolStripItems( m_tsCADFeature.Items ) );
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
		DockContent m_panViewer = new DockContent();
		DockContent m_panObjBrowser = new DockContent();
		DockContent m_panPropertyBar = new DockContent();
		DockContent m_panHint = new DockContent();

		// tube editor property
		TubeCADEditor m_TubeCADEditor;

		// language manager
		LanguageManager m_LanguageManager = new LanguageManager( "CADEditMainForm" );
	}
}
