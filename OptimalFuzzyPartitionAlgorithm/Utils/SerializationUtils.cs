using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace OptimalFuzzyPartitionAlgorithm.Utils
{
    public static class SerializationUtils
    {
        public static byte[] ToBytes(this object obj)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                var bytes = ms.ToArray();
                return bytes;
            }
        }

        public static T ConvertTo<T>(this byte[] bytes)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream(bytes))
            {
                var obj = bf.Deserialize(ms);
                var data = (T)obj;
                return data;
            }
        }
    }
}