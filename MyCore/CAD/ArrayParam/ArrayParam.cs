using System;

namespace MyCore.CAD
{
	public enum ArrayDirection
	{
		Positive = 0,
		Negative = 1
	}

	[Serializable]
	public class ArrayParam
	{
		public ArrayParam()
		{
			LinearCount = 1;
			LinearDistance = 50;
			AngularCount = 1;
			AngularDistance_Deg = 90;
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

		public bool IsValid()
		{
			if( LinearCount < 1 || LinearDistance <= 0 || AngularCount < 1 || AngularDistance_Deg <= 0 ) {
				return false;
			}
			return true;
		}
	}
}
