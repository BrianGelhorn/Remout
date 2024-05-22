using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remout.StartupTools
{
    class AbstractFactory<T> : IAbstractFactory<T>
    {
        public T Create()
        {
            throw new NotImplementedException();
        }
    }
}
