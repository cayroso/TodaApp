using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.CQRS
{
    public interface IContainer
    {
        T Resolve<T>();
    }
}
