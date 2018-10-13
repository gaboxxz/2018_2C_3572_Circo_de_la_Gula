using BulletSharp;
using BulletSharp.Math;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using TGC.Core.BulletPhysics;
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
        private TGCVector3 direccion = new TGCVector3(0, 1, 0);
        private int Frecuencia = 0;
        private int sentido = 1;

        // Data of the VertexBuffer triangles (with usingHeightmap = true)
        private CustomVertex.PositionTextured[] triangleDataVB;

        private RigidBody ball;
        public RigidBody bandicootRigidBody;
        private RigidBody staticPlatform;
        private RigidBody dynamicPlatform;

        private List<TGCBox> stairsMesh = new List<TGCBox>();
        private TGCBox staticPlatformMesh;
        private TGCBox dynamicPlatformMesh;
        private TGCSphere sphereMesh;
        private TGCVector3 director;

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
            var centerOfMass = new TGCVector3(-50f, 30, -200f);
            TGCVector3 size = TGCVector3.Empty;

            #region Stone Sphere
            ball = BulletRigidBodyConstructor.CreateBall(radius, mass, centerOfMass);
            ball.SetDamping(0.1f, 0.5f);
            ball.Restitution = 1f;
            ball.Friction = 1;
            dynamicsWorld.AddRigidBody(ball);

            var ballTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, $"{MediaDir}\\Textures\\rockwall.jpg");
            sphereMesh = new TGCSphere(radius, ballTexture, TGCVector3.Empty);
            sphereMesh.BoundingSphere.setValues(centerOfMass, radius);
            sphereMesh.updateValues();

            director = new TGCVector3(1, 0, 0);
            #endregion

            #region BandicootRigidBody
            //Cuerpo rigido de una caja basica
            var position = new TGCVector3(0, 1, 0);
            mass = 1.5f;
            bandicootRigidBody = BulletRigidBodyConstructor.CreateCapsule(10, 5, position, mass, false);

            //Valores que podemos modificar a partir del RigidBody base
            bandicootRigidBody.SetDamping(0.1f, 0f);
            bandicootRigidBody.Restitution = 0f;
            bandicootRigidBody.Friction = 0.5f;
            bandicootRigidBody.InvInertiaDiagLocal = TGCVector3.Empty.ToBsVector;
            //Agregamos el RidigBody al World
            dynamicsWorld.AddRigidBody(bandicootRigidBody);
            #endregion

            #region Stairs
            mass = 0;
            size = new TGCVector3(50, 20, 30);
            var platformTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, $"{MediaDir}\\textures\\rockwall.jpg");
            float angle = 0.75f * FastMath.PI;

            for (float i = 0, x = size.Z, y = size.Y, z = -size.X; i < 10; i++)
            {
                staticPlatform = BulletRigidBodyConstructor.CreateBox(size, mass, centerOfMass, 0, 0, 0, 0.7f);
                staticPlatform.CenterOfMassTransform = TGCMatrix.RotationY(angle).ToBsMatrix * TGCMatrix.Translation(x, y, z).ToBsMatrix;
                dynamicsWorld.AddRigidBody(staticPlatform);

                staticPlatformMesh = TGCBox.fromSize(2 * size, platformTexture);
                staticPlatformMesh.Transform = new TGCMatrix(staticPlatform.InterpolationWorldTransform);
                stairsMesh.Add(staticPlatformMesh);

                x += 35;
                y += 40;
                z -= 25;
                angle -= 0.1f;
            }
            #endregion

            #region Dynamic Platform

            position = new TGCVector3(0, 0, 0);
            mass = 1f;
            size = new TGCVector3(70, 10, 30);
            dynamicPlatform = BulletRigidBodyConstructor.CreateBox(size, mass, position, 0, 0, 0, 2f);
            dynamicPlatform.CenterOfMassTransform = TGCMatrix.Translation(-300, 60, -200).ToBsMatrix;
            dynamicPlatform.AngularFactor = (new Vector3(0, 0, 0));
            dynamicsWorld.AddRigidBody(dynamicPlatform);

            // mesh para visualizar plataforma

            var platformtexture = TgcTexture.createTexture(D3DDevice.Instance.Device, $"{MediaDir}\\textures\\rockwall.jpg");
            dynamicPlatformMesh = TGCBox.fromSize(2 * size, platformtexture);
            dynamicPlatformMesh.Transform = new TGCMatrix(dynamicPlatform.InterpolationWorldTransform);
            dynamicPlatformMesh.AutoTransformEnable = false;
            dynamicPlatformMesh.updateValues();

            #endregion
        }

        public void Update(TgcD3dInput input)
        {
            dynamicsWorld.StepSimulation(1 / 60f, 100);
            var strength = 10f;
            var angle = 0;

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

            if (Frecuencia < 360)
            {
                dynamicPlatform.ActivationState = ActivationState.ActiveTag;
                Frecuencia++;
                if (sentido == 1)
                {
                    dynamicPlatform.LinearVelocity = new Vector3(5, 10, 0);
                }
                else
                {
                    dynamicPlatform.LinearVelocity = new Vector3(-5, -10, 0);
                }
            }
            else
            {
                Frecuencia = 0;
                if (sentido == 1)
                    sentido = 0;
                else
                    sentido = 1;
            }
            dynamicPlatformMesh.Transform =
                new TGCMatrix(dynamicPlatform.InterpolationWorldTransform);
        }

        public void Render()
        {
            sphereMesh.Transform = TGCMatrix.Scaling(30, 30, 30)
                * new TGCMatrix(ball.InterpolationWorldTransform);
            sphereMesh.Render();
            dynamicPlatformMesh.Render();

            foreach (TGCBox stair in stairsMesh)
            {
                stair.Render();
            }
        }

        public void Dispose()
        {
            foreach (TGCBox stair in stairsMesh)
            {
                stair.Dispose();
            }

            sphereMesh.Dispose();
            dynamicsWorld.Dispose();
            dispatcher.Dispose();
            collisionConfiguration.Dispose();
            constraintSolver.Dispose();
            overlappingPairCache.Dispose();
        }
    }
}
