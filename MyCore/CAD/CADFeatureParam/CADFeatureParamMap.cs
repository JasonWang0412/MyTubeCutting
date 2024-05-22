using System.Collections.Generic;

namespace MyCore.CAD
{
	// TODO: this class should belong to UI project
	public class CADFeatureParamMap
	{
		public CADft_MainTubeParam MainTubeParam
		{
			get
			{
				return m_MainTubeParam;
			}
			set
			{
				m_MainTubeParam = value;
			}
		}

		public Dictionary<string, ICADFeatureParam> FeatureMap
		{
			get
			{
				return m_CADFeatureNameParamMap;
			}
		}

		CADft_MainTubeParam m_MainTubeParam;
		Dictionary<string, ICADFeatureParam> m_CADFeatureNameParamMap = new Dictionary<string, ICADFeatureParam>();
	}
}
