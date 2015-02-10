using System;
using NUnit.Framework;
using Biters;
using Unity;
using UnityEngine;
using Biters.World;
using Biters.Game;

namespace Biters.Testing
{
	/*
	 * Tests movements and autopilot.
	 */
	[TestFixture()]
	public class CustomTileAutoPilotFactoryDelegateTests
	{
		
		[Test()]
		public void TestTUpSection() {
			
			MazeTileAutoPilotFactoryDelegate Delegate = new MazeTileAutoPilotFactoryDelegate (DirectionalGameTileType.T_Up);
			
			//Test Upward
			PositionalVectorWrapper center = new PositionalVectorWrapper (new Vector3(0,0));
			PositionalVectorWrapper target = new PositionalVectorWrapper (new Vector3(0,1));
			
			HeadingSuggestion TSuggestion = new HeadingSuggestion (WorldDirection.North)
				.AllowAllBut (WorldDirection.South)
					.Suggest (IfThenSuggestion.Random (WorldDirection.South, WorldDirection.East, WorldDirection.West));
			
			//Check the alignment is what is expected.
			WorldPositionAlignment side = WorldPositionAlignmentInfo.GetAlignment(center.Position, target.Position);
			Assert.IsTrue(side == WorldPositionAlignment.Top);
			
			//Check the TSuggestion
			WorldDirection ResultSuggestion = TSuggestion.GetSuggestion (WorldDirection.South).Value;
			Assert.IsTrue(ResultSuggestion == WorldDirection.East || ResultSuggestion == WorldDirection.West);
			
			//Check the HeadingSuggestion
			WorldDirection GoingSouth = Delegate.HeadingForElement(center, target);
			Assert.IsTrue(GoingSouth  == WorldDirection.East || GoingSouth == WorldDirection.West);
			
			Console.WriteLine ("ResultSuggestion {0} vs {1} : GoingSouth -> {2}", ResultSuggestion, side, GoingSouth);
			
		}
		
		[Test()]
		public void TestRotateScript() {
			
			RotateTileAutoPilotFactoryDelegate Delegate = new RotateTileAutoPilotFactoryDelegate (DirectionalGameTileType.T_Up);
			
			//Test Upward
			PositionalVectorWrapper center = new PositionalVectorWrapper (new Vector3(0,0));
			PositionalVectorWrapper target = new PositionalVectorWrapper (new Vector3(0,1));

			//Check the HeadingSuggestion
			WorldDirection GoingSouthA = Delegate.HeadingForElement(center, target);
			WorldDirection GoingSouthB = Delegate.HeadingForElement(center, target);
			
			Console.WriteLine ("A {0}, B {1}", GoingSouthA, GoingSouthB);

			Assert.IsTrue(GoingSouthA == WorldDirection.East || GoingSouthA == WorldDirection.West);
			Assert.IsTrue(GoingSouthB != GoingSouthA);

		}

	}
}

