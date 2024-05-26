using MyCore.CAD;

namespace MyTubeCutting
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
			// check validility
			if( m_CADFeatureParam.IsValid() == false ) {
				return;
			}

			// add the new feature
			m_CADFeatureParamMap.FeatureMap.Add( m_szFeatureName, m_CADFeatureParam );

			// invoke the event
			EditFinished?.Invoke( EditType.AddCADFeature, m_szFeatureName );
		}

		public void Undo()
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

		public event CADEditFinishEventHandler EditFinished;

		string m_szFeatureName;
		ICADFeatureParam m_CADFeatureParam;
		CADFeatureParamMap m_CADFeatureParamMap;
	}
}
