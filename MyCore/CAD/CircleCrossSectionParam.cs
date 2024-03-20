namespace MyCore.CAD
{
	public class CircleCrossSectionParam : ICrossSectionParam
	{
		public double Radius
		{
			get; set;
		}

		public double Thickness
		{
			get; set;
		}

		public double Perimeter
		{
			get
			{
				return default;
			}
		}
	}
}
