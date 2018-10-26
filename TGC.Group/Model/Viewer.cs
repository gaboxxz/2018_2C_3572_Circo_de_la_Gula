using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Input;
using TGC.Core.Shaders;
using TGC.Core.Sound;
using TGC.Core.Text;
using TGC.Core.Textures;
using TGC.Group.Form;
using static TGC.Core.Sound.TgcMp3Player;

namespace TGC.Group.Model
{
    // La clase Viewer es el componente que gestiona la carga de los modelos en el GameForm.
    class Viewer
    {
        private GameForm Form { get; set; }
        public bool ApplicationRunning { get; set; }
        public TgcExample Model { get; set; }
        public TgcD3dInput Input { get; set; }
        public TgcDirectSound DirectSound { get; set; }
        private TgcMp3Player mp3Player;
        public Panel Panel3D { get; set; }
        private string mediaDir;
        private string shadersDir;
        
        public Viewer(GameForm ctx, Panel panel)
        {
            Form = ctx;
            Panel3D = panel;
        }

        public void Run()
        {
            mediaDir = $"{Environment.CurrentDirectory}\\{Game.Default.MediaDirectory}";
            shadersDir = $"{Environment.CurrentDirectory}\\{Game.Default.ShadersDirectory}";

            InitSoundtrack();

            InitGraphics();

            Panel3D.Focus();

            InitRenderLoop();
        }

        #region Init Graphics
        /// <summary>
        ///     Inicio todos los objetos necesarios para cargar el ejemplo y directx.
        /// </summary>
        public void InitGraphics()
        {
            //Se inicio la aplicación
            ApplicationRunning = true;

            //Inicio Device
            D3DDevice.Instance.InitializeD3DDevice(Panel3D);
            Form.Text = "CDLG - Crash Bandicoot";

            //Inicio inputs
            Input = new TgcD3dInput();
            Input.Initialize(Form, Panel3D);

            //Inicio sonido
            DirectSound = new TgcDirectSound();
            DirectSound.InitializeD3DDevice(Panel3D);

            //Cargar shaders del framework
            TgcShaders.Instance.loadCommonShaders(shadersDir);

            Model = new GameModelMenu(mediaDir, shadersDir);
            ExecuteModel();
        }
        #endregion

        #region Init Soundtrack
        private void InitSoundtrack()
        {
            mp3Player = new TgcMp3Player
            {
                FileName = $"{Game.Default.MediaDirectory}\\Soundtracks\\01-naughty-dog-logo.mp3"
            };
            //mp3Player.play(true);
        }
        #endregion

        #region Init Render Loop
        /// <summary>
        ///     Comienzo el loop del juego.
        /// </summary>
        public void InitRenderLoop()
        {
            while (ApplicationRunning)
            {
                //Renderizo si es que hay un ejemplo activo.
                if (Model != null)
                {
                    //Solo renderizamos si la aplicacion tiene foco, para no consumir recursos innecesarios.
                    if (ApplicationActive())
                    {
                        /* Momentamenteamente se puede switchear entre niveles 
                           apretando la tecla K o L. Lo dejo como ejemplo ahora pero luego 
                           hare una funcion que encapsule esta logica. Si quieren hacerla ustedes 
                           sientanse libres de hacerlo :). */
                        if(Input.keyPressed(Key.K))
                        {
                            StopCurrentExample();
                            Model = new GameModelCanyon(mediaDir, shadersDir);
                            ExecuteModel();
                        }

                        if (Input.keyPressed(Key.L))
                        {
                            StopCurrentExample();
                            Model = new GameModelIsla(mediaDir, shadersDir);
                            ExecuteModel();
                        }

                        Model.Update();
                        Model.Render();
                    }
                    else
                    {
                        //Si no tenemos el foco, dormir cada tanto para no consumir gran cantidad de CPU.
                        Thread.Sleep(100);
                    }
                }
                // Process application messages.
                Application.DoEvents();
            }
        }
        #endregion


        /// <summary>
        ///     Ejecuta un modelo.
        /// </summary>
        public void ExecuteModel()
        {
            if (mp3Player.getStatus() == States.Playing)
            {
                mp3Player.closeFile();
            }

            //Ejecutar Init
            try
            {
                Model.ResetDefaultConfig();
                Model.DirectSound = DirectSound;
                Model.Input = Input;
                Model.Init();
                Panel3D.Focus();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error en Init() del juego", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        ///     Indica si la aplicacion esta activa.
        ///     Busca si la ventana principal tiene foco o si alguna de sus hijas tiene.
        /// </summary>
        public bool ApplicationActive()
        {
            if (Form.ContainsFocus)
            {
                return true;
            }

            foreach (var form in Form.OwnedForms)
            {
                if (form.ContainsFocus)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Deja de ejecutar el ejemplo actual
        /// </summary>
        public void StopCurrentExample()
        {
            if (Model != null)
            {
                Model.Dispose();
                Model = null;
            }
        }

        /// <summary>
        ///     Finalizar aplicacion
        /// </summary>
        public void ShutDown()
        {
            ApplicationRunning = false;

            StopCurrentExample();

            //Liberar Device al finalizar la aplicacion
            D3DDevice.Instance.Dispose();
            TexturesPool.Instance.clearAll();
        }
    }
}
