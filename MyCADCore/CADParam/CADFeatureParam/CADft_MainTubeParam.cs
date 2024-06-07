using MyUIDisplayModel;
using System;
using System.ComponentModel;

namespace MyCADCore
{
	[Serializable]
	public class CADft_MainTubeParam : ICADFeatureParam
	{
		public CADft_MainTubeParam( CrossSection crossSection, double length )
		{
			CrossSection = crossSection;
			Length = length;
		}

		[Browsable( false )]
		public CADFeatureType Type
		{
			get
			{
				return CADFeatureType.MainTube;
			}
		}

		[TypeConverter( typeof( MyObjectConverter ) )]
		[MyDisplayName( "CADft_MainTubeParam", "CrossSection" )]
		public CrossSection CrossSection
		{
			get;
		}

		[MyDisplayName( "CADft_MainTubeParam", "Length" )]
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
