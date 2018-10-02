using BulletSharp;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using System;
using TGC.Core.BulletPhysics;
using TGC.Core.Collision;
using TGC.Core.Direct3D;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.Textures;

namespace TGC.Group.Model.Utils
{
    public class Physics
    {
        // Physics dynamic world configuration objects
        private DiscreteDynamicsWorld dynamicsWorld;
        private CollisionDispatcher dispatcher;
        private DefaultCollisionConfiguration collisionConfiguration;
        private SequentialImpulseConstraintSolver constraintSolver;
        private BroadphaseInterface overlappingPairCache;

        // Data of the VertexBuffer triangles (with usingHeightmap = true)
        private CustomVertex.PositionTextured[] triangleDataVB;

        private RigidBody ball;
        private TGCSphere sphereMesh;
        private TGCBox platformMesh;
        private TGCBox platformMesh2;
        private TGCVector3 director;
        public RigidBody bandicootRigidBody;
        private RigidBody staticPlatform;
        private RigidBody movingPlatform;
        public bool UsingHeightmap { get; set; }

        public void SetTriangleDataVB(CustomVertex.PositionTextured[] data)
        {
            triangleDataVB = data;
        }

        public void Init(String MediaDir)
        {
            #region world configuration
            // Create a physics world using default config
            collisionConfiguration = new DefaultCollisionConfiguration();
            dispatcher = new CollisionDispatcher(collisionConfiguration);
            GImpactCollisionAlgorithm.RegisterAlgorithm(dispatcher);
            constraintSolver = new SequentialImpulseConstraintSolver();
            overlappingPairCache = new DbvtBroadphase();
            dynamicsWorld = new DiscreteDynamicsWorld(dispatcher, overlappingPairCache, constraintSolver, collisionConfiguration);

            var gravity = new TGCVector3(0, -60f, 0).ToBsVector;
            dynamicsWorld.Gravity = gravity;

            if (UsingHeightmap)
            {
                var heightMap = BulletRigidBodyConstructor.CreateSurfaceFromHeighMap(triangleDataVB);
                heightMap.Restitution = 0;
                dynamicsWorld.AddRigidBody(heightMap);
            }

            #endregion

            float radius = 30f;
            float mass = 0.75f;
            var position = new TGCVector3(-50f, 30, -200f);
            TGCVector3 size = TGCVector3.Empty;

            #region Stone Sphere
            ball = BulletRigidBodyConstructor.CreateBall(radius, mass, position);
            ball.SetDamping(0.1f, 0.5f);
            ball.Restitution = 1f;
            ball.Friction = 1;
            dynamicsWorld.AddRigidBody(ball);

            var ballTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, $"{MediaDir}\\Textures\\rockwall.jpg");
            sphereMesh = new TGCSphere(radius, ballTexture, TGCVector3.Empty);
            sphereMesh.BoundingSphere.setValues(position, radius);
            sphereMesh.updateValues();

            director = new TGCVector3(1, 0, 0);
            #endregion

            #region BandicootRigidBody
            //Cuerpo rigido de una caja basica
            position = new TGCVector3(0, 1, 0);
            mass = 1.5f;
            bandicootRigidBody = BulletRigidBodyConstructor.CreateCapsule(10, 5, position, mass, false);

            //Valores que podemos modificar a partir del RigidBody base
            bandicootRigidBody.SetDamping(0.1f, 0f);
            bandicootRigidBody.Restitution = 0f;
            bandicootRigidBody.Friction = 0.6f;
            bandicootRigidBody.InvInertiaDiagLocal = TGCVector3.Empty.ToBsVector;
            //Agregamos el RidigBody al World
            dynamicsWorld.AddRigidBody(bandicootRigidBody);
            #endregion

            #region static platform
            position = new TGCVector3(-250, 5, 200);
            mass = 0;
            size = new TGCVector3(70, 30, 30);
            staticPlatform = BulletRigidBodyConstructor.CreateBox(size, mass, position, 0, 0, 0, 0.7f);
            dynamicsWorld.AddRigidBody(staticPlatform);

            //mesh para visualizar plataforma
            var platformTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, $"{MediaDir}\\Textures\\rockwall.jpg");
            size = size * 2;
            platformMesh = TGCBox.fromSize(size, platformTexture);
            platformMesh.Transform = new TGCMatrix(staticPlatform.InterpolationWorldTransform);
            platformMesh.updateValues();
            #endregion

            //#region mobile platform
            //position = new TGCVector3(-250, 5, 200);
            //mass = 0;
            //size = new TGCVector3(70, 30, 30);
            //movingPlatform = BulletRigidBodyConstructor.CreateBox(size, mass, position, 0, 0, 0, 0.7f);
            //dynamicsWorld.AddRigidBody(movingPlatform);

            ////mesh para visualizar plataforma
            ////var platformTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, $"{MediaDir}\\Textures\\rockwall.jpg");
            //platformMesh2 = TGCBox.fromSize(size, platformTexture);
            //platformMesh2.Transform = new TGCMatrix(staticPlatform.InterpolationWorldTransform);
            //platformMesh2.updateValues();
            //#endregion
        }

        public void Update(TgcD3dInput input)
        {
            dynamicsWorld.StepSimulation(1 / 60f, 100);
            var strength = 10f;
            var angle = 0;
            /*
                        if (input.keyDown(Key.W))
                        {
                            //moving = true;
                            //Activa el comportamiento de la simulacion fisica para la capsula
                            bandicootRigidBody.ActivationState = ActivationState.ActiveTag;
                            //bandicootRigidBody.AngularVelocity = TGCVector3.Empty.ToBsVector;
                            bandicootRigidBody.ApplyCentralImpulse(-strength * new TGCVector3(0,0,1.3f).ToBsVector);
                        }
            */
            if (input.keyDown(Key.I))
            {
                ball.ActivationState = ActivationState.ActiveTag;
                ball.ApplyCentralImpulse(-strength * director.ToBsVector);
            }

            if (input.keyDown(Key.K))
            {
                ball.ActivationState = ActivationState.ActiveTag;
                ball.ApplyCentralImpulse(strength * director.ToBsVector);
            }

            if (input.keyDown(Key.J))
            {
                director.TransformCoordinate(TGCMatrix.RotationY(-angle * 0.001f));
            }

            if (input.keyDown(Key.L))
            {
                director.TransformCoordinate(TGCMatrix.RotationY(angle * 0.001f));
            }

            if (input.keyPressed(Key.G))
            {
                ball.ActivationState = ActivationState.ActiveTag;
                ball.ApplyCentralImpulse(TGCVector3.Up.ToBsVector * 150);
            }
        }

        public void Render()
        {
            sphereMesh.Transform = TGCMatrix.Scaling(30, 30, 30)
                * new TGCMatrix(ball.InterpolationWorldTransform);
            sphereMesh.Render();
            platformMesh.Render();

        }

        public void Dispose()
        {
            sphereMesh.Dispose();
            dynamicsWorld.Dispose();
            dispatcher.Dispose();
            collisionConfiguration.Dispose();
            constraintSolver.Dispose();
            overlappingPairCache.Dispose();
        }
    }
}
