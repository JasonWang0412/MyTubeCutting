namespace MyCore.CAD
{
	public enum BranchIntersectDir
	{
		Positive = 0,
		Negative = 1,
		Both = 2
	}

	public class BranchTubeParam
	{
		public double X
		{
			get;
			set;
		}

		public double Y
		{
			get;
			set;
		}

		public double Z
		{
			get;
			set;
		}

		public double RotateX
		{
			get;
			set;
		}

		public double RotateY
		{
			get;
			set;
		}

		public double RotateZ
		{
			get;
			set;
		}

		public IBasicGeom Shape
		{
			get;
			set;
		}

		public BranchIntersectDir Direction
		{
			get;
			set;
		}
	}
}
