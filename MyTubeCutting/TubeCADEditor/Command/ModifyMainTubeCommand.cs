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

			// send whole param map to yield the pointer of the main tube param
			m_CADFeatureParamMap = paramMap;
		}

		public void Do()
		{
			// check validility
			if( m_NewMainTubeParam.IsValid() == false ) {
				return;
			}

			// data protection
			if( m_CADFeatureParamMap.MainTubeParam == null ) {
				return;
			}

			// backup the old value
			m_OldMainTubeParam = m_CADFeatureParamMap.MainTubeParam;

			// clone and update the main tube
			m_CADFeatureParamMap.MainTubeParam = CloneHelper.Clone( m_NewMainTubeParam ) as CADft_MainTubeParam;

			// invoke the event
			EditFinished?.Invoke( EditType.ModifyMainTube, m_szMainTubeName );
		}

		public void Undo()
		{
			// data protection
			if( m_CADFeatureParamMap.MainTubeParam == null ) {
				return;
			}

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
