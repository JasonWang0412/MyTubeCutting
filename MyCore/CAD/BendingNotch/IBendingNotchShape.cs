namespace MyCore.CAD
{
	public interface IBendingNotchShape
	{
		BendingNotchShape_Type Type
		{
			get;
		}

		bool IsValid();
	}
}
