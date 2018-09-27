using Microsoft.DirectX.DirectInput;

namespace TGC.Group.Model.Utils.Commands
{
    class MoveUpCommand : Command
    {
        private GameModelCanyon model;

        public MoveUpCommand(GameModelCanyon model)
        {
            this.model = model;
        }

        public void execute()
        {
            if (model.Input.keyDown(Key.Up) || model.Input.keyDown(Key.W))
            {
                model.bandicootMovement.X = Core.Mathematica.FastMath.Sin(model.anguloDirector);
                model.bandicootMovement.Z = Core.Mathematica.FastMath.Cos(model.anguloDirector);
                model.bandicootMovement.Y = 0;

                model.banditcamara.OffsetForward -= 1*model.ElapsedTime;
                model.banditcamara.Target = model.Bandicoot.Position;
            }
        }
    }
}
