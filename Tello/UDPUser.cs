using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tello {
    class UdpUser : UdpBase {
        private UdpUser() { }

        public static UdpUser ConnectTo(string hostname, int port) {
            var connection = new UdpUser();
            connection.client.Connect(hostname, port);
            return connection;
        }

        public void Send(string message) {
            Send(Encoding.ASCII.GetBytes(message));
        }

        public void Send(byte[] message) {
            StringBuilder builder = new StringBuilder("[");
            foreach (byte data in message) builder.Append(" 0x").Append(data.ToString("X2"));
            builder.Append(" ]");
            Console.WriteLine(builder.ToString());
            client.Send(message, message.Length);
        }
    }
}
