﻿using System;
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
        //05 バス/路面等
        //07 自動券売機?
        //08 自動券売機
        //09 SMART ICOCA クイックチャージ機、nimocaポイント交換機
        //12 自動券売機
        //14 駅窓口 (窓口でSMART ICOCAを作ると14になる)
        //15 定期券発売機
        //16 自動改札機
        //17 簡易改札機
        //18 駅窓口 (再発行など) (指定席券売機も?)
        //19 窓口処理機(有人改札)
        //1A 窓口処理機(有人改札)
        //1B パソリ等
        //1C のりこし精算機
        //1D 他社線のりかえ自動改札機
        //1F 入金機、簡易入金機
        //20 窓口端末(名鉄)
        //21 精算機
        //22 窓口処理機/簡易改札機/バス等 (カード種類ごとに異なる用途がある模様)
        //23 新幹線改札機
        //24 車内補充券発行機
        //46 VIEW ALTTE、特典など
        //48 ポイント交換機(nimoca)
        //C7 物販/タクシー等
        //C8 物販/タクシー等
    }

    public enum UsageType : byte
    {
        Unknown = 0x00,
        //01 自動改札機出場/有人改札出場
        //02 SFチャージ
        //03 乗車券類購入
        //04 精算(乗り越し等)
        //05 精算(乗り越し等)
        //06 窓口出場
        //07 新規
        //08 チャージ控除(返金)
        //0C バス/路面等(均一運賃?)
        //0D バス/路面等(均一運賃)
        //0F バス/路面等
        //10 再発行?
        //11 再発行?
        //13 自動改札機出場?(新幹線)
        //14 オートチャージ
        //17 オートチャージ(PiTaPa)
        //19 バスの精算?
        //1A バスの精算?
        //1B バスの精算 (障害者割引などの精算)
        //1D リムジンバス等
        //1F チャージ(バス/窓口)、チャージ機(OKICA)
        //23 乗車券類購入 (都バスIC一日乗車券など)
        //33 取り消し(残高返金)
        //46 物販
        //48 ポイントチャージ
        //49 SFチャージ(物販扱い)
        //4A 物販の取消
    }

    public enum PaymentType : byte
    {
        //00 通常決済
        Normal = 0x00,
        //02 VIEWカード
        //0B PiTaPa (物販等)
        //0C 一般のクレジットカード?
        //0D パスネット/PASMO
        //13 nimoca (nimocaポイント交換機でのクイックチャージ)
        //1E nimoca (チャージ時のSF還元)
        //3F モバイルSuicaアプリ(クレジットカード)
    }

    public enum EntryType : byte
    {
        //00 通常出場および精算以外(新規、チャージ、乗車券類購入、物販等)
        Normal = 0x00,
        //01 入場(オートチャージ)
        //02 入場+出場(SF)
        //03 定期入場→乗り越し精算出場(SF)
        //04 定期券面前乗車入場(SF)→定期出場
        //05 乗継割引(鉄道)
        //0E 窓口出場
        //0F バス/路面等の精算
        //17 乗継割引(バス→鉄道?)
        //1D 乗継割引(バス)
        //21 乗継精算(筑豊電鉄 指定駅乗継、熊本市交通局 辛島町電停 A系統↔B系統など)
        //22 券面外乗降?
    }

}
