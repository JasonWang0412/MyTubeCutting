using MyUIDisplayModel;
using System;
using System.ComponentModel;

namespace MyCADCore
{
	[Serializable]
	public class BN_BothSide : IBendingNotchShape
	{
		public BN_BothSide( double filletRadius, double bendingAngle_deg, bool isOverCut, double jointGapLength )
		{
			BendingRadius = filletRadius;
			BendingAngle_deg = bendingAngle_deg;
			IsOverCut = isOverCut;
			JointGapLength = jointGapLength;
		}

		[Browsable( false )]
		public BendingNotch_Type Type
		{
			get
			{
				return BendingNotch_Type.BothSide;
			}
		}

		[MyDisplayName( "BN_BothSide", "BendingRadius" )]
		public double BendingRadius
		{
			get; set;
		}

		[MyDisplayName( "BN_BothSide", "BendingAngle_deg" )]
		public double BendingAngle_deg
		{
			get; set;
		}

		[MyDisplayName( "BN_BothSide", "IsOverCut" )]
		public bool IsOverCut
		{
			get; set;
		}

		[MyDisplayName( "BN_BothSide", "JointGapLength" )]
		public double JointGapLength
		{
			get; set;
		}

		public bool IsValid()
		{
			// fillet radius should be greater than 0
			if( BendingRadius <= 0 ) {
				return false;
			}

			// angle should be greater than 0 and less than 180 degrees
			if( BendingAngle_deg <= 0 || BendingAngle_deg >= 180 ) {
				return false;
			}

			return true;
		}
	}
}
