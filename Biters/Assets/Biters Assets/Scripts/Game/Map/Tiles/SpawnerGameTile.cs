using System;
using UnityEngine;

namespace Biters.Game
{
	/*
	 * Directional Tile that spawns things, and sets them on their way.
	 */ 
	public class SpawnerGameTile : DirectionalGameTile
	{
		public Timer SpawnTimer = new Timer(10.0f);

		//Spawning Delegate
		public ISpawnerGameTileDelegate SpawnDelegate;

		#region Constructors

		public SpawnerGameTile (WorldDirection Direction) : base (Direction) {}
		
		public SpawnerGameTile (Vector3 Direction) : base (Direction) {}
		
		#endregion

		#region Update
		
		public override void AddedToMap(Map<IMapTile> Map, WorldPosition Position) {
			base.AddedToMap (Map, Position);
		}
		
		public override void RemovedFromMap(Map<IMapTile> Map) {
			base.RemovedFromMap (Map);
		}

		public override void Update() {
			this.TrySpawning ();
		}
		
		#endregion

		#region Spawning

		public void TrySpawning() {
			if (this.SpawnTimer.Done) {
				this.SpawnEntity();
			}
		}

		public void SpawnEntity() {
			BitersMapEntity entity = this.SpawnDelegate.Make ();
			this.Map.AddEntity (entity as IGameMapEntity, this.MapTilePosition);
			this.SpawnTimer.Reset ();
		}

		#endregion

	}

	/*
	 * Factory for entities that are spawned.
	 */
	public interface ISpawnerGameTileDelegate : IFactory<BitersMapEntity> {
		
	}

}

