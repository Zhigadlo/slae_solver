using System.Net.Sockets;
using System.Text;

namespace Domain
{
    public class DataManipulation
    {
        private static string _startMarker = "<msg>";
        private static string _endMarker = "</msg>";
        public static string GetMessage(NetworkStream stream)
        {
            if (stream.CanRead)
            {
                byte[] messageBuffer = new byte[256];
                StringBuilder data = new StringBuilder();
                int bytesRead;
                do
                {
                    bytesRead = stream.Read(messageBuffer, 0, messageBuffer.Length);
                    data.AppendFormat("{0}", Encoding.UTF8.GetString(messageBuffer, 0, bytesRead));
                }
                while (!data.ToString().EndsWith(_endMarker));

                var message = data.ToString();

                if (message.StartsWith(_startMarker) && message.EndsWith(_endMarker))
                    return message.Substring(_startMarker.Length, data.Length - _endMarker.Length - _startMarker.Length);

                throw new Exception("Received corrupted message");
            }
            else
            {
                throw new InvalidOperationException("Cannot read from this NetworkStream.");
            }
        }
        public static void SendMessage(NetworkStream stream, string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(string.Concat(_startMarker, message, _endMarker));
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
