using System;
using System.Collections.Generic;
using System.Text;

namespace ICHistoryReader.Cybernetics
{
    public class SFEntryInfo
    {
        //1ブロック目
        //+0〜+1 (2バイト): 領域1
        public ushort Region1 { get; set; }
        //+2〜+3 (2バイト): 領域2
        public ushort Region2 { get; set; }
        //+4〜+5 (2バイト): 領域3
        public ushort Region3 { get; set; }
        //+6〜+7 (2バイト): 領域4
        public ushort Region4 { get; set; }
        //+4〜+F(12バイト) : (不明)

        //2ブロック目
        //+0〜+1 (2バイト): 中間改札年月日[年 / 7ビット、月 / 4ビット、日 / 5ビット]
        public DateTime TransitionDate { get; set; }
        //+2〜+6 (5バイト): 入場中間改札
        public byte[] EntryTransitGate { get; set; }
        //+2〜+3 (2バイト): 時刻(BCD)
        public DateTime EntryTime { get; set; }
        //+4〜+5 (2バイト): 入場駅コード
        public ushort EntryStationCode { get; set; }
        //+6 (1バイト): (不明)
        //+7〜+B(5バイト) : 出場中間改札
        public byte[] ExitTransitGate { get; set; }
        //+7〜+8 (2バイト): 時刻(BCD)
        public DateTime ExitTime { get; set; }
        //+9〜+A(2バイト) : 出場駅コード
        public ushort ExitStationCode { get; set; }
        //+B(1バイト) : (不明)
        //+C(1バイト) : (不明)
        //7〜6 (2ビット): 地域コード(書き込まれないこともある)
        public ushort RegionCode { get; set; }
        //5〜0 (6ビット): 不明
        //+D(1バイト) : (不明)
        //7〜6 (2ビット): 不明(0〜3全てを確認)
        //5〜0 (6ビット): 不明(0)
        //+E〜+F(2バイト) : (不明)
    }
}
