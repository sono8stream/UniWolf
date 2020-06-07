using UnityEngine.Networking;
using System.Threading.Tasks;

namespace WodiLib.UnityUtil.IO
{
    public class WoditorFileReader<TFile>
    {
        protected BinaryReadStatus ReadStatus { get; set; }

        public async Task<TFile> ReadFileAsync(string filePath)
        {
            byte[] rawData = await FetchFile(filePath);
            ReadStatus = new BinaryReadStatus(rawData);
            return Read();
        }

        private async Task<byte[]> FetchFile(string filePath)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(filePath))
            {
                await www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    return null;
                }

                return www.downloadHandler.data;
            }
        }

        protected virtual TFile Read() { return default; }
    }
}
