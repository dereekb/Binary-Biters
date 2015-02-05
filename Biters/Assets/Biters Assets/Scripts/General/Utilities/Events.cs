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
		
	}

	/*
	 * Basic event handling system.
	 * 
	 * Allows elements to register and listen for specific types.
	 */
	public sealed class EventSystem<T, I> where I : EventInfo
	{

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

			if (this.listeners.TryGetValue (EventType, out set)) {
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
				
				if (set.Count == 0) {
					RemoveObserverSet(EventType);
				}
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

