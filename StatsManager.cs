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
                conn.Execute("create table if not exists Stats (GameMode TEXT NOT NULL UNIQUE, Wins  INTEGER, Losses INTEGER, Ties  INTEGER, PRIMARY KEY(GameMode))");
                conn.Execute("insert or ignore into Stats (GameMode) values ('Classic')");
                conn.Execute("insert or ignore into Stats (GameMode) values ('Custom')");
                conn.Execute("create table if not exists Foils (ID INTEGER UNIQUE, Name TEXT NOT NULL UNIQUE, File TEXT, PRIMARY KEY(ID AUTOINCREMENT))");
                conn.Execute("insert or ignore into Foils (Name, File) values ('Default', '/Resources/FL_Default.png')");
                conn.Execute("update or ignore Foils set Name = 'Contrast', File = '/Resources/FL_Contrast.png' where Name = 'Secondary'");
                conn.Execute("insert or ignore into Foils (Name, File) values ('Contrast', '/Resources/FL_Contrast.png')");
            }
        }

        public static void PerfectWin()
        {
            VerifyTable();
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString(), true))
            {
                conn.Execute("insert or ignore into Foils (Name, File) values ('Gold', '/Resources/FL_Gold.png')");
            }
        }

        public static void UnlockCard(string name, string file)
        {
            VerifyTable();
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString(), true))
            {
                conn.Execute($"insert or ignore into Foils (Name, File) values ('{name}', '/Resources/FL_{file}.png')");
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
