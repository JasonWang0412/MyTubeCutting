namespace MyCore.CAD
{
	public class CS_Circle : ICrossSection
	{
		public CS_Circle( BG_Circle basicGeom )
		{
			BasicGeom = basicGeom;
		}

		public CS_Type Type
		{
			get
			{
				return CS_Type.Circle;
			}
		}

		public IBasicGeom BasicGeom
		{
			get;
		}

		public double Thickness
		{
			get;
			set;
		}
	}
}
