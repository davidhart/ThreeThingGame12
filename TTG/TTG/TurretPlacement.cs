using System;
using System.Collections;
using System.Collections.Generic;

using Sce.Pss.Core;
using Sce.Pss.Core.Environment;
using Sce.Pss.Core.Graphics;
using Sce.Pss.Core.Input;
using Sce.Pss.HighLevel.UI;

namespace TTG
{
	public class TurretPlacement
	{
		Turret placementTurret;
		public Vector2 Position;
		public bool Placed;
		Vector2 gridPos;
		
		public float distanceFromPlacement;
		
		
		public TurretPlacement (Vector2 inGridPos)
		{
			
		}
		
		public void SetPlacement(TurretType type)
		{
		}
		
		public void Update(float dt, Enemy[] enemies)
		{
			if(Placed)
			{
				placementTurret.Update(dt, enemies);
			}
		}
		public void Draw()
		{
			if(Placed)
			{
				placementTurret.Draw();
			}
		}
	}
}

