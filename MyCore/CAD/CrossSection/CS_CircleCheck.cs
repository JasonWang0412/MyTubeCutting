namespace MyCore.CAD
{
	internal class CS_CircleCheck : ICrossSectionCheck
	{
		public CS_CircleCheck( CrossSection crossSection )
		{
			m_CrossSection = crossSection;
		}

		public bool IsValid()
		{
			// the crosssection shape should be valid
			if( m_CrossSection.Shape == null || m_CrossSection.Shape.IsValid() == false ) {
				return false;
			}

			// the crosssection shape should be a circle
			if( m_CrossSection.Shape is Geom2D_Circle circle == false ) {
				return false;
			}

			// the thickness should be positive
			if( m_CrossSection.Thickness <= 0 ) {
				return false;
			}

			// the thickness should be less than the radius
			if( m_CrossSection.Thickness >= circle.Radius ) {
				return false;
			}
			return true;
		}

		CrossSection m_CrossSection;
	}
}
