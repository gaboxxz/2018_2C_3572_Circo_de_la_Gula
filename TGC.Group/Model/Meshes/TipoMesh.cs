using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Meshes
{
    interface TipoMesh
    {
        void EjecutarColision(TgcMesh item, TgcMesh bandicoot, Core.Camara.TgcCamera camara, Core.Mathematica.TGCVector3 movimiento);
        void EjecutarColision(TgcMesh item);


    }
}
