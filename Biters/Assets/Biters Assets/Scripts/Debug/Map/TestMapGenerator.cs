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
		public int chance = 100;
		
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

			/*
			Func<BitersMapEntity> SpawnFunction = () => {
				if (RandomGenerator.Next(-1, 1) == 0) {
					return new Unii ();
				} else {
					return new Nili ();
				}
			};
			
			SpawnerGameTileDelegate spawnerDelegate = new SpawnerGameTileDelegate(SpawnFunction);

			SpawnerGameTile a = new SpawnerGameTile (spawnerDelegate, DirectionalGameTileType.Corner_Top_Right).MakeMazeTile();
			a.SpawnMax = 0;
			world.SetAtPosition(a, new WorldPosition(0, 0));
			
			SpawnerGameTile b = new SpawnerGameTile (spawnerDelegate, DirectionalGameTileType.Corner_Bottom_Right).MakeMazeTile();
			b.SpawnMax = 0;
			world.SetAtPosition(b, new WorldPosition(0, 1));

			SpawnerGameTile c = new SpawnerGameTile (spawnerDelegate, DirectionalGameTileType.Corner_Top_Left).MakeMazeTile();
			c.SpawnMax = 0;
			world.SetAtPosition(c, new WorldPosition(1, 0));
			
			BitersGameTile d = new SpawnerGameTile (spawnerDelegate, DirectionalGameTileType.Corner_Bottom_Left).MakeMazeTile();
			world.SetAtPosition(d, new WorldPosition(1, 1));
			*/

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

			float timer = RandomGenerator.Next (2, 25) / 2.5f;	//Between 3 and 20 seconds
			int max = 10;	// RandomGenerator.Next (20, 500);

			int directionNumber = RandomGenerator.Next (0, (int) DirectionalGameTileType.Corner_Bottom_Left);
			DirectionalGameTileType direction = (DirectionalGameTileType) directionNumber+1;

			/*
			Vector3 direction = new Vector3 ();
			direction.x = RandomGenerator.Next (-1, 2);
			direction.y = RandomGenerator.Next (-1, 2);

			if (direction.x == 0 && direction.y == 0) {
				direction.x = 1;
			}
			*/

			SpawnerGameTile spawnTile = new SpawnerGameTile (spawnerDelegate, direction).MakeMazeTile();
			spawnTile.SpawnTimer.Length = timer;
			spawnTile.TileDirectionFactory.MoveSpeed = RandomGenerator.Next (1, 5) / 2.0f;
			spawnTile.SpawnMax = max;

			tile = spawnTile;
			return tile;
		}

	}

}

