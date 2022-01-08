using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository;

namespace TestWebAPI
{
    public class Database:IDatabase
    {
        internal static string ConnectionString;
        internal static string ConnectionStringLog; 
        internal  IRepository _TestWebAPI;
        internal IRepository _TestWebAPI_Log;
        public Database() 
        {
            System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ConnectionString);
            System.Data.SqlClient.SqlConnection connection_Log = new System.Data.SqlClient.SqlConnection(ConnectionStringLog);
            //_TestWebAPI = RepositoryFactory.CreateRepository(ConnectionString);
            _TestWebAPI = RepositoryFactory.CreateRepository(connection);

            _TestWebAPI.__KeyGenerator = keygenerator; // here you define mechanism of generation : There are two mechanisms for creating an ID

            //_TestWebAPI_Log = RepositoryFactory.CreateRepository(connection_Log);
            _TestWebAPI_Log = RepositoryFactory.CreateRepository(ConnectionStringLog);

        }

        private string keygenerator(string ClassName)
        {
            return Guid.NewGuid().ToString();
        }

        IRepository IDatabase.TestWebAPI 
        {
            get { return _TestWebAPI; }
        }
        IRepository IDatabase.TestWebAPI_Log {
            get { return _TestWebAPI_Log; }
        }

        internal static bool Initialize(string _ConnectionString, string _ConnectionStringLog)
        {
            ConnectionString = _ConnectionString;
            ConnectionStringLog = _ConnectionStringLog;
            RunMigration("TestWebAPI", _ConnectionString);
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
