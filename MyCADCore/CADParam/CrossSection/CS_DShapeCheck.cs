using System;

namespace MyCADCore
{
	[Serializable]
	public class CS_DShapeCheck : ICrossSectionCheck
	{
		public CS_DShapeCheck( CrossSection crossSection )
		{
			m_CrossSection = crossSection;
		}

		public bool IsValid()
		{
			// the crosssection shape should be valid
			if( m_CrossSection.Shape == null || m_CrossSection.Shape.IsValid() == false ) {
				return false;
			}

			// the crosssection shape should be a DShape
			if( m_CrossSection.Shape is Geom2D_DShape dShape == false ) {
				return false;
			}

			// the thickness should be positive
			if( m_CrossSection.Thickness <= 0 ) {
				return false;
			}

			// the thickness should be less than width/2 (i.e. radius)
			if( m_CrossSection.Thickness >= dShape.Width / 2 ) {
				return false;
			}
			return true;
		}

		CrossSection m_CrossSection;
	}
}
