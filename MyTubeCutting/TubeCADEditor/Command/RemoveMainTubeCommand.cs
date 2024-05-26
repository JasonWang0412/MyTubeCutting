using MyCore.CAD;

namespace MyTubeCutting
{
	internal class RemoveMainTubeCommand : ICADEditCommand
	{
		public RemoveMainTubeCommand( string szMainTubeName, CADFeatureParamMap paramMap )
		{
			m_szMainTubeName = szMainTubeName;
			m_BackupMainTubeParam = paramMap.MainTubeParam;
			m_CADFeatureParamMap = paramMap;
		}

		public void Do()
		{
			// can not remove when cad feature is not empty
			if( m_CADFeatureParamMap.FeatureMap.Count != 0 ) {
				return;
			}

			// data protection
			if( m_CADFeatureParamMap.MainTubeParam == null ) {
				return;
			}

			// remove the main tube
			m_CADFeatureParamMap.MainTubeParam = null;

			// invoke the event
			EditFinished?.Invoke( EditType.RemoveMainTube, m_szMainTubeName );
		}

		public void Undo()
		{
			// add the main tube back
			m_CADFeatureParamMap.MainTubeParam = m_BackupMainTubeParam as CADft_MainTubeParam;

			// invoke the event
			EditFinished?.Invoke( EditType.AddMainTube, m_szMainTubeName );
		}

		public event CADEditFinishEventHandler EditFinished;

		string m_szMainTubeName;
		ICADFeatureParam m_BackupMainTubeParam;
		CADFeatureParamMap m_CADFeatureParamMap;
	}
}
