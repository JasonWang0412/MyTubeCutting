using System;
using System.ComponentModel;

namespace MyCADCore
{
	public enum EEndSide
	{
		Left = 0,
		Right = 1
	}

	[Serializable]
	public class CADft_EndCutterParam : ICADFeatureParam
	{
		public CADft_EndCutterParam( double y, double tiltAngle_deg, double rotateAngle_deg, EEndSide side )
		{
			Center_Y = y;
			TiltAngle_deg = tiltAngle_deg;
			RotateAngle_deg = rotateAngle_deg;
			Side = side;
		}

		[Browsable( false )]
		public CADFeatureType Type
		{
			get
			{
				return CADFeatureType.EndCutter;
			}
		}

		public double Center_Y
		{
			get;
			set;
		}

		public double TiltAngle_deg
		{
			get;
			set;
		}

		public double RotateAngle_deg
		{
			get;
			set;
		}

		public EEndSide Side
		{
			get;
			set;
		}

		public bool IsValid()
		{
			// tilt angle should be in range [-90, 90]
			if( TiltAngle_deg < -90 || TiltAngle_deg > 90 ) {
				return false;
			}

			// rotate angle should be in range [-360, 360]
			if( RotateAngle_deg < -360 || RotateAngle_deg > 360 ) {
				return false;
			}

			return true;
		}
	}
}
