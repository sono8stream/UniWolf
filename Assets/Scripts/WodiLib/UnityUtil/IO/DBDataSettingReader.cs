using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WodiLib.Database;
using WodiLib.Sys;

namespace WodiLib.UnityUtil.IO
{
    class DBDataSettingReader 
    {
        public List<DBDataSetting> Read(BinaryReadStatus readStatus, int length)
        {
            var list = new List<DBDataSetting>();
            for (var i = 0; i < length; i++)
            {
                ReadOneDBTypeSetting(readStatus, list);
            }

            return list;
        }

        private void ReadOneDBTypeSetting(BinaryReadStatus readStatus, ICollection<DBDataSetting> result)
        {
            var setting = new DBDataSetting();

            // ヘッダ
            ReadHeader(readStatus);

            // データIDの設定方法
            ReadDataSettingType(readStatus, setting);

            // 設定種別 & 種別順列
            ReadValueType(readStatus, out var types);

            // DBデータ設定値
            ReadDataSettingValue(readStatus, setting, types);

            result.Add(setting);
        }

        /// <summary>
        /// ヘッダ
        /// </summary>
        /// <param name="readStatus">読み込み経過状態</param>
        /// <exception cref="InvalidOperationException">ファイルヘッダが仕様と異なる場合</exception>
        private void ReadHeader(BinaryReadStatus readStatus)
        {
            foreach (var b in DBDataSetting.Header)
            {
                if (readStatus.ReadByte() != b)
                {
                    throw new InvalidOperationException(
                        $"ファイルヘッダがファイル仕様と異なります（offset:{readStatus.Offset}）");
                }

                readStatus.IncreaseByteOffset();
            }
        }

        /// <summary>
        /// データIDの設定方法
        /// </summary>
        /// <param name="readStatus">読み込み経過状態</param>
        /// <param name="setting">結果格納インスタンス</param>
        private void ReadDataSettingType(BinaryReadStatus readStatus, DBDataSetting setting)
        {
            var typeCode = readStatus.ReadInt();
            readStatus.IncreaseIntOffset();

            var settingType = DBDataSettingType.FromValue(typeCode);

            // 「指定DBの指定タイプ」の場合、DB種別とタイプIDを取り出す
            DBKind dbKind = null;
            TypeId typeId = 0;
            if (settingType == DBDataSettingType.DesignatedType)
            {
                dbKind = DbKindFromSettingTypeCode(typeCode);

                typeId = TypeIdFromSettingTypeCode(typeCode);
            }

            setting.SetDataSettingType(settingType, dbKind, typeId);
        }

        /// <summary>
        /// 設定種別 &amp; 種別順列
        /// </summary>
        /// <param name="readStatus">読み込み経過状態</param>
        /// <param name="itemTypes">取得した項目種別リスト格納先</param>
        private void ReadValueType(BinaryReadStatus readStatus, out List<DBItemType> itemTypes)
        {
            var length = readStatus.ReadInt();
            readStatus.IncreaseIntOffset();

            var countDic = new Dictionary<DBItemType, int>
            {
                {DBItemType.Int, 0},
                {DBItemType.String, 0}
            };

            itemTypes = new List<DBItemType>();

            for (var i = 0; i < length; i++)
            {
                var settingCode = readStatus.ReadInt();
                readStatus.IncreaseIntOffset();

                var itemType = DBItemType.FromValue(settingCode);

                // 項目タイプ数集計
                countDic[itemType]++;

                // 種別順位は無視する

                itemTypes.Add(itemType);
            }
        }

        /// <summary>
        /// DBデータ設定値
        /// </summary>
        /// <param name="readStatus">読み込み経過状態</param>
        /// <param name="setting">結果格納インスタンス</param>
        /// <param name="itemTypes">項目種別リスト</param>
        private void ReadDataSettingValue(BinaryReadStatus readStatus, DBDataSetting setting,
            IReadOnlyCollection<DBItemType> itemTypes)
        {
            var length = readStatus.ReadInt();
            readStatus.IncreaseIntOffset();

            var numberItemCount = itemTypes.Count(x => x == DBItemType.Int);
            var stringItemCount = itemTypes.Count(x => x == DBItemType.String);

            var valuesList = new List<List<DBItemValue>>();

            for (var i = 0; i < length; i++)
            {
                ReadOneDataSettingValue(readStatus, valuesList, itemTypes, numberItemCount, stringItemCount);
            }

            setting.SettingValuesList = new DBItemValuesList(valuesList);
        }

        /// <summary>
        /// DBデータ設定値ひとつ分
        /// </summary>
        /// <param name="readStatus">読み込み経過状態</param>
        /// <param name="result">結果格納インスタンス</param>
        /// <param name="itemTypes">項目種別リスト</param>
        /// <param name="numberItemCount">数値項目数</param>
        /// <param name="stringItemCount">文字列項目数</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private void ReadOneDataSettingValue(BinaryReadStatus readStatus, List<List<DBItemValue>> result,
            IEnumerable<DBItemType> itemTypes, int numberItemCount, int stringItemCount)
        {
            var numberItems = new List<DBValueInt>();
            var stringItems = new List<DBValueString>();

            for (var i = 0; i < numberItemCount; i++)
            {
                var numberItem = readStatus.ReadInt();
                readStatus.IncreaseIntOffset();

                numberItems.Add(numberItem);
            }

            for (var i = 0; i < stringItemCount; i++)
            {
                var stringItem = readStatus.ReadString();
                readStatus.AddOffset(stringItem.ByteLength);

                stringItems.Add(stringItem.String);
            }

            var valueList = new List<DBItemValue>();

            var numberIndex = 0;
            var stringIndex = 0;
            foreach (var itemType in itemTypes)
            {
                if (itemType == DBItemType.Int)
                {
                    valueList.Add(numberItems[numberIndex]);
                    numberIndex++;
                }
                else if (itemType == DBItemType.String)
                {
                    valueList.Add(stringItems[stringIndex]);
                    stringIndex++;
                }
                else
                {
                    // 通常ここへは来ない
                    throw new InvalidOperationException(
                        "未対応のデータ種別です。");
                }
            }

            result.Add(valueList);
        }

        // _/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/
        //     Private Method
        // _/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/

        /// <summary>
        /// データIDの設定方法コードからDB種別を取得する。
        /// </summary>
        /// <param name="code">設定種別コード</param>
        /// <returns>DB種別</returns>
        private DBKind DbKindFromSettingTypeCode(int code)
        {
            var dbKindCode = (byte) code.SubInt(4, 1);
            return DBKind.FromDBDataSettingTypeCode(dbKindCode);
        }

        /// <summary>
        /// データIDの設定方法コードからタイプIDを取得する。
        /// </summary>
        /// <param name="code">設定種別コード</param>
        /// <returns>タイプID</returns>
        private TypeId TypeIdFromSettingTypeCode(int code)
        {
            return code.SubInt(0, 4);
        }
    }
}
