using System;
using UnityEngine;

namespace Biters.Game
{
	/*
	 * Represents a Default Biters Map Entity.
	 */ 
	public abstract class BitersGameEntity : BitersGameElement, IGameMapEntity
	{

		public BitersGameEntity () : this (GameObject.CreatePrimitive(PrimitiveType.Cube)) {}

		public BitersGameEntity (GameObject GameObject) : base (GameObject) {}

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

