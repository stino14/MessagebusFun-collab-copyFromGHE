using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using Dapper;
using System.IO;

namespace MessageBusFun.Core.Database
{
    public class SqlLiteDataAccess
    {
        private static string _connectionString = @"Data Source=.\MessageBusFun.sqlite;Version=3";

        private static string LoadConnectionString()
        {
            return @"Data Source=.\MessageBusFun.sqlite;Version=3";
        }
        
        public static List<User> LoadUsers()
        {
            using (IDbConnection con = new SQLiteConnection(LoadConnectionString()))
            {
                var output = con.Query<User>("select * from User", new DynamicParameters());
                return output.ToList();
            }
        }

        public static List<Message> LoadMessages()
        {
            using (var con = new SQLiteConnection(LoadConnectionString()))
            {
                var output = con.Query<Message>("select * from Message", new DynamicParameters());
                return output.ToList();
            }
        }

        public static List<Subscriber> LoadSubscribers()
        {
            using (var con = new SQLiteConnection(LoadConnectionString()))
            {
                var output = con.Query<Subscriber>("select * from Subscriber", new DynamicParameters());
                return output.ToList();
            }
        }

        public static List<Provider> LoadProviders()
        {
            using (var con = new SQLiteConnection(LoadConnectionString()))
            {
                var output = con.Query<Provider>("select * from Provider", new DynamicParameters());
                return output.ToList();
            }
        }

        private static void CreateDatabase()
        {
            var dbFilename = "./MessageBusFun.sqlite";
            if (!File.Exists(dbFilename))
            {
                SQLiteConnection.CreateFile(dbFilename);
                SeedDatabase();

            }
        }

        private static void SeedDatabase()
        {
            using (var con = new SQLiteConnection(LoadConnectionString()))
            {
                con.Execute(@"
                    CREATE TABLE IF NOT EXISTS [Users] (
                        [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                        [Username] NVARCHAR(64) NOT NULL,
                        [Password] NVARCHAR(128) NOT NULL
                    )");

                con.Execute(@"
                    INSERT INTO Users
                        (Username, Password)
                    VALUES
                        ('test', 'test')");

                con.Execute(@"
                    CREATE TABLE IF NOT EXISTS [Channel] (
                        [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                        [Name] NVARCHAR(64) NOT NULL
                    )");

                con.Execute(@"
                    CREATE TABLE IF NOT EXISTS [Message] (
                        [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                        [ChannelId] INTEGER NOT NULL,
                        [SubscriberId] INTEGER NOT NULL,
                        [MessageString] NVARCHAR(1024) NOT NULL
                    )");

                con.Execute(@"
                    CREATE TABLE IF NOT EXISTS [Subscriber] (
                        [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                        [Username] NVARCHAR(64) NOT NULL,
                        [UserId] INTEGER NOT NULL,
                        [ClientId] INTEGER NOT NULL,
                        [ChannelId] INTEGER NOT NULL
                    )");

                con.Execute(@"
                    CREATE TABLE IF NOT EXISTS [Provider] (
                        [Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                        [Username] NVARCHAR(64) NOT NULL,
                        [UserId] INTEGER NOT NULL,
                        [ClientId] INTEGER NOT NULL,
                        [ChannelId] INTEGER NOT NULL
                    )");
            }
        }
    }
}
