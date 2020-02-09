using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Common.Cache.Utility
{
    public class Helper
    {
        public string Serialize(object objectData)
        {
            string result = null;

            if (objectData == null)
            {
                //return null;
                result = null;
            }
            else
            {
                try
                {
                    //BinaryFormatter objBinaryFormatter = new BinaryFormatter();
                    //using (MemoryStream objMemoryStream = new MemoryStream())
                    //{
                    //    objBinaryFormatter.Serialize(objMemoryStream, objectData);
                    //    byte[] objDataAsByte = objMemoryStream.ToArray();
                    //    return objDataAsByte;
                    //}

                    result = JsonConvert.SerializeObject(objectData);

                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception:" + e.Message);
                }

            }

            return result;
        }

        public T Deserialize<T>(string data)
        {
            if (data == null)
            {
                return default(T); 
            }

            try
            {

                //BinaryFormatter objBinaryFormatter = new BinaryFormatter();
                //if (bytes == null)
                //    return default(T);

                //using (MemoryStream objMemoryStream = new MemoryStream(bytes))
                //{
                //    T result = (T)objBinaryFormatter.Deserialize(objMemoryStream);
                //    return result;
                //}

                return JsonConvert.DeserializeObject<T>(data);

            }
            catch(Exception e)
            {
                Console.WriteLine("Exception:" + e.Message);
            }

            return default(T);
        }
    }
}
