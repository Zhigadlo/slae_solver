using System.Net.Sockets;
using System.Text;

namespace Domain
{
    public class DataManipulation
    {
        private const string StartMarker = "<msg>";
        private const string EndMarker = "</msg>";
        private const int BufferSize = 8192;
        public static string GetMessage(NetworkStream stream)
        {
            if (stream.CanRead)
            {
                byte[] messageBuffer = new byte[BufferSize];
                StringBuilder data = new StringBuilder();
                int bytesRead;
                while (!data.ToString().EndsWith(EndMarker))
                {
                    bytesRead = stream.Read(messageBuffer, 0, messageBuffer.Length);
                    data.AppendFormat("{0}", Encoding.UTF8.GetString(messageBuffer, 0, bytesRead));
                }

                var message = data.ToString();

                if (message.StartsWith(StartMarker) && message.EndsWith(EndMarker))
                    return message.Substring(StartMarker.Length, message.Length - EndMarker.Length - StartMarker.Length);

                throw new Exception("Received corrupted message");
            }
            else
            {
                throw new InvalidOperationException("Cannot read from this NetworkStream.");
            }
        }
        public static void SendMessage(NetworkStream stream, string message)
        {
            StringBuilder data = new StringBuilder();
            data.Append(StartMarker);
            data.Append(message);
            data.Append(EndMarker);
            byte[] buffer = Encoding.UTF8.GetBytes(data.ToString());
            int bytesSent = 0;
            int bytesLeft = buffer.Length;

            // Отправляем данные частями
            while (bytesLeft > 0)
            {
                int sendSize = Math.Min(bytesLeft, BufferSize); // Определяем размер отправляемых данных
                stream.Write(buffer, bytesSent, sendSize);
                bytesSent += sendSize;
                bytesLeft -= sendSize;
            }
        }
    }
}
