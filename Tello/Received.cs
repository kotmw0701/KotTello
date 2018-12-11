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
            StringBuilder builder = new StringBuilder("[");
            foreach (byte data in bytes) builder.Append(" 0x").Append(data.ToString("X2"));
            builder.Append(" ]");
            return $"Sender={Sender.Address}:{Sender.Port} | {builder.ToString()}";
        }
    }
}
