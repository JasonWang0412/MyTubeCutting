using MyCore.CAD;
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
			if( m_TubeCADEditor.IsExistMainTube() == false ) {
				m_btnEndCutter.Enabled = true;
				m_btnBranchTube.Enabled = true;
				m_btnMainTube.Enabled = false;
			}

			// set main tube parameter
			//double dRadius = 25;
			//double dThickness = 2;
			//double dTubeLength = 100;
			//Geom2D_Circle shape = new Geom2D_Circle( dRadius );
			//CrossSection crossSection = new CrossSection( shape, dThickness );
			//MainTubeParam mainTubeParam = new MainTubeParam( crossSection, dTubeLength );
			double dWidth = 50;
			double dHeight = 50;
			double dFillet = 5;
			double dThickness = 2;
			double dTubeLength = 100;
			Geom2D_Rectangle shape = new Geom2D_Rectangle( dWidth, dHeight, dFillet );
			CrossSection crossSection = new CrossSection( shape, dThickness );
			MainTubeParam mainTubeParam = new MainTubeParam( crossSection, dTubeLength );

			// set main tube to tube editor
			m_TubeCADEditor.SetMainTube( mainTubeParam );
		}

		void m_btnEndCutter_Click( object sender, System.EventArgs e )
		{
			// set end cutter parameter
			EndCutterParam endCutterParam = new EndCutterParam()
			{
				Center_X = 0,
				Center_Y = 0,
				Center_Z = 0,
				TiltAngle_deg = 0,
				RotateAngle_deg = 0,
				Side = EEndSide.Left
			};
			m_TubeCADEditor.AddEndCutter( endCutterParam );
		}

		void m_btnBranchTube_Click( object sender, System.EventArgs e )
		{
			// set branch tube 1 parameter
			//double dRadius = 10;
			//BranchTubeParam branchTubeParam = new BranchTubeParam()
			//{
			//	Center_X = 0,
			//	Center_Y = 50,
			//	Center_Z = 0,
			//	SelfRotateAngle = 0,
			//	AAngle = 0,
			//	BAngle = 0,
			//	IntersectDir = BranchIntersectDir.Positive,
			//	Length = 50,
			//	Shape = new Geom2D_Circle( dRadius ),
			//};
			double dWidth = 20;
			double dHeight = 20;
			double dFillet = 5;
			BranchTubeParam branchTubeParam = new BranchTubeParam()
			{
				Center_X = 0,
				Center_Y = 50,
				Center_Z = 0,
				SelfRotateAngle_deg = 0,
				AAngle_deg = 0,
				BAngle_deg = 0,
				IntersectDir = BranchIntersectDir.Positive,
				Length = 50,
				Shape = new Geom2D_Rectangle( dWidth, dHeight, dFillet ),
			};
			m_TubeCADEditor.AddBranchTube( branchTubeParam );
		}

		void m_treeObjBrowser_NodeMouseClick( object sender, TreeNodeMouseClickEventArgs e )
		{
			string szObjectName = e.Node.Text;
			m_TubeCADEditor.SetEditObject( szObjectName );
		}

		void m_propgrdPropertyBar_PropertyValueChanged( object s, PropertyValueChangedEventArgs e )
		{
			m_TubeCADEditor.UpdateObjectProperty( s, e );
		}

		private void m_treeObjBrowser_KeyUp( object sender, KeyEventArgs e )
		{
			if( e.KeyCode == Keys.Delete ) {
				m_TubeCADEditor.RemoveObject();
			}
		}
	}
}
