using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model.Utils.Commands
{
    public class GenericClass<T> where T : IModelAccessors
    {
        public T DynamicObject { get; set; }

        public GenericClass(T obj)
        {
            DynamicObject = obj;
        }
    }
}
