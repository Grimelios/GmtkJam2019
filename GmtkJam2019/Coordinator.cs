using System.Collections.Generic;
using Engine.Input.Data;
using Engine.Interfaces;
using Engine.Messaging;
using Engine.Timing;
using Engine.UI;
using GlmSharp;
using GmtkJam2019.Entities;
using GmtkJam2019.UI;
using GmtkJam2019.UI.Speech;

namespace GmtkJam2019
{
	public class Coordinator : IDynamic, IReceiver
	{
		private Canvas canvas;
		private Player player;
		private RepeatingTimer timer;
		private SpeechDisplay[] speechBoxes;

		private int eventIndex;

		public Coordinator(Canvas canvas, Player player)
		{
			this.canvas = canvas;
			this.player = player;

			speechBoxes = new SpeechDisplay[2];

			float[] durations =
			{
				1.4f,
				2.6f,
				1.3f,
				5.2f
			};

			string[] lines =
			{
				"The aliens are attacking from all sides, soldier.",
				"Keep your eyes open."
			};

			timer = new RepeatingTimer(progress =>
			{
				switch (eventIndex)
				{
					// Trigger two speech lines.
					case 0:
					case 1:
						TriggerLine(lines[eventIndex], 210 + eventIndex * 50);
						timer.Duration = durations[eventIndex + 1];

						break;

					// Trigger the screen transition (fading from black), unlock the player, and update the canvas.
					case 2:
						canvas.GetElement<FullscreenFade>().BeginFade();
						canvas.GetElement<PlayerVisionDisplay>().Visible = true;
						canvas.Add(new Reticle());

						player.InputLocked = false;

						break;

					// Trigger both speech boxes to fade out.
					case 3:
						foreach (var box in speechBoxes)
						{
							box.BeginFade();
						}

						break;
				}
						
				return ++eventIndex < 4;
			}, durations[0]);

			timer.Repeatable = true;
			timer.Paused = true;

			MessageSystem.Subscribe(this, CoreMessageTypes.Keyboard, (messageType, data, dt) =>
			{
				var kbData = (KeyboardData)data;

				if (kbData.Query(Engine.GLFW.GLFW_KEY_P, InputStates.PressedThisFrame))
				{
					timer.Paused = false;
				}
			});
		}

		private void TriggerLine(string text, int offset)
		{
			var speechBox = new SpeechDisplay();
			speechBox.Offset = new ivec2(0, offset);
			speechBoxes[eventIndex] = speechBox;

			// Adding the speech box positions it properly on the screen (required before refreshing text).
			canvas.Add(speechBox);
			speechBox.Refresh(text);
		}

		public void Update(float dt)
		{
			timer.Update(dt);
		}

		public void Dispose()
		{
		}

		public List<MessageHandle> MessageHandles { get; set; }
	}
}
