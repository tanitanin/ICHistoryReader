using System;
using System.Collections.Generic;
using System.Text;

namespace ICHistoryReader.Cybernetics
{
    public class CardInfo
    {
        //+0〜+7 (8バイト): (不明)

        //+8 (1バイト)
        //7〜4 (4ビット): カード種別
        //0=EX-IC
        //2=Suica、PASMO、TOICA、manaca、PiTaPa、nimoca、SUGOCA、はやかけん
        //3=ICOCA
        //3〜0 (4ビット): 最終決済地域コード(随時書き換わるもので、カード種別判定には使えない)
        //0=旧国鉄と関東私鉄/バス
        //1=中部私鉄/バス
        //2=関西私鉄/バス
        //3=その他地域私鉄/バス
        public CardType CardType { get; set; }
        public LastPaymentRegionCode LastPaymentRegionCode { get; set; }

        //+9〜+A(2バイト) : 0x00 0x00 (不明)

        //+B〜+C(2バイト) : カード残額(LE)
        public ushort Balance { get; set; }

        //+D(1バイト) : 0x00

        //+E〜+F(2バイト) : 更新番号
        public ushort UpdateNumber { get; set; }
    }
}
