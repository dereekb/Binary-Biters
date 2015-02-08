using System;
using NUnit.Framework;
using UnityEngine;
using Biters;

namespace Biters.Testing
{

	[TestFixture()]
	public class WorldTests
	{
		[Test()]
		public void TestWorldPositionMapper () {

			WorldPositionMapper mapper = new WorldPositionMapper ();
			
			Vector3 ShouldBeOrigin = new Vector3 (0.5f, 0.5f);

			WorldPosition? ShouldBeOriginCheck = mapper.PositionForVector (ShouldBeOrigin);

			Assert.True (ShouldBeOriginCheck.HasValue);
			Assert.True (ShouldBeOriginCheck.Value == new WorldPosition (0, 0));

          Vector3 ShouldBeOneOne = new Vector3 (1.5f, 1.5f);
			WorldPosition? ShouldBeOneOneCheck = mapper.PositionForVector (ShouldBeOneOne);
			
			Assert.True (ShouldBeOneOneCheck.HasValue);
			Assert.True (ShouldBeOneOneCheck.Value == new WorldPosition (1, 1));

		}

	}

}

