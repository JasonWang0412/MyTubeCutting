namespace MyCore.CAD
{
	public interface ICrossSection
	{
		CS_Type Type
		{
			get;
		}

		IGeom2D Shape
		{
			get;
		}

		double Thickness
		{
			get; set;
		}

		bool IsValid();
	}
}
