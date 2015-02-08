using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Biters;
using Biters.Game;
using Biters.Utility;

namespace Biters.Debugging
{

	public class TestMapGenerator : IGameMapTileImporter<BitersGameTile, BitersMapEntity>, IFactory<IGameMapDelegate<BitersGameTile, BitersMapEntity>>
	{
		public TestMapGenerator () {
			this.Factory = new TestMapGeneratorFactory ();
		}

		public IFactory<BitersGameTile> Factory;
		
		public IImportedMap<BitersGameTile> Import(string map) {
			TestMapGeneratorMap import = new TestMapGeneratorMap (this.Factory);
			return import;
		}

		public IGameMapDelegate<BitersGameTile, BitersMapEntity> Make() {
			TestMapGeneratorMap import = new TestMapGeneratorMap (this.Factory);
			return import;
		}

	}
	
	public class TestMapGeneratorMap : IImportedMap<BitersGameTile>, IGameMapDelegate<BitersGameTile, BitersMapEntity> {
		
		public int xSize = 25;
		public int ySize = 15;
		public int chance = 75;
		
		private System.Random RandomGenerator = new System.Random();

		public IFactory<BitersGameTile> Factory;

		public TestMapGeneratorMap(IFactory<BitersGameTile> Factory) {
			this.Factory = Factory;
		}

		public World<BitersGameTile> Make() {
			World<BitersGameTile> world = new World<BitersGameTile> ();

			for (int x = 0; x < xSize; x += 1) {
				for (int y = 0; y < ySize; y += 1) {
					if (chance > RandomGenerator.Next(100)) {
						BitersGameTile tile = Factory.Make();
						WorldPosition position = new WorldPosition(x, y);
						world.SetAtPosition(tile, position);
					}
				}
			}

			return world;
		}
		
		public World<BitersGameTile> GenerateNewWorld(IGameMap<BitersGameTile, BitersMapEntity> Map) {
			World<BitersGameTile> world = this.Make ();

			foreach (BitersGameTile tile in world) {
				tile.Map = Map;
			}

			return world;
		}

	}

	public class TestMapGeneratorFactory : IFactory<BitersGameTile> {

		private System.Random RandomGenerator = new System.Random();

		public BitersGameTile Make() {
			BitersGameTile tile = null;

			Func<BitersMapEntity> SpawnFunction = () => {
				if (RandomGenerator.Next(-1, 1) == 0) {
					return new Unii ();
				} else {
					return new Nili ();
				}
			};

			SpawnerGameTileDelegate spawnerDelegate = new SpawnerGameTileDelegate(SpawnFunction);

			//TODO: Spawn and make them go random directions.

			float timer = RandomGenerator.Next (4, 10);	//Between 3 and 20 seconds
			int max = 500; // RandomGenerator.Next (20, 500);

			int directionNumber = RandomGenerator.Next (-1, (int) DirectionalGameTileType.T_Right);

			DirectionalGameTileType direction = (DirectionalGameTileType) directionNumber+1;

			/*
			Vector3 direction = new Vector3 ();
			direction.x = RandomGenerator.Next (-1, 2);
			direction.y = RandomGenerator.Next (-1, 2);

			if (direction.x == 0 && direction.y == 0) {
				direction.x = 1;
			}
			*/

			SpawnerGameTile spawnTile = new SpawnerGameTile (spawnerDelegate, direction);
			spawnTile.SpawnTimer.Length = timer;
			spawnTile.SpawnMax = max;

			tile = spawnTile;
			return tile;
		}

	}

}

