/*using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Terrain;
using TGC.Core.Collision;
using TGC.Group.Model.Utils;
using System;
using TGC.Core.Geometry;
using TGC.Group.Model.Meshes;
using TGC.Group.Camara;

namespace TGC.Group.Model
{
    public class GameModelLevel1 : TgcExample
    {
        // Atributtes
        private const float alturaMaximaSalto = 70f;
        private const float MOVEMENT_SPEED = 100f;
        private float posInicialBandicoot;
        // Properties
        public bool IsJumping { get; set; }
        public int JumpDirection { get; set; }
        private InputHandler handler;
        public bool BoundingBox { get; set; }
        public TgcMesh Bandicoot { get; set; }
        public TgcThirdPersonCamera BandicootCamera { get; set; }
        public float DirectorAngle { get; set; }
        public TGCVector3 BandicootMovement { get; set; }
        public TGCMatrix Rotation { get; set; }
        public Physics Physics { get; set;}
        public TGCMatrix Scale { get; set; }
        public TGCMatrix Translation { get; set; }

        private List<TgcMesh> Parte1 = new List<TgcMesh>();
        private List<TgcMesh> Parte2 = new List<TgcMesh>();

        private List<Mesh> Lista = new List<Mesh>();

        public GameModelLevel1(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
           // handler = new InputHandler(this);
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


            Bandicoot.AutoTransformEnable = false;

            Bandicoot.Position = (new TGCVector3(200, 0, 500));

            Scale = TGCMatrix.Scaling(new TGCVector3(0.1f, 0.1f, 0.1f));
            Rotation = TGCMatrix.RotationYawPitchRoll(3.12f, 0, 0);
            Translation = TGCMatrix.Translation(Bandicoot.Position);

            Bandicoot.Transform = Scale * Rotation * TGCMatrix.Translation(Bandicoot.Position);

            Console.WriteLine("posicion:" + Bandicoot.Position);

            Bandicoot.BoundingBox.setExtremes(pMin, pMax);

            posInicialBandicoot = Bandicoot.Position.Y;
            DirectorAngle = FastMath.ToRad(180);
            JumpDirection = 1;




            Parte1 = sceneLoader.loadSceneFromFile($"{MediaDir}/Level_1/saveLevel1MeshCreator-TgcScene.xml").Meshes;
            // Parte2 = sceneLoader.loadSceneFromFile($"{MediaDir}/Nivel1-2-TgcScene.xml").Meshes;

            foreach (TgcMesh Item in Parte1)
            {
                Item.Move(0, 0, -1090f);
            }

            Lista = new Escena().ObtenerMeshesDeEscena($"{MediaDir}/Level_1/saveLevel1MeshCreator-TgcScene.xml");

            foreach (Mesh Item in Lista)
            {
                Item.Malla.Move(0, 0, -1090f);
            }
        }

        public void InitCamera()
        {
            BandicootCamera = new TgcThirdPersonCamera(Bandicoot.Position, 50f, 150f);
            Camara = BandicootCamera;
        }


        public void InitPhysics()
        {
            Physics = new Physics
            {
                UsingHeightmap = false
            };
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

        public override void Init()
        {
            var d3dDevice = D3DDevice.Instance.Device;
            
            InitMeshes();
            InitCamera();
            InitPhysics();
        }

        public override void Update()
        {
            PreUpdate();

            var anguloCamara = TGCVector3.Empty;

            ListenInputs();

            var originalPos = Bandicoot.Position;
            anguloCamara = Bandicoot.Position;

            BandicootMovement *= MOVEMENT_SPEED * ElapsedTime;

            Physics.Update(Input);

            Physics.bandicootRigidBody.ActivationState = BulletSharp.ActivationState.ActiveTag;
            Physics.bandicootRigidBody.ApplyCentralImpulse(BandicootMovement.ToBsVector);

            // Desplazar camara para seguir al personaje
            var posCamara = new TGCVector3(Bandicoot.Position);
            BandicootCamera.Target = posCamara;

            foreach (Mesh mesh in Lista)
            {
                if (TgcCollisionUtils.testAABBAABB(Bandicoot.BoundingBox, mesh.Malla.BoundingBox))
                {
                    mesh.ExecuteCollision(Bandicoot, Camara, BandicootMovement);
                }
            }

            Lista.RemoveAll(mesh => mesh.Malla.BoundingBox == null);
   
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
            
            foreach (Mesh item in Lista)
            {
                item.RenderMesh();
            }

            if (BoundingBox)
            {
                Bandicoot.BoundingBox.Render();
                foreach (Mesh item in Lista)
                {
                    item.RenderBoundingBox();
                }
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
            
        }
    }
}*/