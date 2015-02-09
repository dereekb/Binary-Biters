using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Biters.World
{
	
	/*
	 * Interface for suggesting what directions take. 
	 * 
	 * The context is up to the implementation.
	 */
	public interface IDirectionSuggestion {
		
		WorldDirection? GetSuggestion(WorldPositionAlignment Alignment);

		/*
		 * Returns a suggestion for the direction to take given the currnet direction.
		 */
		WorldDirection? GetSuggestion(WorldDirection Heading);

	}

	/*
	 * Wraps a direction to implement IDirectionSuggestion.
	 */
	public class DirectionSuggestion : IDirectionSuggestion 
	{
		public WorldDirection Direction;

		public DirectionSuggestion(WorldDirection Direction) {
			this.Direction = Direction;
		}

		public WorldDirection? GetSuggestion(WorldPositionAlignment Alignment) { return this.Direction; }
		
		public WorldDirection? GetSuggestion(WorldDirection Direction) { return this.Direction; }

		public override string ToString ()
		{
			return string.Format ("[DirectionSuggestion: {0}]", this.Direction);
		}

	}

	/*
	 * Wraps an If/Else statement to implement IDirectionSuggestion.
	 */
	public class IfThenSuggestion : IDirectionSuggestion
	{
		public readonly WorldDirection IfDirection;
		public readonly IDirectionSuggestion Then;
		
		public IfThenSuggestion(WorldDirection IfDirection, WorldDirection Then)
			: this (IfDirection, new DirectionSuggestion (Then)) {}

		public IfThenSuggestion(WorldDirection IfDirection, IDirectionSuggestion Then) {
			this.IfDirection = IfDirection;
			this.Then = Then;
		}

		public WorldDirection? GetSuggestion(WorldPositionAlignment Alignment) { return null; }
		
		public WorldDirection? GetSuggestion(WorldDirection Direction) {
			WorldDirection? suggestion = null;
			
			if (IfDirection == Direction) {
				suggestion = Then.GetSuggestion(Direction);
			}
			
			return suggestion;
		}
		
		public static IfThenSuggestion Reverse(WorldDirection Direction) {
			return new IfThenSuggestion (Direction, Direction.Opposite ());
		}
		
		public static IfThenSuggestion Random(WorldDirection Direction, params WorldDirection[] Randoms) {
			return new IfThenSuggestion (Direction, new RandomSuggestion(Randoms));
		}
		
		public static IfThenSuggestion Rotate(WorldDirection Direction, params WorldDirection[] Rotating) {
			return new IfThenSuggestion (Direction, new RotatingSuggestion(Rotating));
		}

		public override string ToString ()
		{
			return string.Format ("[IfThenSuggestion: If={0} Then={1}]", IfDirection, Then);
		}
		
	}

	public class DirectionSuggestionList : IDirectionSuggestion {
		
		public List<IDirectionSuggestion> Suggestions = new List<IDirectionSuggestion>();
		
		public DirectionSuggestionList() {}

		public DirectionSuggestionList(params IDirectionSuggestion[] Suggestions) {
			this.Suggest (Suggestions);
		}

		public DirectionSuggestionList Suggest(params IDirectionSuggestion[] Suggestions) {
			foreach (IDirectionSuggestion suggestion in Suggestions) {
				this.Suggestions.Add(suggestion);
			}
			return this;
		}
		
		protected virtual WorldDirection? CheckSuggestions(WorldDirection Direction) {
			WorldDirection? directionSuggestion = null;
			
			foreach (IDirectionSuggestion suggestion in this.Suggestions) {
				directionSuggestion = suggestion.GetSuggestion(Direction);
				
				if (directionSuggestion.HasValue) {
					break;
				}
			}
			
			return directionSuggestion;
		}
		
		public virtual WorldDirection? GetSuggestion(WorldDirection Heading) {
			return this.CheckSuggestions(Heading);
		}
		
		public virtual WorldDirection? GetSuggestion(WorldPositionAlignment Alignment) {
			throw new InvalidOperationException ("Invalid.");
		}

		public override string ToString ()
		{
			return string.Format ("[DirectionSuggestionList: {0}-{1}]",this.Suggestions,this.Suggestions.Count);
		}

	}

	/*
	 * Suggestion that iterates through the Suggestion List.
	 */
	public class RotatingSuggestion : DirectionSuggestionList {
		
		private int previous = 0;

		public RotatingSuggestion() : base () {}
		
		public RotatingSuggestion (params WorldDirection[] Directions) : base () {
			foreach (WorldDirection Direction in Directions) {
				this.Suggestions.Add (new DirectionSuggestion (Direction));
			}
		}
		
		public RotatingSuggestion (params IDirectionSuggestion[] Suggestions) : base () {}

		public virtual int NextIndex {
			get {
				int next = (this.previous + 1);

				if (next >= Suggestions.Count) {
					next = 0;
				}

				return next;
			}
		}

		public int PreviousIndex {
			get {
				return this.previous;
			}
		}

		public virtual IDirectionSuggestion Next() {
			int index = this.NextIndex;		//0 -> 1
			this.previous = index;				//1
			IDirectionSuggestion suggestion = this.Suggestions [index];
			return suggestion;
		}
		
		public override WorldDirection? GetSuggestion(WorldDirection Heading) {
			IDirectionSuggestion next = this.Next ();
			return next.GetSuggestion (Heading);
		}
		
		public override WorldDirection? GetSuggestion(WorldPositionAlignment Alignment) {
			IDirectionSuggestion next = this.Next ();
			return next.GetSuggestion(Alignment);
		}

	}

	/*
	 * Returns a random suggestion.
	 */
	public class RandomSuggestion : RotatingSuggestion {

		public System.Random Random = new System.Random ();

		public RandomSuggestion() : base () {}
		
		public RandomSuggestion (params WorldDirection[] Directions) : base () {
			foreach (WorldDirection Direction in Directions) {
				this.Suggestions.Add (new DirectionSuggestion (Direction));
			}
		}

		public RandomSuggestion (params IDirectionSuggestion[] Suggestions) : base () {}
		
		public override int NextIndex {
			get {
				return this.Random.Next (0, this.Suggestions.Count);
			}
		}
		
		public virtual IDirectionSuggestion Next() {
			int index = this.NextIndex;
			IDirectionSuggestion suggestion = this.Suggestions [index];
			return suggestion;
		}

	}

	/*
	 * Tells a possible movement whether or not the current direction is acceptable or not.
	 * 
	 * If not, it will use the suggested direction.
	 */
	public sealed class HeadingSuggestion : IDirectionSuggestion {

		public IDirectionSuggestion DefaultSuggestion;
		public IDirectionSuggestion Suggestions;
		private HashSet<WorldDirection> avoid = new HashSet<WorldDirection>();
		
		public HeadingSuggestion() : this (WorldDirection.North) {}
		
		public HeadingSuggestion(WorldDirection direction) : this (new DirectionSuggestion(direction)) {}
		
		public HeadingSuggestion(IDirectionSuggestion Default) {
			this.DefaultSuggestion = Default;
			this.Suggestions = Default;
		}

		public HeadingSuggestion(IDirectionSuggestion Default, IDirectionSuggestion Suggestions) {
			this.DefaultSuggestion = Default;
			this.Suggestions = Suggestions;
		}

		public HeadingSuggestion Avoid(WorldDirection Direction) {
			this.avoid.Add(Direction);
			return this;
		}
		
		public HeadingSuggestion Avoid(params WorldDirection[] Directions) {

			foreach (WorldDirection direction in Directions) {
				this.avoid.Add(direction);
			}

			return this;
		}
		
		public HeadingSuggestion AvoidUpDown() {
			this.avoid.Add(WorldDirection.North);
			this.avoid.Add(WorldDirection.South);
			return this;
		}
		
		public HeadingSuggestion AvoidEastWest() {
			this.avoid.Add(WorldDirection.East);
			this.avoid.Add(WorldDirection.West);
			return this;
		}
		
		public HeadingSuggestion AvoidAll() {
			this.avoid = new HashSet<WorldDirection> (WorldDirectionInfo.All);
			return this;
		}

		public HeadingSuggestion AvoidAllBut(WorldDirection Direction) {
			this.avoid = new HashSet<WorldDirection> (Direction.AllExcept());
			return this;
		}
		
		public HeadingSuggestion Allow(WorldDirection Direction) {
			this.avoid.Remove (Direction);
			return this;
		}
		
		public HeadingSuggestion AllowAllBut(WorldDirection Direction) {
			this.avoid.Clear ();
			this.Avoid (Direction);
			return this;
		}
		
		public HeadingSuggestion Suggest(IDirectionSuggestion Suggestion) {
			this.Suggestions = Suggestion;
			return this;
		}

		public HeadingSuggestion Suggest(params IDirectionSuggestion[] Suggestions) {
			this.Suggestions = new DirectionSuggestionList(Suggestions);
			return this;
		}

		public WorldDirection? GetSuggestion(WorldDirection Heading) {
			WorldDirection? suggestion = null;

			if (this.avoid.Contains (Heading)) {
				/*
				 * If the primary Suggestions return null, return the default suggestion's answer, if a Default is set.
				 */
				suggestion = this.Suggestions.GetSuggestion (Heading);

				if (suggestion.HasValue == false) {
					suggestion = this.DefaultSuggestion.GetSuggestion (Heading);
				}
			} else {
				//Continue on current path.
				suggestion = Heading;
			}

			return suggestion;
		}

		public WorldDirection? GetSuggestion(WorldPositionAlignment Alignment) {

			//Reverse the alignment to show where the entity is moving TO.
			WorldDirection heading = Alignment.OppositeAlignment().ImpliedDirection();

			WorldDirection? suggestion = this.GetSuggestion(heading);
			return suggestion;
		}
		
	}

}

