using System;
using System.Collections.Generic;
using System.Linq;
using libertad.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace libertad
{
    public class MousePicker
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Effect fillTexEffect;
        RenderTarget2D currentObjectMap;

        Texture2D tex;
        SpriteFont font;
        string debugtext ="";


        Texture2D objectMap = null;

        List<WorldObject> worldObjects = new List<WorldObject>();
        Dictionary<Color, WorldObject> lookup = new Dictionary<Color, WorldObject>();

        MouseState prevMS;
        KeyboardState prevKS;

        Viewport vp1;
        Viewport vp2;
        Camera cam1;

		public MousePicker()
        {

        }

        protected override void Initialize()
        {
            vp1 = GraphicsDevice.Viewport;
            vp1.Width /= 2;
            vp2 = vp1;
            vp2.X = vp1.Width;
            cam1 = new Camera(vp1);

            worldObjects.Add(new WorldObject(25, 35));
            worldObjects.Add(new WorldObject(35, 35));


            currentObjectMap = new RenderTarget2D(GraphicsDevice, vp1.Width, vp1.Height);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            fillTexEffect = Content.Load<Effect>("fillTexture");
            tex = Content.Load<Texture2D>("enemy");
            font = Content.Load<SpriteFont>("font");
        }


        protected override void Update(GameTime gameTime)
        {


            MouseState ms = Mouse.GetState();
            //check for mouse selection
            WorldObject selection = null;


            if (ms.LeftButton == ButtonState.Pressed)
            {
                //find clicked object
                Color[] dat = new Color [objectMap.Width*objectMap.Height];
                objectMap.GetData(dat);
                int i =ms.X + ms.Y * objectMap.Width;

                if (dat[i] != Color.Black)
                    selection = lookup[dat[i]];
            }

            //check for mouse scrolling
			/*if (ms.RightButton == ButtonState.Pressed && prevMS.RightButton==ButtonState.Pressed)
            {
                Vector2 scrolling = new Vector2(ms.X - prevMS.X, ms.Y - prevMS.Y);
                cam1.position -= scrolling;
            }*/

            debugtext = "" + ms.X + " , " + ms.Y + " - "+((selection!=null)?selection.id:-7);


            //refresh for next frame
            prevMS = ms;
        }

        protected override void Draw(GameTime gameTime)
        {
            Viewport original = graphics.GraphicsDevice.Viewport;
            
            lookup.Clear();
            int color = 100;

            
            //skip irrelevant
            //continue;

            GraphicsDevice.Viewport = vp1;

            GraphicsDevice.SetRenderTarget(currentObjectMap);
            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, fillTexEffect);

            GraphicsDevice.Clear(Color.Black);
            
            foreach (WorldObject item in worldObjects)
            {
                Color curcol = new Color(0, color / 256, color % 256,255);
                spriteBatch.Draw(tex, item.position, curcol);

                //change color for map
                color += 10;
                //add object to color-lookup-table
                lookup.Add(curcol, item);
            }
            spriteBatch.End();

            //draw world view
            GraphicsDevice.SetRenderTarget(null);

            //save object mapping for handling in next frame
            objectMap = (Texture2D)currentObjectMap;

            spriteBatch.Begin();
            GraphicsDevice.Clear(Color.CornflowerBlue);
            //draw worldobject
            foreach (WorldObject item2 in worldObjects)
            {
                spriteBatch.Draw(tex, item2.position, Color.White);
            }
            spriteBatch.End();


            //Draw debug    
            GraphicsDevice.Viewport = vp2;
            GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin();
            //spriteBatch.Draw(currentObjectMap, Vector2.Zero, Color.White);
            spriteBatch.DrawString(font, debugtext, Vector2.Zero, Color.White);
            spriteBatch.End();



            graphics.GraphicsDevice.Viewport = original;
        }
    }
}
