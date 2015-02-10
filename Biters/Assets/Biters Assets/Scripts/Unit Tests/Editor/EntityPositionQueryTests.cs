using System;
using System.Collections;
using System.Collections.Generic;
using NUnit;
using NUnit.Framework;
using UnityEditor;
using Biters;
using Biters.Game;
using Biters.Utility;

namespace Biters.Testing
{

	[TestFixture()]
	public class EntityPositionQueryTests
	{

		//TODO: Create implementation of IEntityWorldPositionQueriable to use for testing, since we cannot use GameObject.

		public EntityPositionQueryTests () {}
		
		[Test()]
		public void TestQueries() {
			/*
			IEntityWorldPositionQuery<BitersMapEntity> query = map.EntityPositionQuery;

			//List<BitersMapEntity> entities = new List<BitersMapEntity> ();
			
			BitersMapEntity a = new Unii ();
			BitersMapEntity b = new Unii ();
			BitersMapEntity c = new Unii ();

			map.AddEntity (a, new WorldPosition (5, 5));
			map.AddEntity (b, new WorldPosition (5, 5));
			map.AddEntity (c, new WorldPosition (7, 7));

			query.TargetWorldPosition = new WorldPosition (5, 5);

			List<BitersMapEntity> EntitiesAtFiveFive = query.SearchEntities();
			Assert.IsTrue (EntitiesAtFiveFive.Count == 2);
			*/
		}

	}

}

