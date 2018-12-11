using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tello {
    internal class TelloMain {
        private static UdpUser client;
        private static DateTime lastMessageTime;//タイムアウトのアレ
        public static int wifiStrength = 0;
        private static ConnectionState state = ConnectionState.Disconnected;
        private CancellationTokenSource token = new CancellationTokenSource();


        private static readonly int VIDEO_PORT = 0x1796;
        private static ushort sequence = 1;
        private bool connected = false;
        
        public TelloMain() {
            Connection();
        }

        private void Connection() {
            Task.Factory.StartNew(async () => {
                var timeout = new TimeSpan(3000);
                while(true) {
                    switch (state) {
                        case ConnectionState.Disconnected:
                            Connect();
                            lastMessageTime = DateTime.Now;
                            Listener();
                            break;
                        case ConnectionState.Connecting:
                        case ConnectionState.Connected:
                            var elapsed = DateTime.Now - lastMessageTime;
                            if (elapsed.Seconds > 2) {
                                Console.WriteLine("Connection Timeout");
                                //Disconnect;
                            }
                            break;
                        case ConnectionState.Paused:
                            lastMessageTime = DateTime.Now;
                            break;
                    }
                    await Task.Delay(500);
                }
            });
        }

        private void Listener() {

            CancellationToken token = this.token.Token;
            Task.Factory.StartNew(async () => {
                while (!token.IsCancellationRequested) {
                    var received = await client.Receive();
                    lastMessageTime = DateTime.Now;
                    if (state == ConnectionState.Connecting && received.Message.StartsWith("conn_ack")) {
                        connected = true;
                        continue;
                    }
                    int cmdID = (received.bytes[5] | (received.bytes[6] << 8));
                    Console.WriteLine($"0x{cmdID.ToString("X2")}");
                }
            }, token);

            var videoServer = new UdpListener(VIDEO_PORT);

            Task.Factory.StartNew(async () => {
                try {
                    while (!token.IsCancellationRequested) {
                        Console.WriteLine("Video");
                        var received = await videoServer.Receive();
                        Console.WriteLine(received);
                    }
                } catch (Exception e) {
                    Console.WriteLine("Video server Exception : " + e.Message);
                    Console.WriteLine(e.StackTrace);
                }
            }, token);
        }

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

        public static void requestIFrame() {
            byte[] packets = PacketCopy(Commands.REQUEST_VIDEO);
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

        private static void Connect() {
            client = UdpUser.ConnectTo("192.168.10.1", 8889);

            state = ConnectionState.Connecting;
            //                       0     1     2     3     4     5     6     7     8     9     10    11    12    13    14    15    16    17    18    19    20    21    22
            //                       head  packetLen   crc8  type  commandID    sequence      data
            //byte[] connectPacket = { 0xCC, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00};
            byte[] connectPacket = Encoding.UTF8.GetBytes("conn_req:\x00\x00");
            connectPacket[connectPacket.Length - 2] = (byte)(VIDEO_PORT & 0xFF);
            connectPacket[connectPacket.Length - 1] = (byte)((VIDEO_PORT >> 8) & 0xFF);
            Console.WriteLine($"{connectPacket[connectPacket.Length - 1] << 8} : {connectPacket[connectPacket.Length - 2]}");
            client.Send(connectPacket);
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
