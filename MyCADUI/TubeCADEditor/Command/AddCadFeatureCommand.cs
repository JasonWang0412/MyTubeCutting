using MyCADCore;

namespace MyCADUI
{
	internal class AddCadFeatureCommand : ICADEditCommand
	{
		public AddCadFeatureCommand( string szFeatureID, ICADFeatureParam cadFeatureParam, CADFeatureParamMap paramMap )
		{
			m_szFeatureID = szFeatureID;
			m_CADFeatureParam = cadFeatureParam;
			m_CADFeatureParamMap = paramMap;
		}

		public void Do()
		{
			// data protection
			if( string.IsNullOrEmpty( m_szFeatureID )
				|| m_CADFeatureParam == null
				|| m_CADFeatureParamMap == null || m_CADFeatureParamMap.FeatureMap == null ) {
				return;
			}

			// add the new feature
			m_CADFeatureParamMap.FeatureMap.Add( m_szFeatureID, m_CADFeatureParam );

			// invoke the event
			CommandFinished?.Invoke( EditType.AddCADFeature, m_szFeatureID );
		}

		public void Undo()
		{
			// remove the feature
			m_CADFeatureParamMap.FeatureMap.Remove( m_szFeatureID );

			// invoke the event
			CommandFinished?.Invoke( EditType.RemoveCADFeature, m_szFeatureID );
		}

		public event CADEditFinishEventHandler CommandFinished;

		string m_szFeatureID;
		ICADFeatureParam m_CADFeatureParam;
		CADFeatureParamMap m_CADFeatureParamMap;
	}
}
