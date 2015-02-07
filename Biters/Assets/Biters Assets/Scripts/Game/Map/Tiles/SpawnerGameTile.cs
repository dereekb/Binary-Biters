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

		public SpawnerGameTile (ISpawnerGameTileDelegate SpawnDelegate) : base () {
			this.SpawnDelegate = SpawnDelegate;
		}

		public SpawnerGameTile (ISpawnerGameTileDelegate SpawnDelegate, WorldDirection Direction) : base (Direction) {
			this.SpawnDelegate = SpawnDelegate;	
		}
		
		public SpawnerGameTile (ISpawnerGameTileDelegate SpawnDelegate, Vector3 Direction) : base (Direction) {
			this.SpawnDelegate = SpawnDelegate;	
		}
		
		#endregion

		#region Update

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
			entity.Map = this.Map;	//Share the map.
			this.Map.AddEntity(entity, this.MapTilePosition);
			this.SpawnTimer.Reset ();
		}

		#endregion

	}

	/*
	 * Factory for entities that are spawned.
	 */
	public interface ISpawnerGameTileDelegate : IFactory<BitersMapEntity> {}

	/*
	 * Default class that uses a Lambda expression to spawn elements.
	 */
	public class SpawnerGameTileDelegate : ISpawnerGameTileDelegate {

		private readonly Func<BitersMapEntity> SpawnFunction;

		public SpawnerGameTileDelegate(Func<BitersMapEntity> SpawnFunction) {
			this.SpawnFunction = SpawnFunction;
		}

		public BitersMapEntity Make() {
			return this.SpawnFunction.Invoke ();
		}
	}

}

