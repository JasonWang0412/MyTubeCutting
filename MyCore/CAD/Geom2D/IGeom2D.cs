namespace MyCore.CAD
{
	public interface IGeom2D
	{
		Geom2D_Type Type
		{
			get;
		}

		bool IsValid();
	}
}
