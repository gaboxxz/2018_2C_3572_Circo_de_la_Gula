using System;
using System.Collections.Generic;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Text;
using TGC.Core.Textures;

namespace TGC.Group.Model
{
    class GameModelMenu : TgcExample
    {
        #region Properties
        private TgcPlane Floor { get; set; }
        private TgcPlane LeftWall { get; set; }
        private TgcPlane RightWall { get; set; }
        private TGCBox FrontWall { get; set; }
        private List<TgcMesh> Vegetation { get; set; }
        private TgcText2D screenText;
        #endregion

        public GameModelMenu(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        #region Init
        public void InitCoverPage()
        {
            var center = TGCVector3.Empty;
            var size = new TGCVector3(1000, 0, 1000);
            var orientation = TgcPlane.Orientations.XZplane;
            var device = D3DDevice.Instance.Device;
            string filePath = $"{MediaDir}\\Textures\\tierra3.jpg";
            var texture = TgcTexture.createTexture(device, filePath);
            Floor = new TgcPlane(center, size, orientation, texture);

            center = new TGCVector3(0, 0, 200);
            size = new TGCVector3(0, 1000, 1000);
            orientation = TgcPlane.Orientations.YZplane;
            filePath = $"{MediaDir}\\Textures\\rock_wall.jpg";
            texture = TgcTexture.createTexture(device, filePath);
            LeftWall = new TgcPlane(center, size, orientation, texture);

            center = new TGCVector3(200, 0, 0);
            size = new TGCVector3(1000, 1000, 0);
            orientation = TgcPlane.Orientations.XYplane;
            RightWall = new TgcPlane(center, size, orientation, texture);

            center = TGCVector3.Empty;
            size = new TGCVector3(400, 400, 0);
            filePath = $"{MediaDir}\\Textures\\phobos_bk.jpg";
            texture = TgcTexture.createTexture(device, filePath);
            FrontWall = TGCBox.fromSize(center, size, texture);
            FrontWall.Transform =
                TGCMatrix.Scaling(2,2,1)
                * TGCMatrix.RotationY(FastMath.QUARTER_PI) 
                * TGCMatrix.Translation(0, size.Y/2, 0);

            var loader = new TgcSceneLoader();
            filePath = $"{MediaDir}\\Meshes\\Vegetation\\Palmera3\\Palmera3-TgcScene.xml";
            var mesh = loader.loadSceneFromFile(filePath).Meshes[0];
            mesh.AutoTransformEnable = false;
            Vegetation = new List<TgcMesh>();

            for (int i = 1; i < 4; i++)
            {
                var meshRight = mesh.clone($"Palmera-right{i}");
                var meshLeft = mesh.clone($"Palmera-left{i}");

                meshRight.Transform = 
                    TGCMatrix.Scaling(1, i*0.7f, 1) * TGCMatrix.Translation(0, 0, i * 200);
                Vegetation.Add(meshRight);

                meshLeft.Transform = 
                    TGCMatrix.RotationY(FastMath.PI) 
                    * TGCMatrix.Scaling(1, i * 0.7f, 1) 
                    * TGCMatrix.Translation(i*200, 0, 0);
                Vegetation.Add(meshLeft);
            }

            var font = new Font("verdana", 50);
            screenText = new TgcText2D();
            screenText.changeFont(font);
        }

        public void InitCamera()
        {
            var position = new TGCVector3(1000, 300, 1000);
            var lookAt = TGCVector3.Empty;
            Camara.SetCamera(position, lookAt);
        }

        public override void Init()
        {
            InitCoverPage();
            InitCamera();
        }
        #endregion

        #region Update
        public override void Update()
        {
            PreUpdate();

            PostUpdate();
        }
        #endregion

        #region Render
        public override void Render()
        {
            PreRender();

            // TODO: Hacer que sea responsive..
            screenText.drawText("Crash Bandicoot\n\n K: Nivel Canyon\n L: Nivel Isla", 400, 250, Color.OrangeRed);
            
            Floor.Render();
            LeftWall.Render();
            RightWall.Render();
            FrontWall.Render();
            foreach (var mesh in Vegetation)
            {
                mesh.Render();
            }

            PostRender();
        }
        #endregion

        #region Dispose
        public override void Dispose()
        {
            Floor.Dispose();
            LeftWall.Dispose();
            RightWall.Dispose();
            FrontWall.Dispose();
            foreach (var mesh in Vegetation)
            {
                mesh.Dispose();
            }
        }
        #endregion
    }
}
