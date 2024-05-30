using System;

namespace MyCADCore
{
	[Serializable]
	public class CS_FlatOvalCheck : ICrossSectionCheck
	{
		public CS_FlatOvalCheck( CrossSection crossSection )
		{
			m_CrossSection = crossSection;
		}

		public bool IsValid()
		{
			// the crosssection shape should be valid
			if( m_CrossSection.Shape == null || m_CrossSection.Shape.IsValid() == false ) {
				return false;
			}

			// the crosssection shape should be an flat oval
			if( m_CrossSection.Shape is Geom2D_FlatOval flatOval == false ) {
				return false;
			}

			// the thickness should be positive
			if( m_CrossSection.Thickness <= 0 ) {
				return false;
			}

			// the thickness should be less than radius
			if( m_CrossSection.Thickness >= flatOval.Radius ) {
				return false;
			}
			return true;
		}

		CrossSection m_CrossSection;
	}
}
