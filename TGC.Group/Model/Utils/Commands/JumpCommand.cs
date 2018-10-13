using Microsoft.DirectX.DirectInput;

namespace TGC.Group.Model.Utils.Commands
{
    class JumpCommand : Command
    {
        private IGameModel model;

        public JumpCommand(IGameModel ctx)
        {
            model = ctx;
        }

        public void execute()
        {
            if (model.Input.keyPressed(Key.Space) && !model.IsJumping)
            {
                model.IsJumping = true;
                model.JumpDirection = 1;
                model.Physics.BandicootRigidBody.ApplyCentralImpulse(new Core.Mathematica.TGCVector3(0, 250, 0).ToBsVector);
            }
            else
                model.IsJumping = false;
        }
    }
}
