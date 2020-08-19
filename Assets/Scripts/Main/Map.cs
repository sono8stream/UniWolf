using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;
using WodiLib.UnityUtil.IO;
using System;
using WodiLib.Map;
using System.Linq;
using UnityEditor.Experimental.GraphView;

public class Map : MonoBehaviour
{
    [SerializeField]
    int mapNo;

    [SerializeField]
    SpriteRenderer mapSprite;

    [SerializeField]
    Text pathText;

    [SerializeField]
    Text infoText;

    // Start is called before the first frame update
    void Start()
    {
        ReadMap();
    }

    // Update is called once per frame
    void Update()
    {

    }

    async void ReadMap()
    {
        string dataPath = Application.streamingAssetsPath + "/Project/Data/";

        string datPath = dataPath+"BasicData/SysDataBase.dat";
        string projectPath = dataPath+"BasicData/SysDataBase.project";

        var dbReader = new DatabaseMergedDataReader();
        WodiLib.Database.DatabaseMergedData data = await dbReader.ReadFilesAsync(datPath, projectPath);
        var mapDataList = data.GetDataDescList(0);
        if(mapNo>= mapDataList.Count)
        {
            return;
        }

        string mapPath = dataPath + mapDataList[mapNo].ItemValueList[0].StringValue.ToString();
        var mpsReader = new MpsFileReader();
        MapData mapData = await mpsReader.ReadFileAsync(mapPath);

        string tileSetPath = dataPath + "BasicData/TileSetData.dat";
        var reader = new TileSetDataFileReader();
        TileSetData setData = await reader.ReadFileAsync(tileSetPath);
        TileSetSetting tileSetting = setData.TileSetSettingList[mapData.TileSetId];

        string baseTilePath = dataPath + tileSetting.BaseTileSetFileName;
        infoText.text = baseTilePath;
        byte[] baseTileBytes = System.IO.File.ReadAllBytes(baseTilePath);
        Texture2D baseTileTexture = new Texture2D(1, 1);
        baseTileTexture.LoadImage(baseTileBytes);
        baseTileTexture.filterMode = FilterMode.Point;

        Texture2D[] autoTileTextures = new Texture2D[tileSetting.AutoTileFileNameList.Count];
        for (int i = 0; i < tileSetting.AutoTileFileNameList.Count; i++)
        {
            string autoTilePath = dataPath + tileSetting.AutoTileFileNameList[i].ToString();
            byte[] autoTileBytes = System.IO.File.ReadAllBytes(autoTilePath);
            autoTileTextures[i] = new Texture2D(1, 1);
            autoTileTextures[i].LoadImage(autoTileBytes);
            autoTileTextures[i].filterMode = FilterMode.Point;
        }

        const int chipSize = 16;

        Texture2D mapTexture
            = new Texture2D(mapData.MapSizeWidth * chipSize, mapData.MapSizeHeight * chipSize);
        for (int i = 0; i < mapData.MapSizeHeight; i++)
        {
            for (int j = 0; j < mapData.MapSizeWidth; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    int id = mapData.GetLayer(k).Chips[j][i];
                    if (mapData.GetLayer(k).Chips[j][i].IsAutoTile)
                    {
                        id = id / 100000 - 2;
                        if (id >= 0)
                        {
                            mapTexture.SetPixels(j * chipSize, (mapData.MapSizeHeight - i - 1) * chipSize, chipSize, chipSize,
                                autoTileTextures[id].GetPixels(0, autoTileTextures[id].height - 1 - chipSize, chipSize, chipSize));
                        }
                    }
                    else
                    {
                        mapTexture.SetPixels(j * chipSize, (mapData.MapSizeHeight - i - 1) * chipSize, chipSize, chipSize,
                            baseTileTexture.GetPixels((id % 8) * chipSize, baseTileTexture.height - (id / 8 + 1) * chipSize, chipSize, chipSize));
                    }
                }
            }
        }
        mapTexture.Apply();

        Sprite sprite = Sprite.Create(mapTexture,
            new Rect(0.0f, 0.0f, mapTexture.width, mapTexture.height), new Vector2(0.5f, 0.5f), 1.0f);
        mapSprite.sprite = sprite;
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
        var list = data.GetDataDescList(0).ToList();

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

        RenderMap(settingList[0]);
    }

    void RenderMap(TileSetSetting setting)
    {
        string baseTilePath = Application.streamingAssetsPath
            + "/Project/Data/" + setting.BaseTileSetFileName;
        infoText.text = baseTilePath;
        byte[] baseTileBytes = System.IO.File.ReadAllBytes(baseTilePath);
        Texture2D baseTileTexture = new Texture2D(1, 1);
        baseTileTexture.LoadImage(baseTileBytes);

        Texture2D[] autoTileTextures = new Texture2D[setting.AutoTileFileNameList.Count];
        for(int i = 0; i < setting.AutoTileFileNameList.Count; i++)
        {
            string autoTilePath = Application.streamingAssetsPath
            + "/Project/Data/" + setting.AutoTileFileNameList[i].ToString();
            byte[] autoTileBytes = System.IO.File.ReadAllBytes(autoTilePath);
            autoTileTextures[i] = new Texture2D(1, 1);
            autoTileTextures[i].LoadImage(autoTileBytes);
        }

        const int chipSize = 16;


        //Sprite sprite= Sprite.Create(tex,
        //    new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 1.0f);
        //mapSprite.sprite = sprite;
    }
}
