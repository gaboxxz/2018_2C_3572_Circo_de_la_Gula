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
                model.BandicootCamera.OffsetHeight += 5;
                model.BandicootCamera.OffsetForward += 1;
            }
        }
    }
}
