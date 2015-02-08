using System;
using UnityEngine;

namespace Biters.Game
{
	/*
	 * Represents a Default Biters Map Entity.
	 */ 
	public abstract class BitersMapEntity : BitersGameEntity, IGameMapEntity
	{

		public BitersMapEntity () : this (GameObject.CreatePrimitive(PrimitiveType.Cube)) {}

		public BitersMapEntity (GameObject GameObject) : base (GameObject) {}

		#region Map

		public sealed override void AddedToGameMap(WorldPosition Position) {
			this.GameObject.transform.position = this.Map.GetPositionVector (Position);
			base.AddedToGameMap (Position);
		}

		#endregion

		#region Update
		
		public override void Update() {
			this.movement.Update();
		}
		
		#endregion

	}
}

