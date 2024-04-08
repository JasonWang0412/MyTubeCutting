using MyCore.CAD;
using MyCore.Tool;
using OCC.AIS;
using OCC.Geom;
using OCC.gp;
using OCC.Graphic3d;
using OCC.Quantity;
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
		MainTubeParam m_MainTubeParam;
		Dictionary<string, EndCutterParam> m_EndCutterNameParamMap = new Dictionary<string, EndCutterParam>();
		Dictionary<string, BranchTubeParam> m_BranchTubeNameParamMap = new Dictionary<string, BranchTubeParam>();

		// display shape map
		AIS_Shape m_ResultTubeAIS;
		Dictionary<string, AIS_Plane> m_EndCutterNameAISMap = new Dictionary<string, AIS_Plane>();
		Dictionary<string, AIS_Shape> m_BranchTubeNameAISMap = new Dictionary<string, AIS_Shape>();

		// object browser map
		TreeNode m_MainTubeNode;

		// interaction
		int m_nEndCutterCount = 1;
		int m_nBranchTubeCount = 1;
		const string MAIN_TUBE_NAME = "MainTube";
		string m_szEditObjName;
		ITubeMakeParam m_EdiObjParam;

		internal TubeCADEditor( OCCViewer viewer, TreeView treeObjBrowser, PropertyGrid propertyGrid )
		{
			m_Viewer = viewer;
			m_treeObjBrowser = treeObjBrowser;
			m_propgrdPropertyBar = propertyGrid;
		}

		internal void SetMainTube( MainTubeParam mainTubeParam )
		{
			if( m_MainTubeNode == null ) {
				m_MainTubeNode = m_treeObjBrowser.Nodes.Add( MAIN_TUBE_NAME, MAIN_TUBE_NAME );
			}
			m_MainTubeParam = mainTubeParam;
			SetEditObject( MAIN_TUBE_NAME );
			UpdateAndRedrawResultTube();
		}

		internal void AddEndCutter( EndCutterParam endCutterParam )
		{
			string szName = GetNewEndCutterName();
			AddEndCutter( szName, endCutterParam );
		}

		internal void AddBranchTube( BranchTubeParam branchTubeParam )
		{
			string szName = GetNewBranchTubeName();
			AddBranchTube( szName, branchTubeParam );
		}

		internal void RemoveObject()
		{
			// cannot remove main tube
			if( m_szEditObjName == MAIN_TUBE_NAME ) {
				return;
			}
			else if( m_EndCutterNameParamMap.ContainsKey( m_szEditObjName ) ) {
				RemoveEndCutter( m_szEditObjName );
			}
			else if( m_BranchTubeNameParamMap.ContainsKey( m_szEditObjName ) ) {
				RemoveBranchTube( m_szEditObjName );
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

			if( m_szEditObjName == MAIN_TUBE_NAME ) {
				m_MainTubeParam = (MainTubeParam)( CloneHelper.Clone( m_EdiObjParam ) );
				UpdateAndRedrawResultTube();
				return;
			}
			else if( m_EndCutterNameParamMap.ContainsKey( m_szEditObjName ) ) {
				m_EndCutterNameParamMap[ m_szEditObjName ] = (EndCutterParam)( CloneHelper.Clone( m_EdiObjParam ) );

				// get ais from map
				AIS_Plane cutPlaneAIS = m_EndCutterNameAISMap[ m_szEditObjName ];

				// remove old ais from viewer
				m_Viewer.GetAISContext().Remove( cutPlaneAIS, false );

				// create new ais
				EndCutterParam endCutterParam = m_EndCutterNameParamMap[ m_szEditObjName ];
				cutPlaneAIS = MakeCutterAIS( endCutterParam );

				// update ais map
				m_EndCutterNameAISMap[ m_szEditObjName ] = cutPlaneAIS;
			}
			else if( m_BranchTubeNameParamMap.ContainsKey( m_szEditObjName ) ) {
				m_BranchTubeNameParamMap[ m_szEditObjName ] = (BranchTubeParam)( CloneHelper.Clone( m_EdiObjParam ) );

				// get ais from map
				AIS_Shape branchTubeAIS = m_BranchTubeNameAISMap[ m_szEditObjName ];

				// remove old ais from viewer
				m_Viewer.GetAISContext().Remove( branchTubeAIS, false );

				// create new ais
				BranchTubeParam branchTubeParam = m_BranchTubeNameParamMap[ m_szEditObjName ];
				branchTubeAIS = MakeBranchTubeAIS( branchTubeParam );

				// update ais map
				m_BranchTubeNameAISMap[ m_szEditObjName ] = branchTubeAIS;
			}

			// display new ais and redraw result tube
			DisplayObjectShape();
			UpdateAndRedrawResultTube();
		}

		internal bool IsExistMainTube()
		{
			return m_MainTubeParam != null;
		}

		void UpdateAndRedrawResultTube()
		{
			// remove old tube
			if( m_ResultTubeAIS != null ) {
				m_Viewer.GetAISContext().Remove( m_ResultTubeAIS, false );
			}

			// make new tube
			List<EndCutterParam> endCutterParams = m_EndCutterNameParamMap.Select( pair => pair.Value ).ToList();
			List<BranchTubeParam> branchTubeParamList = m_BranchTubeNameParamMap.Select( pair => pair.Value ).ToList();
			TopoDS_Shape tubeShape = TubeMaker.MakeResultTube( m_MainTubeParam, endCutterParams, branchTubeParamList );

			// display new tube
			m_ResultTubeAIS = new AIS_Shape( tubeShape );
			Graphic3d_MaterialAspect aspect = new Graphic3d_MaterialAspect( Graphic3d_NameOfMaterial.Graphic3d_NOM_STEEL );
			m_ResultTubeAIS.SetMaterial( aspect );
			m_ResultTubeAIS.SetDisplayMode( 1 );
			m_Viewer.GetAISContext().Display( m_ResultTubeAIS, false );
			m_Viewer.UpdateView();
		}

		void AddEndCutter( string szName, EndCutterParam endCutterParam )
		{
			AIS_Plane cutPlaneAIS = MakeCutterAIS( endCutterParam );

			m_EndCutterNameParamMap.Add( szName, endCutterParam );
			m_MainTubeNode.Nodes.Add( szName, szName );
			m_EndCutterNameAISMap.Add( szName, cutPlaneAIS );
			SetEditObject( szName );
			UpdateAndRedrawResultTube();
		}

		void AddBranchTube( string szName, BranchTubeParam branchTubeParam )
		{
			AIS_Shape branchTubeAIS = MakeBranchTubeAIS( branchTubeParam );

			m_BranchTubeNameAISMap.Add( szName, branchTubeAIS );
			m_MainTubeNode.Nodes.Add( szName, szName );
			m_BranchTubeNameParamMap.Add( szName, branchTubeParam );
			SetEditObject( szName );
			UpdateAndRedrawResultTube();
		}

		void RemoveEndCutter( string szName )
		{
			m_Viewer.GetAISContext().Remove( m_EndCutterNameAISMap[ szName ], false );
			m_EndCutterNameParamMap.Remove( szName );
			m_EndCutterNameAISMap.Remove( szName );
			m_MainTubeNode.Nodes.RemoveByKey( szName );
			UpdateAndRedrawResultTube();
		}

		void RemoveBranchTube( string szName )
		{
			m_Viewer.GetAISContext().Remove( m_BranchTubeNameAISMap[ szName ], false );
			m_BranchTubeNameParamMap.Remove( szName );
			m_BranchTubeNameAISMap.Remove( szName );
			m_MainTubeNode.Nodes.RemoveByKey( szName );
			UpdateAndRedrawResultTube();
		}

		string GetNewEndCutterName()
		{
			return "EndCutter" + m_nEndCutterCount++;
		}

		string GetNewBranchTubeName()
		{
			return "BranchTube" + m_nBranchTubeCount++;
		}

		AIS_Plane MakeCutterAIS( EndCutterParam endCutterParam )
		{
			gp_Pnt center = new gp_Pnt( endCutterParam.Center_X, endCutterParam.Center_Y, endCutterParam.Center_Z );
			OCCTool.GetEndCutterDir( endCutterParam.TiltAngle_deg, endCutterParam.RotateAngle_deg, out gp_Dir dir );
			Geom_Plane cutPlane = new Geom_Plane( center, dir );
			return new AIS_Plane( cutPlane );
		}

		AIS_Shape MakeBranchTubeAIS( BranchTubeParam branchTubeParam )
		{
			AIS_Shape branchTubeAIS = new AIS_Shape( TubeMaker.MakeBranchTube( branchTubeParam ) );
			Graphic3d_MaterialAspect aspect = new Graphic3d_MaterialAspect( Graphic3d_NameOfMaterial.Graphic3d_NOM_STONE );
			aspect.SetTransparency( 0.5f );
			aspect.SetColor( new Quantity_Color( Quantity_NameOfColor.Quantity_NOC_GREEN4 ) );
			branchTubeAIS.SetMaterial( aspect );
			branchTubeAIS.SetDisplayMode( 1 );
			return branchTubeAIS;
		}

		void DisplayObjectShape()
		{
			HideAllShapeExceptMainTube();

			// just hide all shape except main tube
			if( m_szEditObjName == MAIN_TUBE_NAME ) {
				return;
			}

			// display selected object shape
			if( m_EndCutterNameAISMap.ContainsKey( m_szEditObjName ) ) {
				m_Viewer.GetAISContext().Display( m_EndCutterNameAISMap[ m_szEditObjName ], false );
			}
			else if( m_BranchTubeNameAISMap.ContainsKey( m_szEditObjName ) ) {
				m_Viewer.GetAISContext().Display( m_BranchTubeNameAISMap[ m_szEditObjName ], false );
			}
			m_Viewer.UpdateView();
		}

		void ShowObjectProperty()
		{
			if( m_szEditObjName == MAIN_TUBE_NAME ) {
				m_EdiObjParam = CloneHelper.Clone( m_MainTubeParam );
			}
			else if( m_EndCutterNameParamMap.ContainsKey( m_szEditObjName ) ) {
				m_EdiObjParam = CloneHelper.Clone( m_EndCutterNameParamMap[ m_szEditObjName ] );
			}
			else if( m_BranchTubeNameParamMap.ContainsKey( m_szEditObjName ) ) {
				m_EdiObjParam = CloneHelper.Clone( m_BranchTubeNameParamMap[ m_szEditObjName ] );
			}
			m_propgrdPropertyBar.SelectedObject = m_EdiObjParam;
		}

		void HideAllShapeExceptMainTube()
		{
			foreach( KeyValuePair<string, AIS_Plane> pair in m_EndCutterNameAISMap ) {
				m_Viewer.GetAISContext().Erase( pair.Value, false );
			}
			foreach( KeyValuePair<string, AIS_Shape> pair in m_BranchTubeNameAISMap ) {
				m_Viewer.GetAISContext().Erase( pair.Value, false );
			}
			m_Viewer.UpdateView();
		}
	}
}
