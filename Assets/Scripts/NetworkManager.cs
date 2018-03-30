using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class NetworkManager : MonoBehaviour {

    public static NetworkManager defaultManager { get; private set; }

    Encoding big5 = Encoding.GetEncoding("Big5");
    string host = "http://140.115.236.11/";


    private void Awake() {
        defaultManager = this;
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start () {
        GetData("王琛之", 2018, 3, s => print(s));
	}

    public void GetData(int year, int month, Action<string> completed) {
        GetData(UserSetting.userName, year, month, completed);
    }

    public void GetData(string userName, int year, int month, Action<string> completed) {
        StartCoroutine(DownloadDataFromServer(userName, year, month, completed));
    }

    IEnumerator DownloadDataFromServer(string userName, int year, int month, Action<string> completed) {
        string body = $"STR={userName}&MM2={month}&YY2={year}&item2=教師";
        var uploadHandler = new UploadHandlerRaw(big5.GetBytes(body));
        var downloadHandler = new DownloadHandlerBuffer();

        using (var request = new UnityWebRequest(host + "query_tea.asp", UnityWebRequest.kHttpVerbPOST, downloadHandler, uploadHandler)) {
            request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            yield return request.SendWebRequest();

            if (request.error != null) {
                ToastMessage.Show(request.error);
                yield break;
            }

            completed?.Invoke(big5.GetString(request.downloadHandler.data));
        }
    }
}
