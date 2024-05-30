namespace MyCADCore.Tool
{
	public class CloneHelper
	{
		// TODO: dont know shit about this
		public static T Clone<T>( T obj )
		{
			if( obj == null ) {
				return default;
			}
			System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			System.IO.MemoryStream stream = new System.IO.MemoryStream();
			formatter.Serialize( stream, obj );
			stream.Seek( 0, System.IO.SeekOrigin.Begin );
			T clonedObj = (T)formatter.Deserialize( stream );
			stream.Close();
			return clonedObj;
		}
	}
}
