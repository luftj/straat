using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace straat.Control
{
    enum InputCommand
    {
        EXIT,
        DOWN,
        UP,
        LEFT,
        RIGHT,
        SELECT,
    }

    class Input
    {
        public HashSet<InputCommand> commands { get; private set; }

        KeyboardState previousKBState;

        public Input()
        {
            commands = new HashSet<InputCommand>();
            previousKBState = Keyboard.GetState();
        }

        public void Update()
        {
            commands.Clear();
            handleKeyBoard(Keyboard.GetState());
        }

        public void handleKeyBoard(KeyboardState kbstate)
        {
            if (kbstate.IsKeyDown(Keys.Escape))
                 commands.Add(InputCommand.EXIT);

            if (kbstate.IsKeyDown(Keys.Down))
                commands.Add(InputCommand.DOWN);

            if (kbstate.IsKeyDown(Keys.Up))
                commands.Add(InputCommand.UP);

            if (kbstate.IsKeyDown(Keys.Left))
                commands.Add(InputCommand.LEFT);

            if (kbstate.IsKeyDown(Keys.Right))
                commands.Add(InputCommand.RIGHT);

            previousKBState = kbstate;
        }

        public bool pop(InputCommand command)
        {
            return commands.Remove(command);
        }

        public bool peek(InputCommand command)
        {
            return commands.Contains(command);
        }
    }
}
