using System;
using Sce.Pss.Core;
using Sce.Pss.Core.Graphics;

using System.Collections;

namespace TTG
{
	public class Turret
	{
		
		#region Fields
		
		
		protected byte atkDmg;
		public byte AtkDmg
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
		
		Vector2 position;
		
		public Vector2 Position
		{
			get
			{
				return position;
			}
			set
			{
				position = value;
			}
		}
		
		Enemy target = null;
		
		#endregion
		
		public Turret (GraphicsContext graphics)
		{
		}
		
		public void Update (float dt, Enemy[] enemies)
		{
			if(target == null)
			{
				for(int i = 0; i < enemies.Length; ++i)
				{
					float distance = Vector2.Distance(enemies[i].GetPosition().Xz, position);
					if(distance <= atkRange)
					{
						target = enemies[i];
						break;
					}
				}
			}
			else
			{
				if(target.Health <= 0)
				{
					target =  null;
				}
				
				else
				{
					target.Health -= (atkDmg) * dt;
					float distance = Vector2.Distance(target.GetPosition().Xy, position);
					if(distance > atkRange)
					{
						target = null;
					}
				}
				
			}
			    
		}
		public void Draw ()
		{
			
		}
	}
}

