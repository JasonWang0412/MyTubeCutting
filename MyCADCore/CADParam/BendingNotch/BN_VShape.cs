using MyUIDisplayModel;
using System;
using System.ComponentModel;

namespace MyCADCore
{
	public enum ReliefHoleType
	{
		No,
		Side,
		Buttom,
	}

	[Serializable]
	public class BN_VShape : IBendingNotchShape
	{
		public BN_VShape( double bendingAngle_deg, double jointGapLength )
		{
			BendingAngle_deg = bendingAngle_deg;
			JointGapLength = jointGapLength;
			ReliefHoleType = ReliefHoleType.No;
			ReliefHole = null;
		}

		[Browsable( false )]
		public BendingNotch_Type Type
		{
			get
			{
				return BendingNotch_Type.VShape;
			}
		}

		[MyDisplayName( "BN_VShape", "BendingAngle_deg" )]
		public double BendingAngle_deg
		{
			get; set;
		}

		[MyDisplayName( "BN_VShape", "JointGapLength" )]
		public double JointGapLength
		{
			get; set;
		}

		[MyDisplayName( "BN_VShape", "ReliefHoleType" )]
		public ReliefHoleType ReliefHoleType
		{
			get
			{
				return m_ReliefHoleType;
			}
			set
			{
				m_ReliefHoleType = value;
				if( m_ReliefHoleType == ReliefHoleType.No ) {
					ReliefHole = null;
				}
				else {
					ReliefHole = new ReliefHole();
				}
			}
		}

		[TypeConverter( typeof( MyObjectConverter ) )]
		[MyDisplayName( "BN_VShape", "ReliefHole" )]
		public ReliefHole ReliefHole
		{
			get; set;
		}

		public bool IsValid()
		{
			// angle should be greater than 0 and less than 180 degrees
			if( BendingAngle_deg <= 0 || BendingAngle_deg >= 180 ) {
				return false;
			}

			// relief hole should be valid if it is present
			if( m_ReliefHoleType != ReliefHoleType.No && ( ReliefHole == null || ReliefHole.IsValid() == false ) ) {
				return false;
			}

			return true;
		}

		ReliefHoleType m_ReliefHoleType;
	}
}
