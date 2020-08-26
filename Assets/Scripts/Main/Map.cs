using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;
using WodiLib.UnityUtil.IO;
using WodiLib.Map;
using System.Linq;
using WodiLib.Database;
using Models;

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

    string dataPath = Application.streamingAssetsPath + "/Project/Data/";

    Texture2D baseTileTexture;
    Texture2D[] autoTileTextures;

    int chipSize;
    int[] autoTileMasks;
    int[] autoTileXs;
    int[] autoTileYs;

    // Start is called before the first frame update
    void Start()
    {
        chipSize = GameBasicInfo.Instance.TilePixels;

        autoTileMasks = new int[4] { 1, 10, 100, 1000 };
        autoTileXs = new int[4] { chipSize / 2, 0, chipSize / 2, 0 };
        autoTileYs = new int[4] { chipSize / 2, chipSize / 2, 0, 0 };

        RenderMap();
    }

    // Update is called once per frame
    void Update()
    {

    }

    async void RenderMap()
    {
        DatabaseMergedData systemDB = await ReadSystemDB();
        var mapDataList = systemDB.GetDataDescList(0);
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

        ReadBaseTileTexture(tileSetting.BaseTileSetFileName);

        ReadAutoTileTextures(tileSetting.AutoTileFileNameList.ToArray());

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
                        RenderAutoTile(mapTexture, j, i, id);
                    }
                    else
                    {
                        RenderNormalTile(mapTexture, j, i, id);
                    }
                }
            }
        }

        for (int i = 0; i < mapData.MapEvents.Count; i++)
        {
            int x = mapData.MapEvents[i].Position.X;
            int y = mapData.MapEvents[i].Position.Y;
            MapEventPage mapEventPage = mapData.MapEvents[i].MapEventPageList[0];
            if (mapEventPage.GraphicInfo.IsGraphicTileChip)
            {
                int tileId = mapEventPage.GraphicInfo.GraphicTileId;
                RenderNormalTile(mapTexture, x, y, tileId);
            }
            else if (!string.IsNullOrEmpty(mapEventPage.GraphicInfo.CharaChipFilePath))
            {
                string graphicPath = dataPath + mapEventPage.GraphicInfo.CharaChipFilePath;
                Debug.Log(graphicPath);
                CharaChipDirection charaChipDirection = mapEventPage.GraphicInfo.InitDirection;
                Texture2D charaChipTexture = new Texture2D(1, 1);
                byte[] bytes = System.IO.File.ReadAllBytes(graphicPath);
                charaChipTexture.LoadImage(bytes);
                charaChipTexture.filterMode = FilterMode.Point;
                RenderCharaChip(mapTexture, x, y, charaChipTexture, charaChipDirection);
            }
        }

        mapTexture.Apply();
        mapTexture.filterMode = FilterMode.Point;

        Sprite sprite = Sprite.Create(mapTexture,
            new Rect(0.0f, 0.0f, mapTexture.width, mapTexture.height), new Vector2(0.5f, 0.5f), 1.0f);
        mapSprite.sprite = sprite;
    }

    async Task<DatabaseMergedData> ReadSystemDB()
    {
        string datPath = dataPath + "BasicData/SysDataBase.dat";
        string projectPath = dataPath + "BasicData/SysDataBase.project";

        var dbReader = new DatabaseMergedDataReader();
        DatabaseMergedData data = await dbReader.ReadFilesAsync(datPath, projectPath);
        return data;
    }

    void ReadBaseTileTexture(string baseTilePath)
    {
        baseTileTexture = new Texture2D(1, 1);
        byte[] bytes = System.IO.File.ReadAllBytes(dataPath + baseTilePath);
        baseTileTexture.LoadImage(bytes);
        baseTileTexture.filterMode = FilterMode.Point;
    }

    void ReadAutoTileTextures(AutoTileFileName[] autoTilePaths)
    {
        autoTileTextures = new Texture2D[autoTilePaths.Length];

        for (int i = 0; i < autoTilePaths.Length; i++)
        {
            string autoTilePath = dataPath + autoTilePaths[i].ToString();
            byte[] autoTileBytes = System.IO.File.ReadAllBytes(autoTilePath);
            autoTileTextures[i] = new Texture2D(1, 1);
            autoTileTextures[i].LoadImage(autoTileBytes);
            autoTileTextures[i].filterMode = FilterMode.Point;
        }
    }

    void RenderAutoTile(Texture2D target, int x, int y, int id)
    {
        int tileId = id / 100000 - 2;
        if (tileId < 0)
        {
            return;
        }

        for (int i = 0; i < chipSize / 2; i++)
        {
            for (int j = 0; j < chipSize / 2; j++)
            {
                for (int k = 0; k < 4; k++)
                {
                    int code = (id / autoTileMasks[k]) % 10;
                    Color color = autoTileTextures[tileId].GetPixel(autoTileXs[k] + j, autoTileTextures[tileId].height - (code + 1) * chipSize + chipSize / 2 - autoTileYs[k] + i);
                    if (color.a == 1)
                    {
                        target.SetPixel(x * chipSize + autoTileXs[k] + j, target.height - (y + 1) * chipSize + chipSize / 2 - autoTileYs[k] + i, color);
                    }
                }
            }
        }
    }

    void RenderNormalTile(Texture2D target,int x,int y,int id)
    {
        for(int i = 0; i < chipSize; i++)
        {
            for(int j = 0; j < chipSize; j++)
            {
                Color color = baseTileTexture.GetPixel(id % 8 * chipSize + j, baseTileTexture.height - (id / 8 + 1) * chipSize + i);
                if (color.a == 1)
                {
                    target.SetPixel(x * chipSize + j, target.height - (y + 1) * chipSize + i, color);
                }
            }
        }
    }

    void RenderCharaChip(Texture2D target, int x, int y,
        Texture2D charaChipTexture, CharaChipDirection direction)
    {
        int charaWidth = charaChipTexture.width
            / (GameBasicInfo.Instance.CharaAnimations * GameBasicInfo.Instance.CharaImageDirections / 4);
        int charaHeight = charaChipTexture.height / 4;

        int xOffset = (chipSize - charaWidth) / 2;

        for (int i = 0; i < charaHeight; i++)
        {
            for (int j = 0; j < charaWidth; j++)
            {
                Color color = charaChipTexture.GetPixel(j, charaChipTexture.height - charaHeight + i);
                if (color.a == 1 &&
                    x * chipSize + j + xOffset >= 0 && x * chipSize + j + xOffset < target.width)
                {
                    target.SetPixel(x * chipSize + j + xOffset,
                        target.height - (y + 1) * chipSize + i, color);
                }
            }
        }
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
    }
}
