using MyCore.CAD;
using System.Collections.Generic;

namespace MyTubeCutting
{
	internal class RemoveCadFeatureCommand : ICADEditCommand
	{
		public RemoveCadFeatureCommand( string szFeatureName, Dictionary<string, ICADFeatureParam> editCADFeatureNameParamMap )
		{
			m_szFeatureName = szFeatureName;
			m_OldCADFeatureParam = editCADFeatureNameParamMap[ szFeatureName ];
			m_CADFeatureNameParamMap = editCADFeatureNameParamMap;
		}

		public void Do()
		{
			if( m_CADFeatureNameParamMap.ContainsKey( m_szFeatureName ) == false ) {
				return;
			}
			m_CADFeatureNameParamMap.Remove( m_szFeatureName );
			EditFinished?.Invoke( EditType.RemoveCADFeature, m_szFeatureName );
		}

		public void Undo()
		{
			m_CADFeatureNameParamMap.Add( m_szFeatureName, m_OldCADFeatureParam );
			EditFinished?.Invoke( EditType.AddCADFeature, m_szFeatureName );
		}

		public event CADEditFinishEventHandler EditFinished;

		string m_szFeatureName;
		ICADFeatureParam m_OldCADFeatureParam;
		Dictionary<string, ICADFeatureParam> m_CADFeatureNameParamMap;
	}
}
