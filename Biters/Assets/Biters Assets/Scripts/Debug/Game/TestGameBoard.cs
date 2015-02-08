using System;
using UnityEngine;
using Biters.Game;
using Biters;

namespace Biters.Debugging
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
			this.Map = new GameMap<BitersGameTile, BitersMapEntity> (MapObject, MapDelegate);

			Debug.Log ("Creating new world...");
		}

		public void Start ()
		{
			Map.ResetWorld ();
			Debug.Log ("Finished resetting world...");
		}
		
		public void Update ()
		{
			Map.Update ();
		}

	}
	
	/*
	 * Multiple Map Board.
	 */
	public class MultipleMapTestGameBoard : MonoBehaviour
	{
		private TestMapGenerator TestMapGenerator;
		private GameMap<BitersGameTile, BitersMapEntity> Map;
		//private GameMap<BitersGameTile, BitersMapEntity> MapB;

		public MultipleMapTestGameBoard () {}
		
		public void Awake()
		{
			Console.WriteLine ("Waking up...");
			
			TestMapGenerator TestMapGenerator = new TestMapGenerator();
			
			GameObject MapObject = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			MapObject.transform.localScale = new Vector3 (0.2f, 0.2f, 0.2f);
			IGameMapDelegate<BitersGameTile, BitersMapEntity> MapDelegate = TestMapGenerator.Make ();
			
			//TODO: Issue of "Now Game Tiles NEED to have this map set before they are created, but the map also needs a generator first..."
			this.Map = new GameMap<BitersGameTile, BitersMapEntity> (MapObject, MapDelegate);
			
			/*
			 * TODO: Need to ensure that map objects use local position 
			 * (position relative to the map, as it is the Transform Parent) so maps can move all together.
			 * 
			GameObject MapBObject = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			MapBObject.transform.localScale = new Vector3 (0.2f, 0.2f, 0.2f);

			IGameMapDelegate<BitersGameTile, BitersMapEntity> MapBDelegate = TestMapGenerator.Make ();
			this.MapB = new GameMap<BitersGameTile, BitersMapEntity> (MapBObject, MapBDelegate);

			Debug.Log ("Creating new world...");

			Map.ResetWorld ();
			MapB.ResetWorld ();

			MapBObject.transform.position = new Vector3 (-20f, -20f, -4f);
			*/
			
			Debug.Log ("Finished resetting world...");
		}
		
		public void Update ()
		{
			Debug.Log ("Update.");
			//Debug.Log ("Updating: %s", Time.time);
			Map.Update ();
			//MapB.Update ();
		}
		
	}

}

