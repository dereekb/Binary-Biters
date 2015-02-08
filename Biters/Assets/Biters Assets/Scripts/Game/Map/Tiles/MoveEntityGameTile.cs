using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Biters;
using Biters.Utility;

namespace Biters.Game
{

	/*
	 * Abstract game tile that moves entities that enter it's tiles towards the center, and then in a new direction.
	 */
	public abstract class MoveEntityGameTile : BitersGameTile {

		//Factory which builds AutoPilots for entities that enter this tile.
		protected virtual IAutoPilotFactory MovementFactory { get; set; }

		#region Constructors
		
		protected MoveEntityGameTile (IAutoPilotFactory MovementFactory) : base() {}

		#endregion

		
		#region Events

		public override void RegisterForEvents() {
			this.Map.RegisterForGameMapEvent (this, GameMapEvent.EntityEnteredTile);
		}
		
		protected override void HandleGameMapEvent(GameMapEventInfo Info) {
			
			switch (Info.GameMapEvent) {
			case GameMapEvent.EntityEnteredTile:
				if (Info.Position.Equals(this.MapTilePosition)) {
					this.ChangeEntityMovement(Info);
				}
				break;
			}
			
		}
		
		#endregion

		#region Redirection

		protected void ChangeEntityMovement(GameMapEventInfo Info) {
			IGameMapEntity entity = Info.Entity;
			entity.Movement.AutoPilot = this.MovementFactory.Make(this, entity);
		}

		#endregion

	}

}

