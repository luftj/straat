using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace straat.Control
{
    public enum InputCommand
    {
        EXIT,
        DOWN,		// single keypress
		DOWN_CONT,	// continuous keypress
        UP,			// single keypress
		UP_CONT,	// continuous keypress
		LEFT_CONT,	// continuous keypress
		RIGHT_CONT,	// continuous keypress
        SELECT,
		A,
		// ...
		S,
		// ...
    }

	public enum PointerCommand
	{
		NONE,
		PRIMARY,
		SECONDARY,
		SCROLL,
	}

	public struct PointerEvent
	{
		public int X,Y;
		public PointerCommand Command;
		//public int scroll;

		public PointerEvent(int x, int y, PointerCommand command)
		{
			X = x;
			Y = y;
			Command = command;
		}
	}

    public class Input
    {
        public HashSet<InputCommand> commands { get; private set; }

        KeyboardState previousKBState;

		MouseState previousMState;
		public PointerEvent pointerEvent { get; private set; }

		Dictionary<Keys,InputCommand> keyBindings;

        public Input()
        {
            commands = new HashSet<InputCommand>();
            previousKBState = Keyboard.GetState();

			keyBindings = new Dictionary<Keys, InputCommand>();
			keyBindings.Add(Keys.Escape,InputCommand.EXIT);
			// TODO: key bindings
        }

        public void Update()
        {
            commands.Clear();
            handleKeyBoard(Keyboard.GetState());
			handleMouse(Mouse.GetState());
        }

        private void handleKeyBoard(KeyboardState kbstate)
        {
            if (kbstate.IsKeyDown(Keys.Escape))
                 commands.Add(InputCommand.EXIT);

            if (kbstate.IsKeyDown(Keys.Down))
                commands.Add(InputCommand.DOWN_CONT);

			if( kbstate.IsKeyDown( Keys.Down ) && previousKBState.IsKeyUp( Keys.Down ) )
				commands.Add( InputCommand.DOWN );

            if (kbstate.IsKeyDown(Keys.Up))
                commands.Add(InputCommand.UP_CONT);

			if( kbstate.IsKeyDown( Keys.Up ) && previousKBState.IsKeyUp( Keys.Up ) )
				commands.Add( InputCommand.UP );

            if (kbstate.IsKeyDown(Keys.Left))
                commands.Add(InputCommand.LEFT_CONT);

            if (kbstate.IsKeyDown(Keys.Right))
                commands.Add(InputCommand.RIGHT_CONT);

			if( kbstate.IsKeyDown( Keys.Enter ) )
				commands.Add( InputCommand.SELECT );

			if( kbstate.IsKeyDown( Keys.S ) )
				commands.Add( InputCommand.S );

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

		private void handleMouse(MouseState mstate)
		{
			PointerEvent cur = new PointerEvent();
			cur.X = mstate.Position.X;
			cur.Y = mstate.Position.Y;
			if( mstate.LeftButton == ButtonState.Pressed && previousMState.LeftButton == ButtonState.Released && mstate.RightButton == ButtonState.Released )
				cur.Command = PointerCommand.PRIMARY;
			else if( mstate.RightButton == ButtonState.Pressed && previousMState.LeftButton == ButtonState.Released && mstate.LeftButton == ButtonState.Released )
				cur.Command = PointerCommand.SECONDARY;
			else
				cur.Command = PointerCommand.NONE;

			//if(mstate.ScrollWheelValue > previousMState.ScrollWheelValue)

			// todo: distinguish continuous key presses?

			// TODO: scroll wheel

			pointerEvent = cur;
				

			previousMState = mstate;
		}
    }
}
