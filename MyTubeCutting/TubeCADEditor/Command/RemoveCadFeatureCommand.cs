using MyCore.CAD;
using System.Collections.Generic;

namespace MyTubeCutting
{
	internal class RemoveCadFeatureCommand : ICADEditCommand
	{
		public RemoveCadFeatureCommand( string szFeatureName, Dictionary<string, ICADFeatureParam> editCADFeatureNameParamMap )
		{
			m_szFeatureName = szFeatureName;
			m_BackupCADFeatureParam = editCADFeatureNameParamMap[ szFeatureName ];
			m_CADFeatureNameParamMap = editCADFeatureNameParamMap;
		}

		public void Do()
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

		public void Undo()
		{
			// add the feature back
			m_CADFeatureNameParamMap.Add( m_szFeatureName, m_BackupCADFeatureParam );

			// invoke the event
			EditFinished?.Invoke( EditType.AddCADFeature, m_szFeatureName );
		}

		public event CADEditFinishEventHandler EditFinished;

		string m_szFeatureName;
		ICADFeatureParam m_BackupCADFeatureParam;
		Dictionary<string, ICADFeatureParam> m_CADFeatureNameParamMap;
	}
}
