﻿using MyUIDisplayModel;
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
	public class BN_OneSideFillet : IBendingNotchShape
	{
		public BN_OneSideFillet( bool isOverCut, NotchSide side )
		{
			IsOverCut = isOverCut;
			Side = side;
		}

		[Browsable( false )]
		public BendingNotch_Type Type
		{
			get
			{
				return BendingNotch_Type.OneSideFillet;
			}
		}

		[MyDisplayName( "BN_OneSideFillet", "IsOverCut" )]
		public bool IsOverCut
		{
			get; set;
		}

		[MyDisplayName( "BN_OneSideFillet", "Side" )]
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