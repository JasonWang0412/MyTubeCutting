using MyUIDisplayModel;
using System;
using System.ComponentModel;

namespace MyCADCore
{
	[Serializable]
	public class Geom2D_Circle : IGeom2D
	{
		public Geom2D_Circle( double radius )
		{
			Radius = radius;
		}

		[Browsable( false )]
		public Geom2D_Type Type
		{
			get
			{
				return Geom2D_Type.Circle;
			}
		}

		[MyDisplayName( "Geom2D_Circle", "Radius" )]
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

			return true;
		}
	}
}
