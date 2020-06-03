using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using WodiLib.Cmn;
using WodiLib.Sys;

namespace WodiLib.IO
{
    public class WoditorFileFactory<TFilePath,TFileData,TWriter,TReader,TFile>
        where TFilePath: FilePath
        where TWriter : WoditorFileWriterBase<TFilePath, TFileData>
        where TReader: WoditorFileReaderBase<TFilePath, TFileData>
    {

        public static async Task<TFileData> ReadAsync()
        {
            try
            {
                var reader = await BuildFileReaderAsync(FilePath);
                var result = await reader.ReadAsync();

                return result;
            }
        }

        // こんな感じで読み込んでデータを渡せればいい
        public static TFileData ReadAsync<TFilePath, TFileData, TReader, TWriter, TFile>
            (this TFile file, TFilePath filePath)
            where TFilePath : FilePath
        where TReader : WoditorFileReaderBase<TFilePath, TFileData>
        where TWriter : WoditorFileWriterBase<TFilePath, TFileData>
            where TFile : WoditorFileBase<TFilePath, TFileData, TWriter, TReader>
        {
            var file = new TFile(filePath);
            try
            {
                var reader = await
            }
        }
    }
}
