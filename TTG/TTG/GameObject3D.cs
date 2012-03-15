using System;
using Sce.Pss.Core ;
using Sce.Pss.Core.Graphics;
using Sce.Pss.Core.Input;

namespace TTG
{
	public class GameObject3D
	{
		protected GraphicsContext graphics;
		
		public GameObject3D (GraphicsContext inGraphics)
		{
			graphics = inGraphics;
		}
		public virtual void Draw()
		{
		}
		
		public virtual void Update(float dt)
		{
		}
		
		public virtual void Update(GamePadData padData)
		{
		}
	}
}

