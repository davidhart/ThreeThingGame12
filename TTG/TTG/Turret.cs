using System;
using Sce.Pss.Core;
using Sce.Pss.Core.Graphics;

namespace TTG
{
	public class Turret : GameObject3D
	{
		public enum TurretType
		{
			MachineGun,
			Flamethrower,
			Rocket,
			Sniper
		}
		
		int targetRadius;
		
		public TurretType TType;
		
		public Turret (GraphicsContext graphics)
			: base (graphics)
		{
		}
		
		public override void Update (float dt)
		{
			switch (TType)
			{
			case TurretType.MachineGun:
			{
				break;
			}
			case TurretType.Flamethrower:
			{
				break;
			}
			case TurretType.Rocket:
			{
				break;
			}
			case TurretType.Sniper:
			{
				break;
			}
			}
			base.Update (dt);
		}
		
		public override void Draw ()
		{
			switch (TType)
			{
			case TurretType.MachineGun:
			{
				break;
			}
			case TurretType.Flamethrower:
			{
				break;
			}
			case TurretType.Rocket:
			{
				break;
			}
			case TurretType.Sniper:
			{
				break;
			}
			}
			base.Draw ();
		}
	}
}

