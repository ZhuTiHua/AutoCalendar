using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace DataApp_2
{
    public class XmlSerializeHelper
    {
         /// <summary>  
    /// 反序列化xml字符为对象，默认为Utf-8编码  
    /// </summary>  
    /// <typeparam name="T"></typeparam>  
    /// <param name="xml"></param>  
    /// <returns></returns>  
    public static T DeSerialize<T>(string xml)
        where T : new()  
    {  
        return DeSerialize<T>(xml, Encoding.UTF8);  
    }  
  
    /// <summary>  
    /// 反序列化xml字符为对象  
    /// </summary>  
    /// <typeparam name="T"></typeparam>  
    /// <param name="xml"></param>  
    /// <param name="encoding"></param>  
    /// <returns></returns>  
    public static T DeSerialize<T>(string xml, Encoding encoding)
        where T : new()  
    {  
        try  
        {  
            var mySerializer = new XmlSerializer(typeof(T));  
            using (var ms = new MemoryStream(encoding.GetBytes(xml)))  
            {  
                using (var sr = new StreamReader(ms, encoding))  
                {  
                    return (T)mySerializer.Deserialize(sr);  
                }  
            }  
        }  
        catch (Exception e)  
        {  
            return default(T);  
        }  
    }
        public static string Serialize<T>(T obj)
        {
            return Serialize<T>(obj, Encoding.UTF8);
        }

        /// <summary>  
        /// 实体对象序列化成xml字符串  
        /// </summary>  
        /// <typeparam name="T"></typeparam>  
        /// <param name="obj"></param>  
        /// <returns></returns>  
        public static string Serialize<T>(T obj, Encoding encoding)
    {  
        try  
        {  
  
            if (obj == null)  
                throw new ArgumentNullException("obj");  
  
            var ser = new XmlSerializer(obj.GetType());  
            using (var ms = new MemoryStream())  
            {  
                using (var writer = new XmlTextWriter(ms, encoding))  
                {  
                    writer.Formatting = Formatting.Indented;  
                    ser.Serialize(writer, obj);  
                }  
                var xml = encoding.GetString(ms.ToArray());  
                xml = xml.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "");  
                xml = xml.Replace("xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");  
                return xml;  
            }  
        }  
        catch (Exception ex)  
        {  
            throw ex;  
        }  
    }  
    }
}
