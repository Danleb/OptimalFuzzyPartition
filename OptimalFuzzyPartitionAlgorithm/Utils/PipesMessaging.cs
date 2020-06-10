using System;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Serialization.Formatters.Binary;

namespace OptimalFuzzyPartitionAlgorithm.Utils
{
    public static class PipesMessaging
    {
        public static void SendObject(PipeStream pipeStream, object obj)
        {
            using (var ms = new MemoryStream(5000))
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                var bytesCount = ms.Length;
                var sizeBytes = BitConverter.GetBytes(bytesCount);
                pipeStream.Write(sizeBytes, 0, sizeBytes.Length);

                pipeStream.Write(ms.GetBuffer(), 0, (int)ms.Length);
            }
        }

        /// <summary>
        /// Start endless messages listening. There is sense to run it in the separate thread.
        /// </summary>
        /// <param name="pipeStream">PipeStream to read</param>
        /// <param name="onMessageReceived">Delegate to invoke, when the whole message is received</param>
        public static void ReadMessages(PipeStream pipeStream, Action<byte[], int, int> onMessageReceived)
        {
            const int sizeBytesCount = 4;
            byte[] sizeBuffer = new byte[sizeBytesCount];

            byte[] message_buffer = new byte[100_000];
            int receivedBytesCount;

            while (true)
            {
                receivedBytesCount = 0;

                while (receivedBytesCount < sizeBytesCount)
                {
                    receivedBytesCount += pipeStream.Read(sizeBuffer, receivedBytesCount, sizeBytesCount - receivedBytesCount);
                }

                int messageSize = BitConverter.ToInt32(sizeBuffer, 0);

                receivedBytesCount = 0;

                if (messageSize > message_buffer.Length)
                    message_buffer = new byte[messageSize];

                while (receivedBytesCount < messageSize)
                {
                    receivedBytesCount += pipeStream.Read(message_buffer, receivedBytesCount,
                        message_buffer.Length - receivedBytesCount);
                }

                onMessageReceived.Invoke(message_buffer, 0, messageSize);
            }
        }
    }
}