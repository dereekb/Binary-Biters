using System;
using UnityEngine;
using Biters.Game;
using Biters;
using Biters.Debugging.Generators;

namespace Biters.Debugging.Board.Script
{
	
	/*
	 * Test Gameboard. The first of it's kind.
	 */
	public class RandomSpawnerTileBoardScript : MonoBehaviour
	{
		private BitersGameMap Map;
		
		public RandomSpawnerTileBoardScript() {}
		
		public void Awake()
		{
			DebugGameBoardFactory BoardFactory = new DebugGameBoardFactory ();
			BoardFactory.Factory = new DebugRandomTileMapGeneratorFactory ();
			this.Map = BoardFactory.Make();
		}
		
		public void Start ()
		{
			Map.ResetWorld ();
		}
		
		public void Update ()
		{
			Map.Update ();
		}
		
	}

}

