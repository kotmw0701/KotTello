using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Tello {
    abstract class UdpBase {
        protected UdpClient client;

        protected UdpBase() => client = new UdpClient();

        public async Task<Received> Receive() {
            var result = await client.ReceiveAsync();
            return new Received() {
                Sender = result.RemoteEndPoint,
                Message = Encoding.ASCII.GetString(result.Buffer, 0, result.Buffer.Length),
                bytes = result.Buffer.ToArray()
            };
        }
    }
}
