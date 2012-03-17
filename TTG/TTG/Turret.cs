using System;
using Sce.Pss.Core;
using Sce.Pss.Core.Graphics;

using System.Collections;

namespace TTG
{
	public class Turret : GameObject3D
	{
		#region Fields
		protected byte atkDmg;
		public byte AtDmg
		{
			get
			{
				return atkDmg;
			}
			set
			{
				atkDmg = value;
			}
		}
		
		protected float atkRange;
		public float AtkRange
		{
			get
			{
				return atkRange;
			}
			set
			{
				atkRange = value;
			}
		}
		
		protected float atkSpeed;
		public float AtkSpeed
		{
			get
			{
				return atkSpeed;
			}
			set
			{
				atkSpeed = value;
			}
		}
		
		Vector2 postion;
		
		public Vector2 Position
		{
			get
			{
				return postion;
			}
			set
			{
				postion = value;
			}
		}
		
		Enemy target;
		
		#endregion
		
		public Turret (GraphicsContext graphics)
			: base (graphics)
		{
		}
		
		public override void Update (float dt, Enemy[] enemies)
		{
			if(target == null)
			{
				for(int i = 0; i < enemies.Length; ++i)
				{
					float distance = Vector2.Distance(target, postion);
					if(distance <= atkRange)
					{
						target = enemies[i];
						break;
					}
				}
			}
			else if(target.Health <= 0)
			{
				target =  null;
			}
			base.Update (dt);
			    
		}
		public override void Draw ()
		{
			
			base.Draw ();
		}
	}
}

