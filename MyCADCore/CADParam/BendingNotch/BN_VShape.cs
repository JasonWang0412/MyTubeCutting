using System;
using System.ComponentModel;

namespace MyCADCore
{
	[Serializable]
	public class BN_VShape : IBendingNotchShape
	{
		public BN_VShape( double bendingAngle_deg, double jointGapLength )
		{
			BendingAngle_deg = bendingAngle_deg;
			JointGapLength = jointGapLength;
		}

		[Browsable( false )]
		public BendingNotch_Type Type
		{
			get
			{
				return BendingNotch_Type.VShape;
			}
		}

		public double BendingAngle_deg
		{
			get; set;
		}

		public double JointGapLength
		{
			get; set;
		}

		public bool IsValid()
		{
			// angle should be greater than 0 and less than 180 degrees
			if( BendingAngle_deg <= 0 || BendingAngle_deg >= 180 ) {
				return false;
			}

			return true;
		}
	}
}
