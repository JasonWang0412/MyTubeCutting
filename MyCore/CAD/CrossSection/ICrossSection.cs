namespace MyCore.CAD
{
	public interface ICrossSection
	{
		CS_Type Type
		{
			get;
		}

		IBasicGeom BasicGeom
		{
			get;
		}

		double Thickness
		{
			get; set;
		}
	}
}
