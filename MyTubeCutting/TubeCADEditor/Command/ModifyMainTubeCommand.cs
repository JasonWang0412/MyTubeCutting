using MyCore.CAD;
using MyCore.Tool;


namespace MyTubeCutting
{
	internal class ModifyMainTubeCommand : ICADEditCommand
	{
		public ModifyMainTubeCommand( string szMainTubeName, ICADFeatureParam newMainTubeParam, CADFeatureParamMap paramMap )
		{
			m_szMainTubeName = szMainTubeName;
			m_NewMainTubeParam = newMainTubeParam;
			m_CADFeatureParamMap = paramMap;
		}

		public void Do()
		{
			if( m_NewMainTubeParam.IsValid() == false ) {
				return;
			}

			// backup the old value
			m_OldMainTubeParam = m_CADFeatureParamMap.MainTubeParam;

			// update the new value
			m_CADFeatureParamMap.MainTubeParam = CloneHelper.Clone( m_NewMainTubeParam ) as CADft_MainTubeParam;
			EditFinished?.Invoke( EditType.ModifyMainTube, m_szMainTubeName );
		}

		public void Undo()
		{
			m_CADFeatureParamMap.MainTubeParam = m_OldMainTubeParam as CADft_MainTubeParam;
			EditFinished?.Invoke( EditType.RemoveMainTube, m_szMainTubeName );
		}

		public event CADEditFinishEventHandler EditFinished;

		string m_szMainTubeName;
		ICADFeatureParam m_OldMainTubeParam;
		ICADFeatureParam m_NewMainTubeParam;
		CADFeatureParamMap m_CADFeatureParamMap;
	}
}
