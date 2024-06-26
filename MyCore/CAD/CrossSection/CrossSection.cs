﻿using System;
using System.ComponentModel;

namespace MyCore.CAD
{
	[Serializable]
	public class CrossSection : ICrossSection
	{
		public CrossSection( IGeom2D shape, double thickness )
		{
			Shape = shape;
			Thickness = thickness;
		}

		[TypeConverter( typeof( ExpandableObjectConverter ) )]
		public virtual IGeom2D Shape
		{
			get;
		}

		public virtual double Thickness
		{
			get; set;
		}

		public virtual bool IsValid()
		{
			// shape should not be null
			if( Shape == null ) {
				return false;
			}

			// shape should be valid
			if( Shape.IsValid() == false ) {
				return false;
			}

			// thickness should be positive
			if( Thickness <= 0 ) {
				return false;
			}

			return true;
		}
	}
}
