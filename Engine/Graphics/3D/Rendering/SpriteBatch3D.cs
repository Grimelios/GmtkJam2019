using Engine.Core._3D;
using Engine.Lighting;
using Engine.Shaders;
using GlmSharp;
using static Engine.GL;

namespace Engine.Graphics._3D.Rendering
{
	public class SpriteBatch3D : MapRenderer3D<uint, Sprite3D>
	{
		public unsafe SpriteBatch3D(GlobalLight light) : base(light)
		{
			uint bufferId;
			
			glGenBuffers(1, &bufferId);

			var shader = new Shader();
			shader.Attach(ShaderTypes.Vertex, "Sprite3D.vert");
			shader.Attach(ShaderTypes.Fragment, "Sprite3D.frag");
			shader.AddAttribute<float>(3, GL_FLOAT);
			shader.AddAttribute<float>(2, GL_FLOAT);
			shader.Initialize();
			shader.Use();
			shader.SetUniform("shadowSampler", 0);
			shader.SetUniform("textureSampler", 1);

			Bind(shader, bufferId);

			vec2[] points =
			{
				new vec2(-1, -1),
				new vec2(1, -1),
				new vec2(1, 1),
				new vec2(-1, 1)
			};

			float[] data = new float[8];

			for (int i = 0; i < points.Length; i++)
			{
				var p = points[i];

				data[i * 2] = p.x;
				data[i * 2 + 1] = p.y;
			}

			glBindBuffer(GL_ARRAY_BUFFER, bufferId);

			fixed (float* address = &data[0])
			{
				glBufferData(GL_ARRAY_BUFFER, sizeof(float) * 8, address, GL_STATIC_DRAW);
			}
		}

		public override void Add(Sprite3D sprite)
		{
			Add(sprite.Source.Id, sprite);
		}

		public override void Remove(Sprite3D sprite)
		{
			Remove(sprite.Source.Id, sprite);
		}

		public override void PrepareShadow()
		{
			glDisable(GL_CULL_FACE);

			base.PrepareShadow();
		}

		public override void Prepare()
		{
			glDisable(GL_CULL_FACE);
		}

		protected override void Apply(uint key)
		{
			// For 3D sprites, the key is the source ID.
			glBindTexture(GL_TEXTURE0, key);
		}

		public override void Draw(Sprite3D item, mat4? vp)
		{
			PrepareShader(item, vp);

			glDrawArrays(GL_TRIANGLE_STRIP, 0, 4);
		}
	}
}
