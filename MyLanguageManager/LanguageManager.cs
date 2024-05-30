using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace MyLanguageManager
{
	// public
	public partial class LanguageManager
	{
		// supported languages
		public const string CHINESE_Traditional = "zh-TW";
		public const string CHINESE_Simplified = "zh-CN";
		public const string ENGLISH_UnitedStates = "en-US";
		public const string JAPANESE_Japan = "ja-JP";
		public const string RUSSAIN_RussianFederation = "ru-RU";

		public static string CurrentUILanguage
		{
			get
			{
				return m_szCurrentUILanguage;
			}
			set
			{
				string szNewLanguage = value.ToLower();
				foreach( string szLanguage in GetSupportedLanguageList() ) {
					if( szNewLanguage == szLanguage.ToLower() ) {
						m_szCurrentUILanguage = szLanguage;
					}
				}
			}
		}

		public static bool CompareLanguageTag( string szLanguage1, string szLanguage2 )
		{
			return szLanguage1.ToLower() == szLanguage2.ToLower();
		}

		public static List<string> GetSupportedLanguageList()
		{
			return m_SupportedLaguageDataMap.Select( data => data.Key ).ToList();
		}

		public static int GetCNCParamValueByLanguage( string szLanguage )
		{
			// invalid argument
			if( string.IsNullOrEmpty( szLanguage ) ) {
				throw new ArgumentException( ERROR_INVALID_LANGUAGE );
			}

			// the target language is not in supported list
			if( m_SupportedLaguageDataMap.ContainsKey( szLanguage ) == false ) {
				return 0;
			}
			return m_SupportedLaguageDataMap[ szLanguage ].CNCParamValue;
		}

		public static string GetNativeNameByLanguage( string szLanguage )
		{
			// invalid argument
			if( string.IsNullOrEmpty( szLanguage ) ) {
				throw new ArgumentException( ERROR_INVALID_LANGUAGE );
			}

			// the target language is not in supported list
			if( m_SupportedLaguageDataMap.ContainsKey( szLanguage ) == false ) {
				return string.Empty;
			}
			return m_SupportedLaguageDataMap[ szLanguage ].NativeName;
		}

		public static string GetDisplayNameByLanguage( string szLanguage )
		{
			// invalid argument
			if( string.IsNullOrEmpty( szLanguage ) ) {
				throw new ArgumentException( ERROR_INVALID_LANGUAGE );
			}

			// the display name dictionary not found
			if( m_SupportedLaguageDataMap.ContainsKey( CurrentUILanguage ) == false
				|| m_SupportedLaguageDataMap[ CurrentUILanguage ].DisplayName == null ) {
				return string.Empty;
			}

			// the target language is not in supported list
			if( m_SupportedLaguageDataMap[ CurrentUILanguage ].DisplayName.ContainsKey( szLanguage ) == false ) {
				return string.Empty;
			}
			return m_SupportedLaguageDataMap[ CurrentUILanguage ].DisplayName[ szLanguage ];
		}

		public LanguageManager( string szMapConfigFileName )
		{
			m_szMapConfigFileName = szMapConfigFileName;
			InitResourceMap();
		}

		public string GetString( string szID )
		{
			return GetString( szID, CurrentUILanguage );
		}

		public string GetString( string szID, string szLanguage )
		{
			// invalid argument
			if( string.IsNullOrEmpty( szLanguage ) ) {
				throw new ArgumentException( ERROR_INVALID_LANGUAGE );
			}

			// the target language is not in supported list
			if( m_ResourceMap.ContainsKey( szLanguage ) == false || m_ResourceMap[ szLanguage ] == null ) {
				return string.Empty;
			}

			// invalid argument
			if( string.IsNullOrEmpty( szID ) ) {
				throw new ArgumentException( ERROR_INVALID_ID );
			}

			// the target ID not found
			if( m_ResourceMap[ szLanguage ].ContainsKey( szID ) == false ) {
				return string.Empty;
			}
			return m_ResourceMap[ szLanguage ][ szID ];
		}

		public void SetString( string szID, string szNewString )
		{
			SetString( szID, CurrentUILanguage, szNewString );
		}

		public void SetString( string szID, string szLanguage, string szNewString )
		{
			// invalid argument
			if( string.IsNullOrEmpty( szLanguage ) ) {
				throw new ArgumentException( ERROR_INVALID_LANGUAGE );
			}

			// error handling but should not happend
			if( m_ResourceMap.ContainsKey( szLanguage ) == false ) {
				m_ResourceMap.Add( szLanguage, new Dictionary<string, string>() );
			}
			else if( m_ResourceMap.ContainsKey( szLanguage ) && m_ResourceMap[ szLanguage ] == null ) {
				m_ResourceMap[ szLanguage ] = new Dictionary<string, string>();
			}

			// invalid argument
			if( string.IsNullOrEmpty( szID ) ) {
				throw new ArgumentException( ERROR_INVALID_ID );
			}
			m_ResourceMap[ szLanguage ][ szID ] = szNewString;
			WriteResourceMapToXml( szLanguage );
		}
	}

	// protected
	public partial class LanguageManager
	{
	}

	// private
	public partial class LanguageManager
	{
		// config file
		string m_szMapConfigFileName;
		const string CONFIG_DIR_NAME = "Config";
		const string LANGUAGE_DIR_NAME = "Language";
		const string DISPLAYNAME_FILE_NAME = "DisplayName";
		static readonly string XML_DIR = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, CONFIG_DIR_NAME, LANGUAGE_DIR_NAME );

		// config format
		const string XML_EXTENSTION = ".xml";
		const string XML_ROOT_NAME = "ResMap";
		const string XML_NODE_NAME = "Message";
		const string XML_NODE_ATTRIBUTE_ID = "ID";
		const string XML_NODE_ATTRIBUTE_CONTENT = "Content";

		// exception handling
		const string ERROR_INVALID_ID = "Invalid ID";
		const string ERROR_INVALID_LANGUAGE = "Invalid Language";

		// support language
		struct LanguageData
		{
			// pr3209
			public int CNCParamValue;
			public string NativeName;

			// key = language, value = display name
			public Dictionary<string, string> DisplayName;
		}

		static readonly Dictionary<string, LanguageData> m_SupportedLaguageDataMap = new Dictionary<string, LanguageData>(){
			{ CHINESE_Traditional, new LanguageData(){ CNCParamValue = 1, NativeName = "繁體中文", DisplayName = InitDisplayName( CHINESE_Traditional ) } },
			{ CHINESE_Simplified, new LanguageData(){ CNCParamValue = 119, NativeName = "简体中文", DisplayName = InitDisplayName( CHINESE_Simplified ) } },
			{ ENGLISH_UnitedStates, new LanguageData(){ CNCParamValue = 0, NativeName = "English", DisplayName = InitDisplayName( ENGLISH_UnitedStates ) } },
			{ JAPANESE_Japan, new LanguageData(){ CNCParamValue = 175, NativeName = "日本語", DisplayName = InitDisplayName( JAPANESE_Japan ) } },
			{ RUSSAIN_RussianFederation, new LanguageData(){ CNCParamValue = 185, NativeName = "Русский", DisplayName = InitDisplayName( RUSSAIN_RussianFederation ) } },
		};

		static string m_szCurrentUILanguage = CHINESE_Traditional;

		// resource map
		Dictionary<string, Dictionary<string, string>> m_ResourceMap;

		void InitResourceMap()
		{
			m_ResourceMap = new Dictionary<string, Dictionary<string, string>>();
			foreach( string szOneLanguage in GetSupportedLanguageList() ) {
				Dictionary<string, string> oneLanguageMap = LoadResourceMapFromXml( szOneLanguage );
				if( oneLanguageMap == null ) {
					continue;
				}
				m_ResourceMap.Add( szOneLanguage, oneLanguageMap );
			}
		}

		Dictionary<string, string> LoadResourceMapFromXml( string szLanguage )
		{
			// check the xml file exist
			string szFilePath = Path.Combine( XML_DIR, m_szMapConfigFileName + "_" + szLanguage + XML_EXTENSTION );
			if( File.Exists( szFilePath ) == false ) {
				return null;
			}

			// load the xml file
			XDocument xmlDoc = XDocument.Load( szFilePath );
			if( xmlDoc == null ) {
				return null;
			}

			// create the map
			var allNode = xmlDoc.Descendants( XML_NODE_NAME );
			Dictionary<string, string> resourceMap = new Dictionary<string, string>();
			foreach( var node in allNode ) {

				// prevent from repeat
				if( resourceMap.ContainsKey( node.Attribute( XML_NODE_ATTRIBUTE_ID ).Value ) ) {
					continue;
				}
				resourceMap.Add( node.Attribute( XML_NODE_ATTRIBUTE_ID ).Value, node.Attribute( XML_NODE_ATTRIBUTE_CONTENT ).Value );
			}
			return resourceMap;
		}

		void WriteResourceMapToXml( string szLanguage )
		{
			string szFilePath = Path.Combine( XML_DIR, m_szMapConfigFileName + "_" + szLanguage + XML_EXTENSTION );
			XmlDocument xmlDoc = new XmlDocument();

			// root node
			XmlElement root = xmlDoc.CreateElement( XML_ROOT_NAME );

			// each data node
			foreach( KeyValuePair<string, string> data in m_ResourceMap[ szLanguage ] ) {
				XmlElement oneNode = xmlDoc.CreateElement( XML_NODE_NAME );
				oneNode.SetAttribute( XML_NODE_ATTRIBUTE_ID, data.Key );
				oneNode.SetAttribute( XML_NODE_ATTRIBUTE_CONTENT, data.Value );
				root.AppendChild( oneNode );
			}

			// save document
			xmlDoc.AppendChild( root );
			xmlDoc.Save( szFilePath );
		}

		static Dictionary<string, string> InitDisplayName( string szLanguage )
		{
			Dictionary<string, string> displayNameMap = new Dictionary<string, string>();

			// check the xml file exist
			string szFilePath = Path.Combine( XML_DIR, DISPLAYNAME_FILE_NAME + "_" + szLanguage + XML_EXTENSTION );
			if( File.Exists( szFilePath ) == false ) {
				return null;
			}

			// load the xml file
			XDocument xmlDoc = XDocument.Load( szFilePath );
			if( xmlDoc == null ) {
				return null;
			}

			// create the map
			var allNode = xmlDoc.Descendants( XML_NODE_NAME );

			foreach( var node in allNode ) {

				// prevent from repeat
				if( displayNameMap.ContainsKey( node.Attribute( XML_NODE_ATTRIBUTE_ID ).Value ) ) {
					continue;
				}
				displayNameMap.Add( node.Attribute( XML_NODE_ATTRIBUTE_ID ).Value, node.Attribute( XML_NODE_ATTRIBUTE_CONTENT ).Value );
			}
			return displayNameMap;
		}
	}
}
