using MyUIDisplayModel;
using System;
using System.ComponentModel;

namespace MyCADCore
{
	public enum NotchSide
	{
		Left,
		Right,
	}

	[Serializable]
	public class BN_OneSide : IBendingNotchShape
	{
		public BN_OneSide( bool isOverCut, NotchSide side )
		{
			IsOverCut = isOverCut;
			Side = side;
		}

		[Browsable( false )]
		public BendingNotch_Type Type
		{
			get
			{
				return BendingNotch_Type.OneSide;
			}
		}

		[MyDisplayName( "BN_OneSide", "IsOverCut" )]
		public bool IsOverCut
		{
			get; set;
		}

		[MyDisplayName( "BN_OneSide", "Side" )]
		public NotchSide Side
		{
			get; set;
		}

		public bool IsValid()
		{
			return true;
		}
	}
}
