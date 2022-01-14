// Finn O'Brien
// Project 24 (database manager)
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
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString(), true))
            {
                conn.Execute("create table if not exists Stats (GameMode NOT NULL UNIQUE, Wins  INTEGER, Losses INTEGER, Ties  INTEGER, PRIMARY KEY(GameMode))");
                conn.Execute("insert or ignore into Stats (GameMode) values ('Classic')");
                conn.Execute("insert or ignore into Stats (GameMode) values ('Custom')");
                conn.Execute("create table if not exists Foils (ID NOT NULL UNIQUE, Name TEXT, File TEXT, PRIMARY KEY(ID))");
                conn.Execute("insert or ignore into Foils (ID, Name, File) values (0, 'Default', '/Resources/FL_Default.png')");
            }
        }

        public static List<MDL_Foil> LoadFoils()
        {
            VerifyTable();
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString(), true))
            {
                List<MDL_Foil> output = conn.Query<MDL_Foil>("select * from Foils", new DynamicParameters()).ToList();
                return output;
            }
        }

        public static List<MDL_Stats> Load()
        {
            VerifyTable();
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString(), true))
            {
                List<MDL_Stats> output = conn.Query<MDL_Stats>("select * from Stats", new DynamicParameters()).ToList();
                return output;
            }
        }

        public static void Update(MDL_Stats stat)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString(), true))
            {
                conn.Execute("update Stats set Wins = @Wins, Losses = @Losses, Ties = @Ties where GameMode = @GameMode", stat);
            }
        }

        public static void Reset()
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString(), true))
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
