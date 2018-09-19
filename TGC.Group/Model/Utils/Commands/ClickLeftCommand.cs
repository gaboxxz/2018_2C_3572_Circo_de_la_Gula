using TGC.Core.Input;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Utils.Commands
{
    class ClickLeftCommand : Command
    {
        private GameModelCanyon model;

        public ClickLeftCommand(GameModelCanyon model)
        {
            this.model = model;
        }

        public void execute()
        {
            if (model.Input.buttonUp(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                model.Camara.SetCamera(model.Camara.Position + new TGCVector3(0, 10f, 0), model.Camara.LookAt);
            }
        }
    }
}
