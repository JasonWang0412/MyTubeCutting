﻿using MyCore.CAD;
using MyCore.Tool;
using System.Collections.Generic;

namespace MyTubeCutting
{
	internal class ModifyCadFeatureCommand : ICADEditCommand
	{
		public event CADEditFinishEventHandler EditFinished;

		public ModifyCadFeatureCommand( string szFeatureName, ICADFeatureParam newCADFeatureParam,
			Dictionary<string, ICADFeatureParam> editCADFeatureNameParamMap )
		{
			m_szFeatureName = szFeatureName;
			m_newCADFeatureParam = newCADFeatureParam;
			m_CADFeatureNameParamMap = editCADFeatureNameParamMap;
		}

		public void Do()
		{
			if( m_newCADFeatureParam.IsValid() == false ) {
				return;
			}

			if( m_CADFeatureNameParamMap.ContainsKey( m_szFeatureName ) == false ) {
				return;
			}

			// backup the old value
			m_oldCADFeatureParam = m_CADFeatureNameParamMap[ m_szFeatureName ];

			// update the new value
			m_CADFeatureNameParamMap[ m_szFeatureName ] = CloneHelper.Clone( m_newCADFeatureParam );
			EditFinished?.Invoke( EditType.ModifyCADFeature, m_szFeatureName );
		}

		public void Undo()
		{
			if( m_CADFeatureNameParamMap.ContainsKey( m_szFeatureName ) == false ) {
				return;
			}

			// restore the old value
			m_CADFeatureNameParamMap[ m_szFeatureName ] = m_oldCADFeatureParam;
			EditFinished?.Invoke( EditType.ModifyCADFeature, m_szFeatureName );
		}

		string m_szFeatureName;
		ICADFeatureParam m_newCADFeatureParam;
		ICADFeatureParam m_oldCADFeatureParam;
		Dictionary<string, ICADFeatureParam> m_CADFeatureNameParamMap = new Dictionary<string, ICADFeatureParam>();
	}
}
