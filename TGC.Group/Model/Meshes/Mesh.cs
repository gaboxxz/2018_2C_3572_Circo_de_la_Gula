using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Meshes
{
    class Mesh
    {

        public TgcMesh Malla { get; set; }
        public TipoMesh tipo { get; set; }
        //protected TipoMesh Mesh

        public Mesh(TgcMesh malla)
        {
            this.Malla = malla;
            if (malla.Name.Equals("fruta3"))
                tipo = new MeshFruta();
            else
                tipo = new MeshFijo();

        }

        public Boolean EsFruta()
        {
            return Malla.Name.Equals("fruta3");
        }

        public void EjecutarColision(TgcMesh bandicoot, Core.Camara.TgcCamera camara, Core.Mathematica.TGCVector3 movimiento)
        {
            tipo.EjecutarColision(Malla, bandicoot, camara, movimiento);
        }

        public void EjecutarColision()
        {

        }


        public void RenderMesh()
        {
           // if(Malla.Enabled)
                Malla.Render();
        }

        public void RenderBoundingBox()
        {
           if (Malla.BoundingBox != null)
                Malla.BoundingBox.Render();
        }

    }
}
