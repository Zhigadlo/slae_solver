using System.Net.Sockets;
using System.Text;

namespace Domain
{
    public class DataManipulation
    {
        public static string GetMessage(NetworkStream stream)
        {
            byte[] lengthBuffer = new byte[4];
            if (stream.CanRead)
            {
                int bytesRead = stream.Read(lengthBuffer, 0, lengthBuffer.Length);
                int messageLength = BitConverter.ToInt32(lengthBuffer, 0);
                if (bytesRead < 4)
                {
                    Console.WriteLine(messageLength);
                    throw new Exception("Cannot get message length.");
                }
                byte[] messageBuffer = new byte[messageLength];
                stream.Read(messageBuffer, 0, messageLength);
                //StringBuilder data = new StringBuilder();

                //do
                //{
                //    bytesRead = stream.Read(messageBuffer, 0, messageBuffer.Length);
                //    data.AppendFormat("{0}", Encoding.UTF8.GetString(messageBuffer, 0, bytesRead));
                //}
                //while (stream.DataAvailable);
                //var message = data.ToString();

                return Encoding.UTF8.GetString(messageBuffer, 0, messageLength);
            }
            else
            {
                throw new InvalidOperationException("Cannot read from this NetworkStream.");
            }
        }
        public static void SendMessage(NetworkStream stream, string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            byte[] length = BitConverter.GetBytes(buffer.Length);
            stream.Write(length, 0, length.Length);
            stream.Write(buffer, 0, buffer.Length);
        }

        public static void SendArray(NetworkStream stream, float[] array)
        {
            if(array == null)
            {
                int length = 0;
                SendMessage(stream, length.ToString());
                return;
            }
            SendMessage(stream, array.Length.ToString());
            foreach (var item in array)
                SendMessage(stream, item.ToString());
        }

        public static float[] GetArray(NetworkStream stream)
        {
            int length = int.Parse(GetMessage(stream));
            if (length == 0)
                return null;
            float[] array = new float[length];
            for (int i = 0; i < length; i++)
                array[i] = float.Parse(GetMessage(stream));

            return array;
        }
    }
}
