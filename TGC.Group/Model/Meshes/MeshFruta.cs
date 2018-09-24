﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Meshes
{
    class MeshFruta : TipoMesh
    {
        /*public MeshFruta(TgcMesh malla) : base(malla)
        {

        }
        */
        /* public MeshFruta(TgcMesh mesh)
         {
             //this.Malla = mesh;
         }
 */
        public void EjecutarColision(TgcMesh Malla)
        {
            Malla.Enabled = false;
        }

        public void EjecutarColision(TgcMesh Malla, TgcMesh bandicoot, Core.Camara.TgcCamera camara, Core.Mathematica.TGCVector3 movimiento)
        {
            Malla.Enabled = false;
            Malla.BoundingBox = null;
            Console.WriteLine("prueba colision fruta");
        }

        /*public new void RenderMesh()
        {
            
        }*/
    }
}