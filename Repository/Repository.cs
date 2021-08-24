using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Repository.Domain;
using static Dapper.SqlMapper;

namespace Repository
{
    internal class Repository : IRepository
    {
        Dictionary<string, object> TableFactories = new Dictionary<string, object>();
        internal IDbConnection Connection;
        internal IDbTransaction Transaction;
        private string _ConnectionString;
        private Int16 _TransactionCount;
        public Repository(string connectionString)
        {
            _ConnectionString = connectionString;
        }

        internal void OpenConnection()
        {
            if (_ConnectionString == "" || _ConnectionString == null)
            {
                throw new Exception("Connection string is empty.");
            }
            if (Connection == null)
            {
                Connection = new SqlConnection(_ConnectionString);
            }

            if (Connection.State == ConnectionState.Open)
            {
                Connection.Close();
            }
            Connection.Open();
        }
        internal void CloseConnection()
        {
            if (_ConnectionString == "" || _ConnectionString == null)
            {
                throw new Exception("Connection string is empty.");
            }
            if (Connection == null)
            {
                throw new Exception("Connection string is empty.");
            }

            if (Connection.State == ConnectionState.Open)
            {
                Connection.Close();
            }

            Connection = null;
        }
        public bool BeginTransaction()
        {
            if (Transaction is null)
            {
                OpenConnection();
                Transaction = Connection.BeginTransaction();
            }
            _TransactionCount++;
            return true;
        }

        public bool CommitTransaction()
        {
            if (_TransactionCount == 0)
            {
                throw new Exception("You must be run begintransaction first!");
            }
            if (Transaction is null)
            {
                throw new Exception("Developer Must be checked!");
            }
            _TransactionCount--;
            if (_TransactionCount==0)
            {
                Transaction.Commit();
                Transaction = null;
                CloseConnection();
            }
            return true;
        }

        public bool RollbackTransaction()
        {
            if (_TransactionCount == 0)
            {
                throw new Exception("You must be run begintransaction first!");
            }
            if (Transaction is null)
            {
                throw new Exception("Developer Must be checked!");
            }
            _TransactionCount--;
            if (_TransactionCount == 0)
            {
                Transaction.Rollback();
                Transaction = null;
                CloseConnection();
            }
            else
            {
                throw new Exception("Inner Transaction Rollback.");
            }
            return true;
        }

        private GeneralFactory<T> GetGeneralFactory<T>()
        {
            object oGeneralFactory;
            TableFactories.TryGetValue(typeof(T).Name, out oGeneralFactory);
            if (oGeneralFactory is null)
            {
                oGeneralFactory = new GeneralFactory<T>(this);
                TableFactories.Add(typeof(T).Name, oGeneralFactory);
            }
            return (GeneralFactory<T>)oGeneralFactory;
        }

        public bool Save<T>(T o)
        {

            if (_ConnectionString == "" || _ConnectionString == null)
            {
                throw new Exception("Connection string is empty.");
            }
            var oGeneralFactory = GetGeneralFactory<T>();
            return oGeneralFactory.Save(ref o);


        }
        public bool Delete<T>(T o)
        {
            if (_ConnectionString == "" || _ConnectionString == null)
            {
                throw new Exception("Connection string is empty.");
            }
            var oGeneralFactory = GetGeneralFactory<T>();
            return oGeneralFactory.Delete(o);
        }

        public T GetByID<T>(string ID, bool withLock)
        {
            if (_ConnectionString == "" || _ConnectionString == null)
            {
                throw new Exception("Connection string is empty.");
            }
            var oGeneralFactory = GetGeneralFactory<T>();
            return oGeneralFactory.GetByID(ID, withLock);
        }

        public List<T> GetAll<T>()
        {
            if (_ConnectionString == "" || _ConnectionString == null)
            {
                throw new Exception("Connection string is empty.");
            }
            var oGeneralFactory = GetGeneralFactory<T>();
            return oGeneralFactory.GetAll();
        }

        public List<T> Execute_StoredProcedure<T>(string ProcedureName, Dictionary<string, string> Parameters)
        {
            return (List<T>)Execute_StoredProcedure<T, object, object, object, object, object, object>(ProcedureName, Parameters).FirstOrDefault();
        }

        public List<object> Execute_StoredProcedure<TFirst, TSecond>(string ProcedureName, Dictionary<string, string> Parameters)
        {
            return Execute_StoredProcedure<TFirst, TSecond, object, object, object, object, object>(ProcedureName, Parameters).Take(2).ToList();
        }

        public List<object> Execute_StoredProcedure<TFirst, TSecond, TThird>(string ProcedureName, Dictionary<string, string> Parameters)
        {
            return Execute_StoredProcedure<TFirst, TSecond, TThird, object, object, object, object>(ProcedureName, Parameters).Take(3).ToList();
        }

        public List<object> Execute_StoredProcedure<TFirst, TSecond, TThird, TFour>(string ProcedureName, Dictionary<string, string> Parameters)
        {
            return Execute_StoredProcedure<TFirst, TSecond, TThird, TFour, object, object, object>(ProcedureName, Parameters).Take(4).ToList();
        }

        public List<object> Execute_StoredProcedure<TFirst, TSecond, TThird, TFour, TFive>(string ProcedureName, Dictionary<string, string> Parameters)
        {
            return Execute_StoredProcedure<TFirst, TSecond, TThird, TFour, TFive, object, object>(ProcedureName, Parameters).Take(5).ToList();
        }

        public List<object> Execute_StoredProcedure<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(string ProcedureName, Dictionary<string, string> Parameters)
        {
            return Execute_StoredProcedure<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, object>(ProcedureName, Parameters).Take(6).ToList();
        }

        public List<object> Execute_StoredProcedure<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(string ProcedureName, Dictionary<string, string> Parameters)
        {
            return Execute_StoredProcedure<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, object>(ProcedureName, Parameters).Take(7).ToList();
        }

        public List<object> Execute_StoredProcedure<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TEighth>(string ProcedureName, Dictionary<string, string> Parameters)
        {
            return Execute_StoredProcedure<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TEighth, object>(ProcedureName, Parameters).Take(8).ToList();
        }

        public List<object> Execute_StoredProcedure<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TEighth, TNinth>(string ProcedureName, Dictionary<string, string> Parameters)
        {
            return Execute_StoredProcedure<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TEighth, TNinth, object>(ProcedureName, Parameters).Take(9).ToList();
        }

        public List<object> Execute_StoredProcedure<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TEighth, TNinth, TTenth>(string ProcedureName, Dictionary<string, string> Parameters)
        {
            if (typeof(TFirst).Name != "object")
            {
                TableInfoAttribute TFirst_TableInfo = typeof(TFirst).GetTableInfo();
                if (TFirst_TableInfo.TableType == enmTableType.Writeonly)
                {
                    throw new Exception("Object is Writeonly.You can not perform (Execute_StoredProcedure) in this Object. objectName:" + TFirst_TableInfo.TableName);
                }
            }

            if (typeof(TSecond).Name != "object")
            {
                TableInfoAttribute TSecond_TableInfo = typeof(TSecond).GetTableInfo();
                if (TSecond_TableInfo.TableType == enmTableType.Writeonly )
                {
                    throw new Exception("Object is Writeonly.You can not perform (Execute_StoredProcedure) in this Object. objectName:" + TSecond_TableInfo.TableName);
                }
            }

            if (typeof(TThird).Name != "object")
            {
                TableInfoAttribute TThird_TableInfo = typeof(TThird).GetTableInfo();
                if (TThird_TableInfo.TableType == enmTableType.Writeonly)
                {
                    throw new Exception("Object is Writeonly.You can not perform (Execute_StoredProcedure) in this Object. objectName:" + TThird_TableInfo.TableName);
                }
            }

            if (typeof(TFourth).Name != "object")
            {
                TableInfoAttribute TFourth_TableInfo = typeof(TFourth).GetTableInfo();
                if (TFourth_TableInfo.TableType == enmTableType.Writeonly)
                {
                    throw new Exception("Object is Writeonly.You can not perform (Execute_StoredProcedure) in this Object. objectName:" + TFourth_TableInfo.TableName);
                }
            }
            if (typeof(TFifth).Name != "object")
            {
                TableInfoAttribute TFifth_TableInfo = typeof(TFifth).GetTableInfo();
                if (TFifth_TableInfo.TableType == enmTableType.Writeonly)
                {
                    throw new Exception("Object is Writeonly.You can not perform (Execute_StoredProcedure) in this Object. objectName:" + TFifth_TableInfo.TableName);
                }
            }
            if (typeof(TSixth).Name != "object")
            {
                TableInfoAttribute TSixth_TableInfo = typeof(TSixth).GetTableInfo();
                if (TSixth_TableInfo.TableType == enmTableType.Writeonly)
                {
                    throw new Exception("Object is Writeonly.You can not perform (Execute_StoredProcedure) in this Object. objectName:" + TSixth_TableInfo.TableName);
                }
            }
            if (typeof(TSeventh).Name != "object")
            {
                TableInfoAttribute TSeventh_TableInfo = typeof(TSeventh).GetTableInfo();
                if (TSeventh_TableInfo.TableType == enmTableType.Writeonly)
                {
                    throw new Exception("Object is Writeonly.You can not perform (Execute_StoredProcedure) in this Object. objectName:" + TSeventh_TableInfo.TableName);
                }
            }
            if (typeof(TEighth).Name != "object")
            {
                TableInfoAttribute TEighth_TableInfo = typeof(TEighth).GetTableInfo();
                if (TEighth_TableInfo.TableType == enmTableType.Writeonly)
                {
                    throw new Exception("Object is Writeonly.You can not perform (Execute_StoredProcedure) in this Object. objectName:" + TEighth_TableInfo.TableName);
                }
            }
            if (typeof(TNinth).Name != "object")
            {
                TableInfoAttribute TNinth_TableInfo = typeof(TNinth).GetTableInfo();
                if (TNinth_TableInfo.TableType == enmTableType.Writeonly)
                {
                    throw new Exception("Object is Writeonly.You can not perform (Execute_StoredProcedure) in this Object. objectName:" + TNinth_TableInfo.TableName);
                }
            }
            if (typeof(TTenth).Name != "object")
            {
                TableInfoAttribute TTenth_TableInfo = typeof(TTenth).GetTableInfo();
                if (TTenth_TableInfo.TableType == enmTableType.Writeonly)
                {
                    throw new Exception("Object is Writeonly.You can not perform (Execute_StoredProcedure) in this Object. objectName:" + TTenth_TableInfo.TableName);
                }
            }

            if (_ConnectionString == "" || _ConnectionString == null)
            {
                throw new Exception("Connection string is empty.");
            }
            var dbArgs = new DynamicParameters();
            if (Parameters != null)
            {
                foreach (var pair in Parameters) dbArgs.Add(pair.Key, pair.Value);
            }

            List<object> Result = new List<object>();
            GridReader grid;
            using (SqlConnection connection = new SqlConnection(_ConnectionString))
            {
                connection.Open();
                grid = connection.QueryMultiple(ProcedureName, dbArgs, commandType: CommandType.StoredProcedure);
                Result.Add(grid.Read<TFirst>().ToList<TFirst>());
                if (grid.IsConsumed == false) { Result.Add(grid.Read<TSecond>().ToList()); } else { Result.Add(new List<TSecond>()); }
                if (grid.IsConsumed == false) { Result.Add(grid.Read<TThird>().ToList()); } else { Result.Add(new List<TThird>()); }
                if (grid.IsConsumed == false) { Result.Add(grid.Read<TFourth>().ToList()); } else { Result.Add(new List<TFourth>()); }
                if (grid.IsConsumed == false) { Result.Add(grid.Read<TFifth>().ToList()); } else { Result.Add(new List<TFifth>()); }
                if (grid.IsConsumed == false) { Result.Add(grid.Read<TSixth>().ToList()); } else { Result.Add(new List<TSixth>()); }
                if (grid.IsConsumed == false) { Result.Add(grid.Read<TSeventh>().ToList()); } else { Result.Add(new List<TSeventh>()); }
                if (grid.IsConsumed == false) { Result.Add(grid.Read<TEighth>().ToList()); } else { Result.Add(new List<TEighth>()); }
                if (grid.IsConsumed == false) { Result.Add(grid.Read<TNinth>().ToList()); } else { Result.Add(new List<TNinth>()); }
                if (grid.IsConsumed == false) { Result.Add(grid.Read<TTenth>().ToList()); } else { Result.Add(new List<TTenth>()); }
                connection.Close();

            }
            return Result;

        }

        public List<T> Find<T>(string Filter, string orderBy, bool withLock, Dictionary<string, string> Parameters, string FieldNames = "")
        {
            if (_ConnectionString == "" || _ConnectionString == null)
            {
                throw new Exception("Connection string is empty.");
            }
            var oGeneralFactory = GetGeneralFactory<T>();
            return oGeneralFactory.Find(Filter,orderBy,withLock,  Parameters, FieldNames );
        }

        public T FindFirst<T>(string Filter, string orderBy, bool withLock, Dictionary<string, string> Parameters, string FieldNames = "")
        {
            if (_ConnectionString == "" || _ConnectionString == null)
            {
                throw new Exception("Connection string is empty.");
            }
            var oGeneralFactory = GetGeneralFactory<T>();
            return oGeneralFactory.FindFirst(Filter, orderBy, withLock, Parameters, FieldNames);
        }
    }
}
