namespace MyCore.CAD.BendingNotch
{
	internal class BN_BothSideFillet : IBendingNotchShape
	{
		public BendingNotchShape_Type Type
		{
			get
			{
				return BendingNotchShape_Type.BothSideFillet;
			}
		}

		public bool IsValid()
		{
			return true;
		}
	}
}
