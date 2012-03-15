using System;
using Sce.Pss.Core;
using Sce.Pss.Core.Graphics;

namespace TTG
{
	public class Enemy : GameObject3D
	{
		float health;
		public float Health
		{
			get
			{
				return health;
			}
			set
			{
				health = value;
			}
		}
		
		public enum PenguinType
		{
			Normal,
			Fast,
			Slow,
			Tank,
			Motorcycle
		}
		
		public Enemy (GraphicsContext graphics, Model model)
			: base(graphics, model)
		{
		}
		
		public override void Draw ()
		{
			base.Draw ();
		}
		public override void Update ()
		{
			base.Update ();
		}
	}
}

