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

namespace TGC.Group.Model
{
    public class GameModelCanyon : TgcExample
    {
        /* Attributes */
        public bool isJumping;
        public int jumpDirection = 1;
        private float posInicialBandicoot;
        private float alturaMaximaSalto = 20f;
        private const float MOVEMENT_SPEED = 100f;
        public TGCVector3 bandicootMovement;
        private TgcSimpleTerrain terrain;
        private Physics physics;
		public float anguloDirector = FastMath.ToRad(180);
        public TgcThirdPersonCamera banditcamara;

        // Properties
        private InputHandler Handler { get; set; }
        public bool BoundingBox { get; set; }
        public TgcMesh Bandicoot { get; set; }

        /* Methods */

        // Constructor
        public GameModelCanyon(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
            Handler = new InputHandler(this);
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
            var pMin = new TGCVector3(-185f, 0, -100f);
            var pMax = new TGCVector3(0, 225f, 0);
            TGCMatrix escala;
            TGCMatrix rotacion;

            Bandicoot = sceneLoader.loadSceneFromFile(path).Meshes[0];

            escala = TGCMatrix.Scaling(new TGCVector3(0.1f,0.1f,0.1f));
            rotacion = TGCMatrix.RotationYawPitchRoll(0, 3.12f, 0);

            Bandicoot.Transform = rotacion * escala; 
     
            Bandicoot.BoundingBox.setExtremes(pMin, pMax);

            posInicialBandicoot = Bandicoot.Position.Y;
        }

        public void InitCamera()
        {
            /* Suelen utilizarse objetos que manejan el comportamiento de la camara.
               Lo que en realidad necesitamos gr�ficamente es una matriz de View.
               El framework maneja una c�mara est�tica, pero debe ser inicializada.
               Internamente el framework construye la matriz de view con estos dos vectores.
               Luego en nuestro juego tendremos que crear una c�mara que cambie 
               la matriz de view con variables como movimientos o animaciones de escenas. */
            banditcamara = new TgcThirdPersonCamera(Bandicoot.Position, 50f, 150f);
            Camara = banditcamara;
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
            Handler.HandleInput(Key.F);
            Handler.HandleInput(Key.Left);
            Handler.HandleInput(Key.Right);
            Handler.HandleInput(Key.Up);
            Handler.HandleInput(Key.W);
            Handler.HandleInput(Key.Down);
            Handler.HandleInput(Key.S);
            Handler.HandleInput(Key.Space);
            Handler.HandleInput((Key)TgcD3dInput.MouseButtons.BUTTON_LEFT);
            Handler.HandleInput((Key)TgcD3dInput.MouseButtons.BUTTON_RIGHT);
        }

        /// <summary>
        ///     Escribir aqu� todo el c�digo de inicializaci�n: cargar modelos, texturas, 
        ///     estructuras de optimizaci�n y todo procesamiento 
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
            bandicootMovement *= MOVEMENT_SPEED * ElapsedTime;

            Bandicoot.Move(bandicootMovement);
            if (isJumping)
            {
                Bandicoot.Move(0, jumpDirection * MOVEMENT_SPEED * ElapsedTime, 0);

                //Si la posicion en Y es mayor a la maxima altura. 
                if (Bandicoot.Position.Y > alturaMaximaSalto)
                {
                    jumpDirection = -1;
                }

                if (Bandicoot.Position.Y <= posInicialBandicoot)
                {
                    isJumping = false;
                }
            }

            physics.Update(Input);
           
            // Desplazar camara para seguir al personaje
            banditcamara.SetCamera(banditcamara.Position + new TGCVector3(bandicootMovement), anguloCamara);

            PostUpdate();
        }

        /// <summary>
        ///     Se llama cada vez que hay que refrescar la pantalla.
        ///     Escribir aqu� todo el c�digo referido al renderizado.
        ///     Borrar todo lo que no haga falta.
        /// </summary>
        public override void Render()
        {
            //Inicio el render de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones seg�n nuestra conveniencia.
            PreRender();

            //Dibuja un texto por pantalla
            DrawText.drawText("Con la tecla F se dibuja el bounding box.", 0, 20, Color.OrangeRed);
            DrawText.drawText("Con clic izquierdo subimos la camara [Actual]: " + TGCVector3.PrintVector3(Camara.Position), 0, 30, Color.OrangeRed);

            if (BoundingBox)
            {
                Bandicoot.BoundingBox.Render();
            }

            Bandicoot.UpdateMeshTransform();
            Bandicoot.Render();
            terrain.Render();
            physics.Render();
          
            // Finaliza el render y presenta en pantalla, al igual que el preRender se debe usar para casos 
            // puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
            PostRender();
        }

        /// <summary>
        ///     Es muy importante liberar los recursos, sobretodo los gr�ficos
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