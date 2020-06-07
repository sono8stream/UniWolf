using System;
using System.Collections.Generic;
using WodiLib.Event.CharaMoveCommand;

namespace WodiLib.UnityUtil.IO
{
    class CharaMoveCommandListReader
    {

        public List<ICharaMoveCommand> Read(int length,BinaryReadStatus readStatus)
        {
            var charaMoveCommandList = new List<ICharaMoveCommand>();
            for (var i = 0; i < length; i++)
            {
                ReadCharaMoveCommand(readStatus,charaMoveCommandList);
            }

            return charaMoveCommandList;
        }

        private void ReadCharaMoveCommand(BinaryReadStatus readStatus,
            ICollection<ICharaMoveCommand> commandList)
        {
            // 動作指定コード
            var charaMoveCode = readStatus.ReadByte();
            CharaMoveCommandCode commandCode;
            try
            {
                commandCode = CharaMoveCommandCode.FromByte(charaMoveCode);
            }
            catch
            {
                throw new InvalidOperationException(
                    $"存在しない動作指定コマンドコードが読み込まれました。" +
                    $"（コマンドコード値：{charaMoveCode}, offset：{readStatus.Offset}");
            }

            var charaMoveCommand = CharaMoveCommandFactory.CreateRaw(commandCode);
            readStatus.IncreaseByteOffset();

            // 変数の数
            var varLength = readStatus.ReadByte();
            readStatus.IncreaseByteOffset();

            // 変数
            for (var i = 0; i < varLength; i++)
            {
                var value = readStatus.ReadInt();
                charaMoveCommand.SetNumberValue(i, value);
                readStatus.IncreaseIntOffset();
            }

            // 終端コードチェック
            foreach (var b in CharaMoveCommandBase.EndBlockCode)
            {
                if (readStatus.ReadByte() != b)
                {
                    throw new InvalidOperationException(
                        $"動作指定コマンド末尾の値が異なります。（offset: {readStatus.Offset}）");
                }

                readStatus.IncreaseByteOffset();
            }

            // 結果
            commandList.Add(charaMoveCommand);
        }
    }
}
