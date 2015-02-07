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
			IFactory<World<BitersGameTile>> WorldFactory = TestMapGenerator.Import ("test");
			this.Map = new GameMap<BitersGameTile, BitersMapEntity> (MapObject, WorldFactory);
			Map.ResetWorld ();
		}
		
		public void Update () 
		{
			//Map.Update ();
		}

	}
		
}

