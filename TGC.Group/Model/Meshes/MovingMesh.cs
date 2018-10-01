using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Example;

namespace TGC.Group.Model.Meshes
{
    class MovingMesh : TipoMesh
    {

        private const float movingRange = 100f;

        private TGCVector3 director;
        private float moved;

        public void ExecuteCollision(TgcMesh item, TgcMesh bandicoot, Core.Camara.TgcCamera camara, Core.Mathematica.TGCVector3 movimiento)
        {
            var anguloCamara = bandicoot.Position;
            bandicoot.Move(-movimiento);

            //Bandicoot.Move(0, direccionSalto * MOVEMENT_SPEED * ElapsedTime, 0);
            camara.SetCamera((camara.Position - movimiento), anguloCamara);
            Console.WriteLine("colision movingMesh");
            
        }

        public void ExecuteCollision(TgcMesh item)
        {
            //throw new NotImplementedException();
            
        }

        public void ExecuteJumpCollision(TgcMesh MeshColisionado, TgcMesh bandicoot, Core.Camara.TgcCamera camara, Core.Mathematica.TGCVector3 movimiento)
        {
            bandicoot.Move(director);
            var anguloCamara = bandicoot.Position;
            this.Move(bandicoot, 100 * 0.1f);
            camara.SetCamera((camara.Position - movimiento), anguloCamara);
            Console.WriteLine("colision movingMesh");  
        }

        public void Move(TgcMesh mesh, float movimiento)
        {
            moved++;
            if (moved <= movingRange)
            
                director = new TGCVector3(0, 0, -movimiento);
                
            else
                director = new TGCVector3(0, 0, movimiento);


            //mesh.Transform = TGCMatrix.Translation(director);
            mesh.Move(director);
            if (moved == movingRange * 2) moved = 0; 

        }
    }
}
