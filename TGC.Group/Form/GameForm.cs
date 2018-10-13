using System;
using System.Threading;
using System.Windows.Forms;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Input;
using TGC.Core.Shaders;
using TGC.Core.Sound;
using TGC.Core.Textures;
using TGC.Group.Model;

namespace TGC.Group.Form
{
    /// <summary>
    ///     GameForm es el formulario de entrada, el mismo invocara a nuestro modelo que extiende TgcExample, 
    ///     e inicia el render loop.
    /// </summary>
    public partial class GameForm : System.Windows.Forms.Form
    {
        /// <summary>
        ///     Constructor de la ventana.
        /// </summary>
        public GameForm()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Ejemplo del juego a correr
        /// </summary>
        private TgcExample Model { get; set; }

        /// <summary>
        ///     Obtener o parar el estado del RenderLoop.
        /// </summary>
        private bool ApplicationRunning { get; set; }

        /// <summary>
        ///     Permite manejar el sonido.
        /// </summary>
        private TgcDirectSound DirectSound { get; set; }

        /// <summary>
        ///     Permite manejar los inputs de la computadora.
        /// </summary>
        private TgcD3dInput Input { get; set; }

        private void GameForm_Load(object sender, EventArgs e)
        {
            //Iniciar graficos.
            InitGraphics();

            //Titulo de la ventana principal.
            Text = "CDLG - Crash Bandicoot";

            //Focus panel3D.
            panel3D.Focus();

            //Inicio el ciclo de Render.
            InitRenderLoop();
        }

        private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ApplicationRunning)
            {
                ShutDown();
            }
        }

        /// <summary>
        ///     Inicio todos los objetos necesarios para cargar el ejemplo y directx.
        /// </summary>
        public void InitGraphics()
        {
            //Se inicio la aplicación
            ApplicationRunning = true;

            //Inicio Device
            D3DDevice.Instance.InitializeD3DDevice(panel3D);

            //Inicio inputs
            Input = new TgcD3dInput();
            Input.Initialize(this, panel3D);

            //Inicio sonido
            DirectSound = new TgcDirectSound();
            DirectSound.InitializeD3DDevice(panel3D);

            //Directorio actual de ejecución
            var currentDirectory = Environment.CurrentDirectory + "\\";

            //Cargar shaders del framework
            TgcShaders.Instance.loadCommonShaders(currentDirectory + Game.Default.ShadersDirectory);

            //Juego a ejecutar, si quisiéramos tener diferentes modelos aquí podemos cambiar la instancia e invocar a otra clase.
            //Model = new GameModel(currentDirectory + Game.Default.MediaDirectory,
            //    currentDirectory + Game.Default.ShadersDirectory);

            //Cargar juego.
            //ExecuteModel();
        }

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

        /// <summary>
        ///     Indica si la aplicacion esta activa.
        ///     Busca si la ventana principal tiene foco o si alguna de sus hijas tiene.
        /// </summary>
        public bool ApplicationActive()
        {
            if (ContainsFocus)
            {
                return true;
            }

            foreach (var form in OwnedForms)
            {
                if (form.ContainsFocus)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Arranca a ejecutar un ejemplo.
        ///     Para el ejemplo anterior, si hay alguno.
        /// </summary>
        public void ExecuteModel()
        {
            //Ejecutar Init
            try
            {
                Model.ResetDefaultConfig();
                Model.DirectSound = DirectSound;
                Model.Input = Input;
                Model.Init();
                panel3D.Focus();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error en Init() del juego", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        private void ButtonCanyon_Click(object sender, EventArgs e)
        {
            string mediaDir = $"{Environment.CurrentDirectory}\\{Game.Default.MediaDirectory}";
            string shadersDir = $"{Environment.CurrentDirectory}\\{Game.Default.ShadersDirectory}";

            Model = new GameModelCanyon(mediaDir, shadersDir);
            ExecuteModel();
            button1.Hide();
            buttonCanyon.Hide();
            buttonIsland.Hide();
        }

        private void ButtonIsland_Click(object sender, EventArgs e)
        {
            string mediaDir = $"{Environment.CurrentDirectory}\\{Game.Default.MediaDirectory}";
            string shadersDir = $"{Environment.CurrentDirectory}\\{Game.Default.ShadersDirectory}";

            Model = new GameModelIsla(mediaDir, shadersDir);
            ExecuteModel();
            button1.Hide();
            buttonCanyon.Hide();
            buttonIsland.Hide();
        }

        /*private void button1_Click(object sender, EventArgs e)
        {
            string mediaDir = $"{Environment.CurrentDirectory}\\{Game.Default.MediaDirectory}";
            string shadersDir = $"{Environment.CurrentDirectory}\\{Game.Default.ShadersDirectory}";

            Model = new GameModelLevel1(mediaDir, shadersDir);
            ExecuteModel();

            button1.Hide();
            buttonCanyon.Hide();
            buttonIsland.Hide();

        }*/
    }
}