using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
    public static class RepositoryFactory
    {

        public static IRepository CreateRepository(string ConnectionString) {
            Repository _db = new Repository(ConnectionString);
            return _db;
        }
        public static IRepository CreateRepository(System.Data.IDbConnection dbConnection)
        {
            Repository _db = new Repository(dbConnection);
            return _db;
        }
    }
    public  class RepositoryFactory2
    {
        public IRepository CreateRepository(string ConnectionString)
        {
            Repository _db = new Repository(ConnectionString);
            return _db;
        }
        public IRepository CreateRepository(System.Data.IDbConnection dbConnection)
        {
            Repository _db = new Repository(dbConnection);
            return _db;
        }
    }
}
