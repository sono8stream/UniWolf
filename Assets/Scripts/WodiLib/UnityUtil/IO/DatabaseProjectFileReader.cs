using System;
using WodiLib.Database;

namespace WodiLib.UnityUtil.IO
{
    class DatabaseProjectFileReader : WoditorFileReader<DatabaseProject>
    {

        public DatabaseProjectFileReader() { }

        protected override DatabaseProject Read()
        {
            var result = new DatabaseProject();

            ReadTypeSettingList(ReadStatus, result);

            return result;
        }

        private void ReadTypeSettingList(BinaryReadStatus readStatus, DatabaseProject data)
        {

            var length = readStatus.ReadInt();
            readStatus.IncreaseIntOffset();

            var reader = new DBTypeSettingReader();

            var settings = reader.Read(readStatus,length,true);

            data.TypeSettingList.AddRange(settings);
        }
    }
}
