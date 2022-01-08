using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
    internal interface IKeyGenerator
    {
         string GetNextID();
    }
}
