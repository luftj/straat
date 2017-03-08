using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using straat.Model.Map;
using straat.View;

namespace straat
{
	public class SceneRenderer
	{
		GraphicsDevice graphicsDevice;

		public Camera camera;

		Matrix world;
		Matrix view;
		Matrix projection;

		Effect effect;
		EffectParameter worldParameter;
		EffectParameter viewParameter;
		EffectParameter projectionParameter;
		EffectParameter AmbientColorParam;
		EffectParameter AmbientIntensityParam;

		int numTris = 0;
		VertexPositionColor[] vertices; // "mesh" from map

		public SceneRenderer(Game1 game, Effect effect)
		{
			graphicsDevice = game.GraphicsDevice;

			world = Matrix.Identity;

			this.effect = effect;
			effect.CurrentTechnique = effect.Techniques["AmbientLight"];
			worldParameter = effect.Parameters["World"];
			viewParameter = effect.Parameters["View"];
			projectionParameter = effect.Parameters["Projection"];
			AmbientColorParam = effect.Parameters["AmbientColor"];
			AmbientIntensityParam = effect.Parameters["AmbientIntensity"];

			AmbientColorParam.SetValue(new Vector4(1.0f, 1.0f, 1.0f, 1.0f));

			AmbientIntensityParam.SetValue(0.7f);

            graphicsDevice.RasterizerState = RasterizerState.CullNone;
		}

		public void getVertices(Map map)
		{
			numTris = 0;
			var vertices = new List<VertexPositionColor>();

			foreach(Corner c in map.corners.Values)
			{
				if(c.touches.Count != 3) continue;

				foreach(var v in c.touches)
				{
					vertices.Add(new VertexPositionColor(v.position3f,Color.White));
				}
				++numTris;
			}
			this.vertices = vertices.ToArray();
		}

		public void Update(double deltaT)
		{
			Vector3 position = new Vector3(camera.position, camera.height);
			Vector3 target = new Vector3(camera.position+Vector2.UnitX+Vector2.UnitY, 0.0f);
			view = Matrix.CreateLookAt(position, target, Vector3.UnitZ);
			projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f),
															 camera.aspectRatio,
															 1.0f,
															 100000.0f);//MAGIC_NUMBER: think of something useful for clipping


			worldParameter.SetValue(world);
			viewParameter.SetValue(view);
			projectionParameter.SetValue(projection);
		}

		public void Draw(double deltaT)
		{
			RasterizerState rs = new RasterizerState();
			rs.CullMode = CullMode.None;
			graphicsDevice.RasterizerState = rs;

			for(int i = 0; i < effect.CurrentTechnique.Passes.Count;++i)
			{
				effect.CurrentTechnique.Passes[i].Apply();

				graphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList,
				                                                  vertices,0,numTris,
				                                                  VertexPositionColor.VertexDeclaration);
			}
		}
	}
}
