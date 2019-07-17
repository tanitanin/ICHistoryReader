using ICHistoryReader.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICHistoryReader.Core.Cybernetics
{
    public class HistoryInfo
    {
        //+0 (1バイト): 機器種別
        public MachineType MachineType { get; set; }

        //+1 (1バイト): 利用種別
        //ビット7 (1ビット): 1=現金やカード等の併用 (切符の乗り越し精算をICカードで実施した場合など)
        public bool PaymentCombined { get; set; }

        //+1 (1バイト): 利用種別
        //ビット6〜0 (7ビット)
        public UsageType UsageType { get; set; }

        //+2 (1バイト): 決済種別
        public PaymentType PaymentType { get; set; }

        //+3 (1バイト): 入出場種別
        public EntryType EntryType { get; set; }

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
        public StationCodeData EntranceStation { get; set; }
        public StationCodeData ExitStation { get; set; }
        public StationCodeData StationData { get; set; }
        public ushort BusCode { get; set; }
        public ushort BusStopCode { get; set; }
        public DateTime PaymentTime { get; set; }
        public ushort PaymentId { get; set; }

        //+A〜+B (2バイト): 残額(LE)
        public ushort Balance { get; set; }

        /// <summary>
        /// 精算額
        /// </summary>
        public int Amount { get; set; }

        //+D〜+E (2バイト): 履歴連番
        public ushort HistoryNumber { get; set; }

        //+F(1バイト) : 地域コード(0=旧国鉄と関東私鉄/バス、1=中部私鉄/バス、2=関西および沖縄私鉄/バス、3=その他地域私鉄/バス)
        //7〜6 (2ビット): 入場地域コード
        //5〜4 (2ビット): 出場地域コード(新規やチャージでは0)
        //3〜0 (4ビット): 未使用(0)
        public byte RegionCode { get; set; }

        byte[] Raw { get; set; }

        public bool IsBus
        {
            get
            {
                switch ((byte)UsageType)
                {
                    case 13:
                    case 15:
                    case 31:
                    case 35:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public bool IsGoodSale
        {
            get
            {
                switch ((byte)UsageType + (PaymentCombined ? 0x80 : 0x00))
                {
                    case 70:
                    case 73:
                    case 74:
                    case 75:
                    case 198:
                    case 203:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public bool IsTrain
        {
            get => !IsBus && !IsGoodSale;
        }

        public bool IsCommuterPass
        {
            get
            {
                switch ((byte)EntryType)
                {
                    case 3:
                    case 4:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public static HistoryInfo From(ICHistoryReader.Cybernetics.HistoryInfo info, List<StationCodeData> list)
        {
            try
            {
                var result = new HistoryInfo();
                result.Raw = info.Raw;
                result.MachineType = Enum.IsDefined(typeof(MachineType), info.MachineType) ? (MachineType)info.MachineType : MachineType.Unknown;
                result.PaymentCombined = info.PaymentCombined;
                result.UsageType = Enum.IsDefined(typeof(UsageType), info.UsageType) ? (UsageType)info.UsageType : UsageType.Unknown;
                result.PaymentType = Enum.IsDefined(typeof(PaymentType), info.PaymentType) ? (PaymentType)info.PaymentType : PaymentType.Normal;
                result.EntryType = Enum.IsDefined(typeof(EntryType), info.EntryType) ? (EntryType)info.EntryType : EntryType.Normal;
                result.Date = info.Date;
                if (result.IsTrain)
                {
                    result.EntranceStation = list.SingleOrDefault(x => x.RegionCode == info.EntranceRegionCode && x.LineCode == info.EntranceLineCode && x.StationCode == info.EntranceStationCode);
                    result.ExitStation = list.SingleOrDefault(x => x.RegionCode == info.ExitRegionCode && x.LineCode == info.ExitLineCode && x.StationCode == info.ExitStationCode);
                    result.StationData = result.EntranceStation;
                }
                else if (result.IsBus)
                {
                    result.BusCode = info.BusCode;
                    result.BusStopCode = info.BusStopCode;
                }
                else if (result.IsGoodSale)
                {
                    result.PaymentTime = info.PaymentTime;
                    result.PaymentId = info.PaymentId;
                }
                result.Balance = info.Balance;
                result.HistoryNumber = info.HistoryNumber;
                result.RegionCode = info.EntranceRegionCode;
                return result;
            }
            catch
            {
                return default;
            }
        }
    }
}
