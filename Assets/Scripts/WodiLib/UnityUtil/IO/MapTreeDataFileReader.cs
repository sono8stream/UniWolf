using System;
using System.Collections.Generic;
using WodiLib.Map;

namespace WodiLib.UnityUtil.IO
{
    public class MapTreeDataFileReader:WoditorFileReader<MapTreeData>
    {
        public MapTreeDataFileReader() { }

        protected override MapTreeData Read()
        {
            // ヘッダ
            ReadHeader();

            // ツリーノード
            ReadTreeNodeList( out var nodes);

            // フッタ
            ReadFooter();

            return new MapTreeData
            {
                TreeNodeList = new MapTreeNodeList(nodes)
            };
        }

        private void ReadHeader()
        {
            foreach (var b in MapTreeData.Header)
            {
                if (ReadStatus.ReadByte() != b)
                {
                    throw new InvalidOperationException(
                        $"ファイルヘッダがファイル仕様と異なります（offset:{ReadStatus.Offset}）");
                }

                ReadStatus.IncreaseByteOffset();
            }
        }

        private void ReadTreeNodeList(out List<MapTreeNode> nodes)
        {
            // ノード数
            var length = ReadStatus.ReadInt();
            ReadStatus.IncreaseIntOffset();

            nodes = new List<MapTreeNode>();

            for (var i = 0; i < length; i++)
            {
                var parent = ReadStatus.ReadInt();
                ReadStatus.IncreaseIntOffset();

                var me = ReadStatus.ReadInt();
                ReadStatus.IncreaseIntOffset();

                nodes.Add(new MapTreeNode(me, parent));
            }
        }

        private void ReadFooter()
        {
            foreach (var b in MapTreeData.Footer)
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
