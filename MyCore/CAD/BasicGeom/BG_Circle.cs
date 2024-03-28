namespace MyCore.CAD
{
	public class BG_Circle : IBasicGeom
	{
		public BG_Type Type
		{
			get
			{
				return BG_Type.Circle;
			}
		}

		public double Radius
		{
			get; set;
		}
	}
}
