using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tello {
	internal class TelloManager {
		private UdpUser client;
		private DateTime lastMessageTime;//タイムアウトのアレ
		public int wifiStrength = 0;
		private ConnectionState state = ConnectionState.Disconnected;
		private CancellationTokenSource token = new CancellationTokenSource();

		private ControllData controller = new ControllData();
		private readonly int VIDEO_PORT = 0x1796;//6038
		private ushort sequence = 1;
		private bool connected = false;
		private int iFrameRate = 5;


		private void Connection() {
            if (connected)
                return;
			Task.Factory.StartNew(async () => {
				var timeout = new TimeSpan(3000);
				while (true) {
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
					Console.WriteLine("Receive	: " + Commands.GetType(received.bytes).DisplayName());
					lastMessageTime = DateTime.Now;
					if (state == ConnectionState.Connecting && received.Message.StartsWith("conn_ack")) {
						state = ConnectionState.Connected;
						connected = true;
						Heartbeat();
						SetVideoAspect(true);
						RequestIFrame();
						continue;
					}
					CommandType type = Commands.GetType(received.bytes);
				}
			}, token);

			var videoServer = new UdpListener(VIDEO_PORT);
			var videoClient = UdpUser.ConnectTo("127.0.0.1", 7038);
			//ffmpeg -i udp://127.0.0.1:7038 -f sdl "Tello"
			//一時的に
			Task.Factory.StartNew(async () => {
				try {
					while (!token.IsCancellationRequested) {
						var received = await videoServer.Receive();
						videoClient.Send(received.bytes.Skip(2).ToArray());
					}
				} catch (Exception e) {
					Console.WriteLine("Video server Exception : " + e.Message);
					Console.WriteLine(e.StackTrace);
				}
			}, token);
		}

		private void Heartbeat() {
			CancellationToken token = this.token.Token;

			Task.Factory.StartNew(async () => {
				int tick = 0;
				while (!token.IsCancellationRequested) {
					if (state == ConnectionState.Connected) {
						ControllerUpdate();
						tick++;
						if ((tick % 5) == 0) RequestIFrame();
					}
					await Task.Delay(50);
				}
			}, token);
		}

		public void TakeOff() {
			SendPacket(PacketCopy(Commands.TAKEOFF));
		}

		public void Land() {
			SendPacket(PacketCopy(Commands.LAND));
		}

		public void RequestIFrame() {
			SendPacket(PacketCopy(Commands.REQUEST_VIDEO));
		}

		public void SetVideoBitRate(byte rate) {
			byte[] packets = PacketCopy(Commands.SET_VIDEOBITRATE);
			packets[9] = rate;
			SendPacket(packets);
		}

		public void SetVideoAspect(bool wide) {
			byte[] packets = PacketCopy(Commands.SET_VIDEOASPECT);
			packets[9] = (byte)(wide ? 1 : 0);
			SendPacket(packets);
		}

		public void ControllerUpdate() => ControllerUpdate(controller);

		public void ControllerUpdate(ControllData status) => Stick(status.IsFastMode, status.Rotation, status.Throttle, status.Pitch, status.Role);

		private void Stick(bool isFast, double ratioRotation, double ratioThrottle, double ratioPitch, double ratioRole) {
			byte[] packets = PacketCopy(Commands.STICK);
			short fastMode = (short)(isFast ? 1 : 0);

			short rotation = (short)((ratioRotation * 660) + 1024);
			short throttle = (short)((ratioThrottle * 660) + 1024);
			short pitch = (short)((ratioPitch * 660) + 1024);
			short role = (short)((ratioPitch * 660) + 1024);

			var data = (fastMode << 44) + (rotation << 22) + (pitch << 11) + role;

			packets[9] = (byte)(data & 0xFF);
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

		private void Connect() {
			client = UdpUser.ConnectTo("192.168.10.1", 8889);

			state = ConnectionState.Connecting;
			byte[] connectPacket = Encoding.UTF8.GetBytes("conn_req:\x00\x00");
			connectPacket[connectPacket.Length - 2] = (byte)(VIDEO_PORT & 0xFF);
			connectPacket[connectPacket.Length - 1] = (byte)((VIDEO_PORT >> 8) & 0xFF);
			Console.WriteLine($"{(connectPacket[connectPacket.Length - 1] << 8) | (connectPacket[connectPacket.Length - 2])}");
			client.Send(connectPacket);
		}

		private void Disconnect() {
            token.Cancel();
            connected = false;
            if(state == ConnectionState.Disconnected) {

            }
            state = ConnectionState.Disconnected;
		}

		private void SendPacket(byte[] packet) {
			SetPacketSequence(packet);
			SetPacketCRCs(packet);
			client.Send(packet);
		}

		private byte[] PacketCopy(byte[] sourceArray) {
			byte[] packets = new byte[sourceArray.Length];
			Array.Copy(sourceArray, packets, packets.Length);
			return packets;
		}

		private void SetPacketSequence(byte[] packet) {
			packet[7] = (byte)(sequence & 0xFF);
			packet[8] = (byte)((sequence >> 8) & 0xFF);
			sequence++;
		}

		private void SetPacketCRCs(byte[] packet) {
			CRC.CalcUCRC(packet, 4);
			CRC.CalcCRC(packet, packet.Length);
		}
	}
}
