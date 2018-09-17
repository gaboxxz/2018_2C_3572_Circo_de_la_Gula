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
    /// <summary>
    ///     Ejemplo para implementar el TP.
    ///     Inicialmente puede ser renombrado o copiado para hacer m�s ejemplos chicos, 
    ///     en el caso de copiar para que se ejecute el nuevo ejemplo deben cambiar el modelo 
    ///     que instancia GameForm <see cref="Form.GameForm.InitGraphics()" />
    ///     line 97.
    /// </summary>
    public class GameModelCanyon : TgcExample
    {
        /// <summary>
        ///     Constructor del juego.
        /// </summary>
        /// <param name="mediaDir">Ruta donde esta la carpeta con los assets</param>
        /// <param name="shadersDir">Ruta donde esta la carpeta con los shaders</param>

        /* Atributos */
        public bool isJumping;
        public int jumpDirection = 1;
        private float posInicialBandicoot;
        private float alturaMaximaSalto = 20f;

        private const float MOVEMENT_SPEED = 100f;
        private InputHandler Handler { get; set; }
        public bool BoundingBox { get; set; }
        private TgcMesh Bandicoot { get; set; } 
        private TgcSimpleTerrain terrain;
        public TGCVector3 bandicootMovement;

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
            string heightmapPath = $"{MediaDir}\\Heightmap\\hawai.jpg";
            string texturePath = $"{MediaDir}\\Textures\\TerrainTextureHawaii.jpg";
            var center = new TGCVector3(0f, 0f, 0f);
            float scaleXZ = 100f;
            float scaleY = 50f;

            terrain = new TgcSimpleTerrain();
            terrain.loadHeightmap(heightmapPath, scaleXZ, scaleY, center);
            terrain.loadTexture(texturePath);
        }

        public void InitMeshes()
        {
            var sceneLoader = new TgcSceneLoader();
            string path = $"{MediaDir}/crash/CRASH (2)-TgcScene.xml";
            var pMin = new TGCVector3(0, 0, 0);
            var pMax = new TGCVector3(-185f, 225f, -100f);

            Bandicoot = sceneLoader.loadSceneFromFile(path).Meshes[0];
            Bandicoot.Scale = new TGCVector3(0.05f, 0.05f, 0.05f);
            Bandicoot.RotateY(3.12f);
            Bandicoot.BoundingBox.setExtremes(pMin, pMax);
        }

        public void InitCamera()
        {
            /* Suelen utilizarse objetos que manejan el comportamiento de la camara.
               Lo que en realidad necesitamos gr�ficamente es una matriz de View.
               El framework maneja una c�mara est�tica, pero debe ser inicializada.
               Internamente el framework construye la matriz de view con estos dos vectores.
               Luego en nuestro juego tendremos que crear una c�mara que cambie 
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
        ///     Escribir aqu� todo el c�digo de inicializaci�n: cargar modelos, texturas, 
        ///     estructuras de optimizaci�n y todo procesamiento 
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

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la l�gica de computo del modelo,
        ///     as� como tambi�n verificar entradas del usuario y reacciones ante ellas.
        /// </summary>
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

            //Capturar Input Mouse
            if (Input.buttonUp(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                //Como ejemplo podemos hacer un movimiento simple de la c�mara.
                //En este caso le sumamos un valor en Y
                Camara.SetCamera(Camara.Position + new TGCVector3(0, 10f, 0), Camara.LookAt);
                //Ver ejemplos de c�mara para otras operaciones posibles.

                //Si superamos cierto Y volvemos a la posici�n original.
                if (Camara.Position.Y > 300f)
                {
                    Camara.SetCamera(new TGCVector3(Camara.Position.X, 0f, Camara.Position.Z), Camara.LookAt);
                }
            }

            if (Input.buttonUp(TgcD3dInput.MouseButtons.BUTTON_RIGHT))
            {
                //Pruebo si baja camara
                Camara.SetCamera(Camara.Position + new TGCVector3(0, -10f, 0), Camara.LookAt);

                //igual que si sube a cierta altura reinicio camara
                if (Camara.Position.Y < -200f)
                {
                    Camara.SetCamera(new TGCVector3(Camara.Position.X, 0f, Camara.Position.Z), Camara.LookAt);
                }


            }
            
            

            /*  COLISION *//*
             *  Gracias al namespace TGC.Core.Collision
             * Siendo la escena el conjunto de meshes, perteneciente a la clase TGCScene
             * se puede usar la funcion booleana TgcCollisionUtils.testAABBAABB(aabb1, aabb2)
             * Si se tocan los AABB (Axis-Aligned Bounding Box), entonces colisionan, y cumple la funcion.
             * Actualmente, en este caso contrarrestarian el movimiento, evitando que choquen y que queden pegados.
            */


            //No hay escena cargada, por lo tanto lo dejo comentado 

            /*
            foreach (var mesh in Scene.Meshes)
            {
                if (TgcCollisionUtils.testAABBAABB(mesh.BoundingBox, Bandicoot.BoundingBox))
                {
                    Bandicoot.Move(-bandicootMovement);
                }
            }
            */


            PostUpdate();

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

            terrain.Render();

            // Cuando tenemos modelos mesh podemos utilizar un m�todo que hace la matriz de transformaci�n est�ndar.
            // Es �til cuando tenemos transformaciones simples, pero OJO cuando tenemos transformaciones jer�rquicas o complicadas.
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
        ///     Es muy importante liberar los recursos, sobretodo los gr�ficos
        ///     ya que quedan bloqueados en el device de video.
        /// </summary>
        public override void Dispose()
        {
            Bandicoot.Dispose();
            terrain.Dispose();
        }
    }
}