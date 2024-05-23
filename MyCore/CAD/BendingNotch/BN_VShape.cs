namespace MyCore.CAD
{
	public class BN_VShape : IBendingNotchShape
	{
		public BendingNotchShape_Type Type
		{
			get
			{
				return BendingNotchShape_Type.VShape;
			}
		}

		public bool IsValid()
		{
			return true;
		}
	}
}
