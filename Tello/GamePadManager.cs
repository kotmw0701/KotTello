using SlimDX.XInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tello {
    internal class GamePadManager {
        /*
         * メモ
         * RightX → 右に進む / 左に進む (MAX +-660)
         * RightY → 前進 / 後退
         * LeftX → 右回転 / 左回転
         * LeftY → 上昇 / 下降
         * LTrigger → 低速モード   MAX(110-1.0) DEF(330-0.0)
         * RTrigget → 仮高速モード MAX(660-1.0) DEF(330-0.0)
         * RightStick → 押し込みで入力無効化(というかそうしないと数値が安定しない(パッド壊れてんじゃん
         * LeftStick → 同上
         * 
         * 高速モード
         * RightTrigger LeftTrigger同時押し
         * 
         * (ずっと割ってやろうと思ってたけど，これよく考えたら割合取ったほうが早いやんかなってなった)
         */

        #region プロパティ

        /// <summary>
        /// 1.0 - 0.0 の範囲で帰ってきます
        /// </summary>
        public double LeftX { get; private set; }
        /// <summary>
        /// 1.0 - 0.0 の範囲で帰ってきます
        /// </summary>
        public double LeftY { get; private set; }
        /// <summary>
        /// 1.0 - 0.0 の範囲で帰ってきます
        /// </summary>
        public double RightX { get; private set; }
        /// <summary>
        /// 1.0 - 0.0 の範囲で帰ってきます
        /// </summary>
        public double RightY { get; private set; }
        /// <summary>
        /// 1.0 - 0.0 の範囲で帰ってきます
        /// </summary>
        public double LTrigger { get; private set; }
        /// <summary>
        /// 1.0 - 0.0 の範囲で帰ってきます
        /// </summary>
        public double RTrigger { get; private set; }
        public bool IsFastMode { get; private set; }

        public bool A { get; private set; }
        public bool B { get; private set; }
        public bool X { get; private set; }
        public bool Y { get; private set; }
        public bool Start { get; private set; }
        public bool Back { get; private set; }
        public bool Up { get; private set; }
        public bool Down { get; private set; }
        public bool Right { get; private set; }
        public bool Left { get; private set; }
        public bool RightThumb { get; private set; }
        public bool RightSholder { get; private set; }
        public bool LeftThumb { get; private set; }
        public bool LeftSholder { get; private set; }

        #endregion

        private readonly int interval = 20;
        private Controller controller;
        private CancellationTokenSource tokenSource = null;

        public delegate void StickStream(GamePadManager manager);
        public event StickStream Stream;

        public GamePadManager() : this(20) { }

        public GamePadManager(int interval) {
            this.interval = interval;
        }

        public void StartStream() {
            if (tokenSource == null) {
                tokenSource = new CancellationTokenSource();
                controller = new Controller(UserIndex.One);
                if (!controller.IsConnected) throw new XInputException("Controller is not connected");
            } else return;
            CancellationToken token = tokenSource.Token;

            Task.Factory.StartNew(async () => {
                while (!token.IsCancellationRequested && controller.IsConnected) {
                    var pad = controller.GetState().Gamepad;
                    A = pad.Buttons.HasFlag(GamepadButtonFlags.A);
                    B = pad.Buttons.HasFlag(GamepadButtonFlags.B);
                    X = pad.Buttons.HasFlag(GamepadButtonFlags.X);
                    Y = pad.Buttons.HasFlag(GamepadButtonFlags.Y);
                    Start = pad.Buttons.HasFlag(GamepadButtonFlags.Start);
                    Back = pad.Buttons.HasFlag(GamepadButtonFlags.Back);
                    Up = pad.Buttons.HasFlag(GamepadButtonFlags.DPadUp);
                    Down = pad.Buttons.HasFlag(GamepadButtonFlags.DPadDown);
                    Right = pad.Buttons.HasFlag(GamepadButtonFlags.DPadRight);
                    Left = pad.Buttons.HasFlag(GamepadButtonFlags.DPadLeft);
                    RightSholder = pad.Buttons.HasFlag(GamepadButtonFlags.RightShoulder);
                    LeftSholder = pad.Buttons.HasFlag(GamepadButtonFlags.LeftShoulder);
                    LeftThumb = pad.Buttons.HasFlag(GamepadButtonFlags.LeftThumb);
                    RightThumb = pad.Buttons.HasFlag(GamepadButtonFlags.RightThumb);
                    if (LeftThumb) LeftX = LeftY = 0.0;
                    else {
                        LeftX = Math.Round(pad.LeftThumbX / 32767.0 * 100) / 100;
                        LeftY = Math.Round(pad.LeftThumbY / 32767.0 * 100) / 100;
                    }
                    if (RightThumb) RightX = RightY = 0.0;
                    else {
                        RightX = Math.Round(pad.RightThumbX / 32767.0 * 100) / 100;
                        RightY = Math.Round(pad.RightThumbY / 32767.0 * 100) / 100;
                    }
                    LTrigger = pad.LeftTrigger / 255;
                    RTrigger = pad.RightTrigger / 255;
                    IsFastMode = ((LTrigger == 1.0) && (RTrigger == 1.0));
                    Stream(this);
                    await Task.Delay(10);
                }
            }, token).ContinueWith(t => {
                tokenSource.Dispose();
                tokenSource = null;
            });
        }

        public void StopStream() {
            if (tokenSource == null) return;
            tokenSource.Cancel();
        }
    }
}
