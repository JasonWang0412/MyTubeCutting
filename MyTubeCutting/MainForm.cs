using MyCore.CAD;
using OCC.gp;
using System.Windows.Forms;

namespace MyTubeCutting
{
	public partial class MainForm : Form
	{
		// viewer property
		OCCViewer m_Viewer;
		int m_nXMousePosition = 0;
		int m_nYMousePosition = 0;

		// tube editor property
		TubeCADEditor m_TubeCADEditor;

		public MainForm()
		{
			// initalize component
			InitializeComponent();
			m_btnEndCutter.Enabled = false;
			m_btnBranchTube.Enabled = false;

			// initialize viewer
			m_Viewer = new OCCViewer();
			bool isSuccess = m_Viewer.InitViewer( m_panViewer.Handle );
			if( isSuccess == false ) {
				MessageBox.Show( "init failed" );
			}
			m_Viewer.SetBackgroundColor( 0, 0, 0 );
			m_Viewer.IsometricView();

			// initialize tube editor
			m_TubeCADEditor = new TubeCADEditor( m_Viewer, m_treeObjBrowser, m_propgrdPropertyBar );
			m_TubeCADEditor.MainTubeStatusChanged += MainTubeStatusChanged;

			// key event
			m_panViewer.KeyDown += m_panViewer_KeyDown;
			m_treeObjBrowser.KeyDown += m_treeObjBrowser_KeyDown;
		}

		void MainTubeStatusChanged( bool bExistMainTube )
		{
			// set button status
			if( bExistMainTube == true ) {
				m_btnEndCutter.Enabled = true;
				m_btnBranchTube.Enabled = true;
				m_btnMainTube.Enabled = false;
			}
			else {
				m_btnEndCutter.Enabled = false;
				m_btnBranchTube.Enabled = false;
				m_btnMainTube.Enabled = true;
			}
		}

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

		void m_btnMainTube_Click( object sender, System.EventArgs e )
		{
			MainTubeTypeForm mainTubeShapeForm = new MainTubeTypeForm();
			mainTubeShapeForm.MainTubeTypeSelected += MainTubeTypeSelected;

			// TODO: make it show from center of main form
			mainTubeShapeForm.Show();
		}

		void MainTubeTypeSelected( MainTubeType type )
		{
			// set main tube parameter
			CADft_MainTubeParam mainTubeParam;
			double dThickness = 2;
			double dTubeLength = 100;
			if( type == MainTubeType.Circle ) {
				double dRadius = 25;
				Geom2D_Circle shape = new Geom2D_Circle( dRadius );
				CrossSection crossSection = new CrossSection( shape, dThickness );
				mainTubeParam = new CADft_MainTubeParam( crossSection, dTubeLength );
			}
			else if( type == MainTubeType.Rectangle ) {
				double dWidth = 50;
				double dHeight = 50;
				double dFillet = 5;
				Geom2D_Rectangle shape = new Geom2D_Rectangle( dWidth, dHeight, dFillet );
				CrossSection crossSection = new CrossSection( shape, dThickness );
				mainTubeParam = new CADft_MainTubeParam( crossSection, dTubeLength );
			}
			else {
				MessageBox.Show( "The type is currenttly not supported." );
				return;
			}

			// set main tube to tube editor
			m_TubeCADEditor.AddMainTube( mainTubeParam );
		}

		void m_btnEndCutter_Click( object sender, System.EventArgs e )
		{
			// set end cutter parameter
			CADft_EndCutterParam endCutterParam = new CADft_EndCutterParam( 0, 0, 0, EEndSide.Left );
			m_TubeCADEditor.AddEndCutter( endCutterParam );
		}

		void m_btnBranchTube_Click( object sender, System.EventArgs e )
		{
			BranchTubeTypeForm branchTubeTypeForm = new BranchTubeTypeForm();
			branchTubeTypeForm.BranchTubeTypeSelected += BranchTubeTypeSelected;
			branchTubeTypeForm.Show();
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
				double dFillet = 5;
				shape = new Geom2D_Rectangle( dWidth, dHeight, dFillet );
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
			m_TubeCADEditor.AddBranchTube( branchTubeParam );
		}

		void m_propgrdPropertyBar_PropertyValueChanged( object s, PropertyValueChangedEventArgs e )
		{
			m_TubeCADEditor.UpdateObjectProperty( s, e );
		}

		void m_treeObjBrowser_AfterSelect( object sender, TreeViewEventArgs e )
		{
			string szObjectName = e.Node.Text;
			m_TubeCADEditor.SetEditObject( szObjectName );
		}

		void m_panViewer_KeyDown( object sender, KeyEventArgs e )
		{
			if( e.Modifiers == Keys.Control ) {
				if( e.KeyCode == Keys.Z ) {
					m_TubeCADEditor.Undo();
				}
				else if( e.KeyCode == Keys.Y ) {
					m_TubeCADEditor.Redo();
				}
			}
		}

		void m_treeObjBrowser_KeyDown( object sender, KeyEventArgs e )
		{
			if( e.KeyCode == Keys.Delete ) {
				m_TubeCADEditor.RemoveCADFeature();
			}
			else if( e.Modifiers == Keys.Control ) {
				if( e.KeyCode == Keys.Z ) {
					m_TubeCADEditor.Undo();
				}
				else if( e.KeyCode == Keys.Y ) {
					m_TubeCADEditor.Redo();
				}
			}
		}

		void m_btnX_Pos_Click( object sender, System.EventArgs e )
		{
			m_Viewer.RightView();
		}

		void m_btnX_Neg_Click( object sender, System.EventArgs e )
		{
			m_Viewer.LeftView();
		}

		void m_btnY_Pos_Click( object sender, System.EventArgs e )
		{
			m_Viewer.FrontView();
		}

		void m_btnY_Neg_Click( object sender, System.EventArgs e )
		{
			m_Viewer.BackView();
		}

		void m_btnZ_Pos_Click( object sender, System.EventArgs e )
		{
			m_Viewer.TopView();
		}

		void m_btnZ_Neg_Click( object sender, System.EventArgs e )
		{
			m_Viewer.BottomView();
		}

		void m_btnDir_Pos_Click( object sender, System.EventArgs e )
		{
			gp_Dir dir = m_TubeCADEditor.GetEditObjectDir();
			m_Viewer.SetViewDir( dir );
		}

		void m_btnDir_Neg_Click( object sender, System.EventArgs e )
		{
			gp_Dir dir = m_TubeCADEditor.GetEditObjectDir();
			m_Viewer.SetViewDir( dir.Reversed() );
		}

		void m_btnISO_Click( object sender, System.EventArgs e )
		{
			m_Viewer.IsometricView();
		}
	}
}
