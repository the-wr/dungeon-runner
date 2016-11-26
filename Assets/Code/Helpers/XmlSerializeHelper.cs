using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

public static class XmlSerializeHelper
{
    public static void SaveToXml<T>( T obj, string path )
    {
        var serializer = new XmlSerializer( typeof( T ) );
        using ( var stream = new StreamWriter( path ) )
        {
            serializer.Serialize( stream, obj );
        }
    }

    public static T LoadFromXml<T>( string path ) where T: class
    {
        try
        {
            var serializer = new XmlSerializer( typeof (T) );
            using ( var stream = new StreamReader( path ) )
            {
                return serializer.Deserialize( stream ) as T;
            }
        }
        catch ( Exception )
        {
            return null;
        }
    }
}
