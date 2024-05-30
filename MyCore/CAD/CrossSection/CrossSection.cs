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
			else {
				m_CrossSectionCheck = new CS_DefaultCheck();
			}
		}

		[TypeConverter( typeof( ExpandableObjectConverter ) )]
		public IGeom2D Shape
		{
			get;
		}

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
