using MyCore.CAD;
using System.Collections.Generic;

namespace MyTubeCutting
{
	internal class CADFeatureParamMap
	{
		internal CADft_MainTubeParam MainTubeParam
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

		internal Dictionary<string, ICADFeatureParam> ParamMap
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
