using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Core.Example;

namespace TGC.Group.Model.Meshes
{
    interface TipoMesh
    {
        void ExecuteJumpCollision(TgcMesh item, TgcMesh bandicoot, Core.Camara.TgcCamera camara, Core.Mathematica.TGCVector3 movimiento, float realTimeMovement);

        void ExecuteCollision(TgcMesh item, TgcMesh bandicoot, Core.Camara.TgcCamera camara, Core.Mathematica.TGCVector3 movimiento);
        void ExecuteCollision(TgcMesh item);
        void Move(TgcMesh item, float movimiento);

        



    }
}
