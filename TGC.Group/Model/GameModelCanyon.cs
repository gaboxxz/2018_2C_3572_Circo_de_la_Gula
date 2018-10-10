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
        private const float MOVEMENT_SPEED = 100f;
        private TgcSimpleTerrain terrain;
        private TgcSkyBox skyBox;
        
        // Properties
        public bool IsJumping { get; set; }
        public int JumpDirection { get; set; }
        private InputHandler handler;
        public bool BoundingBox { get; set; }
        public TgcMesh Bandicoot { get; set; }
        public TgcThirdPersonCamera BandicootCamera { get ; set; }
		public float DirectorAngle { get; set; }
        public TGCVector3 BandicootMovement { get; set; }
        public TGCMatrix Scale { get; set; }
        public TGCMatrix Translation { get; set; }
        public TGCMatrix Rotation { get; set; }
        public Physics Physics { get; set; }

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

            Scale = TGCMatrix.Scaling(new TGCVector3(0.1f, 0.1f, 0.1f));
            Rotation = TGCMatrix.RotationYawPitchRoll(3.12f, 0, 0);
            Translation = TGCMatrix.Translation(Bandicoot.Position);

            Bandicoot.Transform = Scale * Rotation * TGCMatrix.Translation(Bandicoot.Position);
            Bandicoot.BoundingBox.setExtremes(pMin, pMax);

            posInicialBandicoot = Bandicoot.Position.Y;
            DirectorAngle = FastMath.ToRad(180);
            JumpDirection = 1;
        }

        public void InitCamera()
        {
            BandicootCamera = new TgcThirdPersonCamera(Bandicoot.Position, 50f, 150f);
            Camara = BandicootCamera;
        }

        public void InitPhysics() {
            Physics = new Physics
            {
                UsingHeightmap = true
            };
            Physics.SetTriangleDataVB(terrain.getData());
            Physics.Init(MediaDir);
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
        /// 

        public void InitSkybox()
        {
            //Crear SkyBox
            string path = $"{MediaDir}Skybox\\SkyBox1\\";
            skyBox = new TgcSkyBox();
            skyBox.Center = TGCVector3.Empty;
            skyBox.Size = new TGCVector3(10000, 10000, 10000);
            //var texturesPath = MediaDir + "Texturas\\Quake\\SkyBox3\\";
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, path + "phobos_up.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, path + "phobos_dn.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, path + "phobos_lf.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, path + "phobos_rt.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, path + "phobos_bk.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, path + "phobos_ft.jpg");
            skyBox.Init();
        }

        public override void Init()
        {
            var d3dDevice = D3DDevice.Instance.Device;
            InitSkybox();
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

            //Translation = TGCMatrix.Translation(BandicootMovement);

            Physics.Update(Input);

            Physics.bandicootRigidBody.ActivationState = BulletSharp.ActivationState.ActiveTag;
            Physics.bandicootRigidBody.ApplyCentralImpulse(BandicootMovement.ToBsVector);

            // Desplazar camara para seguir al personaje
            var posCamara = new TGCVector3(Physics.bandicootRigidBody.CenterOfMassPosition);
            BandicootCamera.Target = posCamara;

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
            DrawText.drawText("La posicion del bandicoot es: " + Physics.bandicootRigidBody.CenterOfMassPosition, 0, 50, Color.OrangeRed);

            if (BoundingBox)
            {
                Bandicoot.BoundingBox.Render();
            }
           
            Bandicoot.Transform = Scale * Rotation * new TGCMatrix(Physics.bandicootRigidBody.InterpolationWorldTransform)* TGCMatrix.Translation(0,-10,0);

            skyBox.Render();
            Bandicoot.Render();
            terrain.Render();
            Physics.Render();
          
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
            Physics.Dispose();
            terrain.Dispose();
        }
    }
}