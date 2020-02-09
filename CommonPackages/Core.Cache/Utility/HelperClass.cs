using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Common.Cache.Utility
{
    public class HelperClass
    {
        public byte[] Serialize(object obj)
        {
            if(obj == null)
            {
                return null;
            }

            try
            {
                BinaryFormatter objBinaryFormatter = new BinaryFormatter();
                using (MemoryStream objMemoryStream = new MemoryStream())
                {
                    objBinaryFormatter.Serialize(objMemoryStream, obj);
                    byte[] objDataAsByte = objMemoryStream.ToArray();
                    return objDataAsByte;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception:" + e.Message);
            }

            return null;
        }

        public T Deserialize<T>(byte[] bytes)
        {
            if (bytes == null)
            {
                return default(T);
            }

            try
            {

                BinaryFormatter objBinaryFormatter = new BinaryFormatter();
                if (bytes == null)
                    return default(T);

                using (MemoryStream objMemoryStream = new MemoryStream(bytes))
                {
                    T result = (T)objBinaryFormatter.Deserialize(objMemoryStream);
                    return result;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception:" + e.Message);
            }

            return default(T);
        }
    }
}
