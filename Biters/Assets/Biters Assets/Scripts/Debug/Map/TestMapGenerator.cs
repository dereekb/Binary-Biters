using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Biters;
using Biters.Game;
using Biters.Utility;

namespace Biters.Testing
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
		
		public int xSize = 5;
		public int ySize = 5;

		public IFactory<BitersGameTile> Factory;

		public TestMapGeneratorMap(IFactory<BitersGameTile> Factory) {
			this.Factory = Factory;
		}

		public World<BitersGameTile> Make() {
			World<BitersGameTile> world = new World<BitersGameTile> ();

			for (int x = 0; x < xSize; x += 1) {
				for (int y = 0; y < ySize; y += 1) {
					BitersGameTile tile = Factory.Make();
					WorldPosition position = new WorldPosition(x, y);
					world.SetAtPosition(tile, position);
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

			float timer = RandomGenerator.Next (1, 2) / 4.0f;	//Between 1 and 5 seconds
			int max = RandomGenerator.Next (5, 20);
			Vector3 direction = new Vector3 ();
			direction.x = (RandomGenerator.Next (-5, 5) / 2.0f) - 2.5f;
			direction.y = (RandomGenerator.Next (-5, 5) / 2.0f) - 2.5f;
			
			SpawnerGameTile spawnTile = new SpawnerGameTile (spawnerDelegate, direction);
			spawnTile.SpawnTimer.Length = timer;
			spawnTile.SpawnMax = max;

			tile = spawnTile;
			return tile;
		}

	}

}
