using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Terrain;
using TGC.Core.Collision;
using System;
using TGC.Core.Geometry;

using TGC.Group.Model.Meshes;

namespace TGC.Group.Model
{
    public class GameModelIsla : TgcExample
    {
        /// <summary>
        ///     Constructor del juego.
        /// </summary>
        /// <param name="mediaDir">Ruta donde esta la carpeta con los assets</param>
        /// <param name="shadersDir">Ruta donde esta la carpeta con los shaders</param>

        private bool saltando;
        private int direccionSalto = 1;
        private float posInicialBandicoot;
        private float posBaseBandicoot;

        private const float alturaMaximaInicial = 70f;
        private const float MOVEMENT_SPEED = 100f;
        private float alturaMaximaSalto;
        private TgcSimpleTerrain terrain;

        //pisos, paredes y otros meshes
        private List<TgcMesh> Parte1 = new List<TgcMesh>();
        private List<TgcMesh> Parte2 = new List<TgcMesh>();


        private List<Mesh> ListaMeshes = new List<Mesh>();
        private List<Mesh> ListaMeshes2 = new List<Mesh>();

        private TgcMesh Bandicoot { get; set; }
        
        private Mesh objetoColisionado = null;

        private bool BoundingBox { get; set; }

        public GameModelIsla(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        public void InitTerrain()
        {
            string heightmapPath = $"{MediaDir}\\Heightmaps\\hawai.jpg";
            string texturePath = $"{MediaDir}\\Textures\\TerrainTextureHawaii.jpg";
            var center = new TGCVector3(-22f, 0f, 0f);
            float scaleXZ = 50f;
            float scaleY = 5f;

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
            Bandicoot.Scale = new TGCVector3(0.15f, 0.15f, 0.15f);
            Bandicoot.RotateY(3.12f);
            Bandicoot.BoundingBox.setExtremes(pMin, pMax);
            Bandicoot.Move(0, 1, 830);
            //Bandicoot.Position = TGCVector3.Empty;

            Parte1 = sceneLoader.loadSceneFromFile($"{MediaDir}/Nivel1-1-TgcScene.xml").Meshes;
            Parte2 = sceneLoader.loadSceneFromFile($"{MediaDir}/Nivel1-2-TgcScene.xml").Meshes;


            //Parte2 = sceneLoader.loadSceneFromFile($"{MediaDir}/room-with-fruit3-TgcScene-TgcScene.xml").Meshes;

            

            ListaMeshes = new Escena().ObtenerMeshesDeEscena($"{MediaDir}/Nivel1-1-TgcScene.xml");
            

            ListaMeshes2 = new Escena().ObtenerMeshesDeEscena($"{MediaDir}/Nivel1-2-TgcScene.xml");
            


            foreach (Mesh item in ListaMeshes2)
            {
                item.Malla.Move(0, 0, -1290f);
                //item.Malla.setColor(Color.Black);
            }
            

            ListaMeshes.AddRange(ListaMeshes2);


            ListaMeshes2 = new Escena().ObtenerMeshesDeEscena($"{MediaDir}/movingPlatform-as1-TgcScene.xml");

            foreach (Mesh item in ListaMeshes2)
            {
                Console.WriteLine(item.Malla.Name);
                item.Malla.Move(0, -10, -150);
                //item.Malla.setColor(Color.Black);
            }

            ListaMeshes.AddRange(ListaMeshes2);

            Console.WriteLine(Parte2.Count);
            Console.WriteLine(ListaMeshes.Count);


            foreach (var mesh in ListaMeshes)
            {
                //mesh.Malla.Move(0, 0, -1090f);
            }


            foreach (TgcMesh Item in Parte2)
            {
                Item.Move(0, 0, -1090f);
            }
        }

        public void InitCamera()
        {
            var postition = Bandicoot.Position + (new TGCVector3(0, 100, 120));
            var lookAt = Bandicoot.Position + new TGCVector3(0, 10, 0);
            Camara.SetCamera(postition, lookAt);
        }


        /// <summary>
        ///     Escribir aquí todo el código de inicialización: cargar modelos, texturas, 
        ///     estructuras de optimización y todo procesamiento que podemos pre calcular para nuestro juego.
        /// </summary>
        public override void Init()
        {
            //Device de DirectX para crear primitivas.
            var d3dDevice = D3DDevice.Instance.Device;

            InitTerrain();
            InitMeshes();
            InitCamera();

            posInicialBandicoot = Bandicoot.Position.Y;
            posBaseBandicoot = Bandicoot.Position.Y;

        }

        /// <summary>
        ///     Se llama en cada frame.
        ///     Se debe escribir toda la lógica de computo del modelo,
        ///     así como también verificar entradas del usuario y reacciones ante ellas.
        /// </summary>
        public override void Update()
        {
            PreUpdate();

            alturaMaximaSalto = posBaseBandicoot + alturaMaximaInicial;
            // Capturar Input teclado utilizado para movimiento 
            var anguloCamara = TGCVector3.Empty;
            var movimiento = TGCVector3.Empty;

            var lastPos = Bandicoot.Position;

            if (Input.keyPressed(Key.F))
            {
                BoundingBox = !BoundingBox;
            }

            // movimiento lateral
            if (Input.keyDown(Key.Left) || Input.keyDown(Key.A))
            {
                movimiento.X = 1;
            }
            else if (Input.keyDown(Key.Right) || Input.keyDown(Key.D))
            {
                movimiento.X = -1;
            }

            //Movernos adelante y atras, sobre el eje Z.
            if (Input.keyDown(Key.Up) || Input.keyDown(Key.W))
            {
                movimiento.Z = -1;
            }
            else if (Input.keyDown(Key.Down) || Input.keyDown(Key.S))
            {
                movimiento.Z = 1;
            }

            //salto
            if (Input.keyPressed(Key.Space) && !saltando)
            {
                saltando = true;
                direccionSalto = 1;
               //Bandicoot.Move(0, direccionSalto * MOVEMENT_SPEED * ElapsedTime, 0);
            }

            //Posicion original del mesh principal (o sea del bandicoot)
            var originalPos = Bandicoot.Position;
            anguloCamara = Bandicoot.Position;

            //Multiplicar movimiento por velocidad y elapsedTime
            movimiento *= MOVEMENT_SPEED * ElapsedTime;

         

            Bandicoot.Move(movimiento);
            //Bandicoot.Transform = TGCMatrix.Translation(movimiento);



            if (saltando)
            {
                Bandicoot.Move(0, direccionSalto * MOVEMENT_SPEED * ElapsedTime, 0);
                //movimiento.Y = direccionSalto * MOVEMENT_SPEED * ElapsedTime;

                //Si la posicion en Y es mayor a la maxima altura. 
                if (Bandicoot.Position.Y > alturaMaximaSalto)
                {
                        direccionSalto = -1;
                }
               foreach (var item in ListaMeshes)
                {
                    if (TgcCollisionUtils.testAABBAABB(Bandicoot.BoundingBox, item.Malla.BoundingBox))
                    {
                        if (!item.EsFruta())
                        {
                            //Bandicoot.Transform = TGCMatrix.Translation(-movimiento.X, movimiento.Y / 2 + ((item.Malla.BoundingBox.PMax.Y / 2) + 1), -movimiento.Z);
                            //movimiento = new TGCVector3(0, 1 * MOVEMENT_SPEED * ElapsedTime + 0.1f, 0); ;
                            Bandicoot.Move(0, 1 * MOVEMENT_SPEED * ElapsedTime + 0.1f, 0);

                            //item.ExecuteCollision(Bandicoot,Camara,movimiento);
                            
                            objetoColisionado = item;
                            posBaseBandicoot = item.Malla.BoundingBox.PMax.Y;
                            //if (item.isUpperCollision(Bandicoot, posBaseBandicoot))
                            //    item.ExecuteJumpCollision(Bandicoot, Camara, movimiento);
                            saltando = false;
                            break;
                        }
                    }
                }
            }
            else
            {
                if (Bandicoot.Position.Y > posInicialBandicoot && !objetoColisionado.EsFruta())
                {
                    //saltando = false;
                    
                    if(objetoColisionado.isUpperCollision(Bandicoot, posBaseBandicoot) && objetoColisionado.Malla != null)
                    {
                        Bandicoot.Move(0, 1 * MOVEMENT_SPEED * ElapsedTime + 0.1f, 0);
                        posBaseBandicoot = posInicialBandicoot;
                        saltando = true;
                        objetoColisionado.ExecuteJumpCollision(Bandicoot, Camara, movimiento);
                    }

                }
            }

            /*
            foreach (TgcMesh mesh in Parte1)
            {
                 if (TgcCollisionUtils.testAABBAABB(Bandicoot.BoundingBox, mesh.BoundingBox))
                 {
                   
                    Bandicoot.Move(-movimiento);
                     Camara.SetCamera((Camara.Position - movimiento), anguloCamara);
                 }
                 
            }
            foreach (TgcMesh mesh in Parte2)
            {
                if (TgcCollisionUtils.testAABBAABB(Bandicoot.BoundingBox, mesh.BoundingBox))
                {
                    Bandicoot.Move(-movimiento);
                    Camara.SetCamera((Camara.Position - movimiento), anguloCamara);
                }
            }*/

            foreach (var mesh in ListaMeshes)
            {
                if (mesh.Malla.BoundingBox == null)
                {
                    continue;
                }

                if(mesh.Malla.Name.Contains("movingPlatform"))
                {
                    mesh.Move(MOVEMENT_SPEED * ElapsedTime);
                    //mesh.Malla.Transform = 
                }

                if (TgcCollisionUtils.testAABBAABB(Bandicoot.BoundingBox, mesh.Malla.BoundingBox))
                {
                    //comment = mesh.GetType().ToString();
                    mesh.ExecuteCollision(Bandicoot, Camara, movimiento);
                }
            }
            ListaMeshes.RemoveAll(mesh => mesh.Malla.BoundingBox == null);

            //Desplazar camara para seguir al personaje
            Camara.SetCamera((Camara.Position + movimiento), anguloCamara);

            //Capturar Input Mouse
            if (Input.buttonUp(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                //Como ejemplo podemos hacer un movimiento simple de la cámara.
                //En este caso le sumamos un valor en Y
                Camara.SetCamera(Camara.Position + new TGCVector3(0, 10f, 0), Camara.LookAt);
                //Ver ejemplos de cámara para otras operaciones posibles.

                //Si superamos cierto Y volvemos a la posición original.
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
            DrawText.drawText("La posicion del bandicoot es: " + TGCVector3.PrintVector3(Bandicoot.Position), 0, 40, Color.Black);
            //terrain.Render();

            // Cuando tenemos modelos mesh podemos utilizar un método que hace la matriz de transformación estándar.
            // Es útil cuando tenemos transformaciones simples, pero OJO cuando tenemos transformaciones jerárquicas o complicadas.
            Bandicoot.UpdateMeshTransform();
            Bandicoot.Render();
            /*
            foreach (TgcMesh item in Parte1)
            {
                item.Render();
            }
            
            foreach (TgcMesh item in Parte2)
            {
                item.Render();
            }
            */
            foreach (Mesh item in ListaMeshes)
            {
                item.RenderMesh();
            }

            if (BoundingBox)
            {
                foreach (Mesh mesh in ListaMeshes)
                {
                    mesh.RenderBoundingBox();
                }
                Bandicoot.BoundingBox.Render();

                /*
                foreach (TgcMesh item in Parte1)
                {
                    item.BoundingBox.Render();
                }
                foreach (TgcMesh item in Parte2)
                {
                    item.BoundingBox.Render();
                }
                */
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