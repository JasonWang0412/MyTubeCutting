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
			// TODO: to backup the insert index if needed
			if( m_CADFeatureParam.IsValid() == false ) {
				return;
			}
			m_CADFeatureNameParamMap.Add( m_szFeatureName, m_CADFeatureParam );
			EditFinished?.Invoke( EditType.AddCADFeature, m_szFeatureName );
		}

		public void Undo()
		{
			if( m_CADFeatureNameParamMap.ContainsKey( m_szFeatureName ) == false ) {
				return;
			}
			m_CADFeatureNameParamMap.Remove( m_szFeatureName );
			EditFinished?.Invoke( EditType.RemoveCADFeature, m_szFeatureName );
		}

		public event CADEditFinishEventHandler EditFinished;

		string m_szFeatureName;
		ICADFeatureParam m_CADFeatureParam;
		Dictionary<string, ICADFeatureParam> m_CADFeatureNameParamMap = new Dictionary<string, ICADFeatureParam>();
	}
}
