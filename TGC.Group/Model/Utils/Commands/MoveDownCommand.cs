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
                model.bandicootMovement.X = (-1) * Core.Mathematica.FastMath.Sin(model.anguloDirector);
                model.bandicootMovement.Z = (-1) * Core.Mathematica.FastMath.Cos(model.anguloDirector);
                model.bandicootMovement.Y = 0;

                model.banditcamara.OffsetForward -= 1 * model.ElapsedTime;
                model.banditcamara.Target = model.Bandicoot.Position;
            }
        }
    }
}
