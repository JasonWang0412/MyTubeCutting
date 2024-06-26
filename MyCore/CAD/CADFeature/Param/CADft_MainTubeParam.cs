﻿using System;
using System.ComponentModel;

namespace MyCore.CAD
{

	// TODO: dont know shit about this
	[Serializable]
	public class CADft_MainTubeParam : ICADFeatureParam
	{
		public CADft_MainTubeParam( ICrossSection crossSection, double length )
		{
			CrossSection = crossSection;
			Length = length;
		}


		// TODO: dont know shit about this
		[TypeConverter( typeof( ExpandableObjectConverter ) )]
		public ICrossSection CrossSection
		{
			get;
			set;
		}

		public double Length
		{
			get;
			set;
		}

		public CADFeatureType Type
		{
			get
			{
				return CADFeatureType.MainTube;
			}
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
