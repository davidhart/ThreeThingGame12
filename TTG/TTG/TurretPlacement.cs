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
		public struct Placements
		{
			public Vector2 position;
			public bool placed;
		};
		
		public float distanceFromPlacement;
		
		private Placements[] placement;
		
		public TurretPlacement ()
		{
			placement = new Placements[100];
		}
		
		/// <summary>
		/// Gets the placement details.
		/// </summary>
		/// <returns>
		/// The placement details.
		/// </returns>
		/// <param name='num'>
		/// Position in the list
		/// </param>
		public Placements GetPlacementDetails(int num)
		{
			return placement[num];
		}
		
		/// <summary>
		/// Sets the placement details.
		/// </summary>
		/// <param name='Pos'>
		/// Position.
		/// </param>
		public void SetPlacementDetails(Vector2 Pos)
		{
			Placements p;
			p.position = Pos;
			p.placed = false;
		}
		
		/// <summary>
		/// Places the turret.
		/// </summary>
		/// <param name='Pos'>
		/// Position of player.
		/// </param>
		public void PlaceTurret(Vector2 PlayerPos)
		{
			for(int i = 0; i < placement.Length; ++i)
			{
				if(placement[i].position.Distance(PlayerPos) < distanceFromPlacement)
				{
					placement[i].placed = true;
				}
			}
		}
	}
}

