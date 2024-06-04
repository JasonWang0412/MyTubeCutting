using MyCADCore;
using Utility;


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

		public void Do()
		{
			// data protection
			if( string.IsNullOrEmpty( m_szMainTubeName ) || m_NewMainTubeParam == null || m_CADFeatureParamMap == null ) {
				return;
			}

			// backup the old value
			m_OldMainTubeParam = m_CADFeatureParamMap.MainTubeParam;

			// clone and update the main tube
			m_CADFeatureParamMap.MainTubeParam = CloneHelper.Clone( m_NewMainTubeParam ) as CADft_MainTubeParam;

			// invoke the event
			CommandFinished?.Invoke( EditType.ModifyMainTube, m_szMainTubeName );
		}

		public void Undo()
		{
			// restore the old value
			m_CADFeatureParamMap.MainTubeParam = m_OldMainTubeParam as CADft_MainTubeParam;

			// invoke the event
			CommandFinished?.Invoke( EditType.ModifyMainTube, m_szMainTubeName );
		}

		public event CADEditFinishEventHandler CommandFinished;

		string m_szMainTubeName;
		ICADFeatureParam m_OldMainTubeParam;
		ICADFeatureParam m_NewMainTubeParam;
		CADFeatureParamMap m_CADFeatureParamMap;
	}
}
