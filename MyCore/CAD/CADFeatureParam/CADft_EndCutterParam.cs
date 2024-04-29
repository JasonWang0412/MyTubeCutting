using System;

namespace MyCore.CAD
{
	public enum EEndSide
	{
		Left = 0,
		Right = 1
	}

	[Serializable]
	public class CADft_EndCutterParam : ICADFeatureParam
	{
		public CADft_EndCutterParam()
		{
			Center_X = 0;
			Center_Y = 0;
			Center_Z = 0;
			TiltAngle_deg = 90;
			RotateAngle_deg = 0;
			Side = EEndSide.Left;
		}

		public double Center_X
		{
			get;
			set;
		}

		public double Center_Y
		{
			get;
			set;
		}

		public double Center_Z
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

		public CADFeatureType Type
		{
			get
			{
				return CADFeatureType.EndCutter;
			}
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
