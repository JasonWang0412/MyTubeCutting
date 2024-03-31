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
			Dir_X = 0;
			Dir_Y = 0;
			Dir_Z = 1;
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

		public double Dir_X
		{
			get;
			set;
		}

		public double Dir_Y
		{
			get;
			set;
		}

		public double Dir_Z
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
			// branch tube direction should not be zero
			if( Dir_X == 0 && Dir_Y == 0 && Dir_Z == 0 ) {
				return false;
			}

			// branch tube shape should not be null
			if( Shape == null ) {
				return false;
			}

			// shape should be valid
			if( Shape.IsValid() == false ) {
				return false;
			}

			// branch tube length should be positive
			if( Length <= 0 ) {
				return false;
			}

			return true;
		}
	}
}
