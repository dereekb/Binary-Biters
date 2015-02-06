using System;
using UnityEngine;

namespace Biters.Game
{
	/*
	 * Represents a default entity.
	 */ 
	public abstract class BitersMapEntity : Entity
	{
		public BitersMapEntity (GameObject GameObject) : base (GameObject)
		{

		}

		#region Update
		
		public override void Update() {
			this.movement.Update();
			//TODO: Update anything else.
		}
		
		#endregion
	}
}

