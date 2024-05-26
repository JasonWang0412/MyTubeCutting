using MyCore.CAD;

namespace MyTubeCutting
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
			// check validility
			if( m_MainTubeParam.IsValid() == false ) {
				return;
			}

			// add the new main tube
			m_CADFeatureParamMap.MainTubeParam = m_MainTubeParam as CADft_MainTubeParam;

			// invoke the event
			EditFinished?.Invoke( EditType.AddMainTube, m_szMainTubeName );
		}

		public void Undo()
		{
			// data protection
			if( m_CADFeatureParamMap.MainTubeParam == null ) {
				return;
			}

			// remove the main tube
			m_CADFeatureParamMap.MainTubeParam = null;

			// invoke the event
			EditFinished?.Invoke( EditType.RemoveMainTube, m_szMainTubeName );
		}

		public event CADEditFinishEventHandler EditFinished;

		string m_szMainTubeName;
		ICADFeatureParam m_MainTubeParam;
		CADFeatureParamMap m_CADFeatureParamMap;
	}
}
