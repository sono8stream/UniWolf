using System;
using System.Collections.Generic;
using USEncoder;
using WodiLib.Sys;

namespace WodiLib.UnityUtil.IO
{
    public class StringReader
    {
        private List<Tuple<string, string>> SpecialConversionStringList =
            new List<Tuple<string, string>>
            {
                new Tuple<string, string>("\n", "<\\n>"),
                new Tuple<string, string>("\r\n", "<\\n>"),
                new Tuple<string, string>("\"", "<dqrt>")
            };

        public StringReader() { }

        public string Read(byte[] rawData,long offset)
        {
            var lengthByte = new byte[4];
            for (var i = 0; i < 4; i++)
            {
                lengthByte[i] = rawData[offset];
                offset++;
            }

            var length = lengthByte.ToInt32(Endian.Woditor);
            // 文字列の末尾に '0' が付与されているため、一部の処理では無視する
            var removeZeroLength = length - 1;

            var chars = new byte[removeZeroLength];
            Array.Copy(rawData, offset, chars, 0, removeZeroLength);

            return ToEncoding.ToUnicode(chars);
        }
    }
}
