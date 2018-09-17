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

namespace TGC.Group.Model
{
    public class GameModelCanyon : TgcExample
    {
        /* Atributos */
        public bool isJumping;
        public int jumpDirection = 1;
        private float posInicialBandicoot;
        private float alturaMaximaSalto = 20f;

        private const float MOVEMENT_SPEED = 100f;
        private TgcSimpleTerrain terrain;
        public TGCVector3 bandicootMovement;

        // Propiedades
        private InputHandler Handler { get; set; }
        public bool BoundingBox { get; set; }
        private TgcMesh Bandicoot { get; set; } 
        public TgcScene Scene { get; set; }

        /* Metodos */
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

            Bandicoot = sceneLoader.loadSceneFromFile(path).Meshes[0];
            Bandicoot.Scale = new TGCVector3(0.05f, 0.05f, 0.05f);
            Bandicoot.RotateY(3.12f); // NO HAY QUE USAR ESTE METODO PORQUE HACE CALCULOS sen(x) y cos(x)
            Bandicoot.BoundingBox.setExtremes(pMin, pMax);
        }

        public void InitCamera()
        {
            /* Suelen utilizarse objetos que manejan el comportamiento de la camara.
               Lo que en realidad necesitamos gráficamente es una matriz de View.
               El framework maneja una cámara estática, pero debe ser inicializada.
               Internamente el framework construye la matriz de view con estos dos vectores.
               Luego en nuestro juego tendremos que crear una cámara que cambie 
               la matriz de view con variables como movimientos o animaciones de escenas. */
            var postition = new TGCVector3(-5, 20, 50);
            var lookAt = Bandicoot.Position;

            Camara.SetCamera(postition, lookAt);
        }

        public void ListenInputs () { 
            Handler.HandleInput(Key.F);
            Handler.HandleInput(Key.Left);
            Handler.HandleInput(Key.Right);
            Handler.HandleInput(Key.Up);
            Handler.HandleInput(Key.Down);
            Handler.HandleInput(Key.Space);
        }

        /// <summary>
        ///     Escribir aquí todo el código de inicialización: cargar modelos, texturas, 
        ///     estructuras de optimización y todo procesamiento 
        ///     que podemos pre calcular para nuestro juego.
        /// </summary>
        public override void Init()
        {
            //Device de DirectX para crear primitivas.
            var d3dDevice = D3DDevice.Instance.Device;

            InitTerrain();
            InitMeshes();
            InitCamera();

            posInicialBandicoot = Bandicoot.Position.Y;
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

            //Desplazar camara para seguir al personaje
            Camara.SetCamera(Camara.Position + new TGCVector3(bandicootMovement), anguloCamara);

            if (Input.buttonUp(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                Camara.SetCamera(Camara.Position + new TGCVector3(0, 10f, 0), Camara.LookAt);
            }

            if (Input.buttonUp(TgcD3dInput.MouseButtons.BUTTON_RIGHT))
            {
                Camara.SetCamera(Camara.Position + new TGCVector3(0, -10f, 0), Camara.LookAt);
            }



            //TgcCollisionUtils.testAABBAABB(Bandicoot.BoundingBox, me);
            
            /*foreach (var mesh in Scene.Meshes)
            {
                if (TgcCollisionUtils.testAABBAABB(mesh.BoundingBox, Bandicoot.BoundingBox))
                {
                    Bandicoot.Move(-bandicootMovement);
                }
            }
            */

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

            terrain.Render();

            // Cuando tenemos modelos mesh podemos utilizar un método que hace la matriz de transformación estándar.
            // Es útil cuando tenemos transformaciones simples, pero OJO cuando tenemos transformaciones jerárquicas o complicadas.
            Bandicoot.UpdateMeshTransform();
            Bandicoot.Render();

            if (BoundingBox)
            {
                Bandicoot.BoundingBox.Render();
            }

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
            terrain.Dispose();
        }
    }
}