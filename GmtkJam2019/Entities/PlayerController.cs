using System;
using System.Collections.Generic;
using Engine;
using Engine.Input.Data;
using Engine.Interfaces;
using Engine.Messaging;
using Engine.Utility;
using Engine.View;
using GlmSharp;
using GmtkJam2019.Settings;

namespace GmtkJam2019.Entities
{
	public class PlayerController : IReceiver
	{
		// The aim divisor is an arbitrary value used to make mouse sensitivity more reasonable.
		private const float AimDivisor = 10000f;

		private Player player;
		private PlayerData playerData;
		private PlayerControls controls;
		private Camera3D camera;

		private float pitch;
		private float yaw;

		// This value is stored for use in firing weapons.
		private vec3 aim;

		public PlayerController(Player player, PlayerData playerData, Camera3D camera)
		{
			this.player = player;
			this.playerData = playerData;
			this.camera = camera;

			controls = new PlayerControls();
			InputLocked = true;

			MessageSystem.Subscribe(this, CoreMessageTypes.Input, (messageType, data, dt) =>
			{
				ProcessInput((FullInputData)data, dt);
			});
		}

		public GameSettings Settings { get; set; }
		public List<MessageHandle> MessageHandles { get; set; }

		public bool InputLocked { get; set; }

		public void Dispose()
		{
			MessageSystem.Unsubscribe(this);
		}

		private void ProcessInput(FullInputData data, float dt)
		{
			if (InputLocked)
			{
				return;
			}

			ProcessAim((MouseData)data.GetData(InputTypes.Mouse));
			ProcessRunning(data, dt);
			ProcessAttack(data);
		}

		private void ProcessAim(MouseData data)
		{
			ivec2 delta = data.Location - data.PreviousLocation;

			if (Settings.InvertX)
			{
				delta.x *= -1;
			}

			if (Settings.InvertY)
			{
				delta.y *= -1;
			}

			var sensitivity = Settings.MouseSensitivity / AimDivisor;

			pitch += delta.y * sensitivity;
			pitch = Utilities.Clamp(pitch, -playerData.MaxPitch, playerData.MaxPitch);
			yaw += delta.x * sensitivity;

			if (yaw >= Constants.TwoPi)
			{
				yaw -= Constants.TwoPi;
			}
			else if (yaw <= -Constants.TwoPi)
			{
				yaw += Constants.TwoPi;
			}

			camera.Orientation = quat.FromAxisAngle(pitch, vec3.UnitX) * quat.FromAxisAngle(yaw, vec3.UnitY);
			player.Rotation = yaw;
			aim = -vec3.UnitZ * camera.Orientation;
		}

		private void ProcessRunning(FullInputData data, float dt)
		{
			bool forward = data.Query(controls.RunForward, InputStates.Held);
			bool back = data.Query(controls.RunBack, InputStates.Held);
			bool left = data.Query(controls.StrafeLeft, InputStates.Held);
			bool right = data.Query(controls.StrafeRight, InputStates.Held);

			vec2 acceleration = vec2.Zero;

			if (forward ^ back)
			{
				acceleration.y = forward ? -1 : 1;
			}

			if (left ^ right)
			{
				acceleration.x = left ? -1 : 1;
			}

			var body = player.ControllingBody;
			var velocity = body.Velocity;
			var v = velocity.swizzle.xz;

			// Accelerate.
			if (acceleration != vec2.Zero)
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

		private void ProcessAttack(FullInputData data)
		{
			var weapon = player.Weapon;

			if (data.Query(controls.Attack, InputStates.PressedThisFrame))
			{
				weapon.PrimaryFire(player.Scene, aim);
			}
		}
	}
}
