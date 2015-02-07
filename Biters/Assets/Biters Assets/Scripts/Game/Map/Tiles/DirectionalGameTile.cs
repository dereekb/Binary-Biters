using System;
using UnityEngine;

namespace Biters.Game
{

	/*
	 * Basic game tile that moves entities that enter it's tiles towards the center, and then in a new direction.
	 */
	public class DirectionalGameTile : BitersGameTile {

		//Speed at which to move entites that enter this tile.
		public Vector3 EntityMoveSpeed = new Vector3(1.0f, 1.0f);

		//Direction to move units towards.
		public Vector3 Direction = new Vector3(0.0f, 1.0f);

		#region Constructors

		public DirectionalGameTile () : base () {}

		public DirectionalGameTile (WorldDirection Direction) : this () {
			this.SetDirection (Direction);
		}

		public DirectionalGameTile (Vector3 Direction) : this () {
			this.SetDirection (Direction);
		}

		#endregion

		#region Accessors
		
		public void SetDirection(Vector3 Direction) {
			this.Direction = Direction;
		}
		
		public void SetDirection(WorldDirection Direction) {
			this.Direction = Direction.Vector();
		}

		#endregion
		
		#region Events

		public override void RegisterForEvents() {
			this.Map.RegisterForEvent (this, GameMapEvent.EntityEnteredTile);
			//this.Map.RegisterForEvent (this, GameMapEvent.EntityExitedTile);
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

			//If tile == this tile
			IGameMapEntity entity = Info.Entity;
			Vector3 middle = Info.Map.GetPositionVector(this.MapTilePosition);
			
			AutoPilotQueue movementQueue = new AutoPilotQueue();
			
			//Move Towards Middle
			movementQueue.Add(new WalkToPositionAutoPilot(middle, this.EntityMoveSpeed));
			
			//Move To Redirected Direction
			Vector3 direction = this.Direction;
			direction.Scale(this.EntityMoveSpeed);
			movementQueue.Add(new WalkAutoPilot(direction));
			
			//Set Movement
			entity.Movement.AutoPilot = movementQueue;
		}

		#endregion

	}
	
	//Vector Extension 
	public static class WorldDirectionalGameTileInfo
	{

		public static Vector3 Vector(this WorldPositionChange Change)
		{
			Vector3 direction = new Vector3 ();
			direction.x = Change.dX;
			direction.y = Change.dY;
			return direction;
		}

		public static Vector3 Vector(this WorldDirection direction)
		{
			WorldPositionChange change = direction.PositionChange ();
			return change.Vector();
		}
		
	}

}

