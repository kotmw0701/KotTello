using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tello {
    class Commands {
        //                                        1     2     3     4     5     6     7     8     9     10    11    12    13    14    15    16    17    18    19    20    21    22
        //                                        head  packetLen   crc8  type  commandID    sequence     crc16
        public static readonly byte[] TAKEOFF = { 0xCC, 0x58, 0x00,     , 0x68, 0x54, 0x00, 0x01, 0x00,     ,      };
        //                                        head  packetLen   crc8  type  commandID    sequence   data    crc16
        public static readonly byte[] LAND =    { 0xCC, 0x60, 0x00,     , 0x68, 0x55, 0x00, 0x01, 0x00, 0x00,     ,      };
        //                                        head  packetLen   crc8  type  commandID    sequence  
        public static readonly byte[] STICK =   { 0xCC, 0xB0, 0x00, 0x7F, 0x60, 0x50, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        //                                        0xcc, 0xb0, 0x00, 0x7f, 0x60, 0x50, 0x00, 0x00, 0x00, 0x00, 0x04, 0x20, 0x00, 0x01, 0x08
        //                                        0xcc, 0xb0, 0x00, 0x7f, 0x60, 0x50, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x12, 0x16, 0x01, 0x0e, 0x00, 0x25, 0x54  
    }
}
