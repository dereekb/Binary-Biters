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
		public static readonly Material SpawnerMat = ResourceLoader.Load["Tiles_Concrete"].Material;

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

		public SpawnerGameTile (ISpawnerGameTileDelegate SpawnDelegate, DirectionalGameTileType Type, float MoveSpeed) : base (Type, MoveSpeed) {
			this.SpawnDelegate = SpawnDelegate;
		}

		/*
		public SpawnerGameTile (ISpawnerGameTileDelegate SpawnDelegate, WorldDirection Direction) : base (Direction) {
			this.SpawnDelegate = SpawnDelegate;	
		}
		
		public SpawnerGameTile (ISpawnerGameTileDelegate SpawnDelegate, Vector3 Direction) : base (Direction) {
			this.SpawnDelegate = SpawnDelegate;	
		}
		*/

		#endregion

		#region Initialize

		public override void Initialize ()
		{
			//this.GameObject.renderer.material = SpawnerMat;
			base.Initialize ();
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
			if (this.SpawnCount < SpawnMax) {
				if (this.SpawnTimer.UpdateAndCheck ()) {
						this.SpawnEntity ();
				}
			}
		}

		public void SpawnEntity() {
			BitersMapEntity entity = this.SpawnDelegate.Make ();
			entity.Map = this.Map;	//Share the map.

			this.Map.AddEntity(entity, this.MapTilePosition);
			this.SpawnTimer.Reset ();
			this.SpawnCount += 1;
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

