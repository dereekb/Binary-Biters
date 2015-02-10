using System;
using System.Collections;
using System.Collections.Generic;

namespace Biters.Utility
{
	public interface IEventInfo {

		/*
		 * Unique identifier for the implementing type.
		 */
		string EventInfoId { get; }

		/*
		 * Unique event name.
		 */
		string EventName { get; }

	}

	public interface IEventListener {
		
		void HandleEvent(IEventInfo EventInfo);

		/*
		 * TODO: Add "was unregistered" type function if needed. (Or better yet, extend EventSystem to add that.)
		 * 
		 * This design currently assumes that elements unregister themselves and already have that knowledge.
		 */
	}

	/*
	 * Basic event handling system.
	 * 
	 * Allows elements to register and listen for specific types.
	 */
	public sealed class EventSystem<T, I> where I : IEventInfo
	{
		/*
		 * Keeps the listeners dictionary clean.
		 */
		public bool KeepClean = false;

		private Dictionary<T, HashSet<IEventListener>> listeners;

		public EventSystem() {
			this.listeners = new Dictionary<T, HashSet<IEventListener>> ();
		}

		#region Internal 

		private void InitObserverSet(T EventType) {
			if (listeners.ContainsKey(EventType) == false) {
				this.ResetObserverSet(EventType);
			}
		}

		private void NotifyObservers(T EventType, I EventInfo) {
			HashSet<IEventListener> set = listeners [EventType];

			foreach (IEventListener listener in set) {
				listener.HandleEvent(EventInfo);
			}
		}

		private HashSet<IEventListener> ResetObserverSet(T EventType) {
			HashSet<IEventListener> set = new HashSet<IEventListener>();
			listeners.Add(EventType, set);
			return set;
		}
		
		private void CheckObserverSetRemoval(T EventType) {
			HashSet<IEventListener> set;
			
			if (this.KeepClean && this.listeners.TryGetValue (EventType, out set)) {
				if (set.Count == 0) {
					RemoveObserverSet(EventType);
				}
			}
			
		}

		private void RemoveObserverSet(T EventType) {
			listeners.Remove (EventType);
		}

		#endregion

		#region Observers

		public int EventTypeCount {

			get {
				return this.listeners.Count;
			}

		}

		public int ListenersForEvent(T EventType) {
			int count = 0;

			if (this.listeners.ContainsKey(EventType)) {
				count = this.listeners[EventType].Count;
			}

			return count;
		}

		//Add Observer
		public void AddObserver(IEventListener Listener, T EventType) {
			HashSet<IEventListener> set;
			
			if (this.listeners.TryGetValue (EventType, out set) == false) {
				set = this.ResetObserverSet(EventType);
			}

			set.Add (Listener);
		}

		//Remove Observer
		public void RemoveObserver(IEventListener Listener, T EventType) {
			HashSet<IEventListener> set;
			
			if (this.listeners.TryGetValue (EventType, out set)) {
				set.Remove(Listener);
				this.CheckObserverSetRemoval(EventType);
			}
		}

		public void RemoveObserver(IEventListener Listener) {
			foreach (KeyValuePair<T, HashSet<IEventListener>> pair in this.listeners) {
				HashSet<IEventListener> set = pair.Value;
				set.Remove(Listener);
			}
		}

		//Broadcast
		public void BroadcastEvent(T EventType, I EventInfo) {
			if (listeners.ContainsKey(EventType)) {
				this.NotifyObservers(EventType, EventInfo);
			}
		}

		#endregion

	}

}

