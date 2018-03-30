using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.IO;
using System;

public class SQLiteAccess {

    public string filePath { get; private set; }
    public SqliteConnection connection { get; private set; }
    public SqliteCommand command { get; private set; }
    public SqliteDataReader reader { get; private set; }
    //public SqliteTransaction transaction { get; private set; }

    public bool isClosed => connection == null;

    public SQLiteAccess() { }

    /// <summary>
    /// 讀取指定路徑的SQLite資料庫檔案，並建立連結。
    /// </summary>
    /// <param name="dbFilePath">資料庫檔案絕對路徑</param>
    public SQLiteAccess(string dbFilePath) {
        ConnectTo(dbFilePath);
    }

    /// <summary>
    /// 連線至指定路徑的SQLite資料庫檔案。
    /// </summary>
    /// <param name="filePath"></param>
    public void ConnectTo(string filePath) {
        string connectionString;

        switch (Application.platform) {
            case RuntimePlatform.Android:
                connectionString = "URI=file:" + filePath;
                break;
            default:
                connectionString = "Data Source=" + filePath;
                break;
        }

        if (!isClosed) {
            Close();
        }

        try {
            connection = new SqliteConnection(connectionString);
            connection.Open();
            this.filePath = filePath;
            Debug.Log("Connected to SQLite.");
        }
        catch (System.Exception e) {
            Debug.Log(e.Message);
        }
    }

    /// <summary>
    /// 中斷與資料庫的連結，當物件被GC釋放時會自動執行。
    /// </summary>
    public void Close() {
        filePath = null;
        command?.Dispose();
        command = null;
        reader?.Close();
        reader = null;
        connection?.Close();
        connection?.Dispose();
        connection = null;
    }

    public SqliteDataReader Query(string commandText) {
        if (isClosed) {
            throw new SqliteException("Database wasn't connecting.");
        }

        command = new SqliteCommand(commandText, connection);
        reader = command.ExecuteReader();
        return reader;
    }

    public object QueryFirstRow(string commandText) {
        if (isClosed) {
            throw new SqliteException("Database wasn't connecting.");
        }

        command = new SqliteCommand(commandText, connection);
        return command.ExecuteScalar();
    }

    /// <summary>
    /// 檢查特定名稱的資料表是否存在。
    /// </summary>
    /// <param name="tableName">資料表名稱</param>
    /// <returns></returns>
    public bool IsTableExists(string tableName) {

        string sql = $"SELECT COUNT(*) FROM sqlite_master where type='table' and name='{tableName}';";
        object result = QueryFirstRow(sql);

        return (Int64)result == 1;
    }

    ~SQLiteAccess() {
        if (!isClosed) {
            Close();
        }
    }
}
