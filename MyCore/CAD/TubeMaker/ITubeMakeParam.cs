namespace MyCore.CAD
{
	public interface ITubeMakeParam
	{
		bool IsValid();

		TubeMakeParamType Type
		{
			get;
		}
	}
}
