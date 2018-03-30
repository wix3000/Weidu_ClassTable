using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ClassTableFetcher : MonoBehaviour {

    const string DB_FILE_NAME = "local.db";
    const string API_ENTERPOINT = "http://140.115.236.11/query_tea.asp";

    public static SQLiteAccess defaultAccess;
    public static string dbFilePath => Application.persistentDataPath + "/" + DB_FILE_NAME;

    readonly Encoding big5 = Encoding.GetEncoding("Big5");

    public FetchType type;
    public FetchDetail detail { get; private set; }
    public TableAnalyzer analyzer { get; private set; }
    public event Action<ClassTableFetcher> OnFetchCompleted;

    private void Awake() {
        if (defaultAccess == null) {
            defaultAccess = new SQLiteAccess(dbFilePath);
        }
    }

    // Use this for initialization
    void Start () {
	}

    IEnumerator FetchHandle() {
        var detail = this.detail;
        string body;
        switch (type) {
            case FetchType.Teacher:
                body = $"STR={detail.teacher}&MM2={detail.date.Month}&YY2={detail.date.Year}&item2=教師";
                break;
            case FetchType.ClassCode:
                body = $"STR={detail.classCode}&MM2={detail.date.Month}&YY2={detail.date.Year}&item2=班級";
                break;
            default:
                OnFetchCompleted(null);
                yield break;
        }

        var uploadHandler = new UploadHandlerRaw(big5.GetBytes(body));
        var downloadHandler = new DownloadHandlerBuffer();

        using (var request = new UnityWebRequest(API_ENTERPOINT, UnityWebRequest.kHttpVerbPOST, downloadHandler, uploadHandler)) {
            request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            yield return request.SendWebRequest();

            if (request.error != null) {
                OnFetchCompleted(null);
                yield break;
            }

            var sourceText = big5.GetString(request.downloadHandler.data);
            analyzer = new TableAnalyzer(sourceText, type);
            OnFetchCompleted(this);
        }
    }

    public void Fetch(Action<ClassTableFetcher> completed = null) {
        if (completed != null) {
            OnFetchCompleted += completed;
        }

        StartCoroutine(FetchHandle());
    }

    public static ClassTableFetcher CreateFetch(string keyStr, DateTime date, FetchType type, Action<ClassTableFetcher> completed = null) {
        GameObject gameObject = new GameObject("Class Table Fetcher");
        var fetcher = gameObject.AddComponent<ClassTableFetcher>();
        FetchDetail detail = new FetchDetail() {
            date = date
        };
        switch (type) {
            case FetchType.Teacher:
                detail.teacher = keyStr;
                break;
            case FetchType.ClassCode:
                detail.classCode = keyStr;
                break;
        }
        fetcher.detail = detail;
        fetcher.Fetch(completed);
        return fetcher;
    }
}

public struct FetchDetail {
    public string teacher;
    public string classCode;
    public DateTime date;
}

public enum FetchType {
    Teacher,
    ClassCode
}