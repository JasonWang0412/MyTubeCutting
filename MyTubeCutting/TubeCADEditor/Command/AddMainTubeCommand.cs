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
			if( m_MainTubeParam.IsValid() == false ) {
				return;
			}
			m_CADFeatureParamMap.MainTubeParam = m_MainTubeParam as CADft_MainTubeParam;
			EditFinished?.Invoke( EditType.AddMainTube, m_szMainTubeName );
		}

		public void Undo()
		{
			m_CADFeatureParamMap.MainTubeParam = null;
			EditFinished?.Invoke( EditType.RemoveMainTube, m_szMainTubeName );
		}

		public event CADEditFinishEventHandler EditFinished;

		string m_szMainTubeName;
		ICADFeatureParam m_MainTubeParam;
		CADFeatureParamMap m_CADFeatureParamMap;
	}
}
