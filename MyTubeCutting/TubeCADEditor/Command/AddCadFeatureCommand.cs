using MyCore.CAD;
using System.Collections.Generic;

namespace MyTubeCutting
{
	internal class AddCadFeatureCommand : ICADEditCommand
	{
		public AddCadFeatureCommand( string szFeatureName, ICADFeatureParam cadFeatureParam,
			Dictionary<string, ICADFeatureParam> editCADFeatureNameParamMap )
		{
			m_szFeatureName = szFeatureName;
			m_CADFeatureParam = cadFeatureParam;
			m_CADFeatureNameParamMap = editCADFeatureNameParamMap;
		}

		public void Do()
		{
			// check validility
			if( m_CADFeatureParam.IsValid() == false ) {
				return;
			}

			// add the new feature
			m_CADFeatureNameParamMap.Add( m_szFeatureName, m_CADFeatureParam );

			// invoke the event
			EditFinished?.Invoke( EditType.AddCADFeature, m_szFeatureName );
		}

		public void Undo()
		{
			// data protection
			if( m_CADFeatureNameParamMap.ContainsKey( m_szFeatureName ) == false ) {
				return;
			}

			// remove the feature
			m_CADFeatureNameParamMap.Remove( m_szFeatureName );

			// invoke the event
			EditFinished?.Invoke( EditType.RemoveCADFeature, m_szFeatureName );
		}

		public event CADEditFinishEventHandler EditFinished;

		string m_szFeatureName;
		ICADFeatureParam m_CADFeatureParam;
		Dictionary<string, ICADFeatureParam> m_CADFeatureNameParamMap;
	}
}
