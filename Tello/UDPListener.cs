using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Tello {
    /// <summary>
    /// さーばー？
    /// </summary>
    class UdpListener : UdpBase {
        private IPEndPoint _listenOn;

        public UdpListener(int port) : this(new IPEndPoint(IPAddress.Any, port)) { }

        public UdpListener(IPEndPoint endPoint) {
            _listenOn = endPoint;
            client = new UdpClient(_listenOn);
        }

        public void Reply(string message, IPEndPoint endPoint) {
            var datagram = Encoding.ASCII.GetBytes(message);
            client.Send(datagram, datagram.Length, endPoint);
        }
    }
}
