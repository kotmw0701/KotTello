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
        
        public static void TakeOff() {
            byte[] packets = PacketCopy(Commands.TAKEOFF);
            SetPacketSequence(packets);
            SetPacketCRCs(packets);
            client.Send(packets);
        }

        public static void Land() {
            byte[] packets = PacketCopy(Commands.LAND);
            SetPacketSequence(packets);
            SetPacketCRCs(packets);
            client.Send(packets);
        }

        public static void GamePadStick(GamePadManager manager) {
            Stick(manager.IsFastMode, manager.LeftX, manager.LeftY, manager.RightY, manager.RightX);
        }

        public static void Stick(bool isFast, double ratioRotation, double ratioThrottle, double ratioPitch, double ratioRole) {
            byte[] packets = PacketCopy(Commands.STICK);
            short fastMode = (short)(isFast ? 1 : 0);

            short rotation = (short)((ratioRotation * 660) + 1024);
            short throttle = (short)((ratioThrottle * 660) + 1024);
            short pitch = (short)((ratioPitch * 660) + 1024);
            short role = (short)((ratioPitch * 660) + 1024);

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

            client.Send(packets);
        }

        private static byte[] PacketCopy(byte[] sourceArray) {
            byte[] packets = new byte[sourceArray.Length];
            Array.Copy(sourceArray, packets, packets.Length);
            return packets;
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
