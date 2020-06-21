using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;
using WodiLib.UnityUtil.IO;
using System;
using WodiLib.Map;
using System.Linq;

public class Map : MonoBehaviour
{
    [SerializeField]
    string projectPath;

    [SerializeField]
    Text pathText;

    [SerializeField]
    Text infoText;

    // Start is called before the first frame update
    void Start()
    {
        ReadTileSetTest();
    }

    // Update is called once per frame
    void Update()
    {

    }

    async void ReadTest()
    {
        pathText.text = Application.streamingAssetsPath;
        string path = Application.streamingAssetsPath + "/Project/Data/MapData/SampleMapA.mps";
        Debug.Log(path);
            var mpsReader = new MpsFileReader();
        MapData mapData = await mpsReader.ReadFileAsync(path);
        infoText.text = mapData.MapSizeWidth.ToString();
    }

    async void ReadMapTreeTest()
    {
        pathText.text = Application.streamingAssetsPath;
        string path = Application.streamingAssetsPath + "/Project/Data/BasicData/MapTree.dat";
        var reader = new MapTreeDataFileReader();
        MapTreeData data = await reader.ReadFileAsync(path);
        for(int i = 0; i < data.TreeNodeList.Count; i++)
        {
            MapTreeNode node = data.TreeNodeList[i];
        }
        //infoText.text = mapData.MapSizeWidth.ToString();
    }

    async void ReadSystemDBTest()
    {
        string datPath = Application.streamingAssetsPath
            + "/Project/Data/BasicData/SysDataBase.dat";
        string projectPath = Application.streamingAssetsPath
            + "/Project/Data/BasicData/SysDataBase.project";

        var reader = new DatabaseMergedDataReader();
        WodiLib.Database.DatabaseMergedData data = await reader.ReadFilesAsync(datPath,projectPath);
        var list=data.GetDataDescList(0).ToList();

        infoText.text = "";
        for(int i = 0; i < list.Count; i++)
        {
            infoText.text += list[i].DataName + "\n";
        }
    }

    async void ReadTileSetTest()
    {
        string path = Application.streamingAssetsPath
            + "/Project/Data/BasicData/TileSetData.dat";

        var reader = new TileSetDataFileReader();
        TileSetData setData = await reader.ReadFileAsync(path);
        var settingList = setData.TileSetSettingList.ToList();

        infoText.text = "";
    }
}
