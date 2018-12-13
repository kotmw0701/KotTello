using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tello {
	class ControllerStatus {
		public double Rotation { get; private set; }
		public double Throttle { get; private set; }
		public double Pitch { get; private set; }
		public double Role { get; private set; }
		public bool IsFastMode { get; private set; }
	}
}
