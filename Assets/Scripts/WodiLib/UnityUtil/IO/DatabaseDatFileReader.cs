using System;
using WodiLib.Database;

namespace WodiLib.UnityUtil.IO
{
    class DatabaseDatFileReader:WoditorFileReader<DatabaseDat>
    {
        public DatabaseDatFileReader() { }

        protected override DatabaseDat Read()
        {
            var result = new DatabaseDat();

            // ヘッダチェック
            ReadHeader();

            // DBデータ
            ReadDBData(result);

            // フッタチェック
            ReadFooter();

            return result;
        }

        private void ReadHeader()
        {
            foreach (var b in DatabaseDat.Header)
            {
                if (ReadStatus.ReadByte() != b)
                {
                    throw new InvalidOperationException(
                        $"ファイルヘッダがファイル仕様と異なります（offset:{ReadStatus.Offset}）");
                }

                ReadStatus.IncreaseByteOffset();
            }
        }

        private void ReadDBData(DatabaseDat data)
        {
            var length = ReadStatus.ReadInt();
            ReadStatus.IncreaseIntOffset();

            var reader = new DBDataSettingReader();
            data.SettingList.AddRange(reader.Read(ReadStatus, length));
        }

        private void ReadFooter()
        {
            foreach (var b in DatabaseDat.Footer)
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
