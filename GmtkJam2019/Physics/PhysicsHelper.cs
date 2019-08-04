using System;
using System.Collections.Generic;
using Engine;
using Engine.Core._3D;
using Engine.Graphics;
using Engine.Graphics._3D;
using Engine.Utility;
using GlmSharp;
using GmtkJam2019.Entities.Core;
using GmtkJam2019.Entities.Enemies;
using GmtkJam2019.Interfaces;

namespace GmtkJam2019.Physics
{
	public static class PhysicsHelper
	{
		private const float Epsilon = 0.00001f;

		public static RaycastResults Raycast(vec3 origin, vec3 direction, float range, Scene scene)
		{
			vec3 closestHit = vec3.MaxValue;
			vec3 closestNormal = vec3.Zero;

			bool hitFound = CheckMesh(scene.WorldMesh, origin, direction, range, ref closestHit, ref closestNormal);

			ITargetable target;

			// The returned entity (null or not) determines whether a hit occurred.
			if ((target = CheckEntities(scene.Targets, origin, direction, range, ref closestHit, ref closestNormal,
				out bool isHeadshot)) != null)
			{
				hitFound = true;
			}

			return hitFound ? new RaycastResults(closestHit, closestNormal, target, isHeadshot) : null;
		}

		private static bool CheckMesh(Mesh mesh, vec3 origin, vec3 direction, float range, ref vec3 closestHit,
			ref vec3 closestNormal)
		{
			var points = mesh.Points;
			var normals = mesh.Normals;
			var vertices = mesh.Vertices;
			var indices = mesh.Indices;

			vec3 ray = direction * range;
			vec3 currentHit = vec3.Zero;

			bool hitFound = false;

			// This is a naive algorithm that simply loops through every triangle of the world mesh (rather than
			// optimizing using spatial partitioning).
			for (int i = 0; i < indices.Length; i += 3)
			{
				// See https://en.wikipedia.org/wiki/M%C3%B6ller%E2%80%93Trumbore_intersection_algorithm.
				var v0 = vertices[indices[i]];
				var v1 = vertices[indices[i + 1]];
				var v2 = vertices[indices[i + 2]];
				var p0 = points[v0.x];
				var p1 = points[v1.x];
				var p2 = points[v2.x];

				if (SolveTriangle(p0, p1, p2, origin, ray, ref currentHit))
				{
					if (Utilities.LengthSquared(currentHit) < Utilities.LengthSquared(closestHit))
					{
						closestHit = currentHit;
						closestNormal = normals[v0.z];
					}

					hitFound = true;
				}
			}

			return hitFound;
		}

		private static bool SolveTriangle(vec3 p0, vec3 p1, vec3 p2, vec3 origin, vec3 ray, ref vec3 hitPosition)
		{
			vec3 edge1 = p1 - p0;
			vec3 edge2 = p2 - p0;
			vec3 h = Utilities.Cross(ray, edge2);

			float a = Utilities.Dot(edge1, h);

			if (a > -Epsilon && a < Epsilon)
			{
				// The ray is parallel to the triangle.
				return false;
			}

			vec3 s = origin - p0;

			float f = 1 / a;
			float u = f * Utilities.Dot(s, h);

			if (u < 0 || u > 1)
			{
				return false;
			}

			vec3 q = Utilities.Cross(s, edge1);

			float v = f * Utilities.Dot(ray, q);

			if (v < 0 || u + v > 1)
			{
				return false;
			}

			// At this point, the intersection point can be computed.
			float t = f * Utilities.Dot(edge2, q);

			if (t <= Epsilon)
			{
				// This means there's a line intersection, but not a ray intersection.
				return false;
			}

			hitPosition = origin + ray * t;

			return true;
		}

		private static ITargetable CheckEntities(List<ITargetable> targets, vec3 origin, vec3 direction, float range,
			ref vec3 closestHit, ref vec3 closestNormal, out bool isHeadshot)
		{
			vec3 currentHit = vec3.Zero;

			ITargetable targetHit = null;

			isHeadshot = false;

			foreach (var target in targets)
			{
				if (SolveTarget(target, origin, direction, range, ref currentHit, out vec3 normal,
					out bool localIsHeadshot))
				{
					if (Utilities.LengthSquared(currentHit) < Utilities.LengthSquared(closestHit))
					{
						closestHit = currentHit;
						closestNormal = normal;
						targetHit = target;
						isHeadshot = localIsHeadshot;
					}
				}
			}

			return targetHit;
		}

		private static bool SolveTarget(ITargetable target, vec3 origin, vec3 direction, float range,
			ref vec3 hitPosition, out vec3 normal, out bool isHeadshot)
		{
			isHeadshot = false;

			float r = target.Rotation;

			// All hitscan targets are vertical, billboarded sprites, so some calculations can be simplified.
			vec2 v = Utilities.Direction(r);
			normal = -new vec3(v.x, 0, v.y);

			if (Utilities.Dot(normal, direction) <= Epsilon)
			{
				// This means that the shot is aiming away from the target plane (or parallel).
				return false;
			}

			// See http://geomalgorithms.com/a05-_intersect-1.html.
			vec3 p = target.Position;
			vec3 end = origin + direction * range;
			vec3 u = end - origin;
			vec3 w = origin - p;

			float d = Utilities.Dot(normal, u);
			float n = -Utilities.Dot(normal, w);

			// By this point, the ray is already guaranteed to be aiming towards the plane.
			float s = n / d;

			if (s < 0 || s > 1)
			{
				// No interesection.
				return false;
			}

			vec3 i = origin + s * u;

			// The ray is now known to intersect the plane, but it needs to be further checked for the actual sprite
			// bounds.
			vec3 a = (i - p) * quat.FromAxisAngle(-(r - Constants.PiOverTwo), vec3.UnitY);
			vec2 b = a.swizzle.xy;
			vec2 bounds = target.CollisionBounds;

			if (Math.Abs(b.x) > bounds.x / 2 || Math.Abs(b.y) > bounds.y / 2)
			{
				// The ray hit the plane, but not within the bounds of the sprite.
				return false;
			}

			var texture = target.CollisionTexture;

			ivec2 coords = (ivec2)(b * Sprite3D.PixelDivisor / target.Scale) +
				new ivec2(texture.Width, texture.Height) / 2;
			coords.y = texture.Height - coords.y;

			var pixel = texture.Data[coords.y * texture.Width + coords.x];
			var alpha = BitConverter.GetBytes(pixel)[3];

			// Only non-transparent parts of the texture can be hit.
			if (alpha == 0)
			{
				return false;
			}

			// Only enemies can be headshot.
			if (target is Enemy enemy)
			{
				var data = enemy.Data;
				var squared = Utilities.DistanceSquared(coords, new vec2(data.HeadshotX, data.HeadshotY));

				isHeadshot = squared <= data.HeadshotRadius * data.HeadshotRadius;
			}

			hitPosition = i;
			normal *= -1;

			return true;
		}
	}
}
