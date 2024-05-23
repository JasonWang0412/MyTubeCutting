namespace MyCore.CAD
{
	public interface IBendingNotchShape
	{
		BendingNotch_Type Type
		{
			get;
		}

		bool IsValid();
	}
}
