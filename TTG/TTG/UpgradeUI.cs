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
		private Texture2D[] textures;

		public bool show { get; set; }

		BitmapFont font;
		public Turret turret;
		
		public UpgradeUI ()
		{
			textures = new Texture2D[4];
			textures [0] = new Texture2D ("assets/crossButton.png", false);
			textures [1] = new Texture2D ("assets/circleButton.png", false);
			textures [2] = new Texture2D ("assets/triangleButton.png", false);
			textures [3] = new Texture2D ("assets/squareButton.png", false);
			show = false;
			font = new BitmapFont ("assets/fonts/font.png", 37, 4);
		}
		
		public void Draw (SpriteBatch sb)
		{
			sb.Begin ();
			if (show) {
				if (turret.State == Turret.TurretState.Buy) {
					sb.Draw (textures [3], new Vector2 (100, 100));
					font.DrawText (sb, "MACHINE GUN", new Vector2 (165, 132), 1.0f);
					sb.Draw (textures [0], new Vector2 (200, 200));
					font.DrawText (sb, "BACK", new Vector2 (265, 232), 1.0f);
					sb.Draw (textures [1], new Vector2 (300, 100));
					font.DrawText (sb, "SNIPER", new Vector2 (365, 132), 1.0f);
					sb.Draw (textures [2], new Vector2 (200, 0));
					font.DrawText (sb, "ICER", new Vector2 (265, 32), 1.0f);
				} else {
					if (turret.Level < 5) {
						sb.Draw (textures [3], new Vector2 (100, 100));
						font.DrawText (sb, "UPGRADE " + (1000 * turret.Level).ToString (), new Vector2 (165, 132), 1.0f);
					}
					sb.Draw (textures [0], new Vector2 (200, 200));
					font.DrawText (sb, "BACK", new Vector2 (265, 232), 1.0f);
					sb.Draw (textures [1], new Vector2 (300, 100));
					font.DrawText (sb, "SELL", new Vector2 (365, 132), 1.0f);
				}
			}
			sb.End ();
		}
		
		public bool Update (GamePadData data)
		{
			if (show) {
				if (turret.State == Turret.TurretState.Buy) {
					if (data.ButtonsDown.HasFlag (GamePadButtons.Cross)) {
						//quit
						
						show = false;
					}
					if (data.ButtonsDown.HasFlag (GamePadButtons.Square)) {
						//Machine gun
						turret.SetType (TurretTypes.machineGunTurret);
						turret.State = Turret.TurretState.Upgrade;
						show = false;
					}
					if (data.ButtonsDown.HasFlag (GamePadButtons.Triangle)) {
						//flamer/icer
						turret.State = Turret.TurretState.Upgrade;
						show = false;
					}
					if (data.ButtonsDown.HasFlag (GamePadButtons.Circle)) {
						//sniper
						turret.State = Turret.TurretState.Upgrade;
						show = false;
					}
				} 
				else {
					if (data.ButtonsDown.HasFlag (GamePadButtons.Cross)) 
					{
						//quit
						
						show = false;
					}
					if (data.ButtonsDown.HasFlag (GamePadButtons.Square)) 
					{
						//Upgrade
						if(turret.Level < 5)
						{
							turret.Level++;
							
						}
						show = false;
					}
					if (data.ButtonsDown.HasFlag (GamePadButtons.Triangle)) {
						//flamer/icer
						turret.State = Turret.TurretState.Upgrade;
						show = false;
					}
				}
				return true;
			}
			return false;
		}
	}
}

