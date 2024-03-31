using System;
using System.ComponentModel;

namespace MyCore.CAD
{
	[Serializable]
	public class CS_Circle : CS_Base, ICrossSection
	{
		public CS_Circle( Geom2D_Circle basicGeom, double dThickness )
		{
			Shape = basicGeom;
			Thickness = dThickness;
		}

		[Browsable( false )]
		public CS_Type Type
		{
			get
			{
				return CS_Type.Circle;
			}
		}
	}
}
