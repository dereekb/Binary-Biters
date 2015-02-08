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

			Vector2 c = new Vector2 (0, 0);	//Center
			
			Vector2 r = new Vector2 (1, 0);	//Right
			Vector2 u = new Vector2 (0, 1);	//Up
			Vector2 l = new Vector2 (-1, 0);	//Left
			Vector2 d = new Vector2 (0, -1);	//Down

			Console.WriteLine (WorldPositionAlignmentInfo.GetQuadrant (360));

			Assert.True(WorldPositionAlignmentInfo.GetAngleDegree (c, r) == 360);
			Assert.True(WorldPositionAlignmentInfo.GetAngleDegree (c, u) == 90);
			Assert.True(WorldPositionAlignmentInfo.GetAngleDegree (c, l) == 180);
			Assert.True(WorldPositionAlignmentInfo.GetAngleDegree (c, d) == 270);

			Assert.True (WorldPositionAlignmentInfo.GetQuadrant(360) == 0);

			Assert.True(WorldPositionAlignmentInfo.GetAlignment (c, r) == WorldPositionAlignment.Right);
			Assert.True(WorldPositionAlignmentInfo.GetAlignment (c, u) == WorldPositionAlignment.Top);
			Assert.True(WorldPositionAlignmentInfo.GetAlignment (c, l) == WorldPositionAlignment.Left);
			Assert.True(WorldPositionAlignmentInfo.GetAlignment (c, d) == WorldPositionAlignment.Bottom);

		}

	}

}

