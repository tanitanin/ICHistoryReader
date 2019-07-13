using System;
using System.Collections.Generic;
using System.Text;

namespace ICHistoryReader.Cybernetics
{
    public class HistoryInfo
    {
        //+0 (1バイト): 機器種別
        public byte MachineType { get; set; }

        //+1 (1バイト): 利用種別
        //ビット7 (1ビット): 1=現金やカード等の併用 (切符の乗り越し精算をICカードで実施した場合など)
        public bool PaymentCombined { get; set; }

        //+1 (1バイト): 利用種別
        //ビット6〜0 (7ビット)
        public byte UsageType { get; set; }

        //+2 (1バイト): 決済種別
        public byte PaymentType { get; set; }

        //+3 (1バイト): 入出場種別
        public byte EntryType { get; set; }

        //+4〜+5 (2バイト): 年月日 [年/7ビット、月/4ビット、日/5ビット]
        public DateTime Date { get; set; }

        //鉄道
        //+6〜+7 (2バイト): 入場駅コード(窓口で新規発行の場合も駅コードあり)
        //+8〜+9 (2バイト): 出場駅コード(新規発行やチャージの場合は 0000)
        //鉄道(乗車券類購入)
        //+6〜+7 (2バイト): 駅コード
        //+8〜+9 (2バイト): 券売機番号? (00 00が多いが、まれに 01 01 など)
        //バス/路面等
        //+6〜+7 (2バイト): 事業者コード
        //+8〜+9 (2バイト): 停留所コード
        //物販
        //+6〜+7 (2バイト): 決済時刻[時 / 5ビット、分 / 6ビット、秒÷2 / 5ビット]
        //+8〜+9 (2バイト): 決済端末のID
        public ushort EntranceStationCode { get; set; }
        public ushort ExitStationCode { get; set; }
        public ushort StationCode { get; set; }
        public ushort TicketMachineCode { get; set; }
        public ushort BusCode { get; set; }
        public ushort BusStopCode { get; set; }
        public DateTime PaymentTime { get; set; }
        public ushort PaymentId { get; set; }

        //+A〜+B (2バイト): 残額(LE)
        public ushort Balance { get; set; }

        //+D〜+E (2バイト): 履歴連番
        public ushort HistoryNumber { get; set; }

        //+F(1バイト) : 地域コード(0=旧国鉄と関東私鉄/バス、1=中部私鉄/バス、2=関西および沖縄私鉄/バス、3=その他地域私鉄/バス)
        //7〜6 (2ビット): 入場地域コード
        //5〜4 (2ビット): 出場地域コード(新規やチャージでは0)
        //3〜0 (4ビット): 未使用(0)
        public byte EntranceRegionCode { get; set; }
        public byte ExitRegionCode { get; set; }

        public static HistoryInfo FromBytes(byte[] rawData)
        {
            if (rawData.Length != 16)
            {
                throw new NotSupportedException();
            }

            var result = null as HistoryInfo;
            try
            {
                using (var stream = new System.IO.MemoryStream(rawData))
                {
                    using (var reader = new System.IO.BinaryReader(stream))
                    {
                        result = new HistoryInfo();
                        var b0 = reader.ReadByte();
                        result.MachineType = (byte)b0;
                        var b1 = reader.ReadByte();
                        result.PaymentCombined = (b1 & 0b10000000) > 0;
                        result.UsageType = (byte)(b1 & 0b01111111);
                        result.PaymentType = (byte)reader.ReadByte();
                        result.EntryType = (byte)reader.ReadByte();
                        result.Date = ByteConvert.ToDate(reader.ReadBytes(2));

                        var b67 = reader.ReadUInt16();
                        result.EntranceStationCode = result.StationCode = result.BusCode = b67;
                        result.PaymentTime = result.Date.Add(ToPaymentTime(b67));
                        var b89 = reader.ReadUInt16();
                        result.ExitStationCode = result.TicketMachineCode = result.BusStopCode = b89;
                        result.PaymentId = b89;

                        result.Balance = reader.ReadUInt16();
                        reader.ReadByte();
                        result.HistoryNumber = reader.ReadUInt16();
                        var bF = reader.ReadByte();
                        result.EntranceRegionCode = (byte)((bF & 0b11000000) >> 6);
                        result.ExitRegionCode = (byte)((bF & 0b00110000) >> 4);
                    }
                }
            }
            catch { }
            return result;
        }
        private static TimeSpan ToPaymentTime(ushort data)
        {
            var s = (ushort)((data & 0x00FF << 8) | (data & 0xFF00 >> 8));
            //+6〜+7 (2バイト): 決済時刻[時 / 5ビット、分 / 6ビット、秒÷2 / 5ビット]
            var hh = (s & 0xF800) >> 11;
            var mm = (s & 0x07E0) >> 5;
            var ss = ((s & 0x001F) >> 0) * 2;
            return new TimeSpan(hh, mm, ss);
        }
    }
}
