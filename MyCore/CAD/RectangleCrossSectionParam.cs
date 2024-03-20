namespace MyCore.CAD
{
	internal class RectangleCrossSectionParam : ICrossSectionParam
	{
		public double Width
		{
			get; set;
		}

		public double Height
		{
			get; set;
		}

		public double Fillet
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
