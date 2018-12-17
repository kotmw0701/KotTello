using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Tello {
	class ViewModel : INotifyPropertyChanged {
        private bool _hasController;
        private bool _fastMode;
		private short _rotation;
		private short _throttle;
		private short _pitch;
		private short _role;

		public event PropertyChangedEventHandler PropertyChanged;
		private void RaizePropertyChanged([CallerMemberName]string propertyName = null)
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public bool HasController {
            get => _hasController;
            set {
                _hasController = value;
                RaizePropertyChanged(nameof(HasController));
            }
        }
		public bool FastMode {
			get => _fastMode;
			set {
				_fastMode = value;
				RaizePropertyChanged(nameof(FastMode));
			}
		}
		public short Rotation {
			get => _rotation;
			set {
				_rotation = value;
				RaizePropertyChanged(nameof(Rotation));
			}
		}
		public short Throttle {
			get => _throttle;
			set {
				_throttle = value;
				RaizePropertyChanged(nameof(Throttle));
			}
		}
		public short Pitch {
			get => _pitch;
			set {
				_pitch = value;
				RaizePropertyChanged(nameof(Pitch));
			}
		}
		public short Role {
			get => _role;
			set {
				_role = value;
				RaizePropertyChanged(nameof(Role));
			}
		}
	}
}
