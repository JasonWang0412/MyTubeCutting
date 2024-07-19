using MyUtility.MyOCC;
using System;

namespace MyUtility.General
{
	public class MathHelper
	{
		public static bool IsSameValue( double value1, double value2, double errorValue = OCCHelper.ERROR_VALUE )
		{
			return Math.Abs( value1 - value2 ) < errorValue;
		}
	}
}
