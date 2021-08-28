using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repository;

namespace TestWebAPI
{
    public interface IDatabase
    {
        public IRepository TestWebAPI { get;  }
        public IRepository TestWebAPI_Log { get;  }
    }
}
