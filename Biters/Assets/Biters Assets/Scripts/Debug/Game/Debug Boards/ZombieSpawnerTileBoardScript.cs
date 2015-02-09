using System;
using UnityEngine;
using Biters.Game;
using Biters;
using Biters.Debugging.Zombies;
using Biters.Debugging.Generators;

namespace AssemblyCSharp
{
	/*
	 * Zombies!!!
	 */
	public class ZombieSpawnerTileBoardScript : MonoBehaviour
	{
		private BitersGameMap Map;
		
		public ZombieSpawnerTileBoardScript () {}
		
		public void Awake()
		{
			DebugGameBoardFactory BoardFactory = new DebugGameBoardFactory ();
			
			BoardFactory.Generator.BoardXSize = 25;
			BoardFactory.Generator.BoardYSize = 15;

			DebugZombieMapGeneratorFactory zombies = new DebugZombieMapGeneratorFactory ();
			BoardFactory.Factory = zombies;
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

