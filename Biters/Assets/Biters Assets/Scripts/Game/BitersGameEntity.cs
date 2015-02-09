using System;
using UnityEngine;
using Biters;
using Biters.Utility;

namespace Biters.Game
{

	/*
	 * Basic Game Entity that attaches to a GameMap of BitersGameTiles and BitersMapEntities. 
	 */
	public abstract class BitersGameEntity : Entity, IEventListener
	{
		//Game Map of the tile.
		private BitersGameMap map = null;

		public BitersGameEntity (GameObject GameObject) : base(GameObject) {}

		#region Accessors

		public BitersGameMap Map {
			
			get {
				return this.map;
			}
			
			set {
				if (this.map == null) {
					this.map = value;
				}
			}
			
		}

		#endregion
		
		#region Entity
		
		public abstract string EntityId { get; }
		
		#endregion

		#region Initialization

		#region Optional

		public virtual void Initialize() {
			//Override to initialize entity further.
		}
		
		public virtual void Destroy() {
			//Override to destroy entity elements.
		}
		
		public virtual void RegisterForEvents() {
			//Override to register for events. Is called after initialize.
		}
		
		public virtual void UnregisterFromEvents() {
			//Override to unregister from other events. Is called before destroy.
		}

		#endregion

		#region Required
		
		private void UnregisterFromGameMapEvents() {
			//Automatically unregisters from the map.
			this.map.UnregisterFromGameMapEvents (this);
		}
		
		public virtual void AddedToGameMap(WorldPosition Position) {
			this.Initialize ();
			this.RegisterForEvents ();
		}
		
		public virtual void RemovedFromGameMap() {
			this.UnregisterFromGameMapEvents ();
			this.UnregisterFromEvents ();
			this.map = null;
			this.Destroy ();
			GameObject.Destroy (this.GameObject);
		}

		#endregion

		#endregion
		
		#region Update
		
		public override void Update() {
			//Override to do something with the update.	
		}
		
		#endregion

		#region Events
		
		/*
		 * Helper function that automatically casts the known event types.
		 */
		public void HandleEvent(IEventInfo eventInfo) {

			switch (eventInfo.EventInfoId) {
			case MapEventInfo.MapEventInfoId: 
				this.HandleMapEvent((MapEventInfo) eventInfo); break;
			case GameMapEventInfo.GameMapEventInfoId:
				this.HandleGameMapEvent((GameMapEventInfo) eventInfo); break;
			}
			
		}
		
		protected virtual void HandleMapEvent(MapEventInfo Info) {
			//Override in sub class.
		}
		
		protected virtual void HandleGameMapEvent(GameMapEventInfo Info) {
			//Override in sub class.
		}
		
		#endregion

	}

}

