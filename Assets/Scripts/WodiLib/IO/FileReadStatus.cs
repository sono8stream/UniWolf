// ========================================
// Project Name : WodiLib
// File Name    : FileReadStatus.cs
//
// MIT License Copyright(c) 2019 kameske
// see LICENSE file
// ========================================

using System;
using System.IO;
using WodiLib.Sys;
using UnityEngine.Networking;
using UnityEngine;

namespace WodiLib.IO
{
    /// <summary>
    /// ファイル読み込み状態クラス
    /// </summary>
    internal class FileReadStatus
    {
        // _/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/
        //     Public Property
        // _/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/
        private byte[] DataBuffer { get; }

        private int BufferLength { get; }

        public int Offset { get; private set; }

        // _/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/
        //     Constructor
        // _/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="filePath">読み込みmpsファイルフルパス</param>
        /// <exception cref="FileNotFoundException">ファイルが存在しない場合</exception>
        /// <exception cref="InvalidOperationException">ファイルサイズが 2,147,483,647 bytes（約2GB） を超える場合</exception>
        public FileReadStatus(string filePath)
        {
            using (var www = UnityWebRequest.Get(filePath))
            {
                www.SendWebRequest();
                while (!www.isDone && !www.isNetworkError && !www.isHttpError) { }
                if (www.isNetworkError || www.isHttpError)
                {
                    throw new InvalidOperationException(
                        "ファイルの読み込みに失敗しました。");
                }
                Debug.Log(www.downloadedBytes);
                var bufLength = www.downloadedBytes;
                if (bufLength > int.MaxValue)
                    throw new InvalidOperationException(
                        "ファイルサイズが大きすぎるため、扱うことができません。");
                BufferLength = (int)www.downloadedBytes;
                DataBuffer = www.downloadHandler.data;
            }

            Offset = 0;
        }

        public FileReadStatus(byte[] data)
        {
            BufferLength = data.Length;
            DataBuffer = data;
            Offset = 0;
        }

        // _/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/
        //     Public Method
        // _/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/

        /// <summary>
        /// 現在のオフセットから、1バイトのデータを取得する。
        /// </summary>
        /// <returns>バイトデータ</returns>
        public byte ReadByte()
        {
            return DataBuffer[Offset];
        }

        /// <summary>
        /// 現在のオフセットを起点に、Int値を取得する。
        /// </summary>
        /// <returns>Int値</returns>
        public int ReadInt()
        {
            return DataBuffer.ToInt32(Endian.Woditor, Offset);
        }

        /// <summary>
        /// 現在のオフセットを起点に、文字列を取得する。
        /// </summary>
        /// <returns>文字列</returns>
        public WoditorString ReadString()
        {
            return new WoditorString(DataBuffer, Offset);
        }

        /// <summary>
        /// オフセットに加算する。
        /// </summary>
        /// <param name="i">加算値</param>
        /// <exception cref="ArgumentException">オフセットがバッファサイズを超える場合</exception>
        public void AddOffset(int i)
        {
            if (Offset + i > BufferLength)
                throw new ArgumentException(
                    "オフセットがバッファサイズを超えるため、オフセットを増やせません。");
            Offset += i;
        }

        /// <summary>
        /// オフセットを1バイト分インクリーズする。
        /// </summary>
        /// <exception cref="ArgumentException">オフセットがバッファサイズを超える場合</exception>
        public void IncreaseByteOffset()
        {
            if (Offset + 1 > BufferLength)
                throw new ArgumentException(
                    "オフセットがバッファサイズを超えるため、オフセットを増やせません。");
            AddOffset(1);
        }

        /// <summary>
        /// オフセットを1Int分インクリーズする。
        /// </summary>
        /// <exception cref="ArgumentException">オフセットがバッファサイズを超える場合</exception>
        public void IncreaseIntOffset()
        {
            const int intSize = 4;
            if (Offset + intSize > BufferLength)
                throw new ArgumentException(
                    "オフセットがバッファサイズを超えるため、オフセットを増やせません。");
            AddOffset(intSize);
        }
    }
}