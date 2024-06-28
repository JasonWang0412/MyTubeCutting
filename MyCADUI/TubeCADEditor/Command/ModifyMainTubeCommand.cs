using MyCADCore;
using MyUtility;


namespace MyCADUI
{
	internal class ModifyMainTubeCommand : ICADEditCommand
	{
		public ModifyMainTubeCommand( string szMainTubeID, ICADFeatureParam newMainTubeParam, CADFeatureParamMap paramMap )
		{
			m_szMainTubeID = szMainTubeID;
			m_NewMainTubeParam = newMainTubeParam;
			m_CADFeatureParamMap = paramMap;
		}

		public void Do()
		{
			// data protection
			if( string.IsNullOrEmpty( m_szMainTubeID ) || m_NewMainTubeParam == null || m_CADFeatureParamMap == null ) {
				return;
			}

			// backup the old value
			m_OldMainTubeParam = m_CADFeatureParamMap.MainTubeParam;

			// clone and update the main tube
			m_CADFeatureParamMap.MainTubeParam = CloneHelper.Clone( m_NewMainTubeParam ) as CADft_MainTubeParam;

			// invoke the event
			CommandFinished?.Invoke( EditType.ModifyMainTube, m_szMainTubeID );
		}

		public void Undo()
		{
			// restore the old value
			m_CADFeatureParamMap.MainTubeParam = m_OldMainTubeParam as CADft_MainTubeParam;

			// invoke the event
			CommandFinished?.Invoke( EditType.ModifyMainTube, m_szMainTubeID );
		}

		public event CADEditFinishEventHandler CommandFinished;

		string m_szMainTubeID;
		ICADFeatureParam m_OldMainTubeParam;
		ICADFeatureParam m_NewMainTubeParam;
		CADFeatureParamMap m_CADFeatureParamMap;
	}
}
