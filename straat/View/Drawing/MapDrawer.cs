using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;


namespace straat.View.Drawing
{
	public class MapDrawer
	{
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


		public MapDrawer(Game1 game, Map map)
		{
			this.game = game;
			this.map = map;
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

				Color Ac = elevationColourMap( c.touches.ElementAt( 0 ).elevation );
				Color Bc = elevationColourMap( c.touches.ElementAt( 1 ).elevation );
				Color Cc = elevationColourMap( c.touches.ElementAt( 2 ).elevation );

				GeometryDrawer.fillTriangleGradient(A,B,C,Ac,Bc,Cc);

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


		}

		public Color elevationColourMap(float elevation)
		{
			// ocean
			if( elevation == 0.0f )
				return Color.DarkBlue;

			if( elevation < 0.1f )
				return Color.Lerp( Color.DarkBlue, Color.Beige, elevation * 10.0f );


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
	}
}

