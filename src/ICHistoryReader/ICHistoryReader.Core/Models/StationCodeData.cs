using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public byte RegionCode { get; private set; }

        /// <summary>
        /// 線区コード
        /// </summary>
        public byte LineCode { get; private set; }

        /// <summary>
        /// 駅順コード
        /// </summary>
        public byte StationCode { get; private set; }

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
                            var ac = byte.Parse(token[0]);
                            var lc = byte.Parse(token[1], System.Globalization.NumberStyles.HexNumber);
                            var sc = byte.Parse(token[2], System.Globalization.NumberStyles.HexNumber);
                            list.Add(new StationCodeData()
                            {
                                RegionCode = ac,
                                LineCode = lc,
                                StationCode = sc,
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
