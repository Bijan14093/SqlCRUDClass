using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Dapper;

namespace Repository
{
    internal class KeyGenerator:IKeyGenerator
    {
        private string _Procedure_KeyGenerator_Check;
        private List<KeyValue> Keys;
        private Int64 _FirstID;
        private Int64 _LastID;
        private string _tableName;
        private Repository _Repository;
        internal KeyGenerator(string TableName, Repository repository)
        {
            _tableName = TableName;
            _FirstID = 1;
            _LastID = 0;
            _Repository = repository;
            _Procedure_KeyGenerator_Check = "";
        }

        public Int64 GetNextID()
        {
            Int64 Result;
            if (_FirstID > _LastID)
            {
                ResetIDs();
            }

            Result = _FirstID;
            _FirstID = _FirstID + 1;
            return Result;
        }

        private void ResetIDs()
        {
            var Parameters = new DynamicParameters();
            Parameters.Add("TableName", _tableName);
            if(Keys==null || Keys.Count == 0) 
            {
                if (_Procedure_KeyGenerator_Check == "")
                {
                    CheckAndCreate_Procedure();
                    _Procedure_KeyGenerator_Check = "Checked";
                }
                Keys =_Repository.Connection.Query<KeyValue>("__KeyGenerator", Parameters, commandType: CommandType.StoredProcedure, transaction: _Repository.Transaction).ToList();
            }
            var first = Keys.First();
            _FirstID = first.FirstID;
            _LastID = first.LastID;
            Keys.Remove(first);
        }

        private void CheckAndCreate_Procedure()
        {
            string SqlCmd="";
            SqlCmd = SqlCmd + "IF  Not EXISTS(SELECT * FROM sys.procedures WHERE Name='__KeyGenerator')" + Environment.NewLine;
            SqlCmd = SqlCmd + "EXEC('" + Environment.NewLine;
            SqlCmd = SqlCmd + "CREATE PROCEDURE __KeyGenerator @TableName VARCHAR(max) " + Environment.NewLine;
            SqlCmd = SqlCmd + "-- Please overwrite your own key generation policy : this is sample: be carefule. " + Environment.NewLine;
            SqlCmd = SqlCmd + "AS " + Environment.NewLine;
            SqlCmd = SqlCmd + "	DECLARE @PrimeryKey AS NVARCHAR(max)" + Environment.NewLine;
            SqlCmd = SqlCmd + "	select @PrimeryKey = C.COLUMN_NAME " + Environment.NewLine;
            SqlCmd = SqlCmd + "	FROM  INFORMATION_SCHEMA.TABLE_CONSTRAINTS T  " + Environment.NewLine;
            SqlCmd = SqlCmd + "		JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE C  	ON C.CONSTRAINT_NAME=T.CONSTRAINT_NAME  " + Environment.NewLine;
            SqlCmd = SqlCmd + "	WHERE  C.TABLE_NAME=@TableName " + Environment.NewLine;
            SqlCmd = SqlCmd + "		AND T.CONSTRAINT_TYPE=''PRIMARY KEY''" + Environment.NewLine;
            SqlCmd = SqlCmd + "	IF @PrimeryKey IS NULL" + Environment.NewLine;
            SqlCmd = SqlCmd + "		SET @PrimeryKey = ''ID''" + Environment.NewLine;
            SqlCmd = SqlCmd + "	EXEC(''SELECT MAX('' + @PrimeryKey  + '') + 1 FirstID, MAX('' + @PrimeryKey  + '') + 1 LastID FROM '' + @TableName)" + Environment.NewLine;
            SqlCmd = SqlCmd + "')" + Environment.NewLine;
            _Repository.Connection.Execute(SqlCmd, null, commandType: CommandType.Text, transaction: _Repository.Transaction);
        }

        private class KeyValue
        {
            public Int64 FirstID { get; set; }
            public Int64 LastID { get; set; }
        }
    }
}
