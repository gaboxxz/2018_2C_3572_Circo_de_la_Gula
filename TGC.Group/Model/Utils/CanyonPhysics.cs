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
        private float risingImpulse = 0.35f;
        #endregion

        #region Properties
        public RigidBody BandicootRigidBody { get; set; }
        public RigidBody Rock1Body { get; set; }
        public RigidBody Rock2Body { get; set; }
        public RigidBody Rock3Body { get; set; }
        public RigidBody Rock4Body { get; set; }
        public RigidBody Rock5Body { get; set; }
        public RigidBody DynamicPlatform1Body { get; set; }
        public RigidBody DynamicPlatform2Body { get; set; }
        public RigidBody DynamicPlatform3Body { get; set; }

        public TGCBox Rock1Mesh { get; set; }
        public TGCBox Rock2Mesh { get; set; }
        public TGCBox Rock3Mesh { get; set; }
        public TGCBox Rock4Mesh { get; set; }
        public TGCBox Rock5Mesh { get; set; }
        public TGCBox DynamicPlatform1Mesh { get; set; }
        public TGCBox DynamicPlatform2Mesh { get; set; }
        public TGCBox DynamicPlatform3Mesh { get; set; }
        #endregion

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
                Gravity = new TGCVector3(0, -15f, 0).ToBsVector
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
            var rockFriction = 1.5f;
            var translation = TGCMatrix.Translation(-900, size.Y, 900);

            // Rock1 Body
            Rock1Body = BulletRigidBodyConstructor.CreateBox(size, mass, centerOfMass, 0, 0, 0, rockFriction);
            Rock1Body.CenterOfMassTransform = translation.ToBsMatrix;
            dynamicsWorld.AddRigidBody(Rock1Body);

            // Rock1 Mesh
            var canyonTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, $"{ctx.MediaDir}\\Textures\\canyon-horizontal.png");
            Rock1Mesh = TGCBox.fromSize(centerOfMass, 2 * size, canyonTexture);
            Rock1Mesh.Transform = new TGCMatrix(Rock1Body.InterpolationWorldTransform);

            // Rock2 Body
            size = new TGCVector3(100, 150, 100);
            Rock2Body = BulletRigidBodyConstructor.CreateBox(size, mass, centerOfMass, 0, 0, 0, rockFriction);
            translation *= TGCMatrix.Translation(400, 1, 1);
            Rock2Body.CenterOfMassTransform = translation.ToBsMatrix;
            dynamicsWorld.AddRigidBody(Rock2Body);

            // Rock2 Mesh
            Rock2Mesh = TGCBox.fromSize(centerOfMass, 2 * size, canyonTexture);
            Rock2Mesh.Transform = new TGCMatrix(Rock2Body.InterpolationWorldTransform);

            // Rock3 Body
            Rock3Body = BulletRigidBodyConstructor.CreateBox(size, mass, centerOfMass, 0, 0, 0, rockFriction);
            translation *= TGCMatrix.Translation(800, 1, 1);
            Rock3Body.CenterOfMassTransform = translation.ToBsMatrix;
            dynamicsWorld.AddRigidBody(Rock3Body);

            // Rock3 Mesh
            Rock3Mesh = TGCBox.fromSize(centerOfMass, 2 * size, canyonTexture);
            Rock3Mesh.Transform = new TGCMatrix(Rock3Body.InterpolationWorldTransform);


            // Rock4 Body
            size = new TGCVector3(100, 200, 100);
            Rock4Body = BulletRigidBodyConstructor.CreateBox(size, mass, centerOfMass, 0, 0, 0, rockFriction);
            translation *= TGCMatrix.Translation(0, 1, -700);
            Rock4Body.CenterOfMassTransform = translation.ToBsMatrix;
            dynamicsWorld.AddRigidBody(Rock4Body);

            // Rock4 Mesh
            Rock4Mesh = TGCBox.fromSize(centerOfMass, 2 * size, canyonTexture);
            Rock4Mesh.Transform = new TGCMatrix(Rock4Body.InterpolationWorldTransform);


            // Rock5 Body
            Rock5Body = BulletRigidBodyConstructor.CreateBox(size, mass, centerOfMass, 0, 0, 0, rockFriction);
            translation *= TGCMatrix.Translation(1, 1, -900);
            Rock5Body.CenterOfMassTransform = translation.ToBsMatrix;
            dynamicsWorld.AddRigidBody(Rock5Body);


            // Rock5 Mesh
            Rock5Mesh = TGCBox.fromSize(centerOfMass, 2 * size, canyonTexture);
            Rock5Mesh.Transform = new TGCMatrix(Rock5Body.InterpolationWorldTransform);
            #endregion

            #region Dynamic Platforms
            mass = 1f;
            size = new TGCVector3(70, 10, 40);
            var platformFriction = 2.5f;

            // DynamicPlatform1 Body
            DynamicPlatform1Body = BulletRigidBodyConstructor.CreateBox(size, mass, centerOfMass, 0, 0, 0, platformFriction);
            DynamicPlatform1Body.AngularFactor = new Vector3(0, 0, 0);
            DynamicPlatform1Body.CenterOfMassTransform =
                 TGCMatrix.RotationY(FastMath.PI_HALF).ToBsMatrix
                 * TGCMatrix.Translation(distance, 300, 900).ToBsMatrix;
            dynamicsWorld.AddRigidBody(DynamicPlatform1Body);

            // DynamicPlatform1 Mesh
            var platformtexture = TgcTexture.createTexture(D3DDevice.Instance.Device, $"{ctx.MediaDir}\\textures\\rockwall.jpg");
            DynamicPlatform1Mesh = TGCBox.fromSize(2 * size, platformtexture);
            DynamicPlatform1Mesh.Transform = new TGCMatrix(DynamicPlatform1Body.InterpolationWorldTransform);

            // DynamicPlatform2 Body
            DynamicPlatform2Body = BulletRigidBodyConstructor.CreateBox(size, mass, centerOfMass, 0, 0, 0, platformFriction);
            DynamicPlatform2Body.AngularFactor = new Vector3(0, 0, 0);
            DynamicPlatform2Body.CenterOfMassTransform = TGCMatrix.Translation(300, 300, 700).ToBsMatrix;
            dynamicsWorld.AddRigidBody(DynamicPlatform2Body);

            // DynamicPlatform2 Mesh
            var platformtexture2 = TgcTexture.createTexture(D3DDevice.Instance.Device, $"{ctx.MediaDir}\\textures\\rockwall.jpg");
            DynamicPlatform2Mesh = TGCBox.fromSize(2 * size, platformtexture);
            DynamicPlatform2Mesh.Transform = new TGCMatrix(DynamicPlatform2Body.InterpolationWorldTransform);

            // DynamicPlatform3 Body
            DynamicPlatform3Body = BulletRigidBodyConstructor.CreateBox(size, mass, centerOfMass, 0, 0, 0, platformFriction);
            DynamicPlatform3Body.AngularFactor = new Vector3(0, 0, 0);
            DynamicPlatform3Body.CenterOfMassTransform = TGCMatrix.Translation(300, 350, 0).ToBsMatrix;
            dynamicsWorld.AddRigidBody(DynamicPlatform3Body);

            // DynamicPlatform3 Mesh
            platformtexture2 = TgcTexture.createTexture(D3DDevice.Instance.Device, $"{ctx.MediaDir}\\textures\\rockwall.jpg");
            DynamicPlatform3Mesh = TGCBox.fromSize(2 * size, platformtexture);
            DynamicPlatform3Mesh.Transform = new TGCMatrix(DynamicPlatform3Body.InterpolationWorldTransform);
            #endregion
        }

        public void Update()
        {
            dynamicsWorld.StepSimulation(1 / 60f, 100);

            BandicootRigidBody.ActivationState = ActivationState.ActiveTag;
            BandicootRigidBody.ApplyCentralImpulse(ctx.BandicootMovement.ToBsVector);

            var posCamara = new TGCVector3(BandicootRigidBody.CenterOfMassPosition);
            ctx.BandicootCamera.Target = posCamara;

            var gravity = new Vector3(0, 0, 0);
            DynamicPlatform1Body.Gravity = gravity;
            DynamicPlatform2Body.Gravity = gravity;
            DynamicPlatform3Body.Gravity = gravity;

            MovePlatforms();
        }

        public void Render()
        {
            Rock1Mesh.Render();
            Rock2Mesh.Render();
            Rock3Mesh.Render();
            Rock4Mesh.Render();
            Rock5Mesh.Render();
            DynamicPlatform1Mesh.Render();
            DynamicPlatform2Mesh.Render();
            DynamicPlatform3Mesh.Render();
        }

        public void Dispose()
        {
            #region Dispose Rigid bodies
            Rock1Body.Dispose();
            Rock2Body.Dispose();
            Rock3Body.Dispose();
            Rock4Body.Dispose();
            Rock5Body.Dispose();
            BandicootRigidBody.Dispose();
            DynamicPlatform1Body.Dispose();
            DynamicPlatform2Body.Dispose();
            DynamicPlatform3Body.Dispose();
            #endregion

            #region Dispose Meshes
            Rock1Mesh.Dispose();
            Rock2Mesh.Dispose();
            Rock3Mesh.Dispose();
            Rock4Mesh.Dispose();
            Rock5Mesh.Dispose();
            DynamicPlatform1Mesh.Dispose();
            DynamicPlatform2Mesh.Dispose();
            DynamicPlatform3Mesh.Dispose();
            #endregion

            #region Dispose Physics world
            dynamicsWorld.Dispose();
            dispatcher.Dispose();
            collisionConfiguration.Dispose();
            constraintSolver.Dispose();
            overlappingPairCache.Dispose();
            #endregion 
        }

        public void MovePlatforms()
        {
            if (frecuency < 3000)
            {
                DynamicPlatform1Body.ActivationState = ActivationState.ActiveTag;
                DynamicPlatform2Body.ActivationState = ActivationState.ActiveTag;
                DynamicPlatform3Body.ActivationState = ActivationState.ActiveTag;

                frecuency++;
                        DynamicPlatform1Body.LinearVelocity = new Vector3(10 * direction, 0, 0);
                    

                if (frecuency < 1500)
                {
                    DynamicPlatform2Body.LinearVelocity = new Vector3(0, 0, -10 * direction);
                    DynamicPlatform3Body.LinearVelocity = new Vector3(0, -3 * direction, -10 * direction);
                }
                else
                {
                    DynamicPlatform2Body.LinearVelocity = new Vector3(0, 2 * direction, 0);
                    DynamicPlatform3Body.LinearVelocity = new Vector3(0, 3 * direction, -10 * direction);
                }
                if (BandicootRigidBody.CenterOfMassPosition.Y == DynamicPlatform1Body.CenterOfMassPosition.Y + 22.5f)
                {
                    DynamicPlatform1Body.LinearVelocity += new Vector3(0, risingImpulse, 0);
                }
                if (BandicootRigidBody.CenterOfMassPosition.Y == DynamicPlatform2Body.CenterOfMassPosition.Y + 22.5f)
                {
                    DynamicPlatform2Body.LinearVelocity += new Vector3(0, risingImpulse, 0);
                }
                if (BandicootRigidBody.CenterOfMassPosition.Y == DynamicPlatform3Body.CenterOfMassPosition.Y + 22.5f)
                {
                    DynamicPlatform3Body.LinearVelocity += new Vector3(0, risingImpulse, 0);
                }
            }
            else
            {
                frecuency = 0;

                if (direction == 1)
                    direction = -1;
                else
                    direction = 1;
            }

            DynamicPlatform1Mesh.Transform = new TGCMatrix(DynamicPlatform1Body.InterpolationWorldTransform);
            DynamicPlatform2Mesh.Transform = new TGCMatrix(DynamicPlatform2Body.InterpolationWorldTransform);
            DynamicPlatform3Mesh.Transform = new TGCMatrix(DynamicPlatform3Body.InterpolationWorldTransform);
            


        }
    }
}