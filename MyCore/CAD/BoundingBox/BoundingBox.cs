namespace MyCore.CAD
{
	public enum BoundaryType
	{
		MinX,
		MaxX,
		MinY,
		MaxY,
		MinZ,
		MaxZ,
	}

	public class BoundingBox
	{
		public BoundingBox( double minX, double maxX, double minY, double maxY, double minZ, double maxZ )
		{
			MinX = minX;
			MaxX = maxX;
			MinY = minY;
			MaxY = maxY;
			MinZ = minZ;
			MaxZ = maxZ;
		}

		public double MinX
		{
			get;
			set;
		}

		public double MaxX
		{
			get;
			set;
		}

		public double MinY
		{
			get;
			set;
		}

		public double MaxY
		{
			get;
			set;
		}

		public double MinZ
		{
			get;
			set;
		}

		public double MaxZ
		{
			get;
			set;
		}

		public bool IsValid()
		{
			return MinX < MaxX && MinY < MaxY && MinZ < MaxZ;
		}
	}
}
