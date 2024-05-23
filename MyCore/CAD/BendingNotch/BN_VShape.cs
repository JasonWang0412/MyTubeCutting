using System;
using System.ComponentModel;

namespace MyCore.CAD
{
	[Serializable]
	public class BN_VShape : IBendingNotchShape
	{
		public BN_VShape( double bendingAngle_deg, bool isOverCut, double jointGapLength )
		{
			BendingAngle_deg = bendingAngle_deg;
			IsOverCut = isOverCut;
			JointGapLength = jointGapLength;
		}

		[Browsable( false )]
		public BendingNotchShape_Type Type
		{
			get
			{
				return BendingNotchShape_Type.VShape;
			}
		}

		public double BendingAngle_deg
		{
			get; set;
		}

		public bool IsOverCut
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

			// joint gap length should not be negative
			if( JointGapLength < 0 ) {
				return false;
			}

			return true;
		}
	}
}
