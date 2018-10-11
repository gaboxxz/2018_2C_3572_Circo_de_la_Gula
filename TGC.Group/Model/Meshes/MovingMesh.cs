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
        private const float movingRange = 300f;

        private TGCVector3 director;
        private float moved;

        public MovingMesh(TgcMesh mesh)
        {
            if (mesh.Name.Contains("X"))
                director.X = 1;
            if (mesh.Name.Contains("Y"))
                director.Y = 1;
            if (mesh.Name.Contains("Z"))
                director.Z = 1;
        }

        public void ExecuteCollision(TgcMesh item, TgcMesh bandicoot, Core.Camara.TgcCamera camara, Core.Mathematica.TGCVector3 movimiento)
        {
            var anguloCamara = bandicoot.Position;
            bandicoot.Move(-movimiento);

            //bandicoot.Transform = TGC.Core.Mathematica.TGCMatrix.Translation(-movimiento);

            //Bandicoot.Move(0, direccionSalto * MOVEMENT_SPEED * ElapsedTime, 0);
            camara.SetCamera((camara.Position - movimiento), anguloCamara);
        }

        public void ExecuteCollision(TgcMesh item)
        {
        }

        public void ExecuteJumpCollision(TgcMesh MeshColisionado, TgcMesh bandicoot, Core.Camara.TgcCamera camara, Core.Mathematica.TGCVector3 movimiento, float realTimeMovement)
        {
            //bandicoot.Move(movimiento);
            var anguloCamara = bandicoot.Position;
            this.Move(bandicoot, realTimeMovement*2);
            camara.SetCamera((camara.Position - movimiento), anguloCamara);
        }

        public void Move(TgcMesh mesh, float movimiento)
        {
            moved++;
            if (moved <= movingRange)
            
                director = new TGCVector3(0, 0, -movimiento);
                
            else
                director = new TGCVector3(0, 0, movimiento);
            //mesh.Transform = TGCMatrix.Translation(director*100);
            //mesh.Move(director);




            var orbitaDeRotacion = 2.5f;


            //Muevo las plataformas
            var Mover = TGCMatrix.Translation(0, 0, -10);
            var Mover2 = TGCMatrix.Translation(0, 0, 65);

            //Punto por donde va a rotar
            var Trasladar = TGCMatrix.Translation(0, 0, 10);
            var Trasladar2 = TGCMatrix.Translation(0, 0, -10);

            //Aplico la rotacion
            var Rot = TGCMatrix.RotationX(orbitaDeRotacion);

            //Giro para que la caja quede derecha
            var RotInversa = TGCMatrix.RotationX(-orbitaDeRotacion);

            var transformacionBox = Mover * Trasladar * Rot * Trasladar * RotInversa;
            //transformacionBox2 = Mover2 * Trasladar2 * RotInversa * Trasladar2 * Rot;

            mesh.Transform = transformacionBox;
            mesh.UpdateMeshTransform();

            //plataforma1.Update(transformacionBox);


            //plataforma2.Update(transformacionBox2);










            if (moved == movingRange * 2) moved = 0; 
        }
    }
}
