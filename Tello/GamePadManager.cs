using SlimDX.XInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
         * RTrigger押し込み→LTrigger押し込み？(20ms以内)
         * 
         * (ずっと割ってやろうと思ってたけど，これよく考えたら割合取ったほうが早いやんかなってなった)
         */
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

        private readonly Controller controller;
        private bool fastChangeFlag, triggerFlag;


        public GamePadManager() {
            controller = new Controller(UserIndex.One);
        }

        public void StickStreaming() {

            var pad = controller.GetState().Gamepad;
            bool onLeftThumb = pad.Buttons.HasFlag(GamepadButtonFlags.LeftThumb);
            bool onRightThumb = pad.Buttons.HasFlag(GamepadButtonFlags.RightThumb);
            if (onLeftThumb) LeftX = LeftY = 0.0;
            else {
                LeftX = Math.Round(pad.LeftThumbX / 32767.0 * 100) / 100;
                LeftY = Math.Round(pad.LeftThumbY / 32767.0 * 100) / 100;
            }
            if (onRightThumb) RightX = RightY = 0.0;
            else {
                RightX = Math.Round(pad.RightThumbX / 32767.0 * 100) / 100;
                RightY = Math.Round(pad.RightThumbY / 32767.0 * 100) / 100;
            }
            LTrigger = pad.LeftTrigger / 255;
            RTrigger = pad.RightTrigger / 255;
            if (RTrigger == 1.0) {
                if (fastChangeFlag && LTrigger == 1.0) {
                    IsFastMode = true;
                    return;
                }
                if (!triggerFlag) {
                    fastChangeFlag = true;
                    triggerFlag = true;
                } else
                    fastChangeFlag = false;
            } else {
                triggerFlag = false;
                fastChangeFlag = false;
                IsFastMode = false;
            }
        }
    }
}
