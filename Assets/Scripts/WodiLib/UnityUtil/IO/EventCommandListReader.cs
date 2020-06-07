using System;
using System.Collections.Generic;
using WodiLib.Event;
using WodiLib.Event.EventCommand;

namespace WodiLib.UnityUtil.IO
{
    class EventCommandListReader
    {
        public EventCommandList Read(int length,BinaryReadStatus readStatus)
        {
            var eventCommandList = new List<IEventCommand>();
            for (var i = 0; i < length; i++)
            {
                ReadEventCommand(readStatus, eventCommandList);
            }

            return new EventCommandList(eventCommandList);
        }
        private void ReadEventCommand(BinaryReadStatus readStatus, ICollection<IEventCommand> commandList)
        {
            // 数値変数の数
            var numVarLength = readStatus.ReadByte();
            readStatus.IncreaseByteOffset();

            // 数値変数
            var numVarList = new List<int>();
            for (var i = 0; i < numVarLength; i++)
            {
                var numVar = readStatus.ReadInt();
                numVarList.Add(numVar);
                readStatus.IncreaseIntOffset();
            }

            // インデント
            var indent = (sbyte)readStatus.ReadByte();
            readStatus.IncreaseByteOffset();

            // 文字データ数
            var strVarLength = readStatus.ReadByte();
            readStatus.IncreaseByteOffset();

            // 文字列変数
            var strVarList = new List<string>();
            for (var i = 0; i < strVarLength; i++)
            {
                var woditorString = readStatus.ReadString();
                strVarList.Add(woditorString.String);
                readStatus.AddOffset(woditorString.ByteLength);
            }

            // 動作指定フラグ
            var hasMoveCommand = readStatus.ReadByte() != 0;
            readStatus.IncreaseByteOffset();

            // 動作指定コマンド
            ActionEntry actionEntry = null;
            if (hasMoveCommand)
            {
                actionEntry = new ActionEntry();
                ReadEventActionEntry(readStatus, actionEntry);
            }

            // 引数の数チェック
            if (numVarLength != numVarList.Count)
                throw new InvalidOperationException(
                    "指定された数値引数の数と実際の数値引数の数が一致しません。");
            if (strVarLength != strVarList.Count)
                throw new InvalidOperationException(
                    "指定された文字列引数の数と実際の文字列引数の数が一致しません。");

            // 結果
            var eventCommand = EventCommandFactory.CreateRaw(
                numVarList,
                indent,
                strVarList,
                actionEntry);

            commandList.Add(eventCommand);
        }

        /// <summary>
        /// イベントコマンドの動作指定コマンド
        /// </summary>
        /// <param name="readStatus">読み込み経過状態</param>
        /// <param name="actionEntry">データ格納先</param>
        /// <exception cref="InvalidOperationException">ファイル仕様が異なる場合</exception>
        private void ReadEventActionEntry(BinaryReadStatus readStatus, ActionEntry actionEntry)
        {
            // ヘッダチェック
            foreach (var b in ActionEntry.HeaderBytes)
            {
                if (readStatus.ReadByte() != b)
                {
                    throw new InvalidOperationException(
                        $"イベントコマンド中のイベントコマンドヘッダの値が異なります。（offset: {readStatus.Offset}）");
                }

                readStatus.IncreaseByteOffset();
            }

            // 動作フラグ
            var optionFlag = readStatus.ReadByte();
            actionEntry.SetOptionFlag(optionFlag);
            readStatus.IncreaseByteOffset();

            // 動作コマンドリスト
            actionEntry.CommandList = ReadCharaMoveCommand(readStatus);
        }

        /// <summary>
        /// 動作コマンドリスト
        /// </summary>
        /// <param name="readStatus">読み込み経過状態</param>
        /// <exception cref="InvalidOperationException">ファイル仕様が異なる場合</exception>
        private CharaMoveCommandList ReadCharaMoveCommand(BinaryReadStatus readStatus)
        {
            // 動作コマンド数
            var commandLength = readStatus.ReadInt();
            readStatus.IncreaseIntOffset();

            // 動作指定コマンド
            var reader = new CharaMoveCommandListReader();
            var result = reader.Read(commandLength,readStatus);

            return new CharaMoveCommandList(result);
        }
    }
}
