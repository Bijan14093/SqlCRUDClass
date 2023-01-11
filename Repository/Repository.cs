using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Repository.Domain;

namespace Repository
{
    internal class Repository : IRepository
    {
        Dictionary<string, object> TableFactories = new Dictionary<string, object>();
        internal IDbConnection Connection;
        internal IDbTransaction Transaction;
        private string _ConnectionString;
        private int _TransactionCount;


        public int TransactionCount 
        {
            get { return _TransactionCount; }
        }

        public Func<string, string> __KeyGenerator { get ; set ; }

        public Repository(string connectionString)
        {
            if (connectionString == "" || connectionString == null)
            {
                throw new Exception("Connection string is empty.");
            }
            _ConnectionString = connectionString;
        }

        public Repository(IDbConnection dbConnection)
        {
            if (dbConnection == null)
            {
                throw new Exception("Connection is null.");
            }
            this.Connection = dbConnection;
        }

        internal void OpenConnection()
        {
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
                if (Connection == null)
                {
                    throw new Exception("Connection is null.");
                }

                if (Connection.State == ConnectionState.Open)
                {
                    Connection.Close();
                }


        }
        public bool BeginTransaction()
        {
            Monitor.Enter(this);
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
            if (_TransactionCount == 0)
            {
                Transaction.Commit();
                Transaction = null;
                CloseConnection();
            }
            Monitor.Exit(this);
            return true;


        }

        public bool RollbackTransaction()
        {
            try
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
            finally
            {
                try
                {
                    Monitor.Exit(this);
                }
                catch (Exception)
                {

                    //throw;
                }
            }
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
            lock (this)
            {
                var oGeneralFactory = GetGeneralFactory<T>();
                return oGeneralFactory.Save(ref o,"");
            }


        }
        public bool Delete<T>(T o)
        {
            lock (this)
            {
                var oGeneralFactory = GetGeneralFactory<T>();
                return oGeneralFactory.Delete(o);

            }
        }

        public T GetByID<T>(string ID, bool withLock)
        {
            lock (this)
            {
                var oGeneralFactory = GetGeneralFactory<T>();
                return oGeneralFactory.GetByID(ID, withLock);

            }
        }

        public List<T> GetAll<T>()
        {
            lock (this)
            {
                var oGeneralFactory = GetGeneralFactory<T>();
                return oGeneralFactory.GetAll();

            }
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
            lock (this)
            {
                TableInfoAttribute TFirst_TableInfo = GetGeneralFactory<TFirst>().TableInfo;
                if (TFirst_TableInfo.TableType == enmTableType.Writeonly)
                {
                    throw new Exception("Class is Writeonly.You can not perform (Execute_StoredProcedure) in this Class. ClassName:" + typeof(TFirst).Name);
                }

                TableInfoAttribute TSecond_TableInfo = GetGeneralFactory<TSecond>().TableInfo;
                if (TSecond_TableInfo.TableType == enmTableType.Writeonly)
                {
                    throw new Exception("Class is Writeonly.You can not perform (Execute_StoredProcedure) in this Class. ClassName:" + typeof(TSecond).Name);
                }

                TableInfoAttribute TThird_TableInfo = GetGeneralFactory<TThird>().TableInfo;
                if (TThird_TableInfo.TableType == enmTableType.Writeonly)
                {
                    throw new Exception("Class is Writeonly.You can not perform (Execute_StoredProcedure) in this Class. ClassName:" + typeof(TThird).Name);
                }

                TableInfoAttribute TFourth_TableInfo = GetGeneralFactory<TFourth>().TableInfo;
                if (TFourth_TableInfo.TableType == enmTableType.Writeonly)
                {
                    throw new Exception("Class is Writeonly.You can not perform (Execute_StoredProcedure) in this Class. ClassName:" + typeof(TFourth).Name);
                }
                TableInfoAttribute TFifth_TableInfo = GetGeneralFactory<TFifth>().TableInfo;
                if (TFifth_TableInfo.TableType == enmTableType.Writeonly)
                {
                    throw new Exception("Class is Writeonly.You can not perform (Execute_StoredProcedure) in this Class. ClassName:" + typeof(TFifth).Name);
                }
                TableInfoAttribute TSixth_TableInfo = GetGeneralFactory<TSixth>().TableInfo;
                if (TSixth_TableInfo.TableType == enmTableType.Writeonly)
                {
                    throw new Exception("Class is Writeonly.You can not perform (Execute_StoredProcedure) in this Class. ClassName:" + typeof(TSixth).Name);
                }
                TableInfoAttribute TSeventh_TableInfo = GetGeneralFactory<TSeventh>().TableInfo;
                if (TSeventh_TableInfo.TableType == enmTableType.Writeonly)
                {
                    throw new Exception("Class is Writeonly.You can not perform (Execute_StoredProcedure) in this Class. ClassName:" + typeof(TSeventh).Name);
                }
                TableInfoAttribute TEighth_TableInfo = GetGeneralFactory<TEighth>().TableInfo;
                if (TEighth_TableInfo.TableType == enmTableType.Writeonly)
                {
                    throw new Exception("Class is Writeonly.You can not perform (Execute_StoredProcedure) in this Class. ClassName:" + typeof(TEighth).Name);
                }
                TableInfoAttribute TNinth_TableInfo = GetGeneralFactory<TNinth>().TableInfo;
                if (TNinth_TableInfo.TableType == enmTableType.Writeonly)
                {
                    throw new Exception("Class is Writeonly.You can not perform (Execute_StoredProcedure) in this Class. ClassName:" + typeof(TNinth).Name);
                }
                TableInfoAttribute TTenth_TableInfo = GetGeneralFactory<TTenth>().TableInfo;
                if (TTenth_TableInfo.TableType == enmTableType.Writeonly)
                {
                    throw new Exception("Class is Writeonly.You can not perform (Execute_StoredProcedure) in this Class. ClassName:" + typeof(TTenth).Name);
                }

                var dbArgs = new DynamicParameters();
                if (Parameters != null)
                {
                    foreach (var pair in Parameters) dbArgs.Add(pair.Key, pair.Value);
                }

                List<object> Result = new List<object>();
                SqlMapper.GridReader grid;
                    bool InTransaction;
                    InTransaction = false;
                    if (Transaction != null)
                    {
                        InTransaction = true;
                    }
                    else
                    {
                        OpenConnection();
                        InTransaction = false;
                    }
                    grid = this.Connection.QueryMultiple(ProcedureName, dbArgs, commandType: CommandType.StoredProcedure, commandTimeout: this.Connection.ConnectionTimeout,transaction: this.Transaction);
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
                if (InTransaction == false)
                {
                    CloseConnection();
                }
                return Result;

            }

        }

        public List<T> Find<T>(string Filter, string orderBy, bool withLock, Dictionary<string, string> Parameters, string FieldNames = "")
        {
            lock (this)
            {
                var oGeneralFactory = GetGeneralFactory<T>();
                return oGeneralFactory.Find(Filter, orderBy, withLock, Parameters, FieldNames);

            }
        }

        public T FindFirst<T>(string Filter, string orderBy, bool withLock, Dictionary<string, string> Parameters, string FieldNames = "")
        {
            lock (this)
            {
                var oGeneralFactory = GetGeneralFactory<T>();
                return oGeneralFactory.FindFirst(Filter, orderBy, withLock, Parameters, FieldNames);

            }
        }

        public bool DeleteList<T>(string Filter, Dictionary<string, string> Parameters)
        {
            lock (this)
            {
                var oGeneralFactory = GetGeneralFactory<T>();
                return oGeneralFactory.DeleteList(Filter, Parameters);

            }

        }

        public bool Save<T>(T o, string Filter)
        {
            lock (this)
            {
                var oGeneralFactory = GetGeneralFactory<T>();
                return oGeneralFactory.Save(ref o, Filter);
            }

        }

        public bool SaveList<T>(List<T> list, string basePropertyName)
        {
            lock (this)
            {
                var oGeneralFactory = GetGeneralFactory<T>();
                oGeneralFactory.SaveList(list, basePropertyName);
                return true;

            }
        }
    }

}
