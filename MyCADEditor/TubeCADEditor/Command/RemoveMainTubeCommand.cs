using MyCADCore;

namespace MyCADEditor
{
	internal class RemoveMainTubeCommand : ICADEditCommand
	{
		public RemoveMainTubeCommand( string szMainTubeID, CADFeatureParamMap paramMap )
		{
			m_szMainTubeID = szMainTubeID;
			m_CADFeatureParamMap = paramMap;
		}

		public void Do()
		{
			// data protection
			// data protection
			if( string.IsNullOrEmpty( m_szMainTubeID ) || m_CADFeatureParamMap == null ) {
				return;
			}

			// backup the old value
			m_BackupMainTubeParam = m_CADFeatureParamMap.MainTubeParam;

			// remove the main tube
			m_CADFeatureParamMap.MainTubeParam = null;

			// invoke the event
			CommandFinished?.Invoke( EditType.RemoveMainTube, m_szMainTubeID );
		}

		public void Undo()
		{
			// add the main tube back
			m_CADFeatureParamMap.MainTubeParam = m_BackupMainTubeParam as CADft_MainTubeParam;

			// invoke the event
			CommandFinished?.Invoke( EditType.AddMainTube, m_szMainTubeID );
		}

		public event CADEditFinishEventHandler CommandFinished;

		string m_szMainTubeID;
		ICADFeatureParam m_BackupMainTubeParam;
		CADFeatureParamMap m_CADFeatureParamMap;
	}
}
