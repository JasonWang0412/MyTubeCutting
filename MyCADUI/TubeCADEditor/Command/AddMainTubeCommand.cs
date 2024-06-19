using MyCADCore;

namespace MyCADUI
{
	internal class AddMainTubeCommand : ICADEditCommand
	{
		public AddMainTubeCommand( string szMainTubeID, ICADFeatureParam mainTubeParam, CADFeatureParamMap paramMap )
		{
			m_szMainTubeID = szMainTubeID;
			m_MainTubeParam = mainTubeParam;
			m_CADFeatureParamMap = paramMap;
		}

		public void Do()
		{
			// data protection
			if( string.IsNullOrEmpty( m_szMainTubeID ) || m_MainTubeParam == null || m_CADFeatureParamMap == null ) {
				return;
			}

			// add the new main tube
			m_CADFeatureParamMap.MainTubeParam = m_MainTubeParam as CADft_MainTubeParam;

			// invoke the event
			CommandFinished?.Invoke( EditType.AddMainTube, m_szMainTubeID );
		}

		public void Undo()
		{
			// remove the main tube
			m_CADFeatureParamMap.MainTubeParam = null;

			// invoke the event
			CommandFinished?.Invoke( EditType.RemoveMainTube, m_szMainTubeID );
		}

		public event CADEditFinishEventHandler CommandFinished;

		string m_szMainTubeID;
		ICADFeatureParam m_MainTubeParam;
		CADFeatureParamMap m_CADFeatureParamMap;
	}
}
