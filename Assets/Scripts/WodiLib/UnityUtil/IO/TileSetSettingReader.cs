using System;
using System.Collections.Generic;
using WodiLib.Event.CharaMoveCommand;
using WodiLib.Map;

namespace WodiLib.UnityUtil.IO
{
    class TileSetSettingReader
    {
        public TileSetSetting Read(BinaryReadStatus readStatus)
        {
            var result = ReadOneTileSetSetting(readStatus);

            return result;
        }

        private TileSetSetting ReadOneTileSetSetting(BinaryReadStatus readStatus)
        {
            // 設定名
            ReadName(readStatus, out var name);

            // 基本タイルセットファイル名
            ReadBaseTileSetFileName(readStatus, out var baseTileSetFileName);

            // オートタイルファイル名リスト
            ReadAutoTileSetFileNameList(readStatus, AutoTileFileNameList.MaxCapacity, out var autoTileFileNames);

            // セパレータ
            ReadSeparator(readStatus);

            // タグ番号リスト
            ReadTagNumberList(readStatus, out var tagNumbers);

            // セパレータ
            ReadSeparator(readStatus);

            // タイル設定リスト
            ReadTilePathSettingList(readStatus, out var tilePathSettings);

            var result = new TileSetSetting(new TileTagNumberList(tagNumbers),
                new TilePathSettingList(tilePathSettings),
                new AutoTileFileNameList(autoTileFileNames))
            {
                Name = name,
                BaseTileSetFileName = baseTileSetFileName
            };

            return result;
        }

        /// <summary>
        /// タイプ名
        /// </summary>
        /// <param name="readStatus">読み込み経過状態</param>
        /// <param name="name">結果格納インスタンス</param>
        private void ReadName(BinaryReadStatus readStatus, out TileSetName name)
        {
            var read = readStatus.ReadString();
            name = read.String;

            readStatus.AddOffset(read.ByteLength);
        }

        /// <summary>
        /// 基本タイルセットファイル名
        /// </summary>
        /// <param name="readStatus">読み込み経過状態</param>
        /// <param name="fileName">結果格納インスタンス</param>
        private void ReadBaseTileSetFileName(BinaryReadStatus readStatus, out BaseTileSetFileName fileName)
        {
            var read = readStatus.ReadString();
            fileName = read.String;

            readStatus.AddOffset(read.ByteLength);
        }

        /// <summary>
        /// オートタイルファイル名
        /// </summary>
        /// <param name="readStatus">読み込み経過状態</param>
        /// <param name="listLength">オートタイルファイル名数</param>
        /// <param name="list">結果格納インスタンス</param>
        private void ReadAutoTileSetFileNameList(BinaryReadStatus readStatus, int listLength,
            out List<AutoTileFileName> list)
        {
            list = new List<AutoTileFileName>();

            for (var i = 0; i < listLength; i++)
            {
                var read = readStatus.ReadString();
                AutoTileFileName fileName = read.String;
                list.Add(fileName);

                readStatus.AddOffset(read.ByteLength);
            }
        }

        /// <summary>
        /// セパレータ
        /// </summary>
        /// <param name="readStatus">読み込み経過状態</param>
        private void ReadSeparator(BinaryReadStatus readStatus)
        {
            var read = readStatus.ReadByte();

            if (read != TileSetSetting.DataSeparator)
                throw new InvalidOperationException(
                    $"データセパレータが正しく読み込めませんでした。（offset:{readStatus.Offset}）");

            readStatus.IncreaseByteOffset();
        }

        /// <summary>
        /// タグ番号リスト
        /// </summary>
        /// <param name="readStatus">読み込み経過状態</param>
        /// <param name="list">結果格納インスタンス</param>
        private void ReadTagNumberList(BinaryReadStatus readStatus, out List<TileTagNumber> list)
        {
            var length = readStatus.ReadInt();
            readStatus.IncreaseIntOffset();

            list = new List<TileTagNumber>();

            for (var i = 0; i < length; i++)
            {
                var tagNumber = readStatus.ReadByte();
                readStatus.IncreaseByteOffset();

                list.Add(tagNumber);
            }
        }

        /// <summary>
        /// タイル通行設定リスト
        /// </summary>
        /// <param name="readStatus">読み込み経過状態</param>
        /// <param name="list">結果格納インスタンス</param>
        private void ReadTilePathSettingList(BinaryReadStatus readStatus, out List<TilePathSetting> list)
        {
            var length = readStatus.ReadInt();
            readStatus.IncreaseIntOffset();

            list = new List<TilePathSetting>();

            for (var i = 0; i < length; i++)
            {
                var tilePathSettingCode = readStatus.ReadInt();
                readStatus.IncreaseIntOffset();

                list.Add(new TilePathSetting(tilePathSettingCode));
            }
        }
    }
}
