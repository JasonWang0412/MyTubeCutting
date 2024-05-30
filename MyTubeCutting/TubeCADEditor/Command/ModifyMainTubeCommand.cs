using MyCore.CAD;
using MyCore.Tool;


namespace MyCADUI
{
	internal class ModifyMainTubeCommand : ICADEditCommand
	{
		public ModifyMainTubeCommand( string szMainTubeName, ICADFeatureParam newMainTubeParam, CADFeatureParamMap paramMap )
		{
			m_szMainTubeName = szMainTubeName;
			m_NewMainTubeParam = newMainTubeParam;
			m_CADFeatureParamMap = paramMap;
		}

		public CommandErrorCode Do()
		{
			// data protection
			if( m_CADFeatureParamMap == null ) {
				return CommandErrorCode.InvalidMap;
			}
			if( m_NewMainTubeParam == null || m_NewMainTubeParam.IsValid() == false ) {
				return CommandErrorCode.InvalidParam;
			}

			// backup the old value
			m_OldMainTubeParam = m_CADFeatureParamMap.MainTubeParam;

			// clone and update the main tube
			m_CADFeatureParamMap.MainTubeParam = CloneHelper.Clone( m_NewMainTubeParam ) as CADft_MainTubeParam;

			// invoke the event
			EditFinished?.Invoke( EditType.ModifyMainTube, m_szMainTubeName );
			return CommandErrorCode.OK;
		}

		public void Undo()
		{
			// restore the old value
			m_CADFeatureParamMap.MainTubeParam = m_OldMainTubeParam as CADft_MainTubeParam;

			// invoke the event
			EditFinished?.Invoke( EditType.ModifyMainTube, m_szMainTubeName );
		}

		public event CADEditFinishEventHandler EditFinished;

		string m_szMainTubeName;
		ICADFeatureParam m_OldMainTubeParam;
		ICADFeatureParam m_NewMainTubeParam;
		CADFeatureParamMap m_CADFeatureParamMap;
	}
}
