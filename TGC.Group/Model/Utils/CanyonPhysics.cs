using BulletSharp;
using BulletSharp.Math;
using TGC.Core.BulletPhysics;
using TGC.Core.Direct3D;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.Textures;

namespace TGC.Group.Model.Utils
{
    public class CanyonPhysics : IPhysics
    {
        #region Attributes
        private IGameModel ctx;

        // Physics dynamic world configuration objects
        private DiscreteDynamicsWorld dynamicsWorld;
        private CollisionDispatcher dispatcher;
        private DefaultCollisionConfiguration collisionConfiguration;
        private SequentialImpulseConstraintSolver constraintSolver;
        private BroadphaseInterface overlappingPairCache;

        private int frecuency = 0;
        private int direction = 1;
        private float distance = -290;
        #endregion

        public RigidBody BandicootRigidBody { get; set; }
        public RigidBody Rock1Body { get; set; }
        public RigidBody Rock2Body { get; set; }
        public RigidBody Rock3Body { get; set; }
        public RigidBody DynamicPlatform1Body { get; set; }

        public TGCBox Rock1Mesh { get; set; }
        public TGCBox Rock2Mesh { get; set; }
        public TGCBox Rock3Mesh { get; set; }
        public TGCBox DynamicPlatform1Mesh { get; set; }

        public void Init(IGameModel ctx)
        {
            #region World configuration
            this.ctx = ctx;
            // Create a physics world using default config
            collisionConfiguration = new DefaultCollisionConfiguration();
            dispatcher = new CollisionDispatcher(collisionConfiguration);
            GImpactCollisionAlgorithm.RegisterAlgorithm(dispatcher);
            constraintSolver = new SequentialImpulseConstraintSolver();
            overlappingPairCache = new DbvtBroadphase();
            dynamicsWorld = new DiscreteDynamicsWorld(dispatcher, overlappingPairCache, constraintSolver, collisionConfiguration)
            {
                Gravity = new TGCVector3(0, -50f, 0).ToBsVector
            };

            var heightMap = BulletRigidBodyConstructor.CreateSurfaceFromHeighMap(ctx.Terrain.getData());
            heightMap.Restitution = 0;
            dynamicsWorld.AddRigidBody(heightMap);
            #endregion

            #region BandicootRigidBody
            var position = new TGCVector3(0, 1, 0);
            var mass = 1.5f;
            var radius = 10;
            var height = 5;

            BandicootRigidBody = BulletRigidBodyConstructor.CreateCapsule(radius, height, position, mass, false);
            BandicootRigidBody.SetDamping(0.1f, 0);
            BandicootRigidBody.Restitution = 0f;
            BandicootRigidBody.Friction = 0.15f;
            BandicootRigidBody.InvInertiaDiagLocal = TGCVector3.Empty.ToBsVector;
            BandicootRigidBody.CenterOfMassTransform = TGCMatrix.Translation(-900, 1000, 900).ToBsMatrix;
            dynamicsWorld.AddRigidBody(BandicootRigidBody);
            #endregion

            #region Rocks
            var size = new TGCVector3(200, 150, 200);
            mass = 0;
            var centerOfMass = TGCVector3.Empty;
            var friction = 0.7f;
            var translation = TGCMatrix.Translation(-900, size.Y, 900);

            // Rock1 Body
            Rock1Body = BulletRigidBodyConstructor.CreateBox(size, mass, centerOfMass, 0, 0, 0, friction);
            Rock1Body.CenterOfMassTransform = translation.ToBsMatrix;
            dynamicsWorld.AddRigidBody(Rock1Body);

            // Rock1 Mesh
            var canyonTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, $"{ctx.MediaDir}\\Textures\\canyon-horizontal.png");
            Rock1Mesh = TGCBox.fromSize(centerOfMass, 2 * size, canyonTexture);
            Rock1Mesh.Transform = new TGCMatrix(Rock1Body.InterpolationWorldTransform);

            // Rock2 Body
            size = new TGCVector3(100, 150, 100);
            Rock2Body = BulletRigidBodyConstructor.CreateBox(size, mass, centerOfMass, 0, 0, 0, friction);
            translation *= TGCMatrix.Translation(400, 1, 1);
            Rock2Body.CenterOfMassTransform = translation.ToBsMatrix;
            dynamicsWorld.AddRigidBody(Rock2Body);

            // Rock2 Mesh
            Rock2Mesh = TGCBox.fromSize(centerOfMass, 2 * size, canyonTexture);
            Rock2Mesh.Transform = new TGCMatrix(Rock2Body.InterpolationWorldTransform);

            // Rock3 Body
            Rock3Body = BulletRigidBodyConstructor.CreateBox(size, mass, centerOfMass, 0, 0, 0, friction);
            translation *= TGCMatrix.Translation(800, 1, 1);
            Rock3Body.CenterOfMassTransform = translation.ToBsMatrix;
            dynamicsWorld.AddRigidBody(Rock3Body);

            // Rock3 Mesh
            Rock3Mesh = TGCBox.fromSize(centerOfMass, 2 * size, canyonTexture);
            Rock3Mesh.Transform = new TGCMatrix(Rock3Body.InterpolationWorldTransform);
            #endregion

            #region Dynamic Platforms
            mass = 1f;
            size = new TGCVector3(70, 10, 30);

            // DynamicPlatform1 Body
            DynamicPlatform1Body = BulletRigidBodyConstructor.CreateBox(size, mass, centerOfMass, 0, 0, 0, 1f);
            DynamicPlatform1Body.AngularFactor = new Vector3(0, 0, 0);
            DynamicPlatform1Body.CenterOfMassTransform =
                 TGCMatrix.RotationY(FastMath.PI_HALF).ToBsMatrix
                 * TGCMatrix.Translation(distance, 300, 900).ToBsMatrix;
            dynamicsWorld.AddRigidBody(DynamicPlatform1Body);

            // DynamicPlatform1 Mesh
            var platformtexture = TgcTexture.createTexture(D3DDevice.Instance.Device, $"{ctx.MediaDir}\\textures\\rockwall.jpg");
            DynamicPlatform1Mesh = TGCBox.fromSize(2 * size, platformtexture);
            DynamicPlatform1Mesh.Transform = new TGCMatrix(DynamicPlatform1Body.InterpolationWorldTransform);
            #endregion
        }

        public void Update()
        {
            dynamicsWorld.StepSimulation(1 / 60f, 100);

            BandicootRigidBody.ActivationState = ActivationState.ActiveTag;
            BandicootRigidBody.ApplyCentralImpulse(ctx.BandicootMovement.ToBsVector * 1.5f);

            var posCamara = new TGCVector3(BandicootRigidBody.CenterOfMassPosition);
            ctx.BandicootCamera.Target = posCamara;

            DynamicPlatform1Body.Gravity = new Vector3(0, 0, 0);
            if (frecuency < 3000)
            {
                DynamicPlatform1Body.ActivationState = ActivationState.ActiveTag;
                frecuency++;
                if (direction == 1)
                {
                    DynamicPlatform1Body.LinearVelocity = new Vector3(10, 0, 0);
                }
                else
                {
                    DynamicPlatform1Body.LinearVelocity = new Vector3(-10, 0, 0);
                }
            }
            else
            {
                frecuency = 0;

                if (direction == 1)
                    direction = 0;
                else
                    direction = 1;
            }

            DynamicPlatform1Mesh.Transform = new TGCMatrix(DynamicPlatform1Body.InterpolationWorldTransform);
        }

        public void Render()
        {
            Rock1Mesh.Render();
            Rock2Mesh.Render();
            Rock3Mesh.Render();
            DynamicPlatform1Mesh.Render();
        }

        public void Dispose()
        {
            #region Dispose Rigid bodies
            Rock1Body.Dispose();
            Rock2Body.Dispose();
            Rock3Body.Dispose();
            BandicootRigidBody.Dispose();
            DynamicPlatform1Body.Dispose();
            #endregion

            #region Dispose Meshes
            Rock1Mesh.Dispose();
            Rock2Mesh.Dispose();
            Rock3Mesh.Dispose();
            DynamicPlatform1Mesh.Dispose();
            #endregion

            #region Dispose Physics world
            dynamicsWorld.Dispose();
            dispatcher.Dispose();
            collisionConfiguration.Dispose();
            constraintSolver.Dispose();
            overlappingPairCache.Dispose();
            #endregion 
        }
    }
}