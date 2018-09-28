using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Input;

namespace TGC.Group.Model.Utils.Commands
{
    public interface IModelAccessors
    {
        /* I declare the methods and properties of the model that will be 
           accessible through this interface. */
        bool HasBoundingBox { get; set; }
        TgcD3dInput Input { get; set; }
    }
}
