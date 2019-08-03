using System.Collections.Generic;
using Engine;
using Engine.Graphics._3D.Rendering;
using Engine.Interfaces;
using Engine.View;
using GmtkJam2019.Physics;
using GmtkJam2019.Sensors;

namespace GmtkJam2019.Entities.Core
{
	public class Scene : IDynamic
	{
		private List<HybridEntity> entities;

		public Scene()
		{
			entities = new List<HybridEntity>();

			Renderer = new MasterRenderer3D();
			Renderer.ShadowNearPlane = Properties.GetFloat("shadow.near.plane");
			Renderer.ShadowFarPlane = Properties.GetFloat("shadow.far.plane");
		}

		public Camera3D Camera { get; set; }
		public HybridSpace Space { get; set; }
		public HybridWorld World { get; set; }
		public MasterRenderer3D Renderer { get; }

		public void Add(HybridEntity entity)
		{
			entities.Add(entity);
			entity.Initialize(this);
		}

		public void Update(float dt)
		{
			entities.ForEach(e => e.Update(dt));

			for (int i = entities.Count - 1; i >= 0; i--)
			{
				if (entities[i].IsMarkedForDestruction)
				{
					entities.RemoveAt(i);
				}
			}
		}

		public void Draw()
		{
			Renderer.VpMatrix = Camera.ViewProjection;
			Renderer.Draw();
		}
	}
}
