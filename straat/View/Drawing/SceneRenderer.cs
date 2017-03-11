using System;
using System.Collections.Generic;
using System.Diagnostics;
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
			public Vector3 position { get; }
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
				new VertexElement(sizeof(float) * (3 + 4), VertexElementFormat.Vector4, VertexElementUsage.Normal, 0)
			);
		}

		GraphicsDevice graphicsDevice;

		public Camera camera;

		#region effectparams
		Effect effect;
		// todo: only store the parameters that get changed
		EffectParameter viewParameter;
		EffectParameter projectionParameter;
		EffectParameter AmbientColorParam;
		EffectParameter AmbientIntensityParam;

		EffectParameter diffuseDirection;
		EffectParameter diffuseColor;
		EffectParameter diffuseIntensity;
		#endregion

		public MapDrawer mapDrawer;

		int numTris = 0;
		VertexPositionColorNormal[] vertices; // "mesh" from map

		public SceneRenderer(Game1 game)
		{
			graphicsDevice = game.GraphicsDevice;
		}

		public void init(Effect effect)
		{
			this.effect = effect;
			effect.CurrentTechnique = effect.Techniques["AmbientDiffuseLight"];
			effect.Parameters["World"].SetValue(Matrix.Identity);
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
			diffuseDirection.SetValue(lightDir);
			diffuseColor.SetValue(Color.White.ToVector4());
			diffuseIntensity.SetValue(2.0f);
		}

		public void getVertices(Map map)
		{
			numTris = 0;
			var vertices = new List<VertexPositionColorNormal>();

			getTerrainVertices(map, vertices);

			getRiverVertices(map, vertices);
			getRoadVertices(map, vertices);
			getCityVertices(map, vertices);

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

		private void getPathVertices(List<VertexPositionColorNormal> vertexList, List<Center> path, float width, Color colour)
		{
			for(int i = 1; i < path.Count; ++i)
			{
				var prevC = path[i - 1];
				var curC = path[i];

				Vector2 riverSegment = curC.position - prevC.position;
				// make quad between previous and current river node
				float heightoffset = 0.2f;  // MAGIC_NUMBER: height offset kind of depends on the slope
				float widthOffset = width / 2.0f;
				Vector2 latOffset = new Vector2(widthOffset, 0.0f);
				//find vector orthogonal to river path
				Vector2 riverSpan = (latOffset - riverSegment * 1 / Vector2.Dot(riverSegment, latOffset)) * latOffset.Length();

				// todo: reuse last two vertices
				Vector3 v1 = new Vector3(prevC.position + riverSpan, prevC.elevation + heightoffset);
				Vector3 v2 = new Vector3(prevC.position - riverSpan, prevC.elevation + heightoffset);
				Vector3 v3 = new Vector3(curC.position + riverSpan, curC.elevation + heightoffset);
				Vector3 v4 = new Vector3(curC.position - riverSpan, curC.elevation + heightoffset);

				Vector4 N = new Vector4(prevC.Normal + curC.Normal, 1.0f);
				N.Normalize();

				vertexList.Add(new VertexPositionColorNormal(v1, colour.ToVector4(), N));
				vertexList.Add(new VertexPositionColorNormal(v2, colour.ToVector4(), N));
				vertexList.Add(new VertexPositionColorNormal(v3, colour.ToVector4(), N));
				++numTris;
				vertexList.Add(new VertexPositionColorNormal(v2, colour.ToVector4(), N));
				vertexList.Add(new VertexPositionColorNormal(v3, colour.ToVector4(), N));
				vertexList.Add(new VertexPositionColorNormal(v4, colour.ToVector4(), N));
				++numTris;
			}
		}

		private void getRiverVertices(Map map, List<VertexPositionColorNormal> vertexList)
		{
			foreach(River r in map.rivers)
			{
				getPathVertices(vertexList, r.path, r.width, Color.DarkBlue);
			}
		}

		private void getRoadVertices(Map map, List<VertexPositionColorNormal> vertexList)
		{
			foreach(Road r in map.roads)
			{
				getPathVertices(vertexList, r.path, r.width, Color.Brown);
			}
		}

		private void getCityVertices(Map map, List<VertexPositionColorNormal> vertexList)
		{
			foreach(var s in map.settlements)
			{
				// draw quad
				float rad = 30.0f; // MAGIC_NUMBER
				Vector3 x = new Vector3(rad, 0, 0);
				Vector3 y = new Vector3(0, rad, 0);
				Vector3 z = new Vector3(0, 0, 20.0f);
				Vector4 N = new Vector4(s.region.Normal, 1.0f);
				vertexList.Add(new VertexPositionColorNormal(s.region.position3f - x + z, Color.HotPink.ToVector4(), N));
				vertexList.Add(new VertexPositionColorNormal(s.region.position3f + x + z, Color.HotPink.ToVector4(), N));
				vertexList.Add(new VertexPositionColorNormal(s.region.position3f - y + z, Color.HotPink.ToVector4(), N));
				++numTris;
				vertexList.Add(new VertexPositionColorNormal(s.region.position3f + x + z, Color.HotPink.ToVector4(), N));
				vertexList.Add(new VertexPositionColorNormal(s.region.position3f + y + z, Color.HotPink.ToVector4(), N));
				vertexList.Add(new VertexPositionColorNormal(s.region.position3f - x + z, Color.HotPink.ToVector4(), N));
				++numTris;
			}
		}

		public void Update(double deltaT)
		{
			viewParameter.SetValue(camera.viewMatrix);
			projectionParameter.SetValue(camera.projection);
		}

		public void Draw(double deltaT)
		{
			RasterizerState rs = new RasterizerState();
			rs.CullMode = CullMode.None;
			graphicsDevice.RasterizerState = rs;

			for(int i = 0; i < effect.CurrentTechnique.Passes.Count; ++i)
			{
				effect.CurrentTechnique.Passes[i].Apply();

				graphicsDevice.DrawUserPrimitives<VertexPositionColorNormal>(PrimitiveType.TriangleList,
																			 vertices, 0, numTris,
																			 VertexPositionColorNormal.VertexDeclaration); // todo: argument necessary, when using generic fxn call?
			}
		}

		/// <summary>
		/// Raycast from screen coords to obtain world coords of mesh at this position
		/// </summary>
		/// <returns>The intersection point between the ray and the scene mesh in world coordinates.</returns>
		/// <param name="x">The x screen coordinate.</param>
		/// <param name="y">The y screen coordinate.</param>
		public Vector3? rayCast(int x, int y)
		{
			Stopwatch s = new Stopwatch();
			s.Start();
			Vector3 nearPos = graphicsDevice.Viewport.Unproject(new Vector3(x, y, 0.0f), camera.projection, camera.viewMatrix, Matrix.Identity);
			Vector3 farPos = graphicsDevice.Viewport.Unproject(new Vector3(x, y, 1.0f), camera.projection, camera.viewMatrix, Matrix.Identity);
			Vector3 dir = farPos - nearPos;
			dir.Normalize();

			Ray ray = new Ray(nearPos, dir);
			for(int i = 0; i <= vertices.Length-3; i += 3)
			{
				Vector3? ret = RayIntersectsTriangle(ray, vertices[i].position, vertices[i + 1].position, vertices[i + 2].position);
				if(ret != null)
				{
					// todo: implement multiple intersections (two triangles in front of each other)
					s.Stop();
					System.Console.WriteLine("raycast (found) time needed: "+s.Elapsed.TotalMilliseconds);
					return ret;
				}
			}
			s.Stop();
			System.Console.WriteLine("raycast (null) time needed: " + s.Elapsed.TotalMilliseconds);
			return null;
		}

		/// <summary>
		/// Intersects a ray and a triangle. And returns the position of the intersection in world coordinates or null, if they don't intersect
		/// Method by Möller and Trumbore, "Fast, Minimum Storage Ray-Triangle Intersection".
		/// </summary>
		/// <param name="ray">Ray.</param>
		/// <param name="v0">Vertex V0.</param>
		/// <param name="v1">Vertex V1.</param>
		/// <param name="v2">Vertex V2.</param>
		private Vector3? RayIntersectsTriangle(Ray ray, Vector3 v0, Vector3 v1, Vector3 v2)
		{
			Vector3 D = ray.Direction;
			Vector3 O = ray.Position;

			Vector3 E1 = v1 - v0;
			Vector3 E2 = v2 - v0;

			Vector3 T = O - v0;

			Vector3 P = Vector3.Cross(D, E2);
			Vector3 Q = Vector3.Cross(T, E1);

			float det = Vector3.Dot(P, E1);
			float epsilon = 0.000001f;  // error margin for checking wether ray is coplanar with triangle
			if(det > -epsilon && det < epsilon) // ray is coplanar
				return null; 

			// todo: culling disabled - for culling test >det instead of >1 and /det after checks and det>-epsilon not necessary

			float u = Vector3.Dot(P, T) / det;

			// test u parameter
			if(u < 0 || u > 1) // not in triangle bounds
				return null;

			float v = Vector3.Dot(Q, D) / det;

			// test v parameter
			if(v < 0 || u + v > 1) // not in triangle bounds
				return null;

			//else ray intersects triangle
			float t = Vector3.Dot(Q, E2) / det;

			if(t < 0) // triangle behind ray
				return null;

			// success
			// return world coords
			return O + t * D;
		}
	}
}