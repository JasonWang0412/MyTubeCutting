using System;
using System.ComponentModel;

namespace MyCore.CAD.BendingNotch
{
	[Serializable]
	internal class BN_OneSideFillet : IBendingNotchShape
	{
		public BN_OneSideFillet( bool isOverCut )
		{
			IsOverCut = isOverCut;
		}

		[Browsable( false )]
		public BendingNotchShape_Type Type
		{
			get
			{
				return BendingNotchShape_Type.OneSideFillet;
			}
		}

		public bool IsOverCut
		{
			get; set;
		}

		public bool IsValid()
		{
			return true;
		}
	}
}
