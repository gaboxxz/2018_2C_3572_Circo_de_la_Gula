using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Collision;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Terrain;
using TGC.Group.Model.Utils;
using TGC.Group.Camara;
using System;

namespace TGC.Group.Model
{
    public class GameModelCanyon : TgcExample, IGameModel
    {
        /* Attributes */
        private float posInicialBandicoot;
        private float alturaMaximaSalto = 20f;
        private const float MOVEMENT_SPEED = 100f;
        private TgcSimpleTerrain terrain;
        private Physics physics;

        // Properties
        public bool IsJumping { get; set; }
        public int JumpDirection { get; set; }
        private InputHandler handler;
        public bool BoundingBox { get; set; }
        public TgcMesh Bandicoot { get; set; }
        public TgcThirdPersonCamera BandicootCamera { get ; set; }
		public float DirectorAngle { get; set; }
        public TGCVector3 BandicootMovement { get; set; }
        public TGCMatrix escala;
        public TGCMatrix traslacion;
        public TGCMatrix rotacion;

        /* Methods */

        // Constructor
        public GameModelCanyon(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
            handler = new InputHandler(this);
        }

        public void InitTerrain()
        {
            string heightmapPath = $"{MediaDir}\\Heightmaps\\canyon1.jpg";
            string texturePath = $"{MediaDir}\\Textures\\pjrock16.jpg";
            var center = new TGCVector3(0f, 0f, 0f);
            float scaleXZ = 15f;
            float scaleY = 3f;

            terrain = new TgcSimpleTerrain();
            terrain.loadHeightmap(heightmapPath, scaleXZ, scaleY, center);
            terrain.loadTexture(texturePath);
        }

        public void InitMeshes()
        {
            var sceneLoader = new TgcSceneLoader();
            string path = $"{MediaDir}/crash/CRASH (2)-TgcScene.xml";
            var pMin = new TGCVector3(-18.5f, 0, -10f);
            var pMax = new TGCVector3(0, 22.5f, 0);
            
            Bandicoot = sceneLoader.loadSceneFromFile(path).Meshes[0];

            Bandicoot.AutoTransformEnable = false;

            Bandicoot.Position = TGCVector3.Empty;

            escala = TGCMatrix.Scaling(new TGCVector3(0.1f, 0.1f, 0.1f));
            rotacion = TGCMatrix.RotationYawPitchRoll(3.12f, 0, 0);
            traslacion = TGCMatrix.Translation(Bandicoot.Position);

            Bandicoot.Transform = escala * rotacion * TGCMatrix.Translation(Bandicoot.Position);

            Console.WriteLine("posicion:" + Bandicoot.Position);
            
            Bandicoot.BoundingBox.setExtremes(pMin, pMax);

            posInicialBandicoot = Bandicoot.Position.Y;
            DirectorAngle = FastMath.ToRad(180);
            JumpDirection = 1;
        }

        public void InitCamera()
        {
            /* Suelen utilizarse objetos que manejan el comportamiento de la camara.
               Lo que en realidad necesitamos gráficamente es una matriz de View.
               El framework maneja una cámara estática, pero debe ser inicializada.
               Internamente el framework construye la matriz de view con estos dos vectores.
               Luego en nuestro juego tendremos que crear una cámara que cambie 
               la matriz de view con variables como movimientos o animaciones de escenas. */
            BandicootCamera = new TgcThirdPersonCamera(Bandicoot.Position, 50f, 150f);
            Camara = BandicootCamera;
			//var postition = new TGCVector3(-5, 20, 50);
            //var lookAt = Bandicoot.Position;

            // Camara.SetCamera(postition, lookAt);
        }

        public void InitPhysics() {
            physics = new Physics();
            physics.SetTriangleDataVB(terrain.getData());
            physics.Init(MediaDir);
            
        }

        public void ListenInputs()
        {
            handler.HandleInput(Key.F);
            handler.HandleInput(Key.Left);
            handler.HandleInput(Key.Right);
            handler.HandleInput(Key.Up);
            handler.HandleInput(Key.W);
            handler.HandleInput(Key.Down);
            handler.HandleInput(Key.S);
            handler.HandleInput(Key.Space);
            handler.HandleInput((Key)TgcD3dInput.MouseButtons.BUTTON_LEFT);
            handler.HandleInput((Key)TgcD3dInput.MouseButtons.BUTTON_RIGHT);
        }

        /// <summary>
        ///     Escribir aquí todo el código de inicialización: cargar modelos, texturas, 
        ///     estructuras de optimización y todo procesamiento 
        ///     que podemos pre calcular para nuestro juego.
        /// </summary>
        public override void Init()
        {
            var d3dDevice = D3DDevice.Instance.Device;

            InitTerrain();
            InitMeshes();
            InitCamera();
            InitPhysics();
        }

        public override void Update()
        {
            PreUpdate();

            var anguloCamara = TGCVector3.Empty;

            ListenInputs();

            //Posicion original del mesh principal (o sea del bandicoot)
            var originalPos = Bandicoot.Position;
            anguloCamara = Bandicoot.Position;

            //Multiplicar movimiento por velocidad y elapsedTime
            BandicootMovement *= MOVEMENT_SPEED * ElapsedTime;

            traslacion = TGCMatrix.Translation(BandicootMovement);

            //Bandicoot.Move(BandicootMovement);
            if (IsJumping)
            {
                Bandicoot.Move(0, JumpDirection * MOVEMENT_SPEED * ElapsedTime, 0);

                //Si la posicion en Y es mayor a la maxima altura. 
                if (Bandicoot.Position.Y > alturaMaximaSalto)
                {
                    JumpDirection = -1;
                }

                if (Bandicoot.Position.Y <= posInicialBandicoot)
                {
                    IsJumping = false;
                }
            }

            physics.Update(Input);

            if (Input.keyDown(Key.W))
            {
                //moving = true;
                //Activa el comportamiento de la simulacion fisica para la capsula
                physics.bandicootRigidBody.ActivationState = BulletSharp.ActivationState.ActiveTag;
                //bandicootRigidBody.AngularVelocity = TGCVector3.Empty.ToBsVector;
                physics.bandicootRigidBody.ApplyCentralImpulse(BandicootMovement.ToBsVector);
                //physics.bandicootRigidBody.ApplyForce(BandicootMovement.ToBsVector, ;
                //physics.bandicootRigidBody.ApplyImpulse(BandicootMovement.ToBsVector, (Bandicoot.Position + new TGCVector3(5, 5, 5)).ToBsVector);
            }
/*
            if (input.keyDown(Key.A))
            {
                director.TransformCoordinate(TGCMatrix.RotationY(-angle * 0.01f));
                personaje.Transform = TGCMatrix.Translation(TGCVector3.Empty) * TGCMatrix.RotationY(-angle * 0.01f) * new TGCMatrix(capsuleRigidBody.InterpolationWorldTransform);
                capsuleRigidBody.WorldTransform = personaje.Transform.ToBsMatrix;
            }
            */
            // Desplazar camara para seguir al personaje
            BandicootCamera.SetCamera(BandicootCamera.Position + new TGCVector3(BandicootMovement), anguloCamara);

            PostUpdate();
        }

        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aquí todo el código referido al renderizado.
        ///     Borrar todo lo que no haga falta.
        /// </summary>
        public override void Render()
        {
            //Inicio el render de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones según nuestra conveniencia.
            PreRender();

            //Dibuja un texto por pantalla
            DrawText.drawText("Con la tecla F se dibuja el bounding box.", 0, 20, Color.OrangeRed);
            DrawText.drawText("Con clic izquierdo subimos la camara [Actual]: " + TGCVector3.PrintVector3(Camara.Position), 0, 30, Color.OrangeRed);
            DrawText.drawText("La posicion del bandicoot es: " + TGCVector3.PrintVector3(Bandicoot.Position), 0, 40, Color.OrangeRed);
            DrawText.drawText("La posicion del bandicoot es: " + physics.bandicootRigidBody.CenterOfMassPosition, 0, 50, Color.OrangeRed);


            if (BoundingBox)
            {
                Bandicoot.BoundingBox.Render();
            }


            //Bandicoot.Transform = escala * rotacion * traslacion;
            //Bandicoot.Transform = escala * rotacion * new TGCMatrix(physics.bandicootRigidBody.InterpolationWorldTransform);
            Bandicoot.Transform = escala * rotacion * new TGCMatrix(physics.bandicootRigidBody.InterpolationWorldTransform);

            //Bandicoot.UpdateMeshTransform();
            Bandicoot.Render();
            terrain.Render();
            physics.Render();
          
            // Finaliza el render y presenta en pantalla, al igual que el preRender se debe usar para casos 
            // puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
            PostRender();
        }

        /// <summary>
        ///     Es muy importante liberar los recursos, sobretodo los gráficos
        ///     ya que quedan bloqueados en el device de video.
        /// </summary>
        public override void Dispose()
        {
            Bandicoot.Dispose();
            physics.Dispose();
            terrain.Dispose();
        }
    }
}