using Microsoft.DirectX.DirectInput;

namespace TGC.Group.Model.Utils.Commands
{
    class MoveDownCommand : Command
    {
        private GameModelCanyon model;

        public MoveDownCommand(GameModelCanyon model)
        {
            this.model = model;
        }

        public void execute()
        {
            if (model.Input.keyDown(Key.Down) || model.Input.keyDown(Key.S))
            {
                model.bandicootMovement.Z = 1;
            }
        }
    }
}
