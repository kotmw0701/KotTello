using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tello {
    internal class TelloMain {
        private static UdpUser client;
        private static DateTime lastMessageTime;//タイムアウトのアレ
        public static int wifiStrength = 0;

        private static ushort sequence = 1;
        
        public static void Stick(GamePadManager manager) {
            byte[] packets = new byte[Commands.STICK.Length];
            Array.Copy(Commands.STICK, packets, packets.Length);
            short fastMode = (short)(manager.IsFastMode ? 1 : 0);

            short rotation = (short)((manager.LeftX * 660) + 1024);
            short throttle = (short)((manager.LeftY * 660) + 1024);
            short pitch = (short)((manager.RightY * 660) + 1024);
            short role = (short)((manager.RightX * 660) + 1024);

            var data = (fastMode << 44) + (rotation << 22) + (pitch << 11) + role;

            packets[9]  = (byte)(data & 0xFF);
            packets[10] = (byte)(data >> 8 & 0xFF);
            packets[11] = (byte)(data >> 16 & 0xFF);
            packets[12] = (byte)(data >> 24 & 0xFF);
            packets[13] = (byte)(data >> 32 & 0xFF);
            packets[14] = (byte)(data >> 40 & 0xFF);

            var time = DateTime.Now;
            packets[15] = (byte)time.Hour;
            packets[16] = (byte)time.Minute;
            packets[17] = (byte)time.Second;
            packets[18] = (byte)(time.Millisecond & 0xFF);
            packets[19] = (byte)(time.Millisecond >> 8);

            SetPacketCRCs(packets);

            //まだ送信してないです
        }

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
