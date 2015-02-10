using System;
using UnityEngine;
using Biters.Game;
using Biters.Utility;

namespace Biters.Debugging.Zombies
{

	/*
	 * Debug/Demonstration zombie character.
	 * 
	 * Chases other nearby map entities and eat them. After eating, gets slightly bigger.
	 */
	public class Zombie : BitersMapEntity {
		
		public static readonly string ZombieId = "Entity.Debug.Zombie";
		public static readonly Vector3 ZombieScale = new Vector3 (2.5f, 2.5f, 2.5f);
		public static readonly Material ZombieMat = ResourceLoader.Load["Entity_Zombie"].Material;

		public ProtectedMovement ZombieMovement;
		public BitersMapEntity ZombieTarget;
		public int Health = 10;

		public Zombie() : base () {
			this.ZombieMovement = new ProtectedMovement (this);
			this.movement = this.ZombieMovement;
		}
		
		#region Entity
		
		public override string EntityId {
		
			get {
				return ZombieId;
			}
		
		}
		
		#endregion

		#region Initialization

		public override void Initialize() {
			this.gameObject.renderer.material = ZombieMat;
			this.gameObject.transform.localScale = ZombieScale;
			base.Initialize ();

			this.ChaseNewTarget ();
			Debug.Log(String.Format("Zombies {0} initalized.", this));
		}

		public override void RegisterForEvents() {

			/*
			 * Register for events that can help us find nearby entities, or if our entity exited the world.
			 */
			this.Map.RegisterForGameMapEvent (this, GameMapEvent.RemoveEntity);

		}

		#endregion

		#region Events
		
		protected override void HandleGameMapEvent(GameMapEventInfo Info) {

			switch (Info.GameMapEvent) {
			case GameMapEvent.RemoveEntity:
				//Debug.Log(String.Format("Zombies's {0} saw {1} just die.", this, Info.Entity));

				if (Info.Entity == this.ZombieTarget) {
					Debug.Log(String.Format("Zombies's {0} target '{1}' is exiting the plane of existence...", this, this.ZombieTarget));
					this.ChaseNewTarget();
				}
				break;
			}

		}


		#endregion
		
		#region Update
		
		public override void Update() {
			base.Update ();			//Update movement before checking the target.
			this.CheckTarget ();
		}
		
		#endregion

		#region Zombie
		
		public bool HasTarget {
			get {
				return (this.ZombieTarget != null);
			}
		}

		//Attempt to eat the target.
		public void CheckTarget() {
			if (this.HasTarget && this.Position == this.ZombieTarget.Position) {
				Debug.Log(String.Format("Zombies {0} caught it's target.", this));
				this.EatTarget();
				this.ChaseNewTarget();	//Chase a new target.
			}
		}

		public BitersMapEntity FindNewTarget(BitersMapEntity avoid) {

			//Create a new map query.
			IEntityWorldPositionQuery<BitersMapEntity> query = this.Map.EntityPositionQuery;
			query.TargetPosition = this.Position;

			//Don't want to eat ourselves!
			query.Exclude = new System.Collections.Generic.HashSet<BitersMapEntity>();
			query.Exclude.Add (this);

			//Avoid that other thing too!
			if (avoid != null) {
				query.Exclude.Add(avoid);
			}

			//Find the closest entity.
			BitersMapEntity newTarget = query.SearchClosestEntity ();
			return newTarget;
		}
		
		public void ChaseNewTarget() {
			this.ChaseNewTarget (this.ZombieTarget);
		}

		public void ChaseNewTarget(BitersMapEntity avoid) {
			BitersMapEntity target = this.FindNewTarget (avoid);
			this.ZombieMovement.Clear();
			
			if (target != null && target != this) {
				WalkToTargetAutoPilot pilot = new WalkToTargetAutoPilot(target, this);
				pilot.Speed = (this.Health / 7.5f);
				
				Debug.Log(String.Format("Zombies {0} sopped chasing.", this));

				this.ZombieMovement.Enqueue(pilot);
				this.ZombieTarget = target;

				Debug.Log(String.Format("Zombies {0} started chasing a new target {1}.", this, this.ZombieTarget));
			} else {
				Debug.Log(String.Format("Zombies {0} failed to find a new target.", this));
			}
		}

		public void EatTarget() {
			//TODO: Make bigger or whatever.
			Zombie targetZombie = this.ZombieTarget as Zombie;

			if (targetZombie != null) {
				this.Fight (targetZombie);
			} else {
				this.Map.RemoveEntity (this.ZombieTarget);
				this.Health += 1;
			}

		}

		public void Fight(Zombie Zombie) {
			
			if (this.Health >= Zombie.Health) {
				this.Health -= (Zombie.Health - 1);
				Zombie.Die();
			} else {
				Zombie.Health -= (this.Health - 1);
				this.Die();
			}
			
			Debug.Log(String.Format("Zombies fought: {0} vs {1}", this, Zombie));
		}

		public void Die() {
			Debug.Log(String.Format("Zombies died. {0}", this));
			this.Map.RemoveEntity (this);


			//TODO: Do other things. Maybe spawn a Graveyard?

		}

		#endregion

		public override string ToString ()
		{
			return string.Format ("[Zombie: Position={0}, Health={1}, HasTarget={2}]", this.Position, this.Health, (this.ZombieTarget != null));
		}
		
	}

}

