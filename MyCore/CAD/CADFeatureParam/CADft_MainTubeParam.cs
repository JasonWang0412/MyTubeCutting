using System;
using System.ComponentModel;

namespace MyCADCore
{

	// TODO: dont know shit about this
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

		// TODO: dont know shit about this
		[TypeConverter( typeof( ExpandableObjectConverter ) )]
		public CrossSection CrossSection
		{
			get;
		}

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
