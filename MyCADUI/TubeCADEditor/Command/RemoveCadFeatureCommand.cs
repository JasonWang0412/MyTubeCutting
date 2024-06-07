using MyCADCore;
using Utility;

namespace MyCADUI
{
	internal class RemoveCadFeatureCommand : ICADEditCommand
	{
		public RemoveCadFeatureCommand( string szFeatureName, CADFeatureParamMap paramMap )
		{
			m_szFeatureName = szFeatureName;
			m_CADFeatureParamMap = paramMap;
		}

		public void Do()
		{
			// data protection
			if( string.IsNullOrEmpty( m_szFeatureName )
				|| m_CADFeatureParamMap == null || m_CADFeatureParamMap.FeatureMap == null ) {
				return;
			}

			// backup old value
			// clone whole map here to reserve the order
			m_BackupMap = CloneHelper.Clone( m_CADFeatureParamMap );

			// remove the feature
			m_CADFeatureParamMap.FeatureMap.Remove( m_szFeatureName );

			// invoke the event
			CommandFinished?.Invoke( EditType.RemoveCADFeature, m_szFeatureName );
		}

		public void Undo()
		{
			// restore the feature
			m_CADFeatureParamMap.FeatureMap.Clear();
			foreach( var pair in m_BackupMap.FeatureMap ) {
				m_CADFeatureParamMap.FeatureMap[ pair.Key ] = pair.Value;
			}

			// invoke the event
			CommandFinished?.Invoke( EditType.AddCADFeature, m_szFeatureName );
		}

		public event CADEditFinishEventHandler CommandFinished;

		string m_szFeatureName;
		CADFeatureParamMap m_CADFeatureParamMap;
		CADFeatureParamMap m_BackupMap;
	}
}
