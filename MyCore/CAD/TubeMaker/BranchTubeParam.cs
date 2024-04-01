using System;
using System.ComponentModel;

namespace MyCore.CAD
{
	public enum BranchIntersectDir
	{
		Positive = 0,
		Negative = 1,
		Both = 2
	}

	[Serializable]
	public class BranchTubeParam : ITubeMakeParam
	{
		public BranchTubeParam()
		{
			Center_X = 0;
			Center_Y = 50;
			Center_Z = 0;
			SelfRotateAngle_deg = 0;
			AAngle_deg = 0;
			BAngle_deg = 0;
			Shape = null;
			IntersectDir = BranchIntersectDir.Positive;
			Length = 50;
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

		public double SelfRotateAngle_deg
		{
			get;
			set;
		}

		public double AAngle_deg
		{
			get;
			set;
		}

		public double BAngle_deg
		{
			get;
			set;
		}

		[TypeConverter( typeof( ExpandableObjectConverter ) )]
		public IGeom2D Shape
		{
			get;
			set;
		}

		public BranchIntersectDir IntersectDir
		{
			get;
			set;
		}

		public double Length
		{
			get;
			set;
		}

		public bool IsValid()
		{
			// self rotate angle should be in range [0, 360]
			if( SelfRotateAngle_deg < 0 || SelfRotateAngle_deg > 360 ) {
				return false;
			}

			// A angle should be in range [-360, 360]
			if( AAngle_deg < -360 || AAngle_deg > 360 ) {
				return false;
			}

			// B angle should be in range [-360, 360]
			if( BAngle_deg < -360 || BAngle_deg > 360 ) {
				return false;
			}

			// shape should not be null
			if( Shape == null ) {
				return false;
			}

			// shape should be valid
			if( Shape.IsValid() == false ) {
				return false;
			}

			// length should be positive
			if( Length <= 0 ) {
				return false;
			}

			return true;
		}
	}
}
