using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tello {
    internal class VideoManager {

        public static VideoManager Instance => new VideoManager();
        private VideoManager() { }
        private UdpListener videoServer = new UdpListener(6038);

        public void StartVideoStream() {

            CancellationToken token;

            Task.Factory.StartNew(async () => {
                try {
                    while (!token.IsCancellationRequested) {
                        var received = await videoServer.Receive();
                        Console.WriteLine(received);
                    }
                } catch (Exception e) {
                    Console.WriteLine("Video server Exception : " + e.Message);
                    Console.WriteLine(e.StackTrace);
                }
            }, token);
        }
    }
}
