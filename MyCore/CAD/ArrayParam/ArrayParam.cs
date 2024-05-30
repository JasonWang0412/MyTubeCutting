using System;

namespace MyCADCore
{
	public enum ArrayDirection
	{
		Positive = 0,
		Negative = 1,
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

		public int LinearCount
		{
			get;
			set;
		}

		public double LinearDistance
		{
			get;
			set;
		}

		public ArrayDirection LinearDirection
		{
			get;
			set;
		}

		public int AngularCount
		{
			get;
			set;
		}

		public double AngularDistance_Deg
		{
			get;
			set;
		}

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
