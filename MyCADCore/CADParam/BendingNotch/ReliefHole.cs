using MyUIDisplayModel;
using System;

namespace MyCADCore
{
	[Serializable]
	public class ReliefHole
	{
		public ReliefHole( double width, double height, double fillet )
		{
			Width = width;
			Height = height;
			Fillet = fillet;
		}

		public ReliefHole()
		{
			Width = 10;
			Height = 10;
			Fillet = 2;
		}

		[MyDisplayName( "ReliefHole", "Width" )]
		public double Width
		{
			get; set;
		}

		[MyDisplayName( "ReliefHole", "Height" )]
		public double Height
		{
			get; set;
		}

		[MyDisplayName( "ReliefHole", "Fillet" )]
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

			// fillet should be non-negative and less than min of width/2 and height/2
			if( Fillet < 0 || Fillet >= Width / 2 || Fillet >= Height / 2 ) {
				return false;
			}

			return true;
		}
	}
}
