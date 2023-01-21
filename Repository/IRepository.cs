using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.ExceptionServices;
using Dapper;

namespace Repository
{
    public interface IRepository
    {
        int  TransactionCount { get; }
        bool BeginTransaction();
        bool CommitTransaction();
        List<T> Execute_StoredProcedure<T>(string ProcedureName, Dictionary<string, string> Parameters);
        List<object> Execute_StoredProcedure<TFirst, TSecond>(string ProcedureName, Dictionary<string, string> Parameters);
        List<object> Execute_StoredProcedure<TFirst, TSecond, TThird>(string ProcedureName, Dictionary<string, string> Parameters);
        List<object> Execute_StoredProcedure<TFirst, TSecond, TThird, TFour>(string ProcedureName, Dictionary<string, string> Parameters);
        List<object> Execute_StoredProcedure<TFirst, TSecond, TThird, TFour, TFive>(string ProcedureName, Dictionary<string, string> Parameters);
        List<object> Execute_StoredProcedure<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(string ProcedureName, Dictionary<string, string> Parameters);
        List<object> Execute_StoredProcedure<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(string ProcedureName, Dictionary<string, string> Parameters);
        List<object> Execute_StoredProcedure<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TEighth>(string ProcedureName, Dictionary<string, string> Parameters);
        List<object> Execute_StoredProcedure<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TEighth, TNinth>(string ProcedureName, Dictionary<string, string> Parameters);
        List<object> Execute_StoredProcedure<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TEighth, TNinth, TTenth>(string ProcedureName, Dictionary<string, string> Parameters);
        bool RollbackTransaction();
        bool Save<T>(T o);
        /// <summary>
        /// Saving the object on the selected records.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o">object</param>
        /// <param name="Filter">Used to specify the selected records.</param>
        /// <param name="Parameters">Collection of parameters used in Filter.</param>
        /// <returns></returns>
        bool Save<T>(T o, string Filter, Dictionary<string, string> Parameters = null);
        /// <summary>
        /// list of objects is received and based on the second parameter(basePropertyName)
        /// , if it exists in the database, it is updated
        /// , otherwise it is inserted.
        /// </summary>
        /// <typeparam name="T">domain model</typeparam>
        /// <param name="list"> list of objects for update or insert.</param>
        /// <param name="basePropertyName">propertyName of domain model, based on that, a decision is made to create or update the desired record in the database</param>
        /// <returns></returns>
        bool SaveList<T>(List<T> list,string basePropertyNames);
        bool Delete<T>(T o);
        bool DeleteList<T>(string Filter, Dictionary<string, string> Parameters);
        T GetByID<T>(string ID,bool withLock);
        List<T> Find<T>(string Filter, string orderBy, bool withLock, Dictionary<string, string> Parameters, string FieldNames = "");
        T FindFirst<T>(string Filter, string orderBy, bool withLock, Dictionary<string, string> Parameters,string FieldNames="");
        Func<string, string> __KeyGenerator { get; set; }
    }
}