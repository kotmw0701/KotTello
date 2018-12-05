using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tello {
    public class TelloMain {
        private static UdpUser client;
        private static DateTime lastMessageTime;//タイムアウトのアレ
        public static int wifiStrength = 0;

        private static ushort sequence = 1;

        

        private static void SetPacketSequence(byte[] packet) {
            packet[7] = (byte)(sequence & 0xFF);
            packet[8] = (byte)((sequence >> 8) & 0xFF);
            sequence++;
        }

        private static void SetPacketCRCs(byte[] packet) {
            CRC.CalcUCRC(packet, 4);
            CRC.CalcCRC(packet, packet.Length);
        }
    }
}
