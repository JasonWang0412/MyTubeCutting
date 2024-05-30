namespace MyCADCore
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
