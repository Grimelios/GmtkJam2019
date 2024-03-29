﻿using Engine.Animation;
using Engine.Graphics._3D;
using Engine.Interfaces._3D;
using GlmSharp;

namespace Engine.Core._3D
{
	public class Model : IRenderable3D, IScalable3D
	{
		public Model(string filename) : this(ContentCache.GetMesh(filename))
		{
		}

		public Model(Mesh mesh)
		{
			Mesh = mesh;
			Scale = vec3.Ones;
			Orientation = quat.Identity;
			IsShadowCaster = true;
		}

		public Mesh Mesh { get; }
		public mat4 WorldMatrix { get; private set; }
		public vec3 Position { get; set; }
		public quat Orientation { get; set; }
		public vec3 Scale { get; set; }		

		public bool IsShadowCaster { get; set; }

		public void SetTransform(vec3 position, quat orientation)
		{
			Position = position;
			Orientation = orientation;
		}

		public void RecomputeWorldMatrix()
		{
			WorldMatrix = mat4.Translate(Position) * Orientation.ToMat4 * mat4.Scale(Scale);
		}
	}
}
