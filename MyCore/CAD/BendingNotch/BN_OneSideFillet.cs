using System;
using System.ComponentModel;

namespace MyCore.CAD
{
	[Serializable]
	public class BN_OneSideFillet : IBendingNotchShape
	{
		public BN_OneSideFillet( bool isOverCut )
		{
			IsOverCut = isOverCut;
		}

		[Browsable( false )]
		public BendingNotch_Type Type
		{
			get
			{
				return BendingNotch_Type.OneSideFillet;
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
