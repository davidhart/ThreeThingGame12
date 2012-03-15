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
		
		public Turret (GraphicsContext graphics, Model model)
			: base (graphics, model)
		{
		}
		
		public override void Update ()
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
			base.Update ();
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

