using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Meshes
{
    class MeshFijo : TipoMesh
    {
        public void EjecutarColision(TgcMesh item, TgcMesh bandicoot, Core.Camara.TgcCamera camara, Core.Mathematica.TGCVector3 movimiento)
        {
            var anguloCamara = bandicoot.Position;
            bandicoot.Move(-movimiento);
            camara.SetCamera((camara.Position - movimiento), anguloCamara);
        }

        public void EjecutarColision(TgcMesh item)
        {
            //throw new NotImplementedException();
        }
    }
}
