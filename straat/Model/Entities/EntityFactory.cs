using System;
using straat.Model;
using Microsoft.Xna.Framework.Graphics;
using straat.View.Drawing;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace straat.Model.Entities
{
	public class EntityFactory
	{
		private static EntityFactory instance;
		public static EntityFactory Instance { get {
				if(instance == null)
					instance = new EntityFactory();
				return instance;
			}}

		#region content
		Texture2D unitTex;
		#endregion

		private EntityFactory(){}

		public void LoadContent(ContentManager Content)
		{
			unitTex = Content.Load<Texture2D>( "unit" );
		}

		public Entity createTestEntity(Vector3 position)
		{
			Entity tmp = new Entity(new GraphicsComponent(), new SelectableComponent(),position);
			tmp.gc.texture = unitTex;
			return tmp;
		}
	}
}

