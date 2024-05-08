using MyCore.CAD;

namespace MyTubeCutting
{
	internal class RemoveMainTubeCommand : ICADEditCommand
	{
		public RemoveMainTubeCommand( string szMainTubeName, CADFeatureParamMap paramMap )
		{
			m_szMainTubeName = szMainTubeName;
			m_OldMainTubeParam = paramMap.MainTubeParam;
			m_CADFeatureParamMap = paramMap;
		}

		public void Do()
		{
			// can not remove when cad feature is not empty
			if( m_CADFeatureParamMap.ParamMap.Count != 0 ) {
				return;
			}

			m_CADFeatureParamMap.MainTubeParam = null;
			EditFinished?.Invoke( EditType.RemoveMainTube, m_szMainTubeName );
		}

		public void Undo()
		{
			m_CADFeatureParamMap.MainTubeParam = m_OldMainTubeParam as CADft_MainTubeParam;
			EditFinished?.Invoke( EditType.AddMainTube, m_szMainTubeName );
		}

		public event CADEditFinishEventHandler EditFinished;

		string m_szMainTubeName;
		ICADFeatureParam m_OldMainTubeParam;
		CADFeatureParamMap m_CADFeatureParamMap;
	}
}
