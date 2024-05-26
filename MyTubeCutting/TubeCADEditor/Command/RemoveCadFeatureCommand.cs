using MyCore.CAD;

namespace MyTubeCutting
{
	internal class RemoveCadFeatureCommand : ICADEditCommand
	{
		public RemoveCadFeatureCommand( string szFeatureName, CADFeatureParamMap paramMap )
		{
			m_szFeatureName = szFeatureName;
			m_BackupCADFeatureParam = paramMap.FeatureMap[ szFeatureName ];
			m_CADFeatureParamMap = paramMap;
		}

		public void Do()
		{
			// data protection
			if( m_CADFeatureParamMap.FeatureMap.ContainsKey( m_szFeatureName ) == false ) {
				return;
			}

			// remove the feature
			m_CADFeatureParamMap.FeatureMap.Remove( m_szFeatureName );

			// invoke the event
			EditFinished?.Invoke( EditType.RemoveCADFeature, m_szFeatureName );
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
