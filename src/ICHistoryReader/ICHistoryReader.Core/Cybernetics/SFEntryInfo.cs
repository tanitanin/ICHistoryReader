﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ICHistoryReader.Core.Cybernetics
{
    class SFEntryInfo
    {
        //1ブロック目
        //+0〜+1 (2バイト): 領域1
        //+2〜+3 (2バイト): 領域2
        //+4〜+5 (2バイト): 領域3
        //+6〜+7 (2バイト): 領域4
        //+4〜+F(12バイト) : (不明)

        //2ブロック目
        //+0〜+1 (2バイト): 中間改札年月日[年 / 7ビット、月 / 4ビット、日 / 5ビット]
        //+2〜+6 (5バイト): 入場中間改札
        //+2〜+3 (2バイト): 時刻(BCD)
        //+4〜+5 (2バイト): 入場駅コード
        //+6 (1バイト): (不明)
        //+7〜+B(5バイト) : 出場中間改札
        //+7〜+8 (2バイト): 時刻(BCD)
        //+9〜+A(2バイト) : 出場駅コード
        //+B(1バイト) : (不明)
        //+C(1バイト) : (不明)
        //7〜6 (2ビット): 地域コード(書き込まれないこともある)
        //5〜0 (6ビット): 不明
        //+D(1バイト) : (不明)
        //7〜6 (2ビット): 不明(0〜3全てを確認)
        //5〜0 (6ビット): 不明(0)
        //+E〜+F(2バイト) : (不明)
    }
}
