using System;

namespace MyCADCore
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
