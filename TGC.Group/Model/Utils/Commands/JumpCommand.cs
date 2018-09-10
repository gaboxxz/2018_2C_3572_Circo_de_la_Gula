using Microsoft.DirectX.DirectInput;

namespace TGC.Group.Model.Utils.Commands
{
    class JumpCommand : Command
    {
        private GameModel model;

        public JumpCommand(GameModel model)
        {
            this.model = model;
        }

        public void execute()
        {
            if (model.Input.keyPressed(Key.Space) && !model.isJumping)
            {
                model.isJumping = true;
                model.jumpDirection = 1;
            }
        }
    }
}
