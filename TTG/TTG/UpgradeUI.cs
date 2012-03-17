using System;
using System.Diagnostics;
using System.Collections.Generic;
using Sce.Pss.Core;
using Sce.Pss.HighLevel.UI;
using Sce.Pss.Core.Graphics;
using Sce.Pss.Core.Input;
using Sce.Pss.Core.Imaging;

namespace TTG
{
	public class UpgradeUI
	{		
		private Texture2D [] textures;
		public bool show { get; set; }
		
		public Turret turret;
		
		public UpgradeUI ()
		{
			textures = new Texture2D[2];
			textures[0] = new Texture2D("assets/placeholder.png", false);
			textures[1] = new Texture2D("assets/placeholder.png", false);
			
			show = false;
		}
		
		public void Draw(SpriteBatch sb)
		{
			sb.Begin();
			if(show)
			{
				
				sb.Draw (textures[0], new Vector2(100,100));
				sb.Draw (textures[1], new Vector2(200,200));
				
			}
			sb.End ();
		}
		
		public bool Update(GamePadData data)
		{
			if(show)
			{
				if(data.ButtonsDown.HasFlag(GamePadButtons.Cross))
				{
					//quit
					show = false;
				}
				if(data.ButtonsDown.HasFlag(GamePadButtons.Square))
				{
					//Machine gun
					turret.SetType(TurretTypes.machineGunTurret);
					show = false;
				}
				if(data.ButtonsDown.HasFlag(GamePadButtons.Triangle))
				{
					//flamer/icer
					show = false;
				}
				if(data.ButtonsDown.HasFlag(GamePadButtons.Circle))
				{
					//sniper
					show = false;
				}
				return true;
			}
			return false;
		}
	}
}

