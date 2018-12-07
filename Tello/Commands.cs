using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tello {
    class Commands {
        /* メモ (初期値について)
         * ・crc8とcrc16はここでの定義時は0x00でおｋ → CRCクラスで計算して入れてくれる
         * ・sequenceも0x00でいい → Send時にインクリメントする
         * ・packetLenもこれ計算して入れたほうが良いんじゃね？ → 固定だし大丈夫でしょ
         * 
         * PacketType
         *  0 : Telloからなら 1 になる
         *  1 : Telloへなら 1 になる
         *  2 : ┐
         *  3 : ├ 3-bitのパケットタイプ(？)
         *  4 : ┘
         *  5 : ┐
         *  6 : ├ 3-bitのサブパケットタイプ(？)
         *  7 : ┘
         * 
         */
        //                                        0     1     2     3     4     5     6     7     8     9     10    11    12    13    14    15    16    17    18    19    20    21    22
        //                                        head  packetLen   crc8  type  commandID    sequence     crc16
        public static readonly byte[] TAKEOFF = { 0xCC, 0x58, 0x00, 0x00, 0x68, 0x54, 0x00, 0x00, 0x00, 0x00, 0x00};//データ部分はありません
        //                                        head  packetLen   crc8  type  commandID    sequence   data    crc16
        public static readonly byte[] LAND =    { 0xCC, 0x60, 0x00, 0x00, 0x68, 0x55, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        //                                        head  packetLen   crc8  type  commandID    sequence   [          JoystickData          ]  hour  min   sec   millisecond   crc16
        public static readonly byte[] STICK =   { 0xCC, 0xB0, 0x00, 0x00, 0x60, 0x50, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        public static readonly byte[] TEMP =    { 0xCC, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
    }
}
