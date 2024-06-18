using MyUIDisplayModel;
using System;

namespace MyCADCore
{
	public enum ArrayDirection
	{
		Positive = 0,
		Negative = 1,
		Both = 2
	}

	[Serializable]
	public class ArrayParam
	{
		public ArrayParam()
		{
			LinearCount = 1;
			LinearDistance = 50;
			LinearDirection = ArrayDirection.Positive;
			AngularCount = 1;
			AngularDistance_Deg = 90;
			AngularDirection = ArrayDirection.Positive;
		}

		[MyDisplayName( "ArrayParam", "LinearCount" )]
		public int LinearCount
		{
			get;
			set;
		}

		[MyDisplayName( "ArrayParam", "LinearDistance" )]
		public double LinearDistance
		{
			get;
			set;
		}

		[MyDisplayName( "ArrayParam", "LinearDirection" )]
		public ArrayDirection LinearDirection
		{
			get;
			set;
		}

		[MyDisplayName( "ArrayParam", "AngularCount" )]
		public int AngularCount
		{
			get;
			set;
		}

		[MyDisplayName( "ArrayParam", "AngularDistance" )]
		public double AngularDistance_Deg
		{
			get;
			set;
		}

		[MyDisplayName( "ArrayParam", "AngularDirection" )]
		public ArrayDirection AngularDirection
		{
			get;
			set;
		}

		public bool IsValid()
		{
			if( LinearCount < 1 || LinearDistance <= 0 || AngularCount < 1 || AngularDistance_Deg <= 0 ) {
				return false;
			}
			return true;
		}
	}
}
