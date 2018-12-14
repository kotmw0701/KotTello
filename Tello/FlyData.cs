using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tello {
    class FlyData {
        private int flyMode, height, verticalSpeed, flySpeed, eastSpeed, northSpeed, flyTime;

        private bool isFlying, downVisualState, isDroneHover, iseMOpen, onGround, pressureState;

        private int batteryParcentage, droneBatteryLeft, droneFlyTimeLeft;

        private bool isBatteryLow, isBatteryLower, batteryState, powerState;

        private int cameraState, electricalMachineryState, imuCalibrationState, lightStrength, smartVideoExitMode, temperatureHeight, throwFlyTimer, wifiDisturb, wifiStrength;

        private bool isFactoryMode, isFrontIn, isFrontLSC, isFrontOut, isGravityState, imuState, isOutangeRecording, windState;

        private float velX, velY, velZ, posX, posY, posZ, posUncertainty, velN, velE, velD, quatX, quatY, quatZ, quatW;

        public void Set(byte[] data) {
            int index = 0;
            height =        (int)(data[++index] | data[++index] << 8);
            northSpeed =    (int)(data[++index] | data[++index] << 8);
            eastSpeed =     (int)(data[++index] | data[++index] << 8);
            flySpeed =      (int)Math.Sqrt(Math.Pow(northSpeed, 2.0) + Math.Pow(eastSpeed, 2.0));
            verticalSpeed = (int)(data[++index] | data[++index] << 8);
            flyTime =       (int)(data[++index] | data[++index] << 8);

            imuState =          (data[index] >> 0 & 1) == 1;
            pressureState =     (data[index] >> 1 & 1) == 1;
            downVisualState =   (data[index] >> 2 & 1) == 1;
            powerState =        (data[index] >> 3 & 1) == 1;
            batteryState =      (data[index] >> 4 & 1) == 1;
            isGravityState =    (data[index] >> 5 & 1) == 1;
            windState =         (data[index] >> 6 & 1) == 1;
            index++;

            imuCalibrationState = data[++index];
            batteryParcentage = data[++index];
            droneFlyTimeLeft = (int)(data[++index] | data[++index] << 8);
            droneBatteryLeft = (int)(data[++index] | data[++index] << 8);

            isFlying =              (data[index] >> 0 & 1) == 1;
            onGround =              (data[index] >> 1 & 1) == 1;
            iseMOpen =              (data[index] >> 2 & 1) == 1;
            isDroneHover =          (data[index] >> 3 & 1) == 1;
            isOutangeRecording =    (data[index] >> 4 & 1) == 1;
            isBatteryLow =          (data[index] >> 5 & 1) == 1;
            isBatteryLower =        (data[index] >> 6 & 1) == 1;
            isFactoryMode =         (data[index] >> 7 & 1) == 1;
            index++;

            flyMode = data[++index];
            throwFlyTimer = data[++index];
            cameraState = data[++index];

            electricalMachineryState = data[++index];

            isFrontIn =     (data[index] >> 0 & 1) == 1;
            isFrontOut =    (data[index] >> 1 & 1) == 1;
            isFrontLSC =    (data[index] >> 2 & 1) == 1;
            index++;
            temperatureHeight = (data[index] >> 0 & 1);

            wifiStrength = 0;
        }

        public void parseLog(byte[] data) {
            int pos = 0;
            while (pos < data.Length-2) {
                if (data[pos] != 'U') break;
                var length = data[pos + 1];
                if (data[pos + 2] != 0) break;
                var crc = data[pos + 3];
                var id = BitConverter.ToUInt16(data, pos + 4);
                var xorBuf = new byte[256];
                byte xorValue = data[pos + 6];
                var index = 10;
                switch (id) {
                    case 0x1d:
                        for (var i = 0; i < length; i++) xorBuf[i] = (byte)(data[pos + i] ^ xorValue);
                        var observationCount = BitConverter.ToUInt16(xorBuf, index);
                        index += 2;
                        velX = BitConverter.ToInt16(xorBuf, index);
                        index += 2;
                        velY = BitConverter.ToInt16(xorBuf, index);
                        index += 2;
                        velZ = BitConverter.ToInt16(xorBuf, index);
                        index += 2;
                        posX = BitConverter.ToSingle(xorBuf, index);
                        index += 2;
                        posY = BitConverter.ToSingle(xorBuf, index);
                        index += 2;
                        posZ = BitConverter.ToSingle(xorBuf, index);
                        index += 2;
                        posUncertainty = BitConverter.ToSingle(xorBuf, index) * 10000.0f;
                        index += 4;
                        break;
                    case 0x0800:
                        for (var i = 0; i < length; i++) xorBuf[i] = (byte)(data[pos + i] ^ xorValue);
                        index += 48;
                        quatW = BitConverter.ToSingle(xorBuf, index);
                        index += 4;
                        quatX = BitConverter.ToSingle(xorBuf, index);
                        index += 4;
                        quatY = BitConverter.ToSingle(xorBuf, index);
                        index += 4;
                        quatZ = BitConverter.ToSingle(xorBuf, index);
                        index += 4;

                        index = 10 + 76;
                        velN = BitConverter.ToSingle(xorBuf, index);
                        index += 4;
                        velE = BitConverter.ToSingle(xorBuf, index);
                        index += 4;
                        velD = BitConverter.ToSingle(xorBuf, index);
                        index += 4;
                        break;
                    default:
                        break;
                }
                pos += length;
            }
        }

        public double[] toElter() {
            float qX = quatX, qY = quatY, qZ = quatZ, qW = quatW;

            double sqW = Math.Pow(qW, 2), sqX = Math.Pow(qX, 2), sqY = Math.Pow(qY, 2), sqZ = Math.Pow(qZ, 2);
            double yaw = 0.0, roll = 0.0, pitch = 0.0, unit = sqW + sqX + sqY + sqZ;
            double[] retv = new double[3];
            double test = qW * qX + qY * qZ;

            if(test > 0.499 * unit) {
                yaw = 2 * Math.Atan2(qY, qW);
                pitch = 2 / Math.PI;
                roll = 0;
            } else if (test < -0.499 * unit) {
                yaw = -2 * Math.Atan2(qY, qW);
                pitch = -2 / Math.PI;
                roll = 0;
            } else {
                yaw = Math.Atan2(2.0 * (qW * qZ - qX * qY), 1.0 - 2.0 * (sqZ + sqX));
                roll = Math.Asin(2.0 * test / unit);
                pitch = Math.Atan2(2.0 * (qW * qY - qX * qZ), 1.0 - 2.0 * (sqY + sqX));
            }

            retv[0] = pitch;
            retv[1] = roll;
            retv[2] = yaw;
            return retv;
        }

        public string GetLogHeader() {
            StringBuilder sb = new StringBuilder();
            foreach (FieldInfo property in this.GetType().GetFields()) {
                sb.Append(property.Name);
                sb.Append(",");
            }
            sb.AppendLine();
            return sb.ToString();
        }

        public string GetLogLine() {
            StringBuilder sb = new StringBuilder();
            foreach (FieldInfo property in this.GetType().GetFields()) {
                if (property.FieldType == typeof(Boolean)) {
                    if ((bool)property.GetValue(this))
                        sb.Append("1");
                    else
                        sb.Append("0");
                } else sb.Append(property.GetValue(this));
                sb.Append(",");
            }
            sb.AppendLine();
            return sb.ToString();
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            var count = 0;
            foreach (FieldInfo property in this.GetType().GetFields()) {
                sb.Append(property.Name).Append(": ").Append(property.GetValue(this));
                if (count++ % 2 == 1) sb.Append(System.Environment.NewLine);
                else sb.Append("      ");

            }
            return sb.ToString();
        }
    }
}