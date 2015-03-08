using System;
using UnityEngine;
using Biters.Game;
using Biters.Utility;

namespace Biters.Debugging.Zombies
{

	/*
	 * Debug/Demonstration zombie character.
	 * 
	 * This character is highly commented for explaining how entities in this system work.
	 * 
	 * This entity will chases other nearby map entities and eat them. 
	 * After eating, gets slightly bigger and faster.
	 * If it fights another zombie, the two will fight and lose health equal to the weakest zombie.
	 */
	public class Zombie : BitersGameEntity {
		
		public static readonly string ZombieId = "Entity.Debug.Zombie";
		public static readonly Vector3 ZombieScale = new Vector3 (2.5f, 2.5f, 2.5f);

		/*
		 * Static material to append to the GameObject.
		 * 
		 * Is loaded using the ResourceLoader utility that automatically searches and casts the correct type.
		 */
		public static readonly Material ZombieMat = ResourceLoader.Load["Entity_Zombie"].Material;

		/*
		 * ProtectedMovement. Is explained more in ChaseNewTarget() function.
		 */
		public ProtectedMovement ZombieMovement;

		/*
		 * The current zombie's target.
		 */
		public BitersGameEntity ZombieTarget;
		public int Health = 10;

		public Zombie() : base () {
			this.ZombieMovement = new ProtectedMovement (this);
			this.movement = this.ZombieMovement;
		}

		#region Initialization

		/*
		 * Initialize is called when the entity is added to the map.
		 * 
		 * At this point, the entity has access to the Map, its Movement and its GameObject.
		 * 
		 * Override this function to setup the object's visual components and any other initialization logic.
		 */
		public override void Initialize() {
			this.gameObject.GetComponent<Renderer>().material = ZombieMat;
			this.gameObject.transform.localScale = ZombieScale;
			base.Initialize ();

			this.ChaseNewTarget ();
			//Debug.Log(String.Format("Zombies {0} initalized.", this));
		}
		
		/*
		 * Also called when the entity is added to the map.
		 * 
		 * Override to register for any map events that may be necessary.
		 * 
		 * Register for events that can help us find nearby entities, or if our entity exited the world.
		 */
		public override void RegisterForEvents() {

			/*
			 * The zombie entity registers for when entities are removed to make sure it's target still exists.
			 */
			this.Map.RegisterForGameMapEvent (this, GameMapEvent.EntityRemoved);

		}

		#endregion

		#region Events

		/*
		 * Called when a GameMapEvent that this object has registered for is signaled.
		 * 
		 * Depending on the event, the GameMapEventInfo that is passed with it has a name, type, entity, and/or position.
		 */
		protected override void HandleGameMapEvent(GameMapEventInfo Info) {

			switch (Info.GameMapEvent) {
			case GameMapEvent.EntityRemoved:
				//Debug.Log(String.Format("Zombies's {0} saw {1} just die.", this, Info.Entity));

				if (Info.Entity == this.ZombieTarget) {
					//Debug.Log(String.Format("Zombies's {0} target '{1}' is exiting the plane of existence...", this, this.ZombieTarget));
					this.ChaseNewTarget();
				}
				break;
			}

		}


		#endregion
		
		#region Update

		/*
		 * The zombie's update loop.
		 * 
		 * The BitersGameEntity base class and it's base class update the entity's Movement.
		 */
		public override void Update() {
			//Update movement before checking the target.
			base.Update ();


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
				//Debug.Log(String.Format("Zombies {0} caught it's target.", this));
				this.EatTarget();
				this.ChaseNewTarget();	//Chase a new target.
			}
		}

		/*
		 * We find a new target using a map query.
		 * 
		 * Map queries are very effective ways of finding entities within a the map.
		 * 
		 * Check the IEntityWorldPositionQuery for more available functions.
		 */
		public BitersGameEntity FindNewTarget(BitersGameEntity avoid) {

			//Create a new map query to search for a new target.
			IEntityWorldPositionQuery<BitersGameEntity> query = this.Map.EntityPositionQuery;
			query.TargetPosition = this.Position;

			//The zombie doesn't want to eat itself.
			query.Exclude = new System.Collections.Generic.HashSet<BitersGameEntity>();
			query.Exclude.Add (this);

			//Avoid that other thing too!
			if (avoid != null) {
				query.Exclude.Add(avoid);
			}

			//Find the closest entity.
			BitersGameEntity newTarget = query.SearchClosestEntity ();
			return newTarget;
		}
		
		public void ChaseNewTarget() {
			this.ChaseNewTarget (this.ZombieTarget);
		}

		/*
		 * Chases after the new target.
		 * 
		 * Each Entity has a Movement class that handles moving the element.
		 * 
		 * The Movement has instances that implement the IAutoPilot interface.
		 * 
		 * There are various types of AutoPilots already provideded:
		 * - Walk
		 * - WalkForTime
		 * - WalkToTarget
		 * 
		 * Other elements can influence this element's AutoPilot freely. 
		 * The Zombie class has a ProtectedAutoPilot that only it can edit; any attempted external changes will be ignored.
		 */
		public void ChaseNewTarget(BitersGameEntity avoid) {
			BitersGameEntity target = this.FindNewTarget (avoid);
			this.ZombieMovement.Clear();
			
			if (target != null && target != this) {
				WalkToTargetAutoPilot pilot = new WalkToTargetAutoPilot(target, this);
				pilot.Speed = (this.Health / 7.5f);
				
				//Debug.Log(String.Format("Zombies {0} sopped chasing.", this));

				this.ZombieMovement.Enqueue(pilot);
				this.ZombieTarget = target;

				//Debug.Log(String.Format("Zombies {0} started chasing a new target {1}.", this, this.ZombieTarget));
			} else {
				//Debug.Log(String.Format("Zombies {0} failed to find a new target.", this));
			}
		}

		/*
		 * After the zombie reaches the target, we should remove the target from the map.
		 * 
		 * We can also try casting the target using the as keyword to see if it is another type.
		 */
		public void EatTarget() {

			/*
			 * Check if the current target that we reached is another zombie. If so, fight it!
			 */
			Zombie targetZombie = this.ZombieTarget as Zombie;

			if (targetZombie != null) {
				this.Fight (targetZombie);
			} else {

				/*
				 * Removes the entity from the map.
				 * 
				 * This entity will be removed at the end of the current Map's Update cycle,
				 * but before map events are processed.
				 * 
				 * See the SafeGameMap and GameMap's Update() functions for more info.
				 */
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
			
			//Debug.Log(String.Format("Zombies fought: {0} vs {1}", this, Zombie));
		}

		/*
		 * Remove the zombie from the map.
		 */
		public void Die() {
			//Debug.Log(String.Format("Zombies died. {0}", this));
			this.Map.RemoveEntity (this);
		}

		#endregion
		
		#region Entity
		
		/*
		 * String identifier to uniquely identify an element.
		 * 
		 * Can be used in a switch statement, insteadof/unlike the C# as keyword, if looking for a particular type in an event.
		 * 
		 */
		public override string EntityId {
			
			get {
				return ZombieId;
			}
			
		}
		
		#endregion

		public override string ToString ()
		{
			return string.Format ("[Zombie: Position={0}, Health={1}, HasTarget={2}]", this.Position, this.Health, (this.ZombieTarget != null));
		}
		
	}

}

