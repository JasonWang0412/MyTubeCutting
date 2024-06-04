using MyCADCore;

namespace MyCADUI
{
	internal class AddCadFeatureCommand : ICADEditCommand
	{
		public AddCadFeatureCommand( string szFeatureName, ICADFeatureParam cadFeatureParam, CADFeatureParamMap paramMap )
		{
			m_szFeatureName = szFeatureName;
			m_CADFeatureParam = cadFeatureParam;
			m_CADFeatureParamMap = paramMap;
		}

		public void Do()
		{
			// data protection
			if( string.IsNullOrEmpty( m_szFeatureName )
				|| m_CADFeatureParam == null
				|| m_CADFeatureParamMap == null || m_CADFeatureParamMap.FeatureMap == null ) {
				return;
			}

			// add the new feature
			m_CADFeatureParamMap.FeatureMap.Add( m_szFeatureName, m_CADFeatureParam );

			// invoke the event
			CommandFinished?.Invoke( EditType.AddCADFeature, m_szFeatureName );
		}

		public void Undo()
		{
			// remove the feature
			m_CADFeatureParamMap.FeatureMap.Remove( m_szFeatureName );

			// invoke the event
			CommandFinished?.Invoke( EditType.RemoveCADFeature, m_szFeatureName );
		}

		public event CADEditFinishEventHandler CommandFinished;

		string m_szFeatureName;
		ICADFeatureParam m_CADFeatureParam;
		CADFeatureParamMap m_CADFeatureParamMap;
	}
}
