using MyCADCore;

namespace MyCADUI
{
	internal class AddMainTubeCommand : ICADEditCommand
	{
		public AddMainTubeCommand( string szMainTubeName, ICADFeatureParam mainTubeParam, CADFeatureParamMap paramMap )
		{
			m_szMainTubeName = szMainTubeName;
			m_MainTubeParam = mainTubeParam;
			m_CADFeatureParamMap = paramMap;
		}

		public void Do()
		{
			// data protection
			if( string.IsNullOrEmpty( m_szMainTubeName ) || m_MainTubeParam == null || m_CADFeatureParamMap == null ) {
				return;
			}

			// add the new main tube
			m_CADFeatureParamMap.MainTubeParam = m_MainTubeParam as CADft_MainTubeParam;

			// invoke the event
			CommandFinished?.Invoke( EditType.AddMainTube, m_szMainTubeName );
		}

		public void Undo()
		{
			// remove the main tube
			m_CADFeatureParamMap.MainTubeParam = null;

			// invoke the event
			CommandFinished?.Invoke( EditType.RemoveMainTube, m_szMainTubeName );
		}

		public event CADEditFinishEventHandler CommandFinished;

		string m_szMainTubeName;
		ICADFeatureParam m_MainTubeParam;
		CADFeatureParamMap m_CADFeatureParamMap;
	}
}
