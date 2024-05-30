namespace MyCADCore
{
	public interface ICADFeatureParam
	{
		bool IsValid();

		CADFeatureType Type
		{
			get;
		}
	}
}
