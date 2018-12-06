using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tello {
    class GamePadManager {
        /*
         * メモ
         * RightX → 右に進む / 左に進む
         * RightY → 前進 / 後退
         * LeftX → 右回転 / 左回転
         * LeftY → 上昇 / 下降
         * LTrigger → 低速モード   MAX(330-255) DEF(110-0)
         * RTrigget → 仮高速モード MAX(660-255) DEF(110-0)
         * RightStick → 押し込みで入力無効化(というかそうしないと数値が安定しない(パッド壊れてんじゃん
         * LeftStick → 同上
         * 
         * 高速モード
         * RTrigger押し込み→LTrigger押し込み？
         * 
         * 
         */
        public short LeftX { get; set; }
        public short LeftY { get; set; }
        public short RightX { get; set; }
        public short RightY { get; set; }
        public byte LTrigger { get; set; }
        public byte RTrigget { get; set; }


    }
}
