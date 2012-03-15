using System;
using Sce.Pss.Core ;
using Sce.Pss.Core.Graphics;
using Sce.Pss.Core.Input;

namespace TTG
{
	public class GameObject3D
	{
		protected Model model;
		protected GraphicsContext graphics;
		
		public GameObject3D (GraphicsContext inGraphics, 
		                     Model inModel)
		{
			graphics = inGraphics;
			model = inModel;
		}
		public virtual void Draw()
		{
		}
		
		public virtual void Update()
		{
		}
		
		public virtual void Update(GamePadData padData)
		{
		}
	}
}

