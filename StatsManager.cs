// Finn O'Brien
// Project 24 (Stats database manager)
// 12/9/2021


using System;
using System.Data;
using System.Configuration;
using System.Data.SQLite;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project24
{
    class StatsManager
    {
        private static void VerifyTable()
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                conn.Execute("create table if not exists Stats (GameMode NOT NULL UNIQUE, Wins  INTEGER, Losses INTEGER, Ties  INTEGER, PRIMARY KEY(GameMode))");
                conn.Execute("insert or ignore into Stats (GameMode) values ('Classic')");
                conn.Execute("insert or ignore into Stats (GameMode) values ('Shared')");
            }
        }

        public static List<MDL_Stats> Load()
        {
            VerifyTable();
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                List<MDL_Stats> output = conn.Query<MDL_Stats>("select * from Stats", new DynamicParameters()).ToList();
                return output;
            }
        }

        public static void Update(MDL_Stats stat)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                conn.Execute("update Stats set Wins = @Wins, Losses = @Losses, Ties = @Ties where GameMode = @GameMode", stat);
            }
        }

        public static void Reset()
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                conn.Execute("drop table Stats");
            }
            VerifyTable();
        }

        private static string LoadConnectionString(string id = "DAT_Stats")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
