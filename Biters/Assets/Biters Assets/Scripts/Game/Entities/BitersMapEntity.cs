using System;
using UnityEngine;

namespace Biters.Game
{
	/*
	 * Represents a Default Biters Map Entity.
	 */ 
	public abstract class BitersMapEntity : Entity
	{

		public BitersMapEntity () : this (GameObject.CreatePrimitive(PrimitiveType.Plane)) {}

		public BitersMapEntity (GameObject GameObject) : base (GameObject) {}

		#region Entity

		public abstract string BitersId { get; }

		#endregion

		#region Update
		
		public override void Update() {
			this.movement.Update();
			//TODO: Update anything else.
		}
		
		#endregion
	}
}

