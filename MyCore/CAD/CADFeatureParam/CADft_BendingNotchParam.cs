using System;

namespace MyCore.CAD
{
	[Serializable]
	public class CADft_BendingNotchParam : ICADFeatureParam
	{
		public CADFeatureType Type
		{
			get
			{
				return CADFeatureType.BendingNotch;
			}
		}

		public bool IsValid()
		{
			return true;
		}
	}
}
