using System;
using UnityEngine;
using Biters.Utility;
using Biters.Game;
using Biters;

namespace Biters.Testing
{

	/*
	 * Generates Maps that use the BitersGameTile and BitersMapEntity.
	 * 
	 * Used for testing other functionality.
	 */
	public class UnitTestGameBoardFactory : IFactory<GameMap<BitersGameTile, BitersMapEntity>>
	{

		internal UnitTestGameBoardGenerator Generator = new UnitTestGameBoardGenerator();

		public GameMap<BitersGameTile, BitersMapEntity> Make() {
			GameObject MapObject = GameObject.CreatePrimitive (PrimitiveType.Plane);
			IGameMapDelegate<BitersGameTile, BitersMapEntity> MapDelegate = Generator.Make ();
			return new GameMap<BitersGameTile, BitersMapEntity> (MapObject, MapDelegate);
		}

		internal class UnitTestGameBoardGenerator : IFactory<IGameMapDelegate<BitersGameTile, BitersMapEntity>> {
			
			public IFactory<BitersGameTile> Factory = new UnitTestMapTileFactory();

			public IGameMapDelegate<BitersGameTile, BitersMapEntity> Make() {
				UnitTestGameMapDelegate import = new UnitTestGameMapDelegate(this.Factory);
				return import;
			}

		}
		
		internal class UnitTestGameMapDelegate : IGameMapDelegate<BitersGameTile, BitersMapEntity> {

			public int BoardXSize = 5;
			public int BoardYSize = 5;
			public int BoardTileChance = 100;

			private System.Random RandomGenerator = new System.Random();
			
			public IFactory<BitersGameTile> Factory;
			
			public UnitTestGameMapDelegate(IFactory<BitersGameTile> Factory) {
				this.Factory = Factory;
			}
			
			public World<BitersGameTile> Make() {
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
			
			public World<BitersGameTile> GenerateNewWorld(IGameMap<BitersGameTile, BitersMapEntity> Map) {
				World<BitersGameTile> world = this.Make ();
				
				foreach (BitersGameTile tile in world) {
					tile.Map = Map;
				}
				
				return world;
			}
			
		}
		
		internal class UnitTestMapTileFactory : IFactory<BitersGameTile> {

			public BitersGameTile Make() {
				BitersGameTile tile  = new DirectionalGameTile ();
				return tile;
			}
			
		}

	}

}

