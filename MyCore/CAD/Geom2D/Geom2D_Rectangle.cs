using System;

namespace MyCore.CAD
{
	[Serializable]
	public class Geom2D_Rectangle : IGeom2D
	{
		public Geom2D_Rectangle( double dWidth, double dHeight, double dFillet )
		{
			Width = dWidth;
			Height = dHeight;
			Fillet = dFillet;
		}

		public Geom2D_Type Type
		{
			get
			{
				return Geom2D_Type.Rectangle;
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

		public bool IsValid()
		{
			// width and height should be positive
			if( Width <= 0 || Height <= 0 ) {
				return false;
			}

			// fillet should be non-negative
			if( Fillet < 0 ) {
				return false;
			}

			return true;
		}
	}
}
