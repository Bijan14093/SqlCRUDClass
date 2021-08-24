using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Domain
{
    public enum enmTableType
    {
        None ,Readonly , Writeonly 
    }
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
    public class TableInfoAttribute : System.Attribute
    {
        public string TableName;
        public string keyColumnName;
        public bool KeyIsIdentity;
        public enmTableType TableType;
        public TableInfoAttribute(string TableName, string keyColumnName, bool KeyIsIdentity, enmTableType TableType = enmTableType.None)
        {
            this.TableName = TableName;
            this.keyColumnName = keyColumnName;
            this.KeyIsIdentity = KeyIsIdentity;
            this.TableType = TableType;
        }


    }
    [System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Property, AllowMultiple = true)]
    public class FieldInfoAttribute : System.Attribute
    {
        public string FieldName;
        public bool Ignore;
        public FieldInfoAttribute()
        {
            this.FieldName = "";
            this.Ignore = false;
        }
        public FieldInfoAttribute(string FieldName, bool IsIgnore)
        {
            this.FieldName = FieldName;
            this.Ignore = IsIgnore;
        }

    }
}
