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
    }
}
