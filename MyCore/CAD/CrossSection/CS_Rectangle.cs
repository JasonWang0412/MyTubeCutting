namespace MyCore.CAD
{
	public class CS_Rectangle : ICrossSection
	{
		public CS_Rectangle( BG_Rectangle basicGeom )
		{
			BasicGeom = basicGeom;
		}

		public CS_Type Type
		{
			get
			{
				return CS_Type.Rectangle;
			}
		}

		public IBasicGeom BasicGeom
		{
			get;
		}

		public double Thickness
		{
			get; set;
		}
	}
}
