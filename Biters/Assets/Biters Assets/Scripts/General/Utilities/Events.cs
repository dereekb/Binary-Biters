using System;
using System.Collections;
using System.Collections.Generic;

namespace Biters
{
	public interface EventInfo {
		
		string EventName { get; }
		
	}

	public interface EventListener {
		
		void HandleEvent(EventInfo EventInfo);

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
	public sealed class EventSystem<T, I> where I : EventInfo
	{
		/*
		 * Keeps the listeners dictionary clean.
		 */
		public bool KeepClean = false;

		private Dictionary<T, HashSet<EventListener>> listeners;

		public EventSystem() {
			this.listeners = new Dictionary<T, HashSet<EventListener>> ();
		}

		#region Internal 

		private void InitObserverSet(T EventType) {
			if (listeners.ContainsKey(EventType) == false) {
				this.ResetObserverSet(EventType);
			}
		}

		private void NotifyObservers(T EventType, I EventInfo) {
			HashSet<EventListener> set = listeners [EventType];

			foreach (EventListener listener in set) {
				listener.HandleEvent(EventInfo);
			}
		}

		private HashSet<EventListener> ResetObserverSet(T EventType) {
			HashSet<EventListener> set = new HashSet<EventListener>();
			listeners.Add(EventType, set);
			return set;
		}
		
		private void CheckObserverSetRemoval(T EventType) {
			HashSet<EventListener> set;
			
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

		//Add Observer
		public void AddObserver(EventListener Listener, T EventType) {
			HashSet<EventListener> set;
			
			if (this.listeners.TryGetValue (EventType, out set) == false) {
				set = this.ResetObserverSet(EventType);
			}
		}

		//Remove Observer
		public void RemoveObserver(EventListener Listener, T EventType) {
			HashSet<EventListener> set;
			
			if (this.listeners.TryGetValue (EventType, out set)) {
				set.Remove(Listener);
				this.CheckObserverSetRemoval(EventType);
			}
		}

		public void RemoveObserver(EventListener Listener) {
			foreach (KeyValuePair<T, HashSet<EventListener>> pair in this.listeners) {
				HashSet<EventListener> set = pair.Value;
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

