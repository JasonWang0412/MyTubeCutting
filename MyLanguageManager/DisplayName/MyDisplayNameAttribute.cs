using System.Collections.Generic;
using System.ComponentModel;

namespace MyLanguageManager
{
	public class MyDisplayNameAttribute : DisplayNameAttribute
	{
		public MyDisplayNameAttribute( string szTypeName, string szFiledName )
			: base( szFiledName )
		{
			if( m_LanguageManagerMap.ContainsKey( szTypeName ) == false ) {
				m_LanguageManagerMap[ szTypeName ] = new LanguageManager( szTypeName );
			}
			m_LanguageManager = m_LanguageManagerMap[ szTypeName ];
			m_szFieldName = szFiledName;
		}

		public override string DisplayName
		{
			get
			{
				string szDisplayName = m_LanguageManager.GetString( m_szFieldName );
				if( string.IsNullOrEmpty( szDisplayName ) ) {
					szDisplayName = m_szFieldName;
				}
				return szDisplayName;
			}
		}

		static Dictionary<string, LanguageManager> m_LanguageManagerMap = new Dictionary<string, LanguageManager>();
		LanguageManager m_LanguageManager;
		string m_szFieldName;
	}
}
