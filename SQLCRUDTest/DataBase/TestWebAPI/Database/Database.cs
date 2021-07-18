using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository;

namespace TestWebAPI
{
    public class Database
    {
        internal static string ConnectionString;
        internal static IRepository TestWebAPI;
        internal static IRepository TestWebAPI_Log;

        internal static bool Initialize(string _ConnectionString, string _ConnectionStringLog)
        {
            ConnectionString = _ConnectionString;
            TestWebAPI = RepositoryFactory.CreateRepository(_ConnectionString);
            RunMigration("TestWebAPI", _ConnectionString);
            TestWebAPI_Log = RepositoryFactory.CreateRepository(_ConnectionStringLog);
            RunMigration("TestWebAPI_Log", _ConnectionStringLog);
            return true;
        }
        private static void RunMigration(string ProjectName, string _ConnectionString)
        {
            if (_ConnectionString == "" || _ConnectionString == null)
            {
                return;
            }
            SQLMigrationByQuery.requestMigration objRequest = new SQLMigrationByQuery.requestMigration();
            objRequest.ConnectionString = _ConnectionString;
            objRequest.CallerProjectName = ProjectName;
            objRequest.MigrationMark = ProjectName + "-";
            objRequest.ReplaceTextInQuery = false;
            objRequest.ReplaceTextSource = "";
            objRequest.ReplaceTextTarget = "";
            SQLMigrationByQuery.resultMigration objResult = SQLMigrationByQuery.helperMigration.getApplyMigration(objRequest);
            if (objResult.Success != true)
            {
            }
        }
    }
}
