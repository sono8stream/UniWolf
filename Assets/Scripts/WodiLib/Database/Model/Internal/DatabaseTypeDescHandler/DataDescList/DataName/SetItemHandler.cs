// ========================================
// Project Name : WodiLib
// File Name    : SetItemHandler.cs
//
// MIT License Copyright(c) 2019 kameske
// see LICENSE file
// ========================================

using System;
using WodiLib.Sys;

namespace WodiLib.Database.DatabaseTypeDescHandler.DataDescList.DataName
{
    /// <summary>
    /// DatabaseDataDescList.SetItemのイベントハンドラ
    /// </summary>
    [Obsolete("要素変更通知は CollectionChanged イベントを利用して取得してください。 Ver1.3 で削除します。")]
    internal class SetItemHandler : OnSetItemHandler<Database.DataName>
    {
        // _/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/
        //     Public Constant
        // _/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/

        /// <summary>
        /// リストイベントハンドラにつけるタグ
        /// </summary>
        public static readonly string HandlerTag = "__SYSTEM__";

        // _/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/
        //     Constructor
        // _/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/

        public SetItemHandler(DatabaseTypeDesc outer)
            : base(MakeHandler(outer), HandlerTag, false, canChangeEnabled: false)
        {
        }

        /// <summary>
        /// DatabaseItemDataList.SetItemのイベントを生成する。
        /// </summary>
        /// <param name="outer">連係外部クラスインスタンス</param>
        /// <returns>InsertItemイベント</returns>
        private static Action<int, Database.DataName> MakeHandler(DatabaseTypeDesc outer)
        {
            return (i, name) =>
            {
                var valuesList = outer.WritableItemValuesList.CreateValueListInstance();
                outer.DataDescList[i] = new DatabaseDataDesc(name, (DBItemValueList) valuesList);
                outer.WritableItemValuesList[i] = valuesList;
            };
        }
    }
}