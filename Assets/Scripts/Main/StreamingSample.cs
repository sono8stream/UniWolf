using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class StreamingSample : MonoBehaviour
{
    [SerializeField]
    Text pathText;

    [SerializeField]
    Text infoText;

    // Start is called before the first frame update
    void Start()
    {
        LoadAsync();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    async void LoadAsync()
    {
        infoText.text = "Loading ...";

        string path= Application.streamingAssetsPath + "/aiueo.txt";
        UnityWebRequest www = UnityWebRequest.Get(path);
        await www.SendWebRequest();

        infoText.text = www.downloadHandler.text;
    }

    IEnumerator Loader()
    {
        string path = Application.streamingAssetsPath + "/aiueo.txt";
        pathText.text = path;
        infoText.text = "Loading ...";


        using (UnityWebRequest www = UnityWebRequest.Get(path))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                infoText.text = "Cannot load";
                yield break;
            }

            infoText.text = www.downloadHandler.text;
        }
    }
}

public class UnityWebRequestAwaiter : INotifyCompletion
{
    private UnityWebRequestAsyncOperation asyncOp;
    private Action continuation;

    public UnityWebRequestAwaiter(UnityWebRequestAsyncOperation asyncOp)
    {
        this.asyncOp = asyncOp;
        asyncOp.completed += OnRequestCompleted;
    }

    public bool IsCompleted { get { return asyncOp.isDone; } }

    public void GetResult() { }

    public void OnCompleted(Action continuation)
    {
        this.continuation = continuation;
    }

    private void OnRequestCompleted(AsyncOperation obj)
    {
        continuation?.Invoke();
    }
}

public static class ExtensionMethods
{
    public static UnityWebRequestAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
    {
        return new UnityWebRequestAwaiter(asyncOp);
    }
}