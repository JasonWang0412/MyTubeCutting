using System;

namespace MyCADCore
{
	[Serializable]
	internal class CS_RectangleCheck : ICrossSectionCheck
	{
		public CS_RectangleCheck( CrossSection crossSection )
		{
			m_CrossSection = crossSection;
		}

		public bool IsValid()
		{
			// the crosssection shape should be valid
			if( m_CrossSection.Shape == null || m_CrossSection.Shape.IsValid() == false ) {
				return false;
			}

			// the crosssection shape should be a rectangle
			if( m_CrossSection.Shape is Geom2D_Rectangle rectangle == false ) {
				return false;
			}

			// the thickness should be positive
			if( m_CrossSection.Thickness <= 0 ) {
				return false;
			}

			// the thickness should be less than the half width and height
			if( m_CrossSection.Thickness >= rectangle.Width / 2 || m_CrossSection.Thickness >= rectangle.Height / 2 ) {
				return false;
			}
			return true;
		}

		CrossSection m_CrossSection;
	}
}
