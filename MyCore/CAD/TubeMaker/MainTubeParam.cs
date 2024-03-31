using System;
using System.ComponentModel;

namespace MyCore.CAD
{
	[Serializable]
	[TypeConverter( typeof( ExpandableObjectConverter ) )]
	public class MainTubeParam : ITubeMakeParam
	{
		public MainTubeParam( ICrossSection crossSection, double length )
		{
			CrossSection = crossSection;
			Length = length;
		}

		[Category( "CrossSection" )]
		[TypeConverter( typeof( ExpandableObjectConverter ) )]
		public ICrossSection CrossSection
		{
			get;
			set;
		}

		[Category( "Property" )]
		public double Length
		{
			get;
			set;
		}

		public bool IsValid()
		{
			// cross section should not be null
			if( CrossSection == null ) {
				return false;
			}

			// cross section should be valid
			if( CrossSection.IsValid() == false ) {
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
