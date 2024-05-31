using MyUIDisplayModel;
using System;
using System.ComponentModel;

namespace MyCADCore
{
	public enum BranchIntersectDir
	{
		Positive = 0,
		Negative = 1,
		Both = 2
	}

	[Serializable]
	public class CADft_BranchTubeParam : ICADFeatureParam
	{
		public CADft_BranchTubeParam( double x, double y, double z,
			double selfRotateAngle_deg, double angleA_deg, double angleB_deg,
			IGeom2D shape, BranchIntersectDir intersectDir, bool isCutThrough, double length )
		{
			Center_X = x;
			Center_Y = y;
			Center_Z = z;
			SelfRotateAngle_deg = selfRotateAngle_deg;
			AAngle_deg = angleA_deg;
			BAngle_deg = angleB_deg;
			Shape = shape;
			IntersectDir = intersectDir;
			IsCutThrough = isCutThrough;
			Length = length;
			ArrayParam = new ArrayParam();
		}

		[Browsable( false )]
		public CADFeatureType Type
		{
			get
			{
				return CADFeatureType.BranchTube;
			}
		}

		[MyDisplayName( "CADft_BranchTubeParam", "Center_X" )]
		public double Center_X
		{
			get;
			set;
		}

		[MyDisplayName( "CADft_BranchTubeParam", "Center_Y" )]
		public double Center_Y
		{
			get;
			set;
		}

		[MyDisplayName( "CADft_BranchTubeParam", "Center_Z" )]
		public double Center_Z
		{
			get;
			set;
		}

		[MyDisplayName( "CADft_BranchTubeParam", "SelfRotateAngle_deg" )]
		public double SelfRotateAngle_deg
		{
			get;
			set;
		}

		[MyDisplayName( "CADft_BranchTubeParam", "AAngle_deg" )]
		public double AAngle_deg
		{
			get;
			set;
		}

		[MyDisplayName( "CADft_BranchTubeParam", "BAngle_deg" )]
		public double BAngle_deg
		{
			get;
			set;
		}

		[TypeConverter( typeof( MyObjectConverter ) )]
		[MyDisplayName( "CADft_BranchTubeParam", "Shape" )]
		public IGeom2D Shape
		{
			get;
		}

		[MyDisplayName( "CADft_BranchTubeParam", "IntersectDir" )]
		public BranchIntersectDir IntersectDir
		{
			get;
			set;
		}

		[MyDisplayName( "CADft_BranchTubeParam", "IsCutThrough" )]
		public bool IsCutThrough
		{
			get;
			set;
		}

		[MyDisplayName( "CADft_BranchTubeParam", "Length" )]
		public double Length
		{
			get;
			set;
		}

		[TypeConverter( typeof( MyObjectConverter ) )]
		[MyDisplayName( "CADft_BranchTubeParam", "ArrayParam" )]
		public ArrayParam ArrayParam
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

			// shape should not be null and should be valid
			if( Shape == null || Shape.IsValid() == false ) {
				return false;
			}

			// length should be positive, or must be cut through
			if( Length <= 0 && IsCutThrough == false ) {
				return false;
			}

			// array param should not be null and should be valid
			if( ArrayParam == null || ArrayParam.IsValid() == false ) {
				return false;
			}

			return true;
		}
	}
}
