using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;

public class Test : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        string connection = $"URI=file:{Application.persistentDataPath}/My_Database";
        Debug.Log($"{connection}");
        IDbConnection dbcon = new SqliteConnection(connection);
        dbcon.Open();
        IDbCommand dbcmd;
        IDataReader reader;
        dbcmd = dbcon.CreateCommand();
        string q_createTable = $"CREATE TABLE IF NOT EXISTS my_table (id INTEGER PRIMARY KEY, val INTEGER )";
        dbcmd.CommandText = q_createTable;
        dbcmd.ExecuteReader();

        IDbCommand cmnd = dbcon.CreateCommand();
        cmnd.CommandText = $"INSERT INTO my_table (id, val) VALUES (1, 15)";
        cmnd.ExecuteNonQuery();

        IDbCommand cmnd_read = dbcon.CreateCommand();
        string query = "SELECT * FROM my_table";
        cmnd_read.CommandText = query;
        reader = cmnd_read.ExecuteReader();
        while (reader.Read())
        {
            Debug.Log($"id: {reader[0].ToString()}");
            Debug.Log($"val: {reader[1].ToString()}");
        }
        dbcon.Close();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
