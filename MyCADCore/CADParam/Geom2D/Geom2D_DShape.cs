using MyUIDisplayModel;
using System;
using System.ComponentModel;

namespace MyCADCore
{
	[Serializable]
	public class Geom2D_DShape : IGeom2D
	{
		public Geom2D_DShape( double width, double height, double filletRadius )
		{
			Width = width;
			Height = height;
			FilletRadius = filletRadius;
		}

		[Browsable( false )]
		public Geom2D_Type Type
		{
			get
			{
				return Geom2D_Type.DShape;
			}
		}

		[MyDisplayName( "Geom2D_DShape", "Width" )]
		public double Width
		{
			get; set;
		}

		[MyDisplayName( "Geom2D_DShape", "Height" )]
		public double Height
		{
			get; set;
		}

		[MyDisplayName( "Geom2D_DShape", "FilletRadius" )]
		public double FilletRadius
		{
			get; set;
		}

		public bool IsValid()
		{
			// width radius must be positive
			if( Width <= 0 || FilletRadius <= 0 ) {
				return false;
			}

			// fillet radius should be less than width/2
			if( FilletRadius >= Width / 2 ) {
				return false;
			}

			// height should be greater than fillet radius + width/2
			if( Height <= FilletRadius + Width / 2 ) {
				return false;
			}

			return true;
		}
	}
}
