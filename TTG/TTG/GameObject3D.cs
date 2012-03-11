using System;
using Sce.Pss.Core ;
using Sce.Pss.Core.Graphics ;

namespace TTG
{
	public class GameObject3D
	{
		Model model;
		GraphicsContext graphics;
		
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
	}
}

