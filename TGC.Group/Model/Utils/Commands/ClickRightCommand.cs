using TGC.Core.Input;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Utils.Commands
{
    class ClickRightCommand : Command
    {
        private IGameModel model;

        public ClickRightCommand(IGameModel ctx)
        {
            model = ctx;
        }

        public void execute()
        {
            if (model.Input.buttonUp(TgcD3dInput.MouseButtons.BUTTON_RIGHT))
            {
                model.BandicootCamera.OffsetHeight -= 5;
                model.BandicootCamera.OffsetForward -= 1;
            }
        }
    }
}
