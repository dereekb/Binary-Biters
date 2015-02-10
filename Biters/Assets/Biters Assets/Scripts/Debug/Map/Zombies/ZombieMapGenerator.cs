using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Biters;
using Biters.Game;
using Biters.Utility;
using Biters.Debugging.Generators;

namespace Biters.Debugging.Zombies
{
	/*
	 * Creates a map of Spawner tiles that spawn zombies.
	 */
	public class DebugZombieMapGeneratorFactory : DebugRandomTileMapGeneratorFactory {

		public int GraveyardChance = 10;
		public int GraveyardMax = 2;
		public int GraveyardsSpawned = 0;

		public DebugZombieMapGeneratorFactory() {
			this.UniiNiliSpawnAmount = 1000;
		}

		public override BitersGameTile Make() {
			BitersGameTile tile = null;

			if ((GraveyardMax > GraveyardsSpawned) && (GraveyardChance > this.RandomGenerator.Next(0, 100))) {
				tile = new GraveyardTile();
				this.GraveyardsSpawned += 1;
			} else {
				tile = base.Make();
			}

			return tile;
		}
		
	}

}

