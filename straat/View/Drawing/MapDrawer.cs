using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;


namespace straat.View.Drawing
{
	public class MapDrawer
	{
		public enum SHADING
		{
			TOPOGRAPHIC,
			DIFFUSE,
			ASPECT,
			HILL,

			NUM_SHADINGTYPES
		}

		Game1 game;
		Map map;

		public bool drawVoronoiCenters = false;
		public bool drawVoronoiEdges = false;
		public bool drawVoronoiRegions = false;
		public bool drawVoronoiVertices = false;
		public bool drawDelaunayEdges = false;
		public bool drawRivers = true;
		public bool drawEndOfTheWorld = true;
		public bool drawTopography = false;
		public SHADING shadingStyle {get;private set;}


		public MapDrawer(Game1 game, Map map)
		{
			this.game = game;
			this.map = map;

			shadingStyle = SHADING.TOPOGRAPHIC;
		}

		public void Draw(straat.View.Camera cam)
		{
			// iterate over delaunay
			foreach(Corner c in map.corners.Values)
			{
				if( c.touches.Count < 3 )
					continue;

				if( c.isEndOfTheWorld )
					continue;

				// draw topography
				Vector2 A = cam.getDrawPos(c.touches.ElementAt(0).position);
				Vector2 B = cam.getDrawPos(c.touches.ElementAt(1).position);
				Vector2 C = cam.getDrawPos(c.touches.ElementAt(2).position);

				switch(shadingStyle)
				{
				case SHADING.ASPECT:
					Color colAspect = aspectBasedShading( c.aspect );
					GeometryDrawer.fillTriangleGradient( A, B, C, colAspect, colAspect, colAspect );
					break;
				case SHADING.DIFFUSE:
					Color colDiff = diffuseReflection( c.surfaceNormal );
					GeometryDrawer.fillTriangleGradient( A, B, C, colDiff, colDiff, colDiff );
					break;
				case SHADING.HILL:
					Color colHill = hillShading( c );
					GeometryDrawer.fillTriangleGradient( A, B, C, colHill, colHill, colHill );
					break;
				default:
				case SHADING.TOPOGRAPHIC:
					Color Ac = elevationColourMap( c.touches.ElementAt( 0 ).elevation );
					Color Bc = elevationColourMap( c.touches.ElementAt( 1 ).elevation );
					Color Cc = elevationColourMap( c.touches.ElementAt( 2 ).elevation );
					GeometryDrawer.fillTriangleGradient( A, B, C, Ac, Bc, Cc );
					break;
				}

				if( drawDelaunayEdges )
				{
					GeometryDrawer.drawLine( A, B, Color.White );
					GeometryDrawer.drawLine( C, B, Color.White );
					GeometryDrawer.drawLine( A, C, Color.White );
				}
			}

			// iterate over voronoi
			foreach(Center c in map.centers.Values)
			{
				// region center
				Vector2 d = cam.getDrawPos(c.position);

				if( drawTopography )
				{
					// polygon nodes
					List<Point> poly = new List<Point>();
					List<Color> polyCol = new List<Color>();
					// draw topography
					// todo: draw delaunay triangles instead
					foreach( Corner co in c.polygon )
					{
						Vector2 f = cam.getDrawPos( co.position );

						if( float.IsInfinity( co.position.Length() ) )
							continue;

						poly.Add( f.ToPoint() );

						if( co.isOcean )
							polyCol.Add( Color.DarkBlue );
						else
							polyCol.Add( elevationColourMap( co.elevation ) );
					}
					Color cCol;
					if( c.isOcean )
					{
						cCol = Color.DarkBlue;
						GeometryDrawer.fillPoly( poly, cCol );
					}
					else if( c.isLake )
						GeometryDrawer.fillPoly( poly, Color.Blue );
					else
					{
						cCol = elevationColourMap( c.elevation );
						GeometryDrawer.fillPolyGradient( d, poly, cCol, polyCol.ToArray() );
					}
				}
 
				// draw voronoi edges
				if(drawVoronoiEdges)
					foreach(VDEdge e in c.borders)
					{
						Vector2 a = cam.getDrawPos(e.endpoints[0].position);
						Vector2 b = cam.getDrawPos(e.endpoints[1].position);

						GeometryDrawer.drawLine(a,b,Color.White);
					}

				// draw corners
				if(drawVoronoiVertices)
					foreach(Corner co in c.polygon)
					{
						Point f = cam.getDrawPos(co.position).ToPoint();
						GeometryDrawer.fillRect(f.X,f.Y,3,3,Color.LightBlue);
					}

				// draw region centers
				if( drawVoronoiCenters )
				{
					GeometryDrawer.fillRect( (int)d.X, (int)d.Y, 5, 5, Color.Blue );

					if( drawEndOfTheWorld )
					if( c.isEndOfTheWorld )
						GeometryDrawer.fillRect( (int)d.X, (int)d.Y, 5, 5, Color.HotPink );
				}

			}

			// draw river
			if(drawRivers)
				foreach(River r in map.rivers)
				{
					for(int i = 1;i<r.path.Count;++i)
					{
						Vector2 a = cam.getDrawPos(r.path[i-1].position);
						Vector2 b = cam.getDrawPos(r.path[i].position);
						GeometryDrawer.drawLine(a,b,Color.Blue);
					}
				}
		}

		public Color elevationColourMap(float elevation)
		{
			// ocean
			if( elevation == 0.0f )
				return Color.DarkBlue;

			if( elevation < 0.1f )
				return Color.Lerp( Color.Ivory, Color.ForestGreen, elevation * 10.0f );


			// snowy hilltops 0.9-1.0
			if( elevation > 0.9f )
				return Color.White;

			// lowlands 0.0-0.5
			if(elevation < 0.5f)
			{
				return Color.Lerp(Color.ForestGreen,Color.LightSlateGray,elevation*2.0f);
			}

			// mountains 0.5 - 0.9
			return Color.Lerp(Color.LightSlateGray,Color.DarkSlateGray,(elevation-0.5f)*1.0f/0.4f);
		}

		public Color aspectBasedShading(float aspect)
		{
			float azimuth = (float)Math.PI * 3.0f / 4.0f;	// light comes from north-west

			float theta = aspect - azimuth;

			float value = (float)Math.Abs(Math.Cos(( theta )));

//			value += (float)Math.PI;
//			value /= (float)(Math.PI * 2.0 + 12.0);

			//value = (float)(( (double)value + Math.PI ) / ( Math.PI * 2.0 ));

			return Color.Lerp( Color.White, Color.Black, value );
		}

		public Color diffuseReflection(Vector3 normal)
		{
			Vector3 light = new Vector3( 1, 1, -1 );
			light.Normalize();

			double dot = Vector3.Dot( normal, light );
			double angle = Math.Acos( dot);

			float value = (float)( angle /Math.PI);

			return Color.Lerp( Color.White, Color.Black, value );
		}

		public Color hillShading(Corner roi)
		{
			float S = roi.angle;
			float A = roi.aspect;

			float I = (float)Math.PI * 3.0f / 4.0f;
			float D = (float)Math.PI / 4.0f;

			float BV = (float)(Math.Cos( I ) * Math.Sin( S ) * Math.Cos(Math.Abs(A - D )) + Math.Sin( I ) * Math.Cos( S ));

			return Color.Lerp( Color.White, Color.Black, BV );
		}

		public void cycleShading()
		{
			if( (int)shadingStyle == (int)SHADING.NUM_SHADINGTYPES - 1 )
				shadingStyle = 0;
			else
				++shadingStyle;
		}
	}
}

