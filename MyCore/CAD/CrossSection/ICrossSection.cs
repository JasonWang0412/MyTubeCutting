namespace MyCore.CAD
{
	public interface ICrossSection
	{
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
