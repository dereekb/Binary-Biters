using System;
using UnityEngine;

namespace Biters.Game
{
	/*
	 * Represents a Default Biters Map Entity.
	 */ 
	public abstract class BitersMapEntity : Entity, IGameMapEntity
	{

		public BitersMapEntity () : this (GameObject.CreatePrimitive(PrimitiveType.Cube)) {}

		public BitersMapEntity (GameObject GameObject) : base (GameObject) {}

		private IGameMap<BitersGameTile, BitersMapEntity> map;

		#region Entity

		public abstract string BitersId { get; }

		#endregion
		
		#region Map

		public IGameMap<BitersGameTile, BitersMapEntity> Map {

			get {
				return this.map;
			}

			set {
				if (this.map == null) {
					this.map = value;
				}
			}

		}

		public virtual void Initialize() {
			//Override to initialize entity further.
		}
		
		public virtual void Destroy() {
			//Override to initialize entity further.
		}

		public void AddedToGameMap(WorldPosition Position) {
			this.GameObject.transform.position = this.Map.GetPositionVector (Position);
			this.Initialize ();
		}
		
		public void RemovedFromGameMap() {
			this.map = null;
			this.Destroy ();
		}
		
		#endregion

		#region Update
		
		public override void Update() {
			this.movement.Update();
			this.Transform.rotation = Quaternion.AngleAxis(90, Vector3.forward);
			//TODO: Update anything else.
		}
		
		#endregion
	}
}

