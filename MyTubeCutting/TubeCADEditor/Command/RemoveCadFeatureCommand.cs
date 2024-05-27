using MyCore.CAD;

namespace MyTubeCutting
{
	internal class RemoveCadFeatureCommand : ICADEditCommand
	{
		public RemoveCadFeatureCommand( string szFeatureName, CADFeatureParamMap paramMap )
		{
			m_szFeatureName = szFeatureName;
			m_CADFeatureParamMap = paramMap;
		}

		public CommandErrorCode Do()
		{
			// data protection
			if( m_CADFeatureParamMap == null || m_CADFeatureParamMap.FeatureMap == null ) {
				return CommandErrorCode.InvalidMap;
			}

			// backup old value
			if( m_CADFeatureParamMap.FeatureMap.ContainsKey( m_szFeatureName ) == false
				|| m_CADFeatureParamMap.FeatureMap[ m_szFeatureName ] == null ) {

				// case when old value is not exist, should not go in here
				m_BackupCADFeatureParam = null;

			}
			else {
				m_BackupCADFeatureParam = m_CADFeatureParamMap.FeatureMap[ m_szFeatureName ];
			}
			m_BackupCADFeatureParam = m_CADFeatureParamMap.FeatureMap[ m_szFeatureName ];

			// remove the feature
			m_CADFeatureParamMap.FeatureMap.Remove( m_szFeatureName );

			// invoke the event
			EditFinished?.Invoke( EditType.RemoveCADFeature, m_szFeatureName );
			return CommandErrorCode.OK;
		}

		public void Undo()
		{
			// add the feature back
			m_CADFeatureParamMap.FeatureMap.Add( m_szFeatureName, m_BackupCADFeatureParam );

			// invoke the event
			EditFinished?.Invoke( EditType.AddCADFeature, m_szFeatureName );
		}

		public event CADEditFinishEventHandler EditFinished;

		string m_szFeatureName;
		ICADFeatureParam m_BackupCADFeatureParam;
		CADFeatureParamMap m_CADFeatureParamMap;
	}
}
