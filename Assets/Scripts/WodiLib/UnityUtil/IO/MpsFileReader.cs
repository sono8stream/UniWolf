using System;
using System.Collections.Generic;
using WodiLib.Event;
using WodiLib.Event.EventCommand;
using WodiLib.Map;

namespace WodiLib.UnityUtil.IO
{
    public class MpsFileReader:WoditorFileReader<MapData>
    {
        public MpsFileReader() { }

        protected override MapData Read()
        {
            var result = new MapData();

            // ヘッダチェック
            ReadHeader();

            // ヘッダ文字列
            ReadHeaderMemo(result);

            // タイルセットID
            ReadTileSetId(result);

            // マップサイズ横
            ReadMapSizeWidth(result);

            // マップサイズ縦
            ReadMapSizeHeight(result);

            // マップイベント数
            var mapEventLength = ReadMapEventLength();

            // レイヤー1～3
            ReadLayer(result);

            // マップイベント
            ReadMapEvent(mapEventLength, result);

            // ファイル末尾
            ReadFooter();

            return result;
        }

        private void ReadHeader()
        {
            foreach (var b in MapData.HeaderBytes)
            {
                if (ReadStatus.ReadByte() != b)
                {
                    throw new InvalidOperationException(
                        $"ファイルヘッダがファイル仕様と異なります（offset:{ReadStatus.Offset}）");
                }

                ReadStatus.IncreaseByteOffset();
            }
        }

        private void ReadHeaderMemo(MapData mapData)
        {
            var woditorString = ReadStatus.ReadString();
            mapData.Memo = woditorString.String;
            ReadStatus.AddOffset(woditorString.ByteLength);
        }

        private void ReadTileSetId(MapData mapData)
        {
            mapData.TileSetId = ReadStatus.ReadInt();
            ReadStatus.IncreaseIntOffset();
        }

        private void ReadMapSizeWidth(MapData mapData)
        {
            mapData.UpdateMapSizeWidth(ReadStatus.ReadInt());
            ReadStatus.IncreaseIntOffset();
        }

        private void ReadMapSizeHeight(MapData mapData)
        {
            mapData.UpdateMapSizeHeight(ReadStatus.ReadInt());
            ReadStatus.IncreaseIntOffset();
        }

        private int ReadMapEventLength()
        {
            var length = ReadStatus.ReadInt();
            ReadStatus.IncreaseIntOffset();

            return length;
        }

        private void ReadLayer(MapData mapData)
        {
            for (var layerIndex = 0; layerIndex < 3; layerIndex++)
            {
                ReadOneLayer(mapData, layerIndex);
            }
        }

        private void ReadOneLayer(MapData mapData, int layerNo)
        {
            var chips = new List<List<MapChip>>();
            for (var x = 0; x < (int)mapData.MapSizeWidth; x++)
            {
                ReadLayerOneLine(mapData.MapSizeHeight, chips);
            }

            var layer = new Layer
            {
                Chips = new MapChipList(chips)
            };
            mapData.SetLayer(layerNo, layer);
        }

        private void ReadLayerOneLine(MapSizeHeight mapSizeHeight,
            ICollection<List<MapChip>> chipList)
        {
            var lineChips = new List<MapChip>();
            for (var y = 0; y < mapSizeHeight; y++)
            {
                var chip = (MapChip)ReadStatus.ReadInt();
                lineChips.Add(chip);
                ReadStatus.IncreaseIntOffset();
            }

            chipList.Add(lineChips);
        }

        /// <summary>
        /// マップイベント
        /// </summary>
        /// <param name="size">マップイベント数</param>
        /// <param name="ReadStatus">読み込み経過状態</param>
        /// <param name="mapData">読み込み経過状態</param>
        /// <exception cref="InvalidOperationException">ファイル仕様が異なる場合</exception>
        private void ReadMapEvent(int size, MapData mapData)
        {
            var mapEvents = new List<MapEvent>();
            var count = 0;
            while (true)
            {
                // ヘッダチェック
                var validatedHeader = ReadStatus.ReadByte() == MapEvent.Header[0];
                if (!validatedHeader) break;

                // ヘッダ分オフセット加算
                ReadStatus.AddOffset(MapEvent.Header.Length);

                var mapEvent = new MapEvent();

                // マップイベントID
                mapEvent.MapEventId = ReadStatus.ReadInt();
                ReadStatus.IncreaseIntOffset();

                // イベント名
                var woditorString = ReadStatus.ReadString();
                mapEvent.EventName = woditorString.String;
                ReadStatus.AddOffset(woditorString.ByteLength);

                // X座標
                var posX = ReadStatus.ReadInt();
                ReadStatus.IncreaseIntOffset();

                // Y座標
                var posY = ReadStatus.ReadInt();
                ReadStatus.IncreaseIntOffset();

                mapEvent.Position = (posX, posY);

                // イベントページ数
                var pageLength = ReadStatus.ReadInt();
                ReadStatus.IncreaseIntOffset();

                // 0パディングチェック
                var padding = ReadStatus.ReadInt();
                ReadStatus.IncreaseIntOffset();
                var isCorrectPadding = padding == 0;
                if (!isCorrectPadding)
                {
                    throw new InvalidOperationException(
                        $"マップイベントのパディングが異なります。（offset:{ReadStatus.Offset}）");
                }

                // マップイベントページ
                var mapEventPageList = new List<MapEventPage>();
                for (var i = 0; i < pageLength; i++)
                {
                    ReadMapEventOnePage(mapEventPageList);
                }

                mapEvent.MapEventPageList = new MapEventPageList(mapEventPageList);

                // イベントページ末尾チェック
                foreach (var b in MapEvent.Footer)
                {
                    if (ReadStatus.ReadByte() != b)
                    {
                        throw new InvalidOperationException(
                            $"マップイベント末尾の値が異なります。（offset: {ReadStatus.Offset}）");
                    }

                    ReadStatus.IncreaseByteOffset();
                }

                mapEvents.Add(mapEvent);

                count++;
            }

            if (count != size)
                throw new InvalidOperationException(
                    $"マップイベントデータの数が期待する数と異なります。(期待する数：{size}, 実際のイベント数：{count})");

            mapData.MapEvents = new MapEventList(mapEvents);
        }

        private void ReadMapEventOnePage(ICollection<MapEventPage> mapEventPages)
        {
            var result = new MapEventPage();

            // ヘッダチェック
            foreach (var b in MapEventPage.Header)
            {
                if (ReadStatus.ReadByte() != b)
                {
                    throw new InvalidOperationException(
                        $"マップイベントページのヘッダが異なります。（offset: {ReadStatus.Offset}）");
                }

                ReadStatus.IncreaseByteOffset();
            }

            var graphicInfo = new MapEventPageGraphicInfo();

            // タイル画像ID
            var graphicTileId = (MapEventTileId)ReadStatus.ReadInt();

            if (graphicTileId != MapEventTileId.NotUse)
            {
                graphicInfo.IsGraphicTileChip = true;
                graphicInfo.GraphicTileId = graphicTileId;
            }

            ReadStatus.IncreaseIntOffset();

            // キャラチップ名
            var charaChipString = ReadStatus.ReadString();

            if (!graphicInfo.IsGraphicTileChip)
            {
                graphicInfo.CharaChipFilePath = charaChipString.String;
            }

            ReadStatus.AddOffset(charaChipString.ByteLength);

            // 初期キャラ向き
            var initDirection = ReadStatus.ReadByte();
            graphicInfo.InitDirection = CharaChipDirection.FromByte(initDirection);
            ReadStatus.IncreaseByteOffset();

            // 初期アニメーション番号
            graphicInfo.InitAnimationId = ReadStatus.ReadByte();
            ReadStatus.IncreaseByteOffset();

            // キャラチップ透過度
            graphicInfo.CharaChipOpacity = ReadStatus.ReadByte();
            ReadStatus.IncreaseByteOffset();

            // キャラチップ表示形式
            graphicInfo.CharaChipDrawType = PictureDrawType.FromByte(ReadStatus.ReadByte());
            ReadStatus.IncreaseByteOffset();

            result.GraphicInfo = graphicInfo;

            var bootInfo = new MapEventPageBootInfo();

            // 起動条件
            bootInfo.MapEventBootType = MapEventBootType.FromByte(ReadStatus.ReadByte());
            ReadStatus.IncreaseByteOffset();

            // 条件1～4演算子 & 使用フラグ
            var conditions = new List<MapEventBootCondition>
            {
                new MapEventBootCondition(),
                new MapEventBootCondition(),
                new MapEventBootCondition(),
                new MapEventBootCondition(),
            };
            for (var i = 0; i < 4; i++)
            {
                conditions[i].Operation = CriteriaOperator.FromByte((byte)(ReadStatus.ReadByte() & 0xF0));
                conditions[i].UseCondition = (byte)(ReadStatus.ReadByte() & 0x0F) != 0;
                ReadStatus.IncreaseByteOffset();
            }

            // 条件1～4左辺
            for (var i = 0; i < 4; i++)
            {
                conditions[i].LeftSide = ReadStatus.ReadInt();
                ReadStatus.IncreaseIntOffset();
            }

            // 条件1～4右辺
            for (var i = 0; i < 4; i++)
            {
                conditions[i].RightSide = ReadStatus.ReadInt();
                ReadStatus.IncreaseIntOffset();
                bootInfo.SetEventBootCondition(i, conditions[i]);
            }

            result.BootInfo = bootInfo;

            var moveRouteInfo = new MapEventPageMoveRouteInfo();

            // アニメ速度
            moveRouteInfo.AnimateSpeed = AnimateSpeed.FromByte(ReadStatus.ReadByte());
            ReadStatus.IncreaseByteOffset();

            // 移動速度
            moveRouteInfo.MoveSpeed = MoveSpeed.FromByte(ReadStatus.ReadByte());
            ReadStatus.IncreaseByteOffset();

            // 移動頻度
            moveRouteInfo.MoveFrequency = MoveFrequency.FromByte(ReadStatus.ReadByte());
            ReadStatus.IncreaseByteOffset();

            // 移動ルート
            moveRouteInfo.MoveType = MoveType.FromByte(ReadStatus.ReadByte());
            ReadStatus.IncreaseByteOffset();

            var option = new MapEventPageOption();

            // オプション
            var optionByte = ReadStatus.ReadByte();
            option.SetOptionFlag(optionByte);
            ReadStatus.IncreaseByteOffset();

            result.Option = option;

            // カスタム移動ルートフラグ
            var actionEntry = new ActionEntry();
            var customMoveRouteFlag = ReadStatus.ReadByte();
            actionEntry.SetOptionFlag(customMoveRouteFlag);
            ReadStatus.IncreaseByteOffset();

            // 動作指定コマンド数
            actionEntry.CommandList = ReadCharaMoveCommand();

            moveRouteInfo.CustomMoveRoute = actionEntry;
            result.MoveRouteInfo = moveRouteInfo;

            // イベント行数
            var eventLength = ReadStatus.ReadInt();
            ReadStatus.IncreaseIntOffset();

            // イベントコマンド
            var eventCommandListReader = new EventCommandListReader();

            result.EventCommands = eventCommandListReader.Read(eventLength,ReadStatus);

            // イベントコマンド終端チェック
            foreach (var b in EventCommandList.EndEventCommand)
            {
                if (ReadStatus.ReadByte() != b)
                {
                    throw new InvalidOperationException(
                        $"イベントコマンド後の値が異なります。（offset: {ReadStatus.Offset}）");
                }

                ReadStatus.IncreaseByteOffset();
            }

            // 影グラフィック番号
            result.ShadowGraphicId = ReadStatus.ReadByte();
            ReadStatus.IncreaseByteOffset();

            // 接触範囲拡張X
            var rangeWidth = ReadStatus.ReadByte();
            ReadStatus.IncreaseByteOffset();

            // 接触範囲拡張Y
            var rangeHeight = ReadStatus.ReadByte();
            ReadStatus.IncreaseByteOffset();

            result.HitExtendRange = (rangeWidth, rangeHeight);

            // イベントページ末尾チェック
            foreach (var b in MapEventPage.Footer)
            {
                if (ReadStatus.ReadByte() != b)
                {
                    throw new InvalidOperationException(
                        $"イベントページ末尾の値が異なります。（offset: {ReadStatus.Offset}）");
                }

                ReadStatus.IncreaseByteOffset();
            }

            // 完了
            mapEventPages.Add(result);
        }

        /// <summary>
        /// 動作コマンドリスト
        /// </summary>
        /// <param name="ReadStatus">読み込み経過状態</param>
        /// <exception cref="InvalidOperationException">ファイル仕様が異なる場合</exception>
        private CharaMoveCommandList ReadCharaMoveCommand()
        {
            // 動作コマンド数
            var commandLength = ReadStatus.ReadInt();
            ReadStatus.IncreaseIntOffset();

            // 動作指定コマンド
            var reader = new CharaMoveCommandListReader();
            var result = reader.Read(commandLength, ReadStatus);

            return new CharaMoveCommandList(result);
        }

        /// <summary>
        /// フッタ
        /// </summary>
        /// <param name="ReadStatus">読み込み経過状態</param>
        /// <exception cref="InvalidOperationException">ファイル仕様が異なる場合</exception>
        private void ReadFooter()
        {
            foreach (var b in MapData.Footer)
            {
                if (ReadStatus.ReadByte() != b)
                {
                    throw new InvalidOperationException(
                        $"フッタが正常に取得できませんでした。（offset:{ReadStatus.Offset}）");
                }

                ReadStatus.IncreaseByteOffset();
            }
        }
    }
}
