using System;
using UnityEngine;
using Biters.Game;
using Biters.Utility;

namespace Biters.Debugging
{

	/*
	 * Debug/Demonstration zombie character.
	 * 
	 * Chases other nearby map entities and eat them. After eating, gets slightly bigger.
	 */
	public class Zombie : BitersMapEntity {
		
		public static readonly string ZombieId = "Entity.Debug.Zombie";
		public static readonly Vector3 ZombieScale = new Vector3 (0.5f, 0.5f, 0.5f);
		public static readonly Material ZombieMat = ResourceLoader.Load["Entity_Zombie"].Material;
		
		public Zombie() : base () {}
		
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
		}
		
		public override void RegisterForEvents() {

			/*
			 * Register for events that can help us find nearby entities, or if our entity exited the world.
			 */
			this.Map.RegisterForGameMapEvent (this, GameMapEvent.EntityOutsideWorld);
			this.Map.RegisterForGameMapEvent (this, GameMapEvent.EntityEnteredTile);

		}

		#endregion

		#region Events
		
		protected override void HandleGameMapEvent(GameMapEventInfo Info) {

			switch (Info.GameMapEvent) {
			case GameMapEvent.EntityEnteredTile: 


				//TODO: Check to see if that entity is closer than the current target, if there is a current target.
				break;
				
			case GameMapEvent.EntityOutsideWorld:
				//TODO: Check to see if the target entity just left the world, and to find another target.
				break;
			}

		}

		#endregion

		public override string ToString ()
		{
			return string.Format ("[Zombie: Position={0}]", this.Position);
		}
		
	}

	public class Graveyard
	{


	}

}

