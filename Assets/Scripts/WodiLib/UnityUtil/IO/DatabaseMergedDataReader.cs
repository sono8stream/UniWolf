using System.Threading.Tasks;
using WodiLib.Database;

namespace WodiLib.UnityUtil.IO
{
    public class DatabaseMergedDataReader
    {
        public DatabaseMergedDataReader() { }

        public async Task<DatabaseMergedData> Read(
            string datFilePath,string projectFilePath)
        {
            var datFileReader = new DatabaseDatFileReader();
            var dat = await datFileReader.ReadFileAsync(datFilePath);

            var projectFileReader = new DatabaseProjectFileReader();
            var project = await projectFileReader.ReadFileAsync(projectFilePath);

            return new DatabaseMergedData(project.TypeSettingList, dat.SettingList);
        }
    }
}
