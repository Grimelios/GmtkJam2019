using System;
using System.Collections.Generic;
using Engine;
using Engine.Input.Data;
using Engine.Interfaces;
using Engine.Messaging;
using Engine.Utility;
using GlmSharp;

namespace GmtkJam2019.Entities
{
	public class PlayerController : IReceiver
	{
		private Player player;
		private PlayerData playerData;
		private PlayerControls controls;

		public PlayerController(Player player, PlayerData playerData)
		{
			this.player = player;

			controls = new PlayerControls();

			MessageSystem.Subscribe(this, CoreMessageTypes.Input, (messageType, data, dt) =>
			{
				ProcessInput((FullInputData)data, dt);
			});
		}

		public List<MessageHandle> MessageHandles { get; set; }

		public void Dispose()
		{
			MessageSystem.Unsubscribe(this);
		}

		private void ProcessInput(FullInputData data, float dt)
		{
			bool forward = data.Query(controls.RunForward, InputStates.Held);
			bool back = data.Query(controls.RunBack, InputStates.Held);
			bool left = data.Query(controls.StrafeLeft, InputStates.Held);
			bool right = data.Query(controls.StrafeRight, InputStates.Held);
			bool accelerationActive = false;

			vec2 acceleration = vec2.Zero;

			if (forward ^ back)
			{
				acceleration.y = forward ? 1 : -1;
				accelerationActive = true;
			}

			if (left ^ right)
			{
				acceleration.x = left ? -1 : 1;
				accelerationActive = true;
			}

			var body = player.Body;
			var velocity = body.Velocity.swizzle.xz;
			var v = body.Velocity.swizzle.xz;

			// Accelerate.
			if (accelerationActive)
			{
				float rotation = player.Rotation;
				float max = playerData.RunMaxSpeed;

				acceleration = Utilities.Normalize(acceleration);
				acceleration = Utilities.Rotate(acceleration, rotation);

				v += acceleration * playerData.RunAcceleration * dt;

				if (Utilities.LengthSquared(v) > max * max)
				{
					v = Utilities.Normalize(v) * max;
				}

				// This helps prevent an asymptotic effect where the player nudges closer and closer to the target
				// direction, but never hits it exactly.
				float angle = Utilities.Angle(v);

				if (Math.Abs(angle - rotation) < 0.1f)
				{
					float speed = Utilities.Length(v);

					v = Utilities.Direction(rotation) * speed;
				}
			}
			// Decelerate.
			else
			{
				int oldSign = Math.Sign(v.x != 0 ? v.x : v.y);

				v -= Utilities.Normalize(v) * playerData.RunDeceleration * dt;

				int newSign = Math.Sign(v.x != 0 ? v.x : v.y);

				if (oldSign != newSign)
				{
					v = vec2.Zero;
				}
			}

			body.Velocity = new vec3(v.x, velocity.y, v.y);
		}
	}
}
