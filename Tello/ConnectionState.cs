using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tello {
    public enum ConnectionState {
        Disconnected,
        Connecting,
        Connected,
        Paused, //入力が止まったときに切断されないように
        UnPausing //遷移，この状態になるのは一瞬。
    }
}
