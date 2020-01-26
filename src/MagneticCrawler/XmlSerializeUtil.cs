using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MagneticCrawler
{
       public static class XmlSerializeUtil
        {  
                public static T Deserialize<T>(string xml)
                {
                        using (StringReader sr = new StringReader(xml))
                        {
                                XmlSerializer xmldes = new XmlSerializer(typeof(T));
                                return (T)xmldes.Deserialize(sr);
                        }
                }


                public static T Deserialize<T>(Type type, Stream stream)
                {
                        XmlSerializer xmldes = new XmlSerializer(typeof(T));
                        return (T)xmldes.Deserialize(stream);
                }

                public static string Serializer(object obj)
                {
                        
                        return null;
                }
        }
}
