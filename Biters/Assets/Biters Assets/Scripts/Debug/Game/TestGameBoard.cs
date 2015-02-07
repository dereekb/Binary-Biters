using System;
using UnityEngine;
using Biters.Game;
using Biters;

namespace Biters.Testing
{
	/*
	 * Test Gameboard. The first of it's kind.
	 */
	public class TestGameBoard : MonoBehaviour
	{
		private TestMapGenerator TestMapGenerator;
		private GameMap<BitersGameTile, BitersMapEntity> Map;

		public TestGameBoard () {}
		
		public void Start()
		{
			TestMapGenerator TestMapGenerator = new TestMapGenerator();
			
			GameObject MapObject = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			IGameMapDelegate<BitersGameTile, BitersMapEntity> MapDelegate = TestMapGenerator.Make ();

			//TODO: Issue of "Now Game Tiles NEED to have this map set before they are created, but the map also needs a generator first..."
			this.Map = new GameMap<BitersGameTile, BitersMapEntity> (MapObject, MapDelegate);

			Map.ResetWorld ();

			Console.WriteLine ("Finished resetting world...");
		}
		
		public void Update () 
		{
			Map.Update ();
		}

	}
		
}

