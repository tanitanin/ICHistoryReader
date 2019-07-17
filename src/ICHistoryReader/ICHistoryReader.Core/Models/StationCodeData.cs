using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ICHistoryReader.Core.Models
{
    public class StationCodeData
    {
        /// <summary>
        /// 地区コード
        /// </summary>
        public ushort RegionCode { get; private set; }

        /// <summary>
        /// 線区コード
        /// </summary>
        public ushort LineCode { get; private set; }

        /// <summary>
        /// 駅順コード
        /// </summary>
        public ushort StationCode { get; private set; }

        /// <summary>
        /// 会社名
        /// </summary>
        public string CompanyName { get; private set; }

        /// <summary>
        /// 線区名
        /// </summary>
        public string LineName { get; private set; }

        /// <summary>
        /// 駅名
        /// </summary>
        public string StationName { get; private set; }

        /// <summary>
        /// 備考
        /// </summary>
        public string Note { get; private set; }

        /// <summary>
        /// 緯度
        /// </summary>
        public string Latitude { get; private set; }

        /// <summary>
        /// 経度
        /// </summary>
        public string Longitude { get; private set; }

        /// <summary>
        /// 一覧を取得する
        /// </summary>
        public static async Task<List<StationCodeData>> LoadAsync(string path)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var file = await Windows.Storage.StorageFile.GetFileFromPathAsync(path);
            var list = new List<StationCodeData>();
            using (var stream = await file.OpenReadAsync())
            {
                using (var t = new StreamReader(stream.AsStreamForRead(), System.Text.Encoding.GetEncoding("shift_jis")))
                {
                    while (t.Peek() >= 0)
                    {
                        var line = t.ReadLine();
                        var token = line.Split(new char[] { ',' }, StringSplitOptions.None);
                        try
                        {
                            list.Add(new StationCodeData()
                            {
                                RegionCode = ushort.TryParse(token[0], out var rc) ? rc : (ushort)0,
                                LineCode = ushort.TryParse(token[1], System.Globalization.NumberStyles.HexNumber, null, out var lc) ? lc : (ushort)0,
                                StationCode = ushort.TryParse(token[2], System.Globalization.NumberStyles.HexNumber, null, out var sc) ? sc : (ushort)0,
                                CompanyName = token[3],
                                LineName = token[4],
                                StationName = token[5],
                                Note = token[6],
                                Latitude = token[7],
                                Longitude = token[8],
                            });
                        }
                        catch { }
                    }
                }
            }
            return list;
        }
    }
}
