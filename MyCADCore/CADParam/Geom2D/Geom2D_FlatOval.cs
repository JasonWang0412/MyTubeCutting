using MyLanguageManager;
using System;
using System.ComponentModel;

namespace MyCADCore
{
	[Serializable]
	public class Geom2D_FlatOval : IGeom2D
	{
		public Geom2D_FlatOval( double width, double radius )
		{
			Width = width;
			Radius = radius;
		}

		[Browsable( false )]
		public Geom2D_Type Type
		{
			get
			{
				return Geom2D_Type.FlatOval;
			}
		}

		[MyDisplayName( "Geom2D_FlatOval", "Width" )]
		public double Width
		{
			get; set;
		}

		[MyDisplayName( "Geom2D_FlatOval", "Radius" )]
		public double Radius
		{
			get; set;
		}

		public bool IsValid()
		{
			// radius must be positive
			if( Radius <= 0 ) {
				return false;
			}

			// width must be greater than double radius
			if( Width <= 2 * Radius ) {
				return false;
			}

			return true;
		}
	}
}
