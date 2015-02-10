using System;
using UnityEngine;
using Biters;
using Biters.Utility;

namespace Biters.Game
{
	/*
	 * Directional Tile that spawns things, and sets them on their way.
	 */ 
	public class SpawnerGameTile : DirectionalGameTile
	{
		public int SpawnCount = 0;
		public int SpawnMax = 100;
		public Timer SpawnTimer = new Timer(1.5f);

		//Spawning Delegate
		public ISpawnerGameTileDelegate SpawnDelegate;

		#region Constructors
		
		public SpawnerGameTile (ISpawnerGameTileDelegate SpawnDelegate) : base () {
			this.SpawnDelegate = SpawnDelegate;
		}
		
		public SpawnerGameTile (ISpawnerGameTileDelegate SpawnDelegate, DirectionalGameTileType Type) : base (Type) {
			this.SpawnDelegate = SpawnDelegate;
		}

		public SpawnerGameTile (ISpawnerGameTileDelegate SpawnDelegate, DirectionalGameTileType Type, WorldDirection Direction) : base (Type, Direction) {
			this.SpawnDelegate = SpawnDelegate;
		}

		#endregion

		#region Update

		public override void Update() {
			this.TrySpawning ();
			base.Update ();
		}
	
		#endregion

		#region Spawning

		public void TrySpawning() {
			if (this.CanSpawn()) {
				this.SpawnEntity ();
			}
		}

		public virtual bool CanSpawn() {
			return (this.SpawnCount < SpawnMax) && (this.SpawnTimer.UpdateAndCheck ());
		}

		public virtual void SpawnEntity() {
			BitersGameEntity entity = this.SpawnDelegate.Make ();
			this.Map.AddEntity(entity, this.MapTilePosition);
			this.SpawnTimer.Reset ();
			this.SpawnCount += 1;
		}

		#endregion

	}

	/*
	 * Factory for entities that are spawned.
	 */
	public interface ISpawnerGameTileDelegate : IFactory<BitersGameEntity> {}

	/*
	 * Default class that uses a Lambda expression to spawn elements.
	 */
	public class SpawnerGameTileDelegate : ISpawnerGameTileDelegate {

		private readonly Func<BitersGameEntity> SpawnFunction;

		public SpawnerGameTileDelegate(Func<BitersGameEntity> SpawnFunction) {
			this.SpawnFunction = SpawnFunction;
		}

		public BitersGameEntity Make() {
			return this.SpawnFunction.Invoke ();
		}
	}

}

