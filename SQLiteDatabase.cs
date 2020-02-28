using Mono.Data.Sqlite;
using System.Data;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

[ExecuteInEditMode]
public class SQLiteDatabase : MonoBehaviour
{
    public string databaseName;
    public string tableName = "UKACCIDENTS";
    public List<string> tableNames = new List<string>();
    public List<string> columns = new List<string>();
    
    IDbConnection dbcon;
    List<Dictionary<string, object>> records;


    private void Start()
    {
       Open();
       ReadTable();
    }


    private void OnEnable()
    {
        Open();
        ReadTable();
    }

    /// <summary>
    /// Get path of the database
    /// </summary>
    /// <returns></returns>
    public string GetDBPath()
    {
        string path = "";
#if UNITY_EDITOR
        path = string.Format(@"Assets/StreamingAssets/Database/{0}", databaseName);
#endif

#if UNITY_STANDALONE
        path = string.Format(@Application.streamingAssetsPath + "/StreamingAssets/Database/{0}", databaseName);
#endif
        return path;
    }

    public void Open()
    {
        tableNames = new List<string>();
        columns = new List<string>();
        string connection = "";

#if UNITY_EDITOR
        connection = string.Format(@"URI=file:" + "Assets/StreamingAssets/Database/{0}", databaseName);
#endif

#if UNITY_STANDALONE
        connection = string.Format(@"URI=file:" + Application.streamingAssetsPath + "/Database/{0}", databaseName);
#endif

        dbcon = new SqliteConnection(connection);
        dbcon.Open();


        IDbCommand dbcmd2;
        IDataReader reader2;

        dbcmd2 = dbcon.CreateCommand();
        string query2 =
          "SELECT name FROM sqlite_master WHERE type = 'table'";

        dbcmd2.CommandText = query2;
        reader2 = dbcmd2.ExecuteReader();

        while (reader2.Read())
        {        
           tableNames.Add(reader2[0].ToString());
        }
        reader2.Close();
    }

    public void ReadFields()
    {
        IDbCommand dbcmd;
        IDataReader reader;

        dbcmd = dbcon.CreateCommand();
        string query =
          "SELECT * FROM " + tableName;

        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();

        if (reader.Read())
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columns.Add(reader.GetName(i));
            }
            reader.Close();
        }
    }

    public void ReadTable()
    {
        IDbCommand dbcmd;
        IDataReader reader;

        dbcmd = dbcon.CreateCommand();
        string query =
          "SELECT * FROM " + tableName;

        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();

        if (reader.Read())
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                columns.Add(reader.GetName(i));
            }
            reader.Close();
        }
        records = ReadAllRecords();
    }

    public List<string> GetTableNames()
    {
        return tableNames;
    }

    public List<string> GetFieldNames()
    {
        return columns;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="condition"> = 100, != 100</param>
    /// <returns></returns>
    public List<IDataRecord> GetRecordsBy(string condition)
    {
        IDbCommand dbcmd;
        IDataReader reader;

        dbcmd = dbcon.CreateCommand();
        string query =
          "SELECT * FROM " + tableName + " WHERE " + " " + condition;

        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();

        List<IDataRecord> records = new List<IDataRecord>();
        while (reader.Read())
        {
            records.Add(reader);
        }

        return records;
    }

    public List<Dictionary<string, object>> GetRecordsByField(string fieldname)
    {
        IDbCommand dbcmd;
        IDataReader reader;

        dbcmd = dbcon.CreateCommand();
        string query =
          "SELECT " + fieldname + " FROM " + tableName;

        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();

        List<Dictionary<string, object>> records = new List<Dictionary<string, object>>();
        while (reader.Read())
        {
            Dictionary<string, object> rec = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                rec.Add(reader.GetName(i), reader[i]);
            }
            records.Add(rec);
        }

        return records;
    }

    public List<Dictionary<string, object>> GetRecordsByField(string fieldname, string condition)
    {
        IDbCommand dbcmd;
        IDataReader reader;

        dbcmd = dbcon.CreateCommand();
        string query =
          "SELECT " + fieldname + " FROM " + tableName + " WHERE " + condition;

        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();

        List<Dictionary<string, object>> records = new List<Dictionary<string, object>>();
        while (reader.Read())
        {
            Dictionary<string, object> rec = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                rec.Add(reader.GetName(i), reader[i]);
            }
            records.Add(rec);
        }

        return records;
    }

    public List<float> GetFloatRecordsByField(string fieldname, string con)
    {
        List<float> r = new List<float>();
        List<Dictionary<string, object>> rec = GetRecordsByField(fieldname, con);
        for (int i = 0; i < rec.Count; i++)
        {
            Dictionary<string, object> d = rec[i];
            r.Add(float.Parse(d[fieldname].ToString()));
        }
        return r;
    }

    public List<float> GetFloatRecordsByField(string fieldname)
    {
        List<float> r = new List<float>();
        List<Dictionary<string, object>> rec = GetRecordsByField(fieldname);
        for(int i = 0; i < rec.Count; i++)
        {
            Dictionary<string, object> d = rec[i];
            r.Add(float.Parse(d[fieldname].ToString()));
        }
        return r;
    }

    public List<string> GetStringRecordsByField(string fieldname)
    {
        List<string> r = new List<string>();
        List<Dictionary<string, object>> rec = GetRecordsByField(fieldname);
        for (int i = 0; i < rec.Count; i++)
        {
            Dictionary<string, object> d = rec[i];
            r.Add(d[fieldname].ToString());
        }
        return r;
    }

    public List<Dictionary<string, object>> GetAllRecords()
    {
        return records;
    }


    List<Dictionary<string, object>> ReadAllRecords()
    {
        IDbCommand dbcmd;
        IDataReader reader;

        dbcmd = dbcon.CreateCommand();
        string query =
          "SELECT * FROM " + tableName;

        dbcmd.CommandText = query;
        reader = dbcmd.ExecuteReader();

        List<Dictionary<string, object>> records = new List<Dictionary<string, object>>();
        while (reader.Read())
        {
            Dictionary<string, object> rec = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                rec.Add(reader.GetName(i), reader[i]);
            }
            records.Add(rec);
        }

        return records;
    }

}
