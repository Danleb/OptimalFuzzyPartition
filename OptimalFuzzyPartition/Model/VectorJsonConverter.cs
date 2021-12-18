using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;
using OptimalFuzzyPartitionAlgorithm.Utils;
using System;

namespace OptimalFuzzyPartition.Model
{
    public class VectorJsonConverter : JsonConverter<Vector<double>>
    {
        public override Vector<double> ReadJson(JsonReader reader, Type objectType, Vector<double> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var value = reader.ReadAsDouble() ?? 0;
            var value2 = reader.ReadAsDouble() ?? 0;
            reader.Read();
            return VectorUtils.CreateVector(value, value2);
        }

        public override void WriteJson(JsonWriter writer, Vector<double> value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
