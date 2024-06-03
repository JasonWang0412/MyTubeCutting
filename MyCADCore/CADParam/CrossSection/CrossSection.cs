﻿using MyUIDisplayModel;
using System;
using System.ComponentModel;

namespace MyCADCore
{
	[Serializable]
	public class CrossSection
	{
		public CrossSection( IGeom2D shape, double thickness )
		{
			Shape = shape;
			Thickness = thickness;

			// init parameter checker
			if( shape.Type == Geom2D_Type.Circle ) {
				m_CrossSectionCheck = new CS_CircleCheck( this );
			}
			else if( shape.Type == Geom2D_Type.Rectangle ) {
				m_CrossSectionCheck = new CS_RectangleCheck( this );
			}
			else if( shape.Type == Geom2D_Type.Oval ) {
				m_CrossSectionCheck = new CS_OvalCheck( this );
			}
			else if( shape.Type == Geom2D_Type.FlatOval ) {
				m_CrossSectionCheck = new CS_FlatOvalCheck( this );
			}
			else if( shape.Type == Geom2D_Type.DShape ) {
				m_CrossSectionCheck = new CS_DShapeCheck( this );
			}
			else {
				m_CrossSectionCheck = new CS_DefaultCheck();
			}
		}

		[TypeConverter( typeof( MyObjectConverter ) )]
		[MyDisplayName( "CrossSection", "Shape" )]
		public IGeom2D Shape
		{
			get;
		}

		[MyDisplayName( "CrossSection", "Thickness" )]
		public double Thickness
		{
			get; set;
		}

		public bool IsValid()
		{
			return m_CrossSectionCheck.IsValid();
		}

		ICrossSectionCheck m_CrossSectionCheck;
	}
}