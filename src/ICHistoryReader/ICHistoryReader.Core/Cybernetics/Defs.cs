using System;
using System.Collections.Generic;
using System.Text;

namespace ICHistoryReader.Core.Cybernetics
{

    public enum CardType
    {
        //0=EX-IC
        ExIC = 0x00,
        //2=Suica、PASMO、TOICA、manaca、PiTaPa、nimoca、SUGOCA、はやかけん
        IC = 0x02,
        //3=ICOCA
        ICOCA = 0x03,
    }

    public enum LastPaymentRegionCode
    {
        //0=旧国鉄と関東私鉄/バス
        General = 0x00,
        //1=中部私鉄/バス
        CentralPrivate = 0x01,
        //2=関西私鉄/バス
        WestPrivate = 0x02,
        //3=その他地域私鉄/バス
        Etc = 0x03,
    }

    public enum MachineType : byte
    {
        Unknown = 0x00,
        //03 のりこし精算機
        OverstationFee = 0x03,
        //05 バス/路面等
        Bus = 0x05,
        //07 自動券売機?
        ATM1 = 0x07,
        //08 自動券売機
        ATM2 = 0x08,
        //09 SMART ICOCA クイックチャージ機、nimocaポイント交換機
        SmartICOCA = 0x09,
        //12 自動券売機
        ATM3 = 0x12,
        //14 駅窓口 (窓口でSMART ICOCAを作ると14になる)
        Desk = 0x14,
        //15 定期券発売機
        CommuterATM = 0x15,
        //16 自動改札機
        Gate = 0x16,
        //17 簡易改札機
        SimpleGate = 0x17,
        //18 駅窓口 (再発行など) (指定席券売機も?)
        Desk2 = 0x18,
        //19 窓口処理機(有人改札)
        DeskGate1 = 0x19,
        //1A 窓口処理機(有人改札)
        DeskGate2 = 0x1A,
        //1B パソリ等
        PaSoRi = 0x1B,
        //1C のりこし精算機
        OverStationATM = 0x1C,
        //1D 他社線のりかえ自動改札機
        TransitGate = 0x1D,
        //1F 入金機、簡易入金機
        DepositMachine = 0x1F,
        //20 窓口端末(名鉄)
        DeskMachine = 0x20,
        //21 精算機
        FeeATM = 0x21,
        //22 窓口処理機/簡易改札機/バス等 (カード種類ごとに異なる用途がある模様)
        DeskOrSimpleATM = 0x22,
        //23 新幹線改札機
        ExpressGate = 0x23,
        //24 車内補充券発行機
        OnTrain = 0x24,
        //46 VIEW ALTTE、特典など
        ViewSpecial = 0x46,
        //48 ポイント交換機(nimoca)
        PointExchange = 0x48,
        //C7 物販/タクシー等
        GoodOrTaxi1 = 0xC7,
        //C8 物販/タクシー等
        GoodOrTaxi2 = 0xC8,
    }

    public enum UsageType : byte
    {
        Unknown = 0x00,
        //01 自動改札機出場/有人改札出場
        Gate = 0x01,
        //02 SFチャージ
        SFCharge = 0x02,
        //03 乗車券類購入
        BuyTicket = 0x03,
        //04 精算(乗り越し等)
        Fee1 = 0x04,
        //05 精算(乗り越し等)
        Fee2 = 0x05,
        //06 窓口出場
        BoothExit = 0x06,
        //07 新規
        New = 0x07,
        //08 チャージ控除(返金)
        ChargeCancel = 0x08,
        //0C バス/路面等(均一運賃?)
        BusFee1 = 0x0C,
        //0D バス/路面等(均一運賃)
        BusFee2 = 0x0D,
        //0F バス/路面等
        BusFee3 = 0x0F,
        //10 再発行?
        Reissue1 = 0x10,
        //11 再発行?
        Reissue2 = 0x11,
        //13 自動改札機出場?(新幹線)
        ExpressGate = 0x13,
        //14 オートチャージ
        AutoCharge = 0x14,
        //17 オートチャージ(PiTaPa)
        AutoChargePiTaPa = 0x17,
        //19 バスの精算?
        BusFee4 = 0x19,
        //1A バスの精算?
        BusFee5 = 0x1A,
        //1B バスの精算 (障害者割引などの精算)
        BusFee6 = 0x1B,
        //1D リムジンバス等
        LimousineBus = 0x1D,
        //1F チャージ(バス/窓口)、チャージ機(OKICA)
        ChargeInBusOKICA = 0x1F,
        //23 乗車券類購入 (都バスIC一日乗車券など)
        BuySpecialTicket = 0x23,
        //33 取り消し(残高返金)
        Cancel = 0x33,
        //46 物販
        GoodSale = 0x46,
        //48 ポイントチャージ
        PointCharge = 0x48,
        //49 SFチャージ(物販扱い)
        SFChargeAsGoodSale = 0x49,
        //4A 物販の取消
        GoodSaleCancel = 0x4A,
    }

    public enum PaymentType : byte
    {
        //00 通常決済
        Normal = 0x00,
        //02 VIEWカード
        VIEWCard = 0x02,
        //0B PiTaPa (物販等)
        PiTaPa = 0x0B,
        //0C 一般のクレジットカード?
        CreditCard = 0x0C,
        //0D パスネット/PASMO
        PASMO = 0x0D,
        //13 nimoca (nimocaポイント交換機でのクイックチャージ)
        nimoca = 0x13,
        //1E nimoca (チャージ時のSF還元)
        nimocaSF = 0x1E,
        //3F モバイルSuicaアプリ(クレジットカード)
        MobileSuica = 0x3F,
    }

    public enum EntryType : byte
    {
        //00 通常出場および精算以外(新規、チャージ、乗車券類購入、物販等)
        NormalPay = 0x00,
        //01 入場(オートチャージ)
        EnterAutoCharge = 0x01,
        //02 入場+出場(SF)
        EnterAndExitSF = 0x02,
        //03 定期入場→乗り越し精算出場(SF)
        EnterCommuterAndExitSF = 0x03,
        //04 定期券面前乗車入場(SF)→定期出場
        EnterSFAndExitCommuter = 0x04,
        //05 乗継割引(鉄道)
        TrainTransit = 0x05,
        //0E 窓口出場
        ExitThroughDesk = 0x0E,
        //0F バス/路面等の精算
        BusPayment = 0x0F,
        //17 乗継割引(バス→鉄道?)
        TransitDiscountBusToTrain = 0x17,
        //1D 乗継割引(バス)
        TransitDiscountBus = 0x1D,
        //21 乗継精算(筑豊電鉄 指定駅乗継、熊本市交通局 辛島町電停 A系統↔B系統など)
        TransitFee = 0x21,
        //22 券面外乗降?
        Etc = 0x22,
    }

}
