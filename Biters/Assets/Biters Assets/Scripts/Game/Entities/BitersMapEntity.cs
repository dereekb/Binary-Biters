using System;
using UnityEngine;

namespace Biters.Game
{
	/*
	 * Represents a Default Biters Map Entity.
	 */ 
	public abstract class BitersMapEntity : Entity, IGameMapEntity
	{

		public BitersMapEntity () : this (GameObject.CreatePrimitive(PrimitiveType.Plane)) {}

		public BitersMapEntity (GameObject GameObject) : base (GameObject) {}

		private GameMap<IGameMapTile, IGameMapEntity> map;

		#region Entity

		public abstract string BitersId { get; }

		#endregion
		
		#region Map

		public GameMap<IGameMapTile, IGameMapEntity> Map {

			get {
				return this.Map;
			}

		}

		public virtual void Initialize() {
			//Override to initialize entity further.
		}
		
		public virtual void Destroy() {
			//Override to initialize entity further.
		}

		public void AddedToGameMap(GameMap<IGameMapTile, IGameMapEntity> Map, WorldPosition Position) {
			this.map = Map;
			this.GameObject.transform.position = Map.GetPositionVector (Position);
			this.Initialize ();
		}
		
		public void RemovedFromGameMap(GameMap<IGameMapTile, IGameMapEntity> Map) {
			this.map = null;
			this.Destroy ();
		}
		
		#endregion

		#region Update
		
		public override void Update() {
			this.movement.Update();
			//TODO: Update anything else.
		}
		
		#endregion
	}
}

