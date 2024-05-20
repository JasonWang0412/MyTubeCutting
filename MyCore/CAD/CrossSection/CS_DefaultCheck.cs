using System;

namespace MyCore.CAD
{
	[Serializable]
	internal class CS_DefaultCheck : ICrossSectionCheck
	{
		public bool IsValid()
		{
			return false;
		}
	}
}
