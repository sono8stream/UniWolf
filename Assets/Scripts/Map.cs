using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;
using WodiLib.Database;
using System;

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
        pathText.text = Application.streamingAssetsPath;
        string path = Application.streamingAssetsPath + "/Project";
        Debug.Log(path);
        Task.Run(() => {
            Debug.Log(projectPath);
            var project = new WodiLib.Project.WoditorProject(path);
            Debug.Log("loaded");
            infoText.text = project.GetDatabaseDataDescList(DBKind.System, 0)[0].DataName.ToString();
            Debug.Log("done");
            //var hoge=project.ReadMpsFileSync(new WodiLib.IO.MpsFilePath("Data\\SampleMapA.mps"));
            //Debug.Log("aa");
        });
        //try
        //{
            var mpsReader = new WodiLib.IO.MpsFileReader(new WodiLib.IO.MpsFilePath(Application.streamingAssetsPath + "/Project/Data/MapData/SampleMapA.mps"));
            Debug.Log("aiueo");
            var mapData = mpsReader.ReadSync();
            infoText.text = mapData.MapSizeWidth.ToString();
        //}
        //catch (Exception e)
        //{
        //    infoText.text = e.Message;
        //}

        //var mapData = MpsProvider.Load(projectPath);
        //Debug.Log(mapData.MapSizeWidth);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
