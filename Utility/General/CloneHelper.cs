using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Utility
{
	public class CloneHelper
	{
		public static T Clone<T>( T obj )
		{
			if( obj == null ) {
				return default;
			}
			BinaryFormatter formatter = new BinaryFormatter();
			MemoryStream stream = new MemoryStream();
			formatter.Serialize( stream, obj );
			stream.Seek( 0, SeekOrigin.Begin );
			T clonedObj = (T)formatter.Deserialize( stream );
			stream.Close();
			return clonedObj;
		}
	}
}
