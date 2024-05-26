using MyCore.CAD;
using MyCore.Tool;

namespace MyTubeCutting
{
	internal class ModifyCadFeatureCommand : ICADEditCommand
	{
		public ModifyCadFeatureCommand( string szFeatureName, ICADFeatureParam newCADFeatureParam, CADFeatureParamMap paramMap )
		{
			m_szFeatureName = szFeatureName;
			m_newCADFeatureParam = newCADFeatureParam;
			m_CADFeatureParamMap = paramMap;
		}

		public void Do()
		{
			// check validility
			if( m_newCADFeatureParam.IsValid() == false ) {
				return;
			}

			// data protection
			if( m_CADFeatureParamMap.FeatureMap.ContainsKey( m_szFeatureName ) == false ) {
				return;
			}

			// backup the old value
			m_oldCADFeatureParam = m_CADFeatureParamMap.FeatureMap[ m_szFeatureName ];

			// clone and update the new value
			m_CADFeatureParamMap.FeatureMap[ m_szFeatureName ] = CloneHelper.Clone( m_newCADFeatureParam );

			// invoke the event
			EditFinished?.Invoke( EditType.ModifyCADFeature, m_szFeatureName );
		}

		public void Undo()
		{
			// data protection
			if( m_CADFeatureParamMap.FeatureMap.ContainsKey( m_szFeatureName ) == false ) {
				return;
			}

			// restore the old value
			m_CADFeatureParamMap.FeatureMap[ m_szFeatureName ] = m_oldCADFeatureParam;

			// invoke the event
			EditFinished?.Invoke( EditType.ModifyCADFeature, m_szFeatureName );
		}

		public event CADEditFinishEventHandler EditFinished;

		string m_szFeatureName;
		ICADFeatureParam m_newCADFeatureParam;
		ICADFeatureParam m_oldCADFeatureParam;
		CADFeatureParamMap m_CADFeatureParamMap;
	}
}
