using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tello {
	internal struct ControllData {
		public readonly double Rotation;
		public readonly double Throttle;
		public readonly double Pitch;
		public readonly double Role;
		public readonly bool IsFastMode;

		public ControllData(GamePadManager manager) : this(manager.LeftX, manager.LeftY, manager.RightY, manager.RightX, manager.IsFastMode) { }

		public ControllData(double rotation, double throttle, double pitch, double role, bool isfastmode) {
			Rotation = rotation;
			Throttle = throttle;
			Pitch = pitch;
			Role = role;
			IsFastMode = isfastmode;
		}
	}
}
