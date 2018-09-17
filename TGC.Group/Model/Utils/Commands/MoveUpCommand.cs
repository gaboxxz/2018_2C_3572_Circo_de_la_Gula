﻿using Microsoft.DirectX.DirectInput;

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
            if (model.Input.keyDown(Key.Up))
            {
                model.bandicootMovement.Z = -1;
            }
        }
    }
}