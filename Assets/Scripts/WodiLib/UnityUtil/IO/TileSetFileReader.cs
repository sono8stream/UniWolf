using System;
using System.Collections.Generic;
using WodiLib.Map;

namespace WodiLib.UnityUtil.IO
{
    public class TileSetFileReader : WoditorFileReader<TileSetFileData>
    {
        public TileSetFileReader() { }

        protected override TileSetFileData Read()
        {
            return ReadData();
        }

        private TileSetFileData ReadData()
        {
            // ヘッダ
            ReadHeader();

            // タイルセット設定
            ReadTileSetSetting(out var setting);

            // フッタ
            ReadFooter();

            return new TileSetFileData
            {
                TileSetSetting = setting
            };
        }

        /// <summary>
        /// ヘッダ
        /// </summary>
        /// <param name="ReadStatus">読み込み経過状態</param>
        /// <exception cref="InvalidOperationException">ファイルヘッダが仕様と異なる場合</exception>
        private void ReadHeader()
        {
            foreach (var b in TileSetFileData.Header)
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
        /// <param name="ReadStatus"></param>
        /// <param name="setting"></param>
        private void ReadTileSetSetting(out TileSetSetting setting)
        {
            var reader = new TileSetSettingReader();

            setting = reader.Read(ReadStatus);
        }

        /// <summary>
        /// フッタ
        /// </summary>
        /// <param name="ReadStatus">読み込み経過状態</param>
        /// <exception cref="InvalidOperationException">ファイルフッタが仕様と異なる場合</exception>
        private void ReadFooter()
        {
            foreach (var b in TileSetFileData.Footer)
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
