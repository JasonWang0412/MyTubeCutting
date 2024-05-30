using MyLanguageManager;
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
			Filletadius = filletRadius;
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

		[MyDisplayName( "Geom2D_DShape", "Filletadius" )]
		public double Filletadius
		{
			get; set;
		}

		public bool IsValid()
		{
			// width radius must be positive
			if( Width <= 0 || Filletadius <= 0 ) {
				return false;
			}

			// height should be greater than fillet radius + width/2
			if( Height <= Filletadius + Width / 2 ) {
				return false;
			}

			return true;
		}
	}
}
