using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;
using WodiLib.UnityUtil.IO;
using System;
using WodiLib.Map;

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
        ReadTest();
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
}
