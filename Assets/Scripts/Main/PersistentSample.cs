using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PersistentSample : MonoBehaviour
{
    [SerializeField]
    Text infoText;

    // Start is called before the first frame update
    void Start()
    {
        string filePath = Application.persistentDataPath + "/sample.txt";
        if (File.Exists(filePath))
        {
            using (var reader = new StreamReader(filePath))
            {
                infoText.text = reader.ReadToEnd();
            }
        }
        else
        {
            using(var writer=new StreamWriter(filePath))
            {
                writer.Write("This text will be seen.");
            }
            infoText.text = "Write text";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
