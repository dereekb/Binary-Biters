using System;
using NUnit;
using NUnit.Framework;
using UnityEditor;
using Biters.Utility;

namespace Biters.Testing
{
	
	class TestEventListener : IEventListener {
		
		public int EventsHandled = 0;
		
		public void HandleEvent(IEventInfo EventInfo) {
			EventsHandled += 1;
		}
		
	}

	[TestFixture()]
	public class EventSystemTests
	{
		
		[Test()]
		public void TestEventSystemAdding () {
			EventSystem<MapEvent, MapEventInfo> mapEvents = new EventSystem<MapEvent, MapEventInfo>();
			
			int customCount = 3;
			
			for (int i = 0; i < customCount; i += 1) {
				mapEvents.AddObserver (new TestEventListener (), MapEvent.Custom);
			}
			
			Assert.AreEqual (1, mapEvents.EventTypeCount);
			Assert.AreEqual (customCount, mapEvents.ListenersForEvent(MapEvent.Custom));
		}

		[Test()]
		public void TestEventSystemRemoving () {
			EventSystem<MapEvent, MapEventInfo> mapEvents = new EventSystem<MapEvent, MapEventInfo>();

			TestEventListener listener = new TestEventListener();
			mapEvents.AddObserver (listener, MapEvent.Custom);
			mapEvents.AddObserver (listener, MapEvent.Init);
			mapEvents.AddObserver (listener, MapEvent.Reset);

			Assert.AreEqual (3, mapEvents.EventTypeCount);
			Assert.AreEqual (1, mapEvents.ListenersForEvent(MapEvent.Custom));
			Assert.AreEqual (1, mapEvents.ListenersForEvent(MapEvent.Init));
			Assert.AreEqual (1, mapEvents.ListenersForEvent(MapEvent.Reset));
			
			mapEvents.RemoveObserver (listener, MapEvent.Custom);
			Assert.AreEqual (0, mapEvents.ListenersForEvent(MapEvent.Custom));

			mapEvents.RemoveObserver (listener);
			Assert.AreEqual (3, mapEvents.EventTypeCount);	//Still 3, unless the mapEvents has KeepClean set to true.
			Assert.AreEqual (0, mapEvents.ListenersForEvent(MapEvent.Init));
			Assert.AreEqual (0, mapEvents.ListenersForEvent(MapEvent.Reset));

		}
		
		[Test()]
		public void TestEventSystemCalling () {
			EventSystem<MapEvent, MapEventInfo> mapEvents = new EventSystem<MapEvent, MapEventInfo>();
			
			TestEventListener listener = new TestEventListener();
			mapEvents.AddObserver (listener, MapEvent.Custom);
			
			Assert.AreEqual (1, mapEvents.EventTypeCount);
			Assert.AreEqual (1, mapEvents.ListenersForEvent(MapEvent.Custom));
			Assert.AreEqual (0, listener.EventsHandled);

			mapEvents.BroadcastEvent (MapEvent.Custom, null);
			Assert.AreEqual (1, listener.EventsHandled);
		}

	}
	
}

