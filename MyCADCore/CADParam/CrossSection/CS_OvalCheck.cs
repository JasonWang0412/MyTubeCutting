using System;

namespace MyCADCore
{
	[Serializable]
	public class CS_OvalCheck : ICrossSectionCheck
	{
		public CS_OvalCheck( CrossSection crossSection )
		{
			m_CrossSection = crossSection;
		}

		public bool IsValid()
		{

			// the crosssection shape should be valid
			if( m_CrossSection.Shape == null || m_CrossSection.Shape.IsValid() == false ) {
				return false;
			}

			// the crosssection shape should be an oval
			if( m_CrossSection.Shape is Geom2D_Oval oval == false ) {
				return false;
			}

			// the thickness should be positive
			if( m_CrossSection.Thickness <= 0 ) {
				return false;
			}

			// the thickness should be less than min of width/2 or height/2
			if( m_CrossSection.Thickness >= Math.Min( oval.Width, oval.Height ) / 2 ) {
				return false;
			}

			return true;
		}

		CrossSection m_CrossSection;
	}
}
