//GameObject2d
//A 2D gameobject rendered using a Sprite
//Will also have physics components attatched to it.

using System;
using System.Collections.Generic;

using Sce.Pss.Core;
using Sce.Pss.Core.Environment;
using Sce.Pss.Core.Graphics;
using Sce.Pss.Core.Input;

namespace TTG
{
	public class GameObject2D
	{
		Sprite gameSprite;
		public GameObject2D (GraphicsContext graphics, 
		                     Texture2D tex2D)
		{
			gameSprite = new Sprite(graphics, tex2D);
		}
		
		public virtual void Draw()
		{
			gameSprite.Draw();
		}
		
		public virtual void Update()
		{
		}
		
		//TODO Overloaded Update with input and possibly time variables
	}
}

