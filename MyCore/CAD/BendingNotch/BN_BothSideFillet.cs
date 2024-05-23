using System;
using System.ComponentModel;

namespace MyCore.CAD.BendingNotch
{
	[Serializable]
	internal class BN_BothSideFillet : IBendingNotchShape
	{
		public BN_BothSideFillet( double filletRadius, double bendingAngle_deg, bool isOverCut, double jointGapLength )
		{
			FilletRadius = filletRadius;
			BendingAngle_deg = bendingAngle_deg;
			IsOverCut = isOverCut;
			JointGapLength = jointGapLength;
		}

		[Browsable( false )]
		public BendingNotchShape_Type Type
		{
			get
			{
				return BendingNotchShape_Type.BothSideFillet;
			}
		}

		public double FilletRadius
		{
			get; set;
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
			// fillet radius should be greater than 0
			if( FilletRadius <= 0 ) {
				return false;
			}

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
