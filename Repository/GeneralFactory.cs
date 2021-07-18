using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dapper;
using Repository.Domain;

namespace Repository
{
    internal class GeneralFactory<T>
    {
        private IKeyGenerator _KeyGeneratory;
        private string _tableFullName;
        private string _keycolumnname;
        private bool _keyIsIdentity;
        private Dictionary<string, string> _StaticValues;
        private Dictionary<string, string> _columnNames;
        private string _SelectStatment;
        private string _InsertStatment;
        private string _DeleteStatment;
        private Repository _Repository;
        public GeneralFactory(Repository repository)
        {
            Dictionary<string,string> temp = new Dictionary<string, string>();
            System.Attribute[] tableAttribute = System.Attribute.GetCustomAttributes(typeof(T));
            _tableFullName = "";
            _keycolumnname = "";
            foreach (System.Attribute attr in tableAttribute)
            {
                if (attr is TableInfoAttribute)
                {
                    TableInfoAttribute tableInfo = (TableInfoAttribute)attr;
                    _tableFullName = tableInfo.TableShchema + tableInfo.TableName;
                    _keycolumnname = tableInfo.keyColumnName;
                    _keyIsIdentity = tableInfo.KeyIsIdentity;

                }
            }
            if (_tableFullName == "")
            {
                _tableFullName = typeof(T).Name.ToString();
            }
            if (_keycolumnname == "")
            {
                _keycolumnname = "ID";
            }
            if (_tableFullName == "" || _keycolumnname == "")
            {
                throw new Exception("You must be set ClassAttribue");
            }
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var item in properties)
            {
                string FieldName = item.Name;
                bool IsIgnore = false;
                foreach (System.Attribute attr in item.GetCustomAttributes(false))
                {
                    if (attr is FieldInfoAttribute)
                    {
                        FieldInfoAttribute FieldInfo = (FieldInfoAttribute)attr;
                        IsIgnore = FieldInfo.Ignore;
                        FieldName = FieldInfo.FieldName;

                    }
                }
                if (IsIgnore == false)
                {
                    if (FieldName == "") { FieldName = item.Name; }
                    temp.Add(item.Name,FieldName);
                }


            }

            _columnNames = temp;
            _StaticValues = null;
            _Repository = repository;
            _KeyGeneratory = new KeyGenerator(_tableFullName,_Repository);

        }

        private string InsertStatment
        {
            get
            {
                if (string.IsNullOrEmpty(_InsertStatment))
                {
                    _InsertStatment = "INSERT INTO " + _tableFullName + Environment.NewLine;
                    _InsertStatment = _InsertStatment + "(" + Get_ColumnName(CommandType._Insert, "") + ")" + Environment.NewLine;
                    _InsertStatment = _InsertStatment + "VALUES(" + Get_ColumnName(CommandType._Insert, "@") + ")" + Environment.NewLine;
                }

                return _InsertStatment;
            }
        }

        private string UpdateStatment(T o)
        {
            string _UpdateStatment = null;
            string Columnname;
            string FieldName;
            string Result;
            Result = "";
            foreach (var currentColumnname in _columnNames)
            {
                Columnname = currentColumnname.Value;
                FieldName = currentColumnname.Key;
                if (_keyIsIdentity & (Columnname ?? "") == (_keycolumnname ?? ""))
                {
                }
                // Do nothing
                else if (_StaticValues is object && _StaticValues.ContainsKey(Columnname) && !string.IsNullOrEmpty(_StaticValues[Columnname]))
                {
                    Result = Result + Columnname + " = " + _StaticValues[Columnname] + ",";
                }
                else
                {
                    var FieldValue = GetPropertyValueByName(o, FieldName);
                    if (FieldValue!=null)
                    {
                        Result = Result + Columnname + " = @" + FieldName + ",";
                    }

                }
            }

            Result = Result.Substring(0, (Result.Length - 1));
            _UpdateStatment = "Update " + _tableFullName +  Environment.NewLine;
            _UpdateStatment = _UpdateStatment +"SET " + Result + "" + Environment.NewLine;
            _UpdateStatment = _UpdateStatment + "Where " + _keycolumnname + "=@" + _keycolumnname ;


            return _UpdateStatment;
        }

        private string DeleteStatment
        {
            get
            {
                if (string.IsNullOrEmpty(_DeleteStatment))
                {
                    _DeleteStatment = "DELETE FROM " + _tableFullName + " Where " + _keycolumnname + "=@" + _keycolumnname + "";
                }

                return _DeleteStatment;
            }
        }

        private string get_SelectStatment(string NumberofRecords, bool withLock, string FieldNames = "")
        {
            _SelectStatment = "Select " + Get_ColumnName(CommandType._Select, "", FieldNames) + Environment.NewLine;
            if (withLock)
            {
                _SelectStatment = _SelectStatment + "From " + _tableFullName + " WITH(XLOCK) " + Environment.NewLine;
            }
            else
            {
                _SelectStatment = _SelectStatment + "From " + _tableFullName +  Environment.NewLine;
            }
            return _SelectStatment;
        }
        public bool Save(ref T o)
        {
            bool InTransaction;
            InTransaction = false;
            if (_Repository.Transaction != null)
            {
                InTransaction = true;
            }
            else
            {
                _Repository.OpenConnection();
                InTransaction = false;
            }
            var objectID = GetPropertyValueByName(o, _keycolumnname);
            if (objectID==null)
            {
                objectID = "0";
            }
            if (objectID.ToString()=="")
            {
                objectID = "0";
            }
            if (objectID.ToString() == "0")
            {
                Int64 ID;
                if (_keyIsIdentity)
                {
                    ID = _Repository.Connection.Query<Int64>(InsertStatment + Environment.NewLine + "SELECT SCOPE_IDENTITY() IdentityValue", o, transaction: _Repository.Transaction).FirstOrDefault();
                    SetPropertyValueByName(o, _keycolumnname, ID);
                }
                else
                {
                    ID = _KeyGeneratory.GetNextID();
                    SetPropertyValueByName(o, _keycolumnname, ID);
                    _Repository.Connection.Query<Int64>(InsertStatment + Environment.NewLine, o, transaction: _Repository.Transaction).FirstOrDefault();
                }
            }
            else
            {
                _Repository.Connection.Execute(UpdateStatment(o), o, transaction: _Repository.Transaction);
            }

            if (InTransaction == false)
            {
                _Repository.CloseConnection();
            }
            return true;
        }
        public T GetByID(string ID, bool withLock)
        {
            var Parameters = new DynamicParameters();
            Parameters.Add(_keycolumnname, ID);
            bool InTransaction;
            InTransaction = false;
            if (_Repository.Transaction != null)
            {
                InTransaction = true;
            }
            else
            {
                _Repository.OpenConnection();
                InTransaction = false;
            }
            var result= _Repository.Connection.Query<T>(get_SelectStatment("",withLock) + " Where " + _keycolumnname + "= @" + _keycolumnname + "", Parameters, transaction: _Repository.Transaction).FirstOrDefault();
            if (InTransaction == false)
            {
                _Repository.CloseConnection();
            }
            return result;

        }

        public bool Delete(T o)
        {
            bool InTransaction;
            InTransaction = false;
            if (_Repository.Transaction != null)
            {
                InTransaction = true;
            }
            else
            {
                _Repository.OpenConnection();
                InTransaction = false;
            }
            _Repository.Connection.Execute(DeleteStatment, o, transaction: _Repository.Transaction);
            if (InTransaction == false)
            {
                _Repository.CloseConnection();
            }
            return true;
        }

        internal List<T> GetAll()
        {
            bool InTransaction;
            InTransaction = false;
            if (_Repository.Transaction != null)
            {
                InTransaction = true;
            }
            else
            {
                _Repository.OpenConnection();
                InTransaction = false;
            }
            var result = _Repository.Connection.Query<T>(get_SelectStatment("", false), null, transaction: _Repository.Transaction).ToList();
            if (InTransaction == false)
            {
                _Repository.CloseConnection();
            }
            return result;
        }

        public List<T> Find(string Filter, string orderBy,bool withLock, Dictionary<string, string> Parameters, string FieldNames = "")
        {
            var dbArgs = new DynamicParameters();
            if (Parameters != null)
            {
                foreach (var pair in Parameters) dbArgs.Add(pair.Key, pair.Value);
            }
            bool InTransaction;
            InTransaction = false;
            if (_Repository.Transaction != null)
            {
                InTransaction = true;
            }
            else
            {
                _Repository.OpenConnection();
                InTransaction = false;
            }

            var oResult = new List<T>();
            string sql;
            sql = get_SelectStatment("",withLock, FieldNames);
            if (!string.IsNullOrEmpty(Filter))
            {
                sql = sql + "Where " + Filter + Environment.NewLine;
            }

            if (!string.IsNullOrEmpty(orderBy))
            {
                sql = sql + "Order by " + orderBy;
            }
            var result = _Repository.Connection.Query<T>(sql, param: dbArgs, transaction: _Repository.Transaction).ToList();
            if (InTransaction == false)
            {
                _Repository.CloseConnection();
            }
            return result;
        }

        public T FindFirst(string Filter, string orderBy, bool withLock, Dictionary<string, string> Parameters, string FieldNames = "")
        {
            var dbArgs = new DynamicParameters();
            if (Parameters != null)
            {
                foreach (var pair in Parameters) dbArgs.Add(pair.Key, pair.Value);
            }
            bool InTransaction;
            InTransaction = false;
            if (_Repository.Transaction != null)
            {
                InTransaction = true;
            }
            else
            {
                _Repository.OpenConnection();
                InTransaction = false;
            }
            string sql;
            sql = get_SelectStatment("1", withLock, FieldNames);
            if (!string.IsNullOrEmpty(Filter))
            {
                sql = sql + "Where " + Filter + Environment.NewLine;
            }

            if (!string.IsNullOrEmpty(orderBy))
            {
                sql = sql + "Order by " + orderBy;
            }

            var result=_Repository.Connection.Query<T>(sql,param: dbArgs, transaction: _Repository.Transaction).FirstOrDefault();
            if (InTransaction == false)
            {
                _Repository.CloseConnection();
            }
            return result;
        }

        private string Get_ColumnName(CommandType oCommandType, string Prefix, string FieldNames = "")
        {
            string Columnname;
            string FieldName;
            string Result;
            Result = "";
            Columnname = "";
            List<string> FilterFieldNames=null;
            if (FieldNames!="")
            {
                FilterFieldNames=FieldNames.Split(',').ToList<string>();
            }
            foreach (var currentColumnname in _columnNames)
            {

                Columnname = currentColumnname.Value;
                FieldName = currentColumnname.Key;
                if (FilterFieldNames != null )
                {
                    if (FilterFieldNames.Contains(FieldName) ==false)
                    {
                        continue;
                    }
                }
                switch (oCommandType)
                {
                    case CommandType._Insert:
                        {
                            if (_keyIsIdentity & (Columnname ?? "") == (_keycolumnname ?? ""))
                            {
                            }
                            // Do nothing
                            else if (!string.IsNullOrEmpty(Prefix) && _StaticValues is object && _StaticValues.ContainsKey(Columnname) && !string.IsNullOrEmpty(_StaticValues[Columnname]))
                            {
                                Result = Result + _StaticValues[Columnname] + ",";
                            }
                            else
                            {
                                if (Prefix=="")
                                {
                                    Result = Result + Prefix + Columnname + ",";
                                }
                                else
                                {
                                    Result = Result + Prefix + FieldName + ",";
                                }
                                
                            }

                            break;
                        }

                    case CommandType._update:
                        {
                            if (_keyIsIdentity & (Columnname ?? "") == (_keycolumnname ?? ""))
                            {
                            }
                            // Do nothing
                            else if (_StaticValues is object && _StaticValues.ContainsKey(Columnname) && !string.IsNullOrEmpty(_StaticValues[Columnname]))
                            {
                                Result = Result + Columnname + " = " + _StaticValues[Columnname] + ",";
                            }
                            else
                            {
                                Result = Result + Columnname + " = " + Prefix + FieldName + ",";
                            }

                            break;
                        }

                    case CommandType._Select:
                        {
                            Result = Result + Columnname + " " + FieldName + ",";
                            break;
                        }
                }
            }

            Result = Result.Substring(0, (Result.Length - 1));
            return Result;
        }

        private enum CommandType
        {
            _Select,
            _Insert,
            _update,
            _Delete
        }

        public static bool SetPropertyValueByName(object obj, string name, Int64 value)
        {
            var prop = obj.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            if (prop is null || !prop.CanWrite)
            {
                return false;
            }
            if (prop.PropertyType.Name.ToUpper()=="STRING")
            {
                prop.SetValue(obj, value.ToString(), null);
            }
            else if (prop.PropertyType.Name.ToUpper() == "INT16")
            {
                prop.SetValue(obj, value.ConvertTo<Int16>(), null);
            }
            else if (prop.PropertyType.Name.ToUpper() == "INT32")
            {
                prop.SetValue(obj, value.ConvertTo<Int32>(), null);
            }
            else if (prop.PropertyType.Name.ToUpper() == "INT64")
            {
                prop.SetValue(obj, value.ConvertTo<Int64>(), null);
            }
            else
            {
                prop.SetValue(obj, value, null);
            }
                
            return true;
        }

        public static object GetPropertyValueByName(object obj, string name)
        {
            var prop = obj.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
            if (prop is null || !prop.CanRead)
            {
                return null;
            }

            return prop.GetValue(obj, null);
        }
    }

    
}