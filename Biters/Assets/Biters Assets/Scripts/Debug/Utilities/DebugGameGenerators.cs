using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Biters;
using Biters.Game;
using Biters.Utility;

namespace Biters.Debugging.Generators
{
	#region Random Spawner Tiles

	/*
	 * Creates a map of random SpawnerGameTiles.
	 */
	public class DebugRandomTileMapGeneratorFactory : IFactory<BitersGameTile> {
		
		public System.Random RandomGenerator = new System.Random();

		public int UniiNiliSpawnAmount = 500;

		public virtual int GetSpawnAmount() {
			return this.UniiNiliSpawnAmount;
		}

		public virtual BitersGameTile Make() {
			BitersGameTile tile = null;

			/*
			 * Lambda function for spawning elements.
			 */
			Func<BitersMapEntity> SpawnFunction = () => {
				if (RandomGenerator.Next(-1, 1) == 0) {
					return new Unii ();
				} else {
					return new Nili ();
				}
			};
			
			SpawnerGameTileDelegate spawnerDelegate = new SpawnerGameTileDelegate(SpawnFunction);

			float timer = RandomGenerator.Next (2, 25) / 2.5f;	//Between 3 and 20 seconds
			int max = this.GetSpawnAmount();	// RandomGenerator.Next (20, 500);
			
			int directionNumber = RandomGenerator.Next (0, DirectionalGameTileInfo.All.Length);
			DirectionalGameTileType direction = DirectionalGameTileInfo.All[directionNumber];

			SpawnerGameTile spawnTile = new SpawnerGameTile (spawnerDelegate, direction).MakeRotationTile();
			spawnTile.SpawnTimer.Length = timer;
			spawnTile.TileDirectionFactory.MoveSpeed = RandomGenerator.Next (1, 5) / 2.0f;
			spawnTile.SpawnMax = max;
			
			tile = spawnTile;
			return tile;
		}
		
	}

	#endregion

	#region Junk

	/*
			Vector3 direction = new Vector3 ();
			direction.x = RandomGenerator.Next (-1, 2);
			direction.y = RandomGenerator.Next (-1, 2);

			if (direction.x == 0 && direction.y == 0) {
				direction.x = 1;
			}
			*/

	#endregion

}

