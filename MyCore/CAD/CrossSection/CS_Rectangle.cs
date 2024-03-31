using System;
using System.ComponentModel;

namespace MyCore.CAD
{
	[Serializable]
	public class CS_Rectangle : CS_Base, ICrossSection
	{
		public CS_Rectangle( Geom2D_Rectangle basicGeom, double dThickness )
		{
			Shape = basicGeom;
			Thickness = dThickness;
		}

		[Browsable( false )]
		public CS_Type Type
		{
			get
			{
				return CS_Type.Rectangle;
			}
		}
	}
}
