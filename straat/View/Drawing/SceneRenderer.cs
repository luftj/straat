using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using straat.Model.Map;
using straat.View;

namespace straat.View.Drawing
{
	public class SceneRenderer
	{
		struct VertexPositionColorNormal
		{
			private Vector3 position;
			private Vector4 color;
			private Vector4 normal;

			public VertexPositionColorNormal(Vector3 position, Vector4 color, Vector4 normal)
			{
				this.position = position;
				this.color = color;
				this.normal = normal;
			}

			public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration(
				new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
				new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector4, VertexElementUsage.Color, 0),
				new VertexElement(sizeof(float) * (3+4), VertexElementFormat.Vector4, VertexElementUsage.Normal, 0)
			);
		}

		GraphicsDevice graphicsDevice;

		public Camera camera;

		Matrix world;
		Matrix view;
		Matrix projection;

		Effect effect;
		// todo: only store the parameters that get changed
		EffectParameter worldParameter;
		EffectParameter viewParameter;
		EffectParameter projectionParameter;
		EffectParameter AmbientColorParam;
		EffectParameter AmbientIntensityParam;

		EffectParameter diffuseDirection;
		EffectParameter diffuseColor;
		EffectParameter diffuseIntensity;

		public MapDrawer mapDrawer;

		int numTris = 0;
		VertexPositionColorNormal[] vertices; // "mesh" from map

		public SceneRenderer(Game1 game)
		{
			graphicsDevice = game.GraphicsDevice;

			world = Matrix.Identity;
		}

		public void init(Effect effect)
		{
			this.effect = effect;
			effect.CurrentTechnique = effect.Techniques["AmbientDiffuseLight"];
			worldParameter = effect.Parameters["World"];
			viewParameter = effect.Parameters["View"];
			projectionParameter = effect.Parameters["Projection"];
			AmbientColorParam = effect.Parameters["AmbientColor"];
			AmbientIntensityParam = effect.Parameters["AmbientIntensity"];
			diffuseDirection = effect.Parameters["DiffuseDirection"];
			diffuseColor = effect.Parameters["DiffuseColor"];
			diffuseIntensity = effect.Parameters["DiffuseIntensity"];

			AmbientColorParam.SetValue(Color.White.ToVector4());

			AmbientIntensityParam.SetValue(0.5f);

			Vector4 lightDir = new Vector4(1.0f, -1.0f, -1.0f, 1.0f);
			lightDir.Normalize();
			diffuseDirection.SetValue(lightDir);//new Vector4(lightDir, 1.0f));
			diffuseColor.SetValue(Color.White.ToVector4());
			diffuseIntensity.SetValue(2.0f);
		}

		public void getVertices(Map map)
		{
			numTris = 0;
			var vertices = new List<VertexPositionColorNormal>();

			getTerrainVertices(map, vertices);

			getRiverVertices(map,vertices);

			this.vertices = vertices.ToArray();
		}

		private void getTerrainVertices(Map map, List<VertexPositionColorNormal> vertexList)
		{
			foreach(Corner c in map.corners.Values)
			{
				if(c.touches.Count != 3) continue;

				foreach(var v in c.touches)
				{
					Vector4 N = new Vector4(v.Normal, 1.0f);
					//N.Normalize();
					vertexList.Add(new VertexPositionColorNormal(v.position3f, mapDrawer.elevationColourMap(v.elevation).ToVector4(), N));
				}
				++numTris;
			}
		}

		private void getRiverVertices(Map map, List<VertexPositionColorNormal> vertexList)
		{
			foreach(River r in map.rivers)
			{
				for(int i = 1; i < r.path.Count; ++i)
				{
					var prevC = r.path[i - 1];
					var curC = r.path[i];

					Vector2 riverSegment = curC.position - prevC.position;
					// make quad between previous and current river node
					float heightoffset = 1.0f;
					float widthOffset = r.width / 2.0f;
					Vector2 latOffset = new Vector2(widthOffset, 0.0f);
					//find vector orthogonal to river path
					Vector2 riverSpan = (latOffset - riverSegment*1/Vector2.Dot(riverSegment, latOffset)) * latOffset.Length();

					Vector3 v1 = new Vector3(prevC.position + riverSpan, prevC.elevation + heightoffset);
					Vector3 v2 = new Vector3(prevC.position - riverSpan, prevC.elevation + heightoffset);
					Vector3 v3 = new Vector3(curC.position + riverSpan, prevC.elevation + heightoffset);
					Vector3 v4 = new Vector3(curC.position + riverSpan, prevC.elevation + heightoffset);

					Vector4 N = new Vector4(prevC.Normal + curC.Normal, 1.0f);
					N.Normalize();

					vertexList.Add(new VertexPositionColorNormal(v1, Color.Blue.ToVector4(), N));
					vertexList.Add(new VertexPositionColorNormal(v2, Color.Blue.ToVector4(), N));
					vertexList.Add(new VertexPositionColorNormal(v3, Color.Blue.ToVector4(), N));
					++numTris;
					vertexList.Add(new VertexPositionColorNormal(v2, Color.Blue.ToVector4(), N));
					vertexList.Add(new VertexPositionColorNormal(v3, Color.Blue.ToVector4(), N));
					vertexList.Add(new VertexPositionColorNormal(v4, Color.Blue.ToVector4(), N));
					++numTris;
				}
			}
		}

		public void Update(double deltaT)
		{
			view = camera.viewMatrix;
			projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f),
															 camera.aspectRatio,
															 1.0f,
															 100000.0f); // MAGIC_NUMBER: think of something useful for clipping. relate to possible map height?
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

				graphicsDevice.DrawUserPrimitives<VertexPositionColorNormal>(PrimitiveType.TriangleList,
				                                                             vertices, 0, numTris,
																			 VertexPositionColorNormal.VertexDeclaration); // todo: argument necessary, when using generic fxn call?
			}
		}
	}
}
