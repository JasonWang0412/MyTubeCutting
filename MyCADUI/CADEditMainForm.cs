using MyCADCore;
using MyLanguageManager;
using OCC.gp;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MyCADUI
{
	public partial class CADEditMainForm : Form
	{
		// viewer property
		OCCViewer m_Viewer = new OCCViewer();
		int m_nXMousePosition = 0;
		int m_nYMousePosition = 0;

		// tube editor property
		TubeCADEditor m_TubeCADEditor;

		// language manager
		LanguageManager m_LanguageManager = new LanguageManager( "CADEditMainForm" );

		public CADEditMainForm()
		{
			// initalize component
			InitializeComponent();
			m_tsmiEndCutter.Enabled = false;
			m_tsmiBranchTube.Enabled = false;
			m_tsmiBendingNotch.Enabled = false;
			m_tsmiUndo.Enabled = false;
			m_tsmiRedo.Enabled = false;

			// initialize viewer
			bool isSuccess = m_Viewer.InitViewer( m_panViewer.Handle );
			if( isSuccess == false ) {
				MessageBox.Show( "init failed" );
			}
			m_Viewer.SetBackgroundColor( 0, 0, 0 );
			m_Viewer.IsometricView();

			// initialize tube editor
			m_TubeCADEditor = new TubeCADEditor( m_Viewer, m_treeObjBrowser, m_propgrdPropertyBar );
			m_TubeCADEditor.MainTubeStatusChanged += MainTubeStatusChanged;
			m_TubeCADEditor.CADEditErrorEvent += CADEditError;
			m_TubeCADEditor.CADEditSuccessEvent += CADEditSuccess;
			m_TubeCADEditor.CommandStatusChanged += ( bUndo, bRedo ) =>
			{
				m_tsmiUndo.Enabled = bUndo;
				m_tsmiRedo.Enabled = bRedo;
			};

			// set language zh-TW
			SetLanguage();
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

		// main tube
		void m_tsmiMainTube_Circle_Click( object sender, System.EventArgs e )
		{
			MainTubeTypeSelected( MainTubeType.Circle );
		}

		void m_tsmiMainTube_Rectangle_Click( object sender, System.EventArgs e )
		{
			MainTubeTypeSelected( MainTubeType.Rectangle );
		}

		void m_tsmiMainTube_Oval_Click( object sender, System.EventArgs e )
		{
			MainTubeTypeSelected( MainTubeType.Oval );
		}

		void m_tsmiMainTube_FlatOval_Click( object sender, System.EventArgs e )
		{
			MainTubeTypeSelected( MainTubeType.FlatOval );
		}

		void m_tsmiMainTube_DShape_Click( object sender, System.EventArgs e )
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
		void m_tsmiEndCutter_Click( object sender, System.EventArgs e )
		{
			// set end cutter parameter
			CADft_EndCutterParam endCutterParam = new CADft_EndCutterParam( 0, 0, 0, EEndSide.Left );
			m_TubeCADEditor.AddCADFeature( endCutterParam );
		}

		// branch tube
		void m_tsmiBranchTube_Circle_Click( object sender, System.EventArgs e )
		{
			BranchTubeTypeSelected( BranchTubeType.Circle );
		}

		void m_tsmiBranchTube_Rectangle_Click( object sender, System.EventArgs e )
		{
			BranchTubeTypeSelected( BranchTubeType.Rectangle );
		}

		void m_tsmiBranchTube_Oval_Click( object sender, System.EventArgs e )
		{
			BranchTubeTypeSelected( BranchTubeType.Oval );
		}

		void m_tsmiBranchTube_FlatOval_Click( object sender, System.EventArgs e )
		{
			BranchTubeTypeSelected( BranchTubeType.FlatOval );
		}

		void m_tsmiBranchTube_DShape_Click( object sender, System.EventArgs e )
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
		void m_tsmiBendingNotch_VShape_Click( object sender, System.EventArgs e )
		{
			BendingNotchTypeSelected( BendingNotchType.VShape );
		}

		void m_tsmiBendingNotch_BothSide_Click( object sender, System.EventArgs e )
		{
			BendingNotchTypeSelected( BendingNotchType.BothSide );
		}

		void m_tsmiBendingNotch_OneSide_Click( object sender, System.EventArgs e )
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

		void m_tsmiExport_Click( object sender, System.EventArgs e )
		{
			m_TubeCADEditor.ExportStep();
		}

		// TODO: compelte the implementation
		void openbetaToolStripMenuItem_Click( object sender, System.EventArgs e )
		{
			m_TubeCADEditor.OpenMapFile();
		}

		void m_tsmiUndo_Click( object sender, System.EventArgs e )
		{
			m_TubeCADEditor.Undo();
		}

		void m_tsmiRedo_Click( object sender, System.EventArgs e )
		{
			m_TubeCADEditor.Redo();
		}

		void MainTubeStatusChanged( bool bExistMainTube )
		{
			// set button status
			if( bExistMainTube == true ) {
				m_tsmiEndCutter.Enabled = true;
				m_tsmiBranchTube.Enabled = true;
				m_tsmiBendingNotch.Enabled = true;
				m_tsmiMainTube.Enabled = false;
			}
			else {
				m_tsmiEndCutter.Enabled = false;
				m_tsmiBranchTube.Enabled = false;
				m_tsmiBendingNotch.Enabled = false;
				m_tsmiMainTube.Enabled = true;
			}
		}

		void CADEditError( CADEditErrorCode errorCode )
		{
			// TODO: magic number
			if( (int)errorCode < 100 ) {
				MessageBox.Show( errorCode.ToString() );
			}
			else {
				m_lblWarnning.Text = errorCode.ToString();
			}
		}

		void CADEditSuccess()
		{
			m_lblWarnning.Text = string.Empty;
		}

		// view action
		void m_tsmiX_Pos_Click( object sender, System.EventArgs e )
		{
			m_Viewer.RightView();
			m_Viewer.ZoomAllView();
		}

		void m_tsmiX_Neg_Click( object sender, System.EventArgs e )
		{
			m_Viewer.LeftView();
			m_Viewer.ZoomAllView();
		}

		void m_tsmiY_Pos_Click( object sender, System.EventArgs e )
		{
			m_Viewer.FrontView();
			m_Viewer.ZoomAllView();
		}

		void m_tsmiY_Neg_Click( object sender, System.EventArgs e )
		{
			m_Viewer.BackView();
			m_Viewer.ZoomAllView();
		}

		void m_tsmiZ_Pos_Click( object sender, System.EventArgs e )
		{
			m_Viewer.TopView();
			m_Viewer.ZoomAllView();
		}

		void m_tsmiZ_Neg_Click( object sender, System.EventArgs e )
		{
			m_Viewer.BottomView();
			m_Viewer.ZoomAllView();
		}

		void m_tsmiDir_Pos_Click( object sender, System.EventArgs e )
		{
			gp_Dir dir = m_TubeCADEditor.GetEditObjectDir();
			m_Viewer.SetViewDir( dir );
			m_Viewer.ZoomAllView();
		}

		void m_tsmiDir_Neg_Click( object sender, System.EventArgs e )
		{
			gp_Dir dir = m_TubeCADEditor.GetEditObjectDir();
			m_Viewer.SetViewDir( dir.Reversed() );
			m_Viewer.ZoomAllView();
		}

		void m_tsmiISO_Click( object sender, System.EventArgs e )
		{
			m_Viewer.IsometricView();
			m_Viewer.ZoomAllView();
		}

		void m_tsmiZoomToFit_Click( object sender, System.EventArgs e )
		{
			m_Viewer.ZoomAllView();
		}

		// Other action
		void m_tsmiAbout_Click( object sender, System.EventArgs e )
		{
			MessageBox.Show( "My CAD V2" );
		}

		// this is temporary function
		void SetLanguage()
		{
			LanguageManager.CurrentUILanguage = "zh-TW";
			ApplyComponentResource();
		}

		void ApplyComponentResource()
		{
			List<ToolStripMenuItem> menuItems = FindAllToolStripItems( m_msMainMenu.Items );
			foreach( ToolStripMenuItem item in menuItems ) {
				string szText = m_LanguageManager.GetString( item.Name );
				if( string.IsNullOrEmpty( szText ) == false ) {
					item.Text = szText;
				}
			}
		}

		List<ToolStripMenuItem> FindAllToolStripItems( ToolStripItemCollection items )
		{
			List<ToolStripMenuItem> list = new List<ToolStripMenuItem>();
			foreach( ToolStripItem item in items ) {
				if( item is ToolStripMenuItem ) {
					list.Add( item as ToolStripMenuItem );
					list.AddRange( FindAllToolStripItems( ( item as ToolStripMenuItem ).DropDownItems ) );
				}
			}
			return list;
		}
	}
}
