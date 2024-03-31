using System;

namespace MyCore.CAD
{
	public enum EEndSide
	{
		Left = 0,
		Right = 1
	}

	[Serializable]
	public class EndCutterParam : ITubeMakeParam
	{
		public EndCutterParam()
		{
			Center_X = 0;
			Center_Y = 0;
			Center_Z = 0;
			Dir_X = 0;
			Dir_Y = -1;
			Dir_Z = 0;
			Side = EEndSide.Left;
		}

		public double Center_X
		{
			get;
			set;
		}

		public double Center_Y
		{
			get;
			set;
		}

		public double Center_Z
		{
			get;
			set;
		}

		public double Dir_X
		{
			get;
			set;
		}

		public double Dir_Y
		{
			get;
			set;
		}

		public double Dir_Z
		{
			get;
			set;
		}

		public EEndSide Side
		{
			get;
			set;
		}

		public bool IsValid()
		{
			// end cutter direction should not be zero
			if( Dir_X == 0 && Dir_Y == 0 && Dir_Z == 0 ) {
				return false;
			}

			return true;
		}
	}
}
