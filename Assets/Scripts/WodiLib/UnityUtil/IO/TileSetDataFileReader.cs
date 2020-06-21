using System;
using System.Collections.Generic;
using WodiLib.Map;

namespace WodiLib.UnityUtil.IO
{
    public class TileSetDataFileReader : WoditorFileReader<TileSetData>
    {
        public TileSetDataFileReader() { }

        protected override TileSetData Read()
        {
            return ReadData();
        }

        private TileSetData ReadData()
        {
            // ヘッダ
            ReadHeader();

            // タイルセット設定
            ReadTileSetSetting(out var settings);

            // フッタ
            ReadFooter();

            return new TileSetData
            {
                TileSetSettingList = new TileSetSettingList(settings)
            };
        }

        /// <summary>
        /// ヘッダ
        /// </summary>
        /// <param name="ReadStatus">読み込み経過状態</param>
        /// <exception cref="InvalidOperationException">ファイルヘッダが仕様と異なる場合</exception>
        private void ReadHeader()
        {
            foreach (var b in TileSetData.Header)
            {
                if (ReadStatus.ReadByte() != b)
                {
                    throw new InvalidOperationException(
                        $"ファイルヘッダがファイル仕様と異なります（offset:{ReadStatus.Offset}）");
                }

                ReadStatus.IncreaseByteOffset();
            }
        }

        /// <summary>
        /// タイルセット設定
        /// </summary>
        /// <param name="ReadStatus">読み込み経過状態</param>
        /// <param name="settings">読み込み結果格納インスタンス</param>
        private void ReadTileSetSetting(out List<TileSetSetting> settings)
        {
            // タイルセット数
            var length = ReadStatus.ReadInt();
            ReadStatus.IncreaseIntOffset();

            settings = new List<TileSetSetting>();

            for (var i = 0; i < length; i++)
            {
                var reader = new TileSetSettingReader();

                settings.Add(reader.Read(ReadStatus));
            }
        }

        /// <summary>
        /// フッタ
        /// </summary>
        /// <param name="ReadStatus">読み込み経過状態</param>
        /// <exception cref="InvalidOperationException">ファイルフッタが仕様と異なる場合</exception>
        private void ReadFooter()
        {
            foreach (var b in TileSetData.Footer)
            {
                if (ReadStatus.ReadByte() != b)
                {
                    throw new InvalidOperationException(
                        $"ファイルフッタがファイル仕様と異なります（offset:{ReadStatus.Offset}）");
                }

                ReadStatus.IncreaseByteOffset();
            }
        }
    }
}
