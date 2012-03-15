using System;
using Sce.Pss.Core;
using Sce.Pss.Core.Graphics;

namespace TTG
{
	public class Enemy : GameObject3D
	{
		protected float health;
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
		
		public Enemy (GraphicsContext graphics)
			: base(graphics)
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

