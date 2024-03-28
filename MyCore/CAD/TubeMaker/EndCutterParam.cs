namespace MyCore.CAD
{
	public enum EEndSide
	{
		Left = 0,
		Right = 1
	}

	public class EndCutterParam
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

		public EEndSide Side
		{
			get;
			set;
		}
	}
}
