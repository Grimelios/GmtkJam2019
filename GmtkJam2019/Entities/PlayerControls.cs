using System.Collections.Generic;
using Engine.Input.Data;
using static Engine.GLFW;

namespace GmtkJam2019.Entities
{
	public class PlayerControls
	{
		public PlayerControls()
		{
			RunForward = new List<InputBind>
			{
				new InputBind(InputTypes.Keyboard, GLFW_KEY_W),
				new InputBind(InputTypes.Keyboard, GLFW_KEY_UP)
			};

			RunBack = new List<InputBind>
			{
				new InputBind(InputTypes.Keyboard, GLFW_KEY_S),
				new InputBind(InputTypes.Keyboard, GLFW_KEY_DOWN)
			};

			StrafeLeft = new List<InputBind>
			{
				new InputBind(InputTypes.Keyboard, GLFW_KEY_A),
				new InputBind(InputTypes.Keyboard, GLFW_KEY_LEFT)
			};

			StrafeRight = new List<InputBind>
			{
				new InputBind(InputTypes.Keyboard, GLFW_KEY_D),
				new InputBind(InputTypes.Keyboard, GLFW_KEY_RIGHT)
			};

			Jump = new List<InputBind>
			{
				new InputBind(InputTypes.Keyboard, GLFW_KEY_SPACE)
			};

			Attack = new List<InputBind>
			{
				new InputBind(InputTypes.Mouse, GLFW_MOUSE_BUTTON_LEFT)
			};
		}

		public List<InputBind> RunForward { get; }
		public List<InputBind> RunBack { get; }
		public List<InputBind> StrafeLeft { get; }
		public List<InputBind> StrafeRight { get; }
		public List<InputBind> Jump { get; }
		public List<InputBind> Attack { get; }
	}
}
