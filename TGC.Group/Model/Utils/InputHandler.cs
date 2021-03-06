﻿using System.Collections.Generic;
using Microsoft.DirectX.DirectInput;
using TGC.Core.Example;
using TGC.Core.Input;
using TGC.Group.Model.Utils.Commands;

namespace TGC.Group.Model.Utils
{
    /* Esta clase se encarga de manejar las entradas de usuario y ejecutar el comando correspondiente.
    *  Utilizamos el Patron de diseño Command.
    */
    class InputHandler
    {
        private Dictionary<Key, Command> commands = new Dictionary<Key, Command>();

        // Utiliza el modelo y bindea un comando a un evento.
        public InputHandler(IGameModel model)
        {
            commands[Key.F] = new ShowBoundingBoxCommand(model);
            commands[Key.Left] = new MoveLeftCommand(model);
            commands[Key.A] = commands[Key.Left];
            commands[Key.Right] = new MoveRightCommand(model);
            commands[Key.D] = commands[Key.Right];
            commands[Key.Up] = new MoveUpCommand(model);
            commands[Key.W] = commands[Key.Up];
            commands[Key.Down] = new MoveDownCommand(model);
            commands[Key.S] = commands[Key.Down];
            commands[Key.Space] = new JumpCommand(model);
            commands[(Key) TgcD3dInput.MouseButtons.BUTTON_LEFT] = new ClickLeftCommand(model);
            commands[(Key) TgcD3dInput.MouseButtons.BUTTON_RIGHT] = new ClickRightCommand(model);
        }

        public void HandleInput(Key key)
        {
            Command command = commands[key];

            if (command != null) {
                command.execute();
            }
        }
    }
}
