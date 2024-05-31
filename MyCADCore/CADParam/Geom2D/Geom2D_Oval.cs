using MyUIDisplayModel;
using System;
using System.ComponentModel;

namespace MyCADCore
{
	[Serializable]
	public class Geom2D_Oval : IGeom2D
	{
		public Geom2D_Oval( double width, double height )
		{
			Width = width;
			Height = height;
		}

		[Browsable( false )]
		public Geom2D_Type Type
		{
			get
			{
				return Geom2D_Type.Oval;
			}
		}

		[MyDisplayName( "Geom2D_Oval", "Width" )]
		public double Width
		{
			get; set;
		}

		[MyDisplayName( "Geom2D_Oval", "Height" )]
		public double Height
		{
			get; set;
		}

		public bool IsValid()
		{
			// width and height must be positive
			if( Width <= 0 || Height <= 0 ) {
				return false;
			}

			return true;
		}
	}
}
