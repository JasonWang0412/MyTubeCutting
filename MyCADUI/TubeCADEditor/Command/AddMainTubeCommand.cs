using MyCore.CAD;

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

		public CommandErrorCode Do()
		{
			// data protection
			if( m_CADFeatureParamMap == null ) {
				return CommandErrorCode.InvalidMap;
			}
			if( m_MainTubeParam == null || m_MainTubeParam.IsValid() == false ) {
				return CommandErrorCode.InvalidParam;
			}

			// add the new main tube
			m_CADFeatureParamMap.MainTubeParam = m_MainTubeParam as CADft_MainTubeParam;

			// invoke the event
			EditFinished?.Invoke( EditType.AddMainTube, m_szMainTubeName );
			return CommandErrorCode.OK;
		}

		public void Undo()
		{
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
