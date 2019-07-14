using System;
using System.Collections.Generic;
using System.Linq;
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
        //+2〜+3 (2バイト): 時刻(BCD)
        public DateTime EntryTime { get; set; }
        //+4〜+5 (2バイト): 入場駅コード
        public ushort EntryStationCode { get; set; }
        //+6 (1バイト): (不明)
        //+7〜+B(5バイト) : 出場中間改札
        //+7〜+8 (2バイト): 時刻(BCD)
        public DateTime ExitTime { get; set; }
        //+9〜+A(2バイト) : 出場駅コード
        public ushort ExitStationCode { get; set; }
        //+B(1バイト) : (不明)
        //+C(1バイト) : (不明)
        //7〜6 (2ビット): 地域コード(書き込まれないこともある)
        public byte RegionCode { get; set; }
        //5〜0 (6ビット): 不明
        //+D(1バイト) : (不明)
        //7〜6 (2ビット): 不明(0〜3全てを確認)
        //5〜0 (6ビット): 不明(0)
        //+E〜+F(2バイト) : (不明)

        public byte[] Raw { get; private set; }

        public static SFEntryInfo FromBytes(byte[] rawData)
        {
            if (rawData.Length != 32)
            {
                throw new NotSupportedException();
            }

            var result = null as SFEntryInfo;
            try {
                using (var stream = new System.IO.MemoryStream(rawData))
                {
                    using (var reader = new System.IO.BinaryReader(stream))
                    {
                        result = new SFEntryInfo();
                        result.Raw = rawData;
                        result.Region1 = reader.ReadUInt16();
                        result.Region2 = reader.ReadUInt16();
                        result.Region3 = reader.ReadUInt16();
                        result.Region4 = reader.ReadUInt16();
                        result.TransitionDate = ByteConvert.ToDate(reader.ReadBytes(2));
                        result.EntryTime = result.TransitionDate.Add(ByteConvert.ToTimeBCD(reader.ReadBytes(2)));
                        result.EntryStationCode = reader.ReadUInt16();
                        reader.ReadByte();
                        result.ExitTime = result.TransitionDate.Add(ByteConvert.ToTimeBCD(reader.ReadBytes(2)));
                        result.ExitStationCode = reader.ReadUInt16();
                        reader.ReadByte();
                        var bC = reader.ReadByte();
                        result.RegionCode = (byte)((bC & 0b11000000) >> 6);
                    }
                }
            }
            catch { }
            return result;
        }
    }
}
