using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Tello {
    public struct Received {
        public IPEndPoint Sender;
        public string Message;
        public byte[] bytes;

        public override string ToString() {
            return $"Sender={Sender.Address}:{Sender.Port} | Message=\"{Message}\"";
        }
    }
}
