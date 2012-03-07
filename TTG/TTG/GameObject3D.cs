using System;
using Sce.Pss.Core ;
using Sce.Pss.Core.Graphics ;
using Sce.Pss.HighLevel.Model ;

namespace TTG
{
	public class GameObject3D
	{
		BasicModel model;
		GraphicsContext graphics;
		
		public GameObject3D (GraphicsContext inGraphics, 
		                     BasicModel inModel)
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

