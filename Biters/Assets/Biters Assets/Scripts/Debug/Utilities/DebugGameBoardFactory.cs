using System;
using UnityEngine;
using Biters.Utility;
using Biters.Game;
using Biters;

namespace Biters.Debugging.Generators
{

	/*
	 * Generates Maps that use the BitersGameTile and BitersMapEntity.
	 * 
	 * Used for testing other functionality.
	 */
	public class DebugGameBoardFactory : IFactory<BitersGameMap>
	{
		internal DebugGameBoardGenerator generator = new DebugGameBoardGenerator();
	
		public DebugGameBoardGenerator Generator {
			get {
				return this.generator;
			}
		}

		public IFactory<BitersGameTile> Factory {
			get {
				return this.generator.Factory;
			}

			set {
				this.generator.Factory = value;
			}
		}

		public BitersGameMap Make() {
			GameObject mapObject = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			mapObject.transform.localScale = new Vector3 (0.2f, 0.2f, 0.2f);
			IMapWorldFactory<BitersGameTile> worldFactory = generator.Make ();
			return new BitersGameMap (mapObject, worldFactory);
		}

		public class DebugGameBoardGenerator : IFactory<IMapWorldFactory<BitersGameTile>> {
			
			public IFactory<BitersGameTile> Factory = new DebugMapTileFactory();
			
			public int BoardXSize = 10;
			public int BoardYSize = 10;
			public int BoardTileChance = 100;

			public IMapWorldFactory<BitersGameTile> Make() {
				DebugGameMapDelegate import = new DebugGameMapDelegate(this.Factory);
				import.BoardXSize = this.BoardXSize;
				import.BoardYSize = this.BoardYSize;
				import.BoardTileChance = this.BoardTileChance;
				return import;
			}

		}
		
		internal class DebugGameMapDelegate : IMapWorldFactory<BitersGameTile> {

			public int BoardXSize = 10;
			public int BoardYSize = 10;
			public int BoardTileChance = 100;

			private System.Random RandomGenerator = new System.Random();
			
			public IFactory<BitersGameTile> Factory;
			
			public DebugGameMapDelegate(IFactory<BitersGameTile> Factory) {
				this.Factory = Factory;
			}
			
			public World<BitersGameTile> MakeNewWorld() {
				World<BitersGameTile> world = new World<BitersGameTile> ();
				
				for (int x = 0; x < BoardXSize; x += 1) {
					for (int y = 0; y < BoardYSize; y += 1) {
						if (BoardTileChance > RandomGenerator.Next(100)) {
							BitersGameTile tile = Factory.Make();
							WorldPosition position = new WorldPosition(x, y);
							world.SetAtPosition(tile, position);
						}
					}
				}
				
				return world;
			}

		}
		
		internal class DebugMapTileFactory : IFactory<BitersGameTile> {

			public BitersGameTile Make() {
				BitersGameTile tile  = new DirectionalGameTile ();
				return tile;
			}
			
		}

	}

}

