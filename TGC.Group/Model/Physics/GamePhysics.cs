using System;
using BulletSharp;
using Microsoft.DirectX.Direct3D;
using TGC.Core.BulletPhysics;
using TGC.Core.Direct3D;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.Textures;

namespace TGC.Group.Model.Physics
{
    class GamePhysics
    {
        //Configuracion de la Simulacion Fisica
        private DiscreteDynamicsWorld dynamicsWorld;

        private CollisionDispatcher dispatcher;
        private DefaultCollisionConfiguration collisionConfiguration;
        private SequentialImpulseConstraintSolver constraintSolver;
        private BroadphaseInterface overlappingPairCache;

        //Datos del los triangulos del VertexBuffer
        private CustomVertex.PositionTextured[] triangleDataVB;

        private RigidBody bandicootBodyRigid;

        private TGCBox boxMesh;
        private TGCVector3 director;

        public void SetTriangleDataVB(CustomVertex.PositionTextured[] data)
        {
            triangleDataVB = data;
        }

        public void Init(String MediaDir)
        {
            //Creamos el mundo fisico por defecto.
            collisionConfiguration = new DefaultCollisionConfiguration();
            dispatcher = new CollisionDispatcher(collisionConfiguration);
            GImpactCollisionAlgorithm.RegisterAlgorithm(dispatcher);
            constraintSolver = new SequentialImpulseConstraintSolver();
            overlappingPairCache = new DbvtBroadphase(); //AxisSweep3(new BsVector3(-5000f, -5000f, -5000f), new BsVector3(5000f, 5000f, 5000f), 8192);
            dynamicsWorld = new DiscreteDynamicsWorld(dispatcher, overlappingPairCache, constraintSolver, collisionConfiguration);
            dynamicsWorld.Gravity = new TGCVector3(0, -100f, 0).ToBsVector;


            //Creamos el terreno
            var meshRigidBody = BulletRigidBodyConstructor.CreateSurfaceFromHeighMap(triangleDataVB);
            dynamicsWorld.AddRigidBody(meshRigidBody);

            //Creamos la esfera del dragon
            bandicootBodyRigid = BulletRigidBodyConstructor.CreateBall(30f, 0.75f, new TGCVector3(100f, 500f, 100f));
            bandicootBodyRigid.SetDamping(0.1f, 0.5f);
            bandicootBodyRigid.Restitution = 1f;
            bandicootBodyRigid.Friction = 1;
            dynamicsWorld.AddRigidBody(bandicootBodyRigid);
            var textureBandicoot = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + @"Texturas\dragonball.jpg");
            //boxMesh = new TGCBox(1, textureBandicoot, TGCVector3.Empty);
            boxMesh.updateValues();
            director = new TGCVector3(1, 0, 0);
        }

    }
}
