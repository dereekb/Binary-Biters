using System;
using UnityEngine;
using Biters.Game;
using Biters.Utility;

namespace Biters.Debugging.Zombies
{
	/*
	 * Debug/Demonstration Tile used for spawning Zombie entities.
	 * 
	 * It is recommended you first look at the Zombie class.
	 */
	public class GraveyardTile : BitersGameTile
	{
		
		public const string DirectionalTileId = "Entity.Debug.Graveyard";
		
		public static readonly Material SpawnerMat = ResourceLoader.Load["Tiles_Concrete"].Material;

		/*
		 * Timer helper class. 
		 * 
		 * Use this incase you need to increase a value periodically and do something at a point.
		 */
		public Timer SpawnTimer = new Timer(3.5f);

		public int MaxSpawnCount = 1;
		public int HaveSpawnedCount = 0;

		public int SpawnCount = 1;

		public GraveyardTile () {}

		#region Initialize

		/*
		 * Called when the element is first added to the map. 
		 * 
		 * The Map and TilePosition are available at this point.
		 * 
		 * See description in Zombie class for more info.
		 */
		public override void Initialize ()
		{
			this.GameObject.GetComponent<Renderer>().material = SpawnerMat;
			base.Initialize ();
		}
		
		#endregion

		#region Entity
		
		public override string EntityId {
			get {
				return DirectionalTileId;
			}
		}
		
		#endregion

		#region Events

		/*
		 * Register for map events here.
		 */
		public override void RegisterForEvents() {
			this.Map.RegisterForGameMapEvent (this, GameMapEvent.EntityRemoved);
		}
		
		protected override void HandleGameMapEvent(GameMapEventInfo Info) {
			
			switch (Info.GameMapEvent) {
			case GameMapEvent.EntityRemoved:

				/*
				 * When a zombie dies, reduce the counter to allow another zombie to spawn.
				 */
				if (Info.Entity is Zombie) {
					this.HaveSpawnedCount -= 1;
				}
				break;
			}
			
		}

		#region Update

		/*
		 * Try to spawn a zombie each update cycle.
		 */
		public override void Update ()
		{
			this.TryAndSpawn();
		}

		#endregion

		#region Spawning

		public bool CanSpawn 
		{
			get {
				return !this.AllZombiedOut && (this.SpawnTimer.UpdateAndCheck ());
			}
		}

		public bool AllZombiedOut 
		{	
			get {
				return (this.MaxSpawnCount < this.HaveSpawnedCount);
			}
		}

		public void TryAndSpawn()
		{
			if (this.CanSpawn) {
				this.SpawnZombies ();
			}
		}

		/*
		 * Shows how to add entities to the map.
		 */
		public void SpawnZombies ()
		{
			for (int i = 0; i < SpawnCount; i += 1) {

				/*
				 * To add an entity at this position, this is all you need to do! Pretty simple.
				 */
				Zombie zombie = new Zombie ();
				this.Map.AddEntity(zombie, this.MapTilePosition);
			}
			
			this.HaveSpawnedCount += 1;
			this.SpawnTimer.Reset ();
		}

		#endregion
		
		#endregion

	}

}

