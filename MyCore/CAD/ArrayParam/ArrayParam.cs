using System;

namespace MyCore.CAD
{
	[Serializable]
	public class ArrayParam
	{
		public ArrayParam()
		{
			LinearCount = 1;
			LinerDistance = 10;
			AngularCount = 1;
			AngularDistance = 30;
		}

		public int LinearCount
		{
			get;
			set;
		}

		public double LinerDistance
		{
			get;
			set;
		}

		public int AngularCount
		{
			get;
			set;
		}

		public double AngularDistance
		{
			get;
			set;
		}

		public bool IsValid()
		{
			if( LinearCount < 1 || LinerDistance <= 0 || AngularCount < 1 || AngularDistance <= 0 ) {
				return false;
			}
			return true;
		}
	}
}
