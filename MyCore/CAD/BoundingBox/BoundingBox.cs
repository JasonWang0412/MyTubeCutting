namespace MyCore.CAD
{
	public class BoundingBox
	{
		double MinX
		{
			get;
			set;
		}

		double MaxX
		{
			get;
			set;
		}

		double MinY
		{
			get;
			set;
		}

		double MaxY
		{
			get;
			set;
		}

		double MinZ
		{
			get;
			set;
		}

		double MaxZ
		{
			get;
			set;
		}

		bool IsValid()
		{
			return MinX < MaxX && MinY < MaxY && MinZ < MaxZ;
		}
	}
}
