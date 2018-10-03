using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
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
            //Malla.AutoTransformEnable = false;

            if (EsFruta())
                tipo = new MeshFruta();
            else if (Malla.Name.Contains("movingPlatform"))
                tipo = new MovingMesh(malla);
            else
                tipo = new MeshFijo();

        }

        public Boolean EsFruta()
        {
            return Malla.Name.Equals("fruta3");
        }

        public void ExecuteCollision(TgcMesh bandicoot, Core.Camara.TgcCamera camara, Core.Mathematica.TGCVector3 movimiento)
        {
            tipo.ExecuteCollision(Malla, bandicoot, camara, movimiento);
        }

        public void ExecuteCollision()
        {

        }

        public void ExecuteJumpCollision(TgcMesh bandicoot, Core.Camara.TgcCamera camara, Core.Mathematica.TGCVector3 movimiento, float realTimeMovement)
        {

            tipo.ExecuteJumpCollision(Malla, bandicoot, camara, movimiento, realTimeMovement);
        }

        public Boolean isUpperCollision(TgcMesh Bandicoot, float posBaseBandicoot)
        {
            /*
             * var posicion = new TGCVector3(Bandicoot.BoundingBox.PMin.X, posBaseBandicoot - 0.1f, Bandicoot.BoundingBox.PMin.Z);
            if (((posicion.X > Malla.BoundingBox.PMin.X && posicion.X < Malla.BoundingBox.PMax.X) &&
               posicion.Z > Malla.BoundingBox.PMin.Z && posicion.Z < Malla.BoundingBox.PMax.Z))
            {
                posicion = new TGCVector3(Bandicoot.BoundingBox.PMax.X, posBaseBandicoot - 0.1f, Bandicoot.BoundingBox.PMin.Z);
                if (((posicion.X > Malla.BoundingBox.PMin.X && posicion.X < Malla.BoundingBox.PMax.X) &&
                    posicion.Z > Malla.BoundingBox.PMin.Z && posicion.Z < Malla.BoundingBox.PMax.Z))
                {
                    posicion = new TGCVector3(Bandicoot.BoundingBox.PMin.X, posBaseBandicoot - 0.1f, Bandicoot.BoundingBox.PMax.Z);
                    if (((posicion.X > Malla.BoundingBox.PMin.X && posicion.X < Malla.BoundingBox.PMax.X) &&
                    posicion.Z > Malla.BoundingBox.PMin.Z && posicion.Z < Malla.BoundingBox.PMax.Z))
                    {
                        posicion = new TGCVector3(Bandicoot.BoundingBox.PMax.X, posBaseBandicoot - 0.1f, Bandicoot.BoundingBox.PMax.Z);

                        return (((posicion.X > Malla.BoundingBox.PMin.X && posicion.X < Malla.BoundingBox.PMax.X) &&
                        posicion.Z > Malla.BoundingBox.PMin.Z && posicion.Z < Malla.BoundingBox.PMax.Z));
                        
                    }
                }
            }
            return false;
            */

            /*
             * var posicion = new TGCVector3(Bandicoot.BoundingBox.PMin.X, posBaseBandicoot - 0.1f, Bandicoot.BoundingBox.PMin.Z);
            if (!((posicion.X > Malla.BoundingBox.PMin.X && posicion.X < Malla.BoundingBox.PMax.X) &&
               posicion.Z > Malla.BoundingBox.PMin.Z && posicion.Z < Malla.BoundingBox.PMax.Z)) return false;

            posicion = new TGCVector3(Bandicoot.BoundingBox.PMax.X, posBaseBandicoot - 0.1f, Bandicoot.BoundingBox.PMin.Z);
            if (!((posicion.X > Malla.BoundingBox.PMin.X && posicion.X < Malla.BoundingBox.PMax.X) &&
                    posicion.Z > Malla.BoundingBox.PMin.Z && posicion.Z < Malla.BoundingBox.PMax.Z)) return false;

            posicion = new TGCVector3(Bandicoot.BoundingBox.PMin.X, posBaseBandicoot - 0.1f, Bandicoot.BoundingBox.PMax.Z);
            if (!((posicion.X > Malla.BoundingBox.PMin.X && posicion.X < Malla.BoundingBox.PMax.X) &&
            posicion.Z > Malla.BoundingBox.PMin.Z && posicion.Z < Malla.BoundingBox.PMax.Z)) return false;

            posicion = new TGCVector3(Bandicoot.BoundingBox.PMax.X, posBaseBandicoot - 0.1f, Bandicoot.BoundingBox.PMax.Z);

            if (!((posicion.X > Malla.BoundingBox.PMin.X && posicion.X < Malla.BoundingBox.PMax.X) &&
                   posicion.Z > Malla.BoundingBox.PMin.Z && posicion.Z < Malla.BoundingBox.PMax.Z)) return false;


            return true;
            */

            var posicion1 = new TGCVector3(Bandicoot.BoundingBox.PMin.X, posBaseBandicoot - 0.1f, Bandicoot.BoundingBox.PMin.Z);
            var posicion2 = new TGCVector3(Bandicoot.BoundingBox.PMax.X, posBaseBandicoot - 0.1f, Bandicoot.BoundingBox.PMin.Z);
            var posicion3 = new TGCVector3(Bandicoot.BoundingBox.PMin.X, posBaseBandicoot - 0.1f, Bandicoot.BoundingBox.PMax.Z);
            var posicion4 = new TGCVector3(Bandicoot.BoundingBox.PMax.X, posBaseBandicoot - 0.1f, Bandicoot.BoundingBox.PMax.Z);

            //var posicion = new TGCVector3(Bandicoot.BoundingBox.PMin.X, posBaseBandicoot - 0.1f, Bandicoot.BoundingBox.PMin.Z);
            if ( ((posicion1.X > Malla.BoundingBox.PMin.X && posicion1.X < Malla.BoundingBox.PMax.X) &&
               (posicion1.Z > Malla.BoundingBox.PMin.Z && posicion1.Z < Malla.BoundingBox.PMax.Z)) ||   /*punto 1*/
                 ((posicion2.X > Malla.BoundingBox.PMin.X && posicion2.X < Malla.BoundingBox.PMax.X) &&
                 (posicion2.Z > Malla.BoundingBox.PMin.Z && posicion2.Z < Malla.BoundingBox.PMax.Z)) ||  /*punto 2*/
                    ((posicion3.X > Malla.BoundingBox.PMin.X && posicion3.X < Malla.BoundingBox.PMax.X) &&
                    (posicion3.Z > Malla.BoundingBox.PMin.Z && posicion3.Z < Malla.BoundingBox.PMax.Z)) ||   /*punto 3*/
                         ((posicion4.X > Malla.BoundingBox.PMin.X && posicion4.X < Malla.BoundingBox.PMax.X) &&
                        (posicion4.Z > Malla.BoundingBox.PMin.Z && posicion4.Z < Malla.BoundingBox.PMax.Z)))  /*punto 4*/
                return true;

            return false;
        }

        public void Move(float movimiento)
        {
            tipo.Move(this.Malla, movimiento);
        }

        public void RenderMesh()
        {
            // if(Malla.Enabled)
            Malla.Render();
        }

        public void RenderBoundingBox()
        {
            Malla.BoundingBox.Render();
        }
    }
}
