using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICHistoryReader.Cybernetics
{
    public class GateHistoryInfo
    {
        //+0 (1バイト): 入出場情報
        //7 (1ビット): 入出場(0=出場、1=入場) (EX-ICは常に0)
        //6〜4 (3ビット): 入出場種類(0=精算、2=SF、4=定期券)
        //3〜0 (4ビット): 入出場種類(0=通常、1=精算、2=割引、4=???)
        public bool IsEnterance { get; set; }
        public byte EntryType1 { get; set; }
        public byte EntryType2 { get; set; }

        //+1 (1バイト): (不明) (定数ではなく、ビット情報の可能性もある)
        //00=通常?
        //04=乗継割引(バス→電車乗継割引?)
        //08=乗継割引(バス→バス乗継割引?)
        //0C=乗継割引(電車→バス乗継割引?)
        //40=EX-IC入場?
        public byte TransitType { get; set; }

        //+2〜+5 (4バイト): 改札情報(090Fと違って地域コードは存在しない)
        //鉄道
        //+2〜+3 (2バイト): 入出場駅コード
        //+4〜+5 (2バイト): 改札・装置コード
        //バス
        //+2〜+3 (2バイト): 事業者コード
        //+4〜+5 (2バイト): 車両番号等
        public ushort StationCode { get; set; }
        public ushort GateCode { get; set; }
        public ushort BusCode { get; set; }
        public ushort BusId { get; set; }

        //+6〜+7 (2バイト): 年月日[年 / 7ビット、月 / 4ビット、日 / 5ビット]
        //+8〜+9 (2バイト): 時刻(BCD)
        //15〜8 (8ビット): 時(BCD)
        //7〜0 (8ビット): 分(BCD)
        public DateTime DateTime { get; set; }

        //+A〜+B(2バイト) : 精算金額(LE)
        public ushort Payment { get; set; }

        //+C〜+F(4バイト)
        //鉄道
        //+C〜+F(4バイト) : (不明)
        //バス
        //+C〜+D(2バイト) : (未使用 ?)
        //+E〜+F(2バイト) : 駅コード/停留所コード
        public ushort BusStopCode { get; set; }


        public static GateHistoryInfo FromBytes(byte[] rawData)
        {
            if (rawData.Length != 16)
            {
                throw new NotSupportedException();
            }

            var result = null as GateHistoryInfo;
            try
            {
                using (var stream = new System.IO.MemoryStream(rawData))
                {
                    using (var reader = new System.IO.BinaryReader(stream))
                    {
                        result = new GateHistoryInfo();
                        var b0 = reader.ReadByte();
                        result.IsEnterance = ((b0 & 0b10000000) > 0);
                        result.EntryType1 = (byte)((b0 & 0b01110000) >> 4);
                        result.EntryType2 = (byte)((b0 & 0b00001111) >> 0);
                        var b1 = reader.ReadByte();
                        result.TransitType = b1;
                        var b23 = reader.ReadUInt16();
                        result.StationCode = result.BusCode = b23;
                        var b45 = reader.ReadUInt16();
                        result.GateCode = result.BusId = b45;
                        var b6789 = reader.ReadBytes(4);
                        result.DateTime = ByteConvert.ToDateTimeBCD(b6789);
                        var bAB = reader.ReadUInt16();
                        result.Payment = bAB;
                        var bCD = reader.ReadBytes(2);
                        var bEF = reader.ReadUInt16();
                        result.BusStopCode = bEF;
                    }
                }
            }
            catch { }
            return result;
        }
    }
}
