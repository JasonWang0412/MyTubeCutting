using MyCADCore;

namespace MyCADUI
{
	internal class RemoveMainTubeCommand : ICADEditCommand
	{
		public RemoveMainTubeCommand( string szMainTubeName, CADFeatureParamMap paramMap )
		{
			m_szMainTubeName = szMainTubeName;
			m_CADFeatureParamMap = paramMap;
		}

		public void Do()
		{
			// data protection
			// data protection
			if( string.IsNullOrEmpty( m_szMainTubeName ) || m_CADFeatureParamMap == null ) {
				return;
			}

			// backup the old value
			m_BackupMainTubeParam = m_CADFeatureParamMap.MainTubeParam;

			// remove the main tube
			m_CADFeatureParamMap.MainTubeParam = null;

			// invoke the event
			CommandFinished?.Invoke( EditType.RemoveMainTube, m_szMainTubeName );
		}

		public void Undo()
		{
			// add the main tube back
			m_CADFeatureParamMap.MainTubeParam = m_BackupMainTubeParam as CADft_MainTubeParam;

			// invoke the event
			CommandFinished?.Invoke( EditType.AddMainTube, m_szMainTubeName );
		}

		public event CADEditFinishEventHandler CommandFinished;

		string m_szMainTubeName;
		ICADFeatureParam m_BackupMainTubeParam;
		CADFeatureParamMap m_CADFeatureParamMap;
	}
}
