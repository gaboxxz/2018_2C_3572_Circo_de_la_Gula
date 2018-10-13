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
        // Attributes
        private const float MOVEMENT_SPEED = 100f;
        private TgcSkyBox skyBox;

        #region Properties
        public bool IsJumping { get; set; }
        public int JumpDirection { get; set; }
        private InputHandler handler;
        public bool BoundingBox { get; set; }
        public TgcMesh Bandicoot { get; set; }
        public TgcThirdPersonCamera BandicootCamera { get; set; }
        public float DirectorAngle { get; set; }
        public TGCVector3 BandicootMovement { get; set; }
        public TGCMatrix Scale { get; set; }
        public TGCMatrix Translation { get; set; }
        public TGCMatrix Rotation { get; set; }
        public TgcSimpleTerrain Terrain { get; set; }
        public IPhysics Physics { get; set; }
        #endregion

        // Methods
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
            string texturePath = $"{MediaDir}\\Textures\\canyon-horizontal.png";
            var center = TGCVector3.Empty;
            float scaleXZ = 40f;
            float scaleY = 1.5f;

            Terrain = new TgcSimpleTerrain();
            Terrain.loadHeightmap(heightmapPath, scaleXZ, scaleY, center);
            Terrain.loadTexture(texturePath);
        }

        #region Init
        public void InitMeshes()
        {
            var sceneLoader = new TgcSceneLoader();
            string path = $"{MediaDir}/crash/bandicoot-TgcScene.xml";
            
            Bandicoot = sceneLoader.loadSceneFromFile(path).Meshes[0];

            Scale = TGCMatrix.Scaling(new TGCVector3(0.1f, 0.1f, 0.1f));
            Rotation = TGCMatrix.RotationY(3.12f);
            Translation = TGCMatrix.Translation(Bandicoot.Position);

            Bandicoot.AutoTransformEnable = false;
            Bandicoot.Position = TGCVector3.Empty;
            Bandicoot.Transform = Scale * Rotation * Translation;
            
            DirectorAngle = FastMath.ToRad(180);
            JumpDirection = 1;
        }

        public void InitCamera()
        {
            BandicootCamera = new TgcThirdPersonCamera(Bandicoot.Position, 50f, 150f);
            Camara = BandicootCamera;
        }

        public void InitPhysics() {
            Physics = new CanyonPhysics();
            Physics.Init(this);
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

        public void InitSkybox()
        {
            string path = $"{MediaDir}Skybox\\SkyBox1\\";
            skyBox = new TgcSkyBox
            {
                Center = TGCVector3.Empty,
                Size = new TGCVector3(10000, 10000, 10000)
            };
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, $"{path}\\phobos_up.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, $"{path}\\phobos_dn.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, $"{path}\\phobos_lf.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, $"{path}\\phobos_rt.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, $"{path}\\phobos_bk.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, $"{path}\\phobos_ft.jpg");
            skyBox.Init();
        }
        #endregion 

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

            var cameraAngle = TGCVector3.Empty;
            ListenInputs();

            var originalPos = Bandicoot.Position;
            cameraAngle = Bandicoot.Position;

            BandicootMovement *= MOVEMENT_SPEED * ElapsedTime;

            Translation = TGCMatrix.Translation(BandicootMovement);

            Physics.Update();

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
            Bandicoot.Transform = Scale * Rotation * new TGCMatrix(Physics.BandicootRigidBody.InterpolationWorldTransform);

            skyBox.Render();
            Bandicoot.Render();
            Terrain.Render();
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
            Terrain.Dispose();
        }
    }
}