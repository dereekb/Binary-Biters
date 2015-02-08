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

		[Test()]
		public void TestWorldAlignmentGetAlignment() {
			
			Vector2 u = new Vector2 (0, 1);
			Vector2 d = new Vector2 (0, -1);
			Vector2 c = new Vector2 (0, 0);
			Vector2 l = new Vector2 (1, 0);
			Vector2 r = new Vector2 (-1, 0);
			
			Console.WriteLine(Vector2.Angle (u, c).ToString());
			Console.WriteLine(Vector2.Angle (c-d, c).ToString());
			Console.WriteLine(Vector2.Angle (l, c).ToString());
			Console.WriteLine(Vector2.Angle (r, c).ToString());
			Console.WriteLine(Vector2.Angle (c, c).ToString());
		}

	}

}

