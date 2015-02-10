using System;
using UnityEngine;
using Biters.Game;
using Biters.Utility;

namespace Biters.Debugging.Zombies
{

	public class GraveyardTile : BitersGameTile
	{
		
		public const string DirectionalTileId = "Entity.Debug.Graveyard";
		
		public Timer SpawnTimer = new Timer(3.5f);

		public int MaxSpawnCount = 1;
		public int HaveSpawnedCount = 0;

		public int SpawnCount = 1;

		public GraveyardTile () {}
		
		public static readonly Material SpawnerMat = ResourceLoader.Load["Tiles_Concrete"].Material;
		
		#region Initialize
		
		public override void Initialize ()
		{
			base.Initialize ();
			this.GameObject.renderer.material = SpawnerMat;
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
		
		public override void RegisterForEvents() {
			this.Map.RegisterForGameMapEvent (this, GameMapEvent.RemoveEntity);
		}
		
		protected override void HandleGameMapEvent(GameMapEventInfo Info) {
			
			switch (Info.GameMapEvent) {
			case GameMapEvent.RemoveEntity:

				if (Info.Entity is Zombie) {
					this.HaveSpawnedCount -= 1;		//Respawn a killed zombie.
				}

				//this.TryAndSpawn();

				/*
				 * TODO: This is not allowed here, as it will cause another event to fire while in the events.
				 * 
				 * Figure out a way to safely fire events, or queue up events in the EventSystem.
				 */
				break;
			}
			
		}

		#region Update

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

		public void SpawnZombies ()
		{
			for (int i = 0; i < SpawnCount; i += 1) {
				
				Zombie zombie = new Zombie ();
				zombie.Map = this.Map;	//Share the map.
				
				this.Map.AddEntity(zombie, this.MapTilePosition);
			}
			
			this.HaveSpawnedCount += 1;
			this.SpawnTimer.Reset ();
		}

		#endregion
		
		#endregion

	}

}

