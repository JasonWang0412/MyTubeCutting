using MyCore.CAD;
using MyCore.Tool;

namespace MyCADUI
{
	internal class ModifyCadFeatureCommand : ICADEditCommand
	{
		public ModifyCadFeatureCommand( string szFeatureName, ICADFeatureParam newCADFeatureParam, CADFeatureParamMap paramMap )
		{
			m_szFeatureName = szFeatureName;
			m_NewCADFeatureParam = newCADFeatureParam;
			m_CADFeatureParamMap = paramMap;
		}

		public CommandErrorCode Do()
		{
			// data protection
			if( m_CADFeatureParamMap == null || m_CADFeatureParamMap.FeatureMap == null ) {
				return CommandErrorCode.InvalidMap;
			}
			if( m_NewCADFeatureParam == null || m_NewCADFeatureParam.IsValid() == false ) {
				return CommandErrorCode.InvalidParam;
			}

			ICADFeatureParam cloneNewParam = CloneHelper.Clone( m_NewCADFeatureParam );

			// backup the old value
			if( m_CADFeatureParamMap.FeatureMap.ContainsKey( m_szFeatureName ) == false
				|| m_CADFeatureParamMap.FeatureMap[ m_szFeatureName ] == null ) {

				// case when old value is not exist, should not go in here
				m_oldCADFeatureParam = cloneNewParam;
			}
			else {
				m_oldCADFeatureParam = m_CADFeatureParamMap.FeatureMap[ m_szFeatureName ];
			}

			// clone and update the new value
			m_CADFeatureParamMap.FeatureMap[ m_szFeatureName ] = cloneNewParam;

			// invoke the event
			EditFinished?.Invoke( EditType.ModifyCADFeature, m_szFeatureName );
			return CommandErrorCode.OK;
		}

		public void Undo()
		{
			// restore the old value
			m_CADFeatureParamMap.FeatureMap[ m_szFeatureName ] = m_oldCADFeatureParam;

			// invoke the event
			EditFinished?.Invoke( EditType.ModifyCADFeature, m_szFeatureName );
		}

		public event CADEditFinishEventHandler EditFinished;

		string m_szFeatureName;
		ICADFeatureParam m_NewCADFeatureParam;
		ICADFeatureParam m_oldCADFeatureParam;
		CADFeatureParamMap m_CADFeatureParamMap;
	}
}
