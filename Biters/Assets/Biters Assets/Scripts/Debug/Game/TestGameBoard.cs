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
		
		public void Awake()
		{
			Console.WriteLine ("Waking up...");

			TestMapGenerator TestMapGenerator = new TestMapGenerator();
			
			GameObject MapObject = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			MapObject.transform.localScale = new Vector3 (0.2f, 0.2f, 0.2f);
			IGameMapDelegate<BitersGameTile, BitersMapEntity> MapDelegate = TestMapGenerator.Make ();

			//TODO: Issue of "Now Game Tiles NEED to have this map set before they are created, but the map also needs a generator first..."
			this.Map = new GameMap<BitersGameTile, BitersMapEntity> (MapObject, MapDelegate);

			Debug.Log ("Creating new world...");

			Map.ResetWorld ();

			Debug.Log ("Finished resetting world...");
		}
		
		public void Update ()
		{
			Debug.Log ("Update.");
			//Debug.Log ("Updating: %s", Time.time);
			Map.Update ();
		}

	}
		
}

