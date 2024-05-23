using System;
using System.ComponentModel;

namespace MyCore.CAD
{
	[Serializable]
	public class CADft_BendingNotchParam : ICADFeatureParam
	{
		public CADft_BendingNotchParam( IBendingNotchShape shape, double y, double gap, double angleB_deg )
		{
			Shape = shape;
			YPos = y;
			GapFromButtom = gap;
			BAngle_deg = angleB_deg;
		}

		[Browsable( false )]
		public CADFeatureType Type
		{
			get
			{
				return CADFeatureType.BendingNotch;
			}
		}

		[TypeConverter( typeof( ExpandableObjectConverter ) )]
		public IBendingNotchShape Shape
		{
			get;
		}

		public double YPos
		{
			get; set;
		}

		public double GapFromButtom
		{
			get; set;
		}

		public double BAngle_deg
		{
			get; set;
		}

		public bool IsValid()
		{
			// shape should not be null and should be valid
			if( Shape == null || Shape.IsValid() == false ) {
				return false;
			}

			// gap should not be negative
			if( GapFromButtom < 0 ) {
				return false;
			}

			// B angle should be in rangge [-360,360]
			if( BAngle_deg < -360 || BAngle_deg > 360 ) {
				return false;
			}

			return true;
		}
	}
}
