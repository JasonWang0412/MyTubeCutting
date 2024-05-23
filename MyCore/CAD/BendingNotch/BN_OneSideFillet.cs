namespace MyCore.CAD.BendingNotch
{
	internal class BN_OneSideFillet : IBendingNotchShape
	{
		public BendingNotchShape_Type Type
		{
			get
			{
				return BendingNotchShape_Type.OneSideFillet;
			}
		}

		public bool IsValid()
		{
			return true;
		}
	}
}
