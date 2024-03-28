namespace MyCore.CAD
{
	public class BG_Rectangle : IBasicGeom
	{
		public BG_Type Type
		{
			get
			{
				return BG_Type.Rectangle;
			}
		}

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
	}
}
