using MyCADCore;
using MyUtility.General;

namespace MyCADUI
{
	internal class ModifyCadFeatureCommand : ICADEditCommand
	{
		public ModifyCadFeatureCommand( string szFeatureID, ICADFeatureParam newCADFeatureParam, CADFeatureParamMap paramMap )
		{
			m_szFeatureID = szFeatureID;
			m_NewCADFeatureParam = newCADFeatureParam;
			m_CADFeatureParamMap = paramMap;
		}

		public void Do()
		{
			// data protection
			if( string.IsNullOrEmpty( m_szFeatureID )
				|| m_NewCADFeatureParam == null
				|| m_CADFeatureParamMap == null || m_CADFeatureParamMap.FeatureMap == null ) {
				return;
			}

			ICADFeatureParam cloneNewParam = CloneHelper.Clone( m_NewCADFeatureParam );

			// backup the old value
			if( m_CADFeatureParamMap.FeatureMap.ContainsKey( m_szFeatureID ) == false
				|| m_CADFeatureParamMap.FeatureMap[ m_szFeatureID ] == null ) {

				// case when old value is not exist, should not go in here
				m_oldCADFeatureParam = cloneNewParam;
			}
			else {
				m_oldCADFeatureParam = m_CADFeatureParamMap.FeatureMap[ m_szFeatureID ];
			}

			// clone and update the new value
			m_CADFeatureParamMap.FeatureMap[ m_szFeatureID ] = cloneNewParam;

			// invoke the event
			CommandFinished?.Invoke( EditType.ModifyCADFeature, m_szFeatureID );
		}

		public void Undo()
		{
			// restore the old value
			m_CADFeatureParamMap.FeatureMap[ m_szFeatureID ] = m_oldCADFeatureParam;

			// invoke the event
			CommandFinished?.Invoke( EditType.ModifyCADFeature, m_szFeatureID );
		}

		public event CADEditFinishEventHandler CommandFinished;

		string m_szFeatureID;
		ICADFeatureParam m_NewCADFeatureParam;
		ICADFeatureParam m_oldCADFeatureParam;
		CADFeatureParamMap m_CADFeatureParamMap;
	}
}
