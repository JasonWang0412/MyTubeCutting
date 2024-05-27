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

		public CommandErrorCode Do()
		{
			// data protection
			if( m_CADFeatureParamMap == null || m_CADFeatureParamMap.FeatureMap == null ) {
				return CommandErrorCode.InvalidMap;
			}
			if( m_CADFeatureParam == null || m_CADFeatureParam.IsValid() == false ) {
				return CommandErrorCode.InvalidParam;
			}

			// add the new feature
			m_CADFeatureParamMap.FeatureMap.Add( m_szFeatureName, m_CADFeatureParam );

			// invoke the event
			EditFinished?.Invoke( EditType.AddCADFeature, m_szFeatureName );
			return CommandErrorCode.OK;
		}

		public void Undo()
		{
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
