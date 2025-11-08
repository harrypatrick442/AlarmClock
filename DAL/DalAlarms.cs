using AlarmClock.Models;
using Core.DTOs;
using Database;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Claims;
using static ManagedCuda.NPP.NPPNativeMethods.NPPi;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

public sealed class DalAlarms
{
    private static readonly Lazy<DalAlarms> _instance = new(() => new DalAlarms());
    public static DalAlarms Instance => _instance.Value;

    public event EventHandler<Alarm>? OnSaved;
    public event EventHandler<int>? OnDeleted;
    private LocalSQLite _LocalSQLite;

    private const string
        CREATE_COMMAND =
            "CREATE TABLE IF NOT EXISTS tblAlarms(" +
                "id INTEGER PRIMARY KEY," +
                " hours INTEGER NOT NULL, " +
                " minutes INTEGER NOT NULL," +
                " enabled BOOL NOT NULL" +
            ");",
        SET_COMMAND = "INSERT OR REPLACE INTO tblAlarms (id, hours, minutes, enabled) VALUES(@id, @hours, @minutes, @enabled);",
        GET_ALL_COMMAND = "SELECT id, hours, minutes, enabled FROM tblAlarms;";

    private DalAlarms() {
        string filePath = Path.Combine(AppContext.BaseDirectory, $"alarms.sqlite");
        _LocalSQLite = new LocalSQLite(filePath, useUTF16: false, maxNConnections: 1);
        CreateTableIfNotExists();
    }
    private void CreateTableIfNotExists() {

        _LocalSQLite.UsingConnectionForWrite((connection) =>
        {
            using (SqliteCommand command = new SqliteCommand(
                CREATE_COMMAND,
                connection))
            {
                command.ExecuteNonQuery();
            }
        });
    }
    public List<Alarm> GetAll()
    {
        return _LocalSQLite.UsingConnection((connection) =>
        {
            using (var transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted))
            {
                using (SqliteCommand command = new SqliteCommand(
                    GET_ALL_COMMAND,
                    connection, transaction))
                {
                    using (SqliteDataReader dataReader = command.ExecuteReader())
                    {
                        List<Alarm> alarms = new List<Alarm>();
                        while (dataReader.Read())
                        {
                            alarms.Add(new Alarm
                            {
                                Id = dataReader.GetInt32(0),
                                Hour = dataReader.GetInt32(1),
                                Minute = dataReader.GetInt32(2),
                                Enabled = dataReader.GetBoolean(3)
                            });
                        }
                        return alarms;
                    }
                }
            }
        });
    }

    /// <summary>
    /// Saves an alarm: updates if it exists, creates if not.
    /// </summary>
    public void Set(Alarm alarm)
    {
        _LocalSQLite.UsingConnectionForWrite(connection =>
        {
            using (var transaction = connection.BeginTransaction())
            {
                using (SqliteCommand command = new SqliteCommand(
                    SET_COMMAND,
                    connection, transaction))
                {
                    command.Parameters.Add(new SqliteParameter("@id", alarm.Id));
                    command.Parameters.Add(new SqliteParameter("@hours", alarm.Hour));
                    command.Parameters.Add(new SqliteParameter("@minutes", alarm.Minute));
                    command.Parameters.Add(new SqliteParameter("@enabled", alarm.Enabled));
                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
            }
        });
    }
}
