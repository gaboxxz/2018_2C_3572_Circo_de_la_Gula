using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Utils;

namespace TGC.Group.Model.Meshes
{
    class MeshFijo : TipoMesh
    {
        public void ExecuteCollision(TgcMesh item, TgcMesh bandicoot, Core.Camara.TgcCamera camara, Core.Mathematica.TGCVector3 movimiento)
        {
            var anguloCamara = bandicoot.Position;
            
            bandicoot.Move(-movimiento);
            bandicoot.Transform = TGC.Core.Mathematica.TGCMatrix.Translation(-movimiento);
            
            //Bandicoot.Move(0, direccionSalto * MOVEMENT_SPEED * ElapsedTime, 0);
           camara.SetCamera((camara.Position - movimiento), anguloCamara);
        }

        public void ExecuteCollision(TgcMesh item)
        {
            //throw new NotImplementedException();
        }

        public void ExecuteJumpCollision(TgcMesh MeshColisionado, TgcMesh bandicoot, Core.Camara.TgcCamera camara, Core.Mathematica.TGCVector3 movimiento, float realTimeMovement)
        {

        }

        public void Move(TgcMesh item,float movement)
        {

        }
    }
}
