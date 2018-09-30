using TGC.Core.Input;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Utils.Commands
{
    class ClickLeftCommand : Command
    {
        private IGameModel model;

        public ClickLeftCommand(IGameModel ctx)
        {
            model = ctx;
        }

        public void execute()
        {
            if (model.Input.buttonUp(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                model.BandicootCamera.SetCamera(model.BandicootCamera.Position + new TGCVector3(0, 10f, 0), model.BandicootCamera.LookAt);
            }
        }
    }
}
