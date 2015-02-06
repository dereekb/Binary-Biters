using System;
using UnityEngine;

namespace Biters.Game
{
	/*
	 * Basic game tile that has a direction.
	 */
	public class DirectionalGameTile : BitersGameTile
	{
		public DirectionalGameTile (GameObject GameObject) : base (GameObject)
		{

		}
		
		#region Update
		
		public override void Update() {
			//TODO: Watch for entities to enter this tile, then change their direction as needed.
		}
		
		#endregion
	}

}

