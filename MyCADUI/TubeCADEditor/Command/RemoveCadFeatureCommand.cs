using MyCADCore;

namespace MyCADUI
{
	internal class RemoveCadFeatureCommand : ICADEditCommand
	{
		public RemoveCadFeatureCommand( string szFeatureName, CADFeatureParamMap paramMap )
		{
			m_szFeatureName = szFeatureName;
			m_CADFeatureParamMap = paramMap;
		}

		public void Do()
		{
			// data protection
			if( string.IsNullOrEmpty( m_szFeatureName )
				|| m_CADFeatureParamMap == null || m_CADFeatureParamMap.FeatureMap == null ) {
				return;
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
			CommandFinished?.Invoke( EditType.RemoveCADFeature, m_szFeatureName );
		}

		public void Undo()
		{
			// add the feature back
			m_CADFeatureParamMap.FeatureMap.Add( m_szFeatureName, m_BackupCADFeatureParam );

			// invoke the event
			CommandFinished?.Invoke( EditType.AddCADFeature, m_szFeatureName );
		}

		public event CADEditFinishEventHandler CommandFinished;

		string m_szFeatureName;
		ICADFeatureParam m_BackupCADFeatureParam;
		CADFeatureParamMap m_CADFeatureParamMap;
	}
}
