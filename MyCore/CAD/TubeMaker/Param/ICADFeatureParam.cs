namespace MyCore.CAD
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
