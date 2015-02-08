using System;
using System.Collections;
using System.Collections.Generic;

namespace Biters
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
	public struct DirectionSuggestion : IDirectionSuggestion 
	{
		public WorldDirection Direction;

		public DirectionSuggestion(WorldDirection Direction) {
			this.Direction = Direction;
		}

		public WorldDirection? GetSuggestion(WorldPositionAlignment Alignment) { return this.Direction; }
		
		public WorldDirection? GetSuggestion(WorldDirection Direction) { return this.Direction; }
	}

	//TODO: Add "Random" Direction suggestion.

	/*
	 * Wraps an If/Else statement to implement IDirectionSuggestion.
	 */
	public class IfThenSuggestion : IDirectionSuggestion
	{
		public readonly WorldDirection IfDirection;
		public readonly IDirectionSuggestion Then;
		
		public IfThenSuggestion(WorldDirection IfDirection, WorldDirection Then) {
			this.IfDirection = IfDirection;
			this.Then = new DirectionSuggestion (Then);
		}
		
		public WorldDirection? GetSuggestion(WorldPositionAlignment Alignment) { return null; }
		
		public WorldDirection? GetSuggestion(WorldDirection Direction) {
			WorldDirection? suggestion;
			
			if (IfDirection == Direction) {
				suggestion = Then.GetSuggestion(Direction);
			}
			
			return suggestion;
		}
		
		public static IfThenSuggestion Reverse(WorldDirection Direction) {
			return new IfThenSuggestion (Direction, Direction.Opposite ());
		}
		
		public static IfThenSuggestion Random(WorldDirection Direction, params WorldDirection[] Randoms) {
			//TODO: Add Random Suggestion.
			return new IfThenSuggestion (Direction, Direction.Opposite ());
		}
		
	}
	
	/*
	 * Tells a possible movement whether or not the current direction is acceptable or not.
	 * 
	 * If not, it will use the suggested direction.
	 */
	public sealed class HeadingSuggestion : IDirectionSuggestion {

		public IDirectionSuggestion DefaultSuggestion;
		public List<IDirectionSuggestion> Suggestions = new List<IDirectionSuggestion>();
		private HashSet<WorldDirection> avoid = new HashSet<WorldDirection>();
		
		public HeadingSuggestion() : this (WorldDirection.North) {}
		
		public HeadingSuggestion(WorldDirection direction) : this (new DirectionSuggestion(direction)) {}
		
		public HeadingSuggestion(IDirectionSuggestion Suggestion) {
			this.DefaultSuggestion = Suggestion;
		}

		public HeadingSuggestion(IDirectionSuggestion Suggestion, IDirectionSuggestion Suggestions) {
			this.DefaultSuggestion = Suggestion;
			this.Suggestions.Add (Suggestions);
		}

		public HeadingSuggestion(IDirectionSuggestion Suggestion, List<IDirectionSuggestion> Suggestions) {
			this.DefaultSuggestion = Suggestion;
			this.Suggestions = Suggestions;
		}
		
		public HeadingSuggestion Suggest(params IDirectionSuggestion[] Suggestions) {
			
			foreach (IDirectionSuggestion suggestion in Suggestions) {
				this.Suggestions.Add(suggestion);
			}
			
			return this;
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

		public HeadingSuggestion AvoidAllBut(WorldDirection Direction) {
			this.avoid = new HashSet<WorldDirection> (Direction.AllExcept());
			return this;
		}
		
		public HeadingSuggestion Allow(WorldDirection Direction) {
			this.avoid.Remove (Direction);
			return this;
		}
		
		public HeadingSuggestion AllowAllBut(WorldDirection Direction) {
			this.avoid = new HashSet<WorldDirection> ();
			this.Avoid (Direction);
			return this;
		}

		private WorldDirection? CheckSuggestions(WorldDirection Direction) {
			WorldDirection? directionSuggestion;

			foreach (IDirectionSuggestion suggestion in this.Suggestions) {
				directionSuggestion = suggestion.GetSuggestion(Direction);

				if (directionSuggestion.HasValue) {
					break;
				}
			}

			return directionSuggestion;
		}

		public WorldDirection? GetSuggestion(WorldDirection Heading) {
			return this.CheckSuggestions(Heading);
		}

		public WorldDirection? GetSuggestion(WorldPositionAlignment Alignment) {
			//Reverse the alignment to show where the entity is moving TO.
			WorldDirection heading = Alignment.OppositeAlignment().ImpliedDirection();
			
			if (this.avoid.Contains(heading)) {
				WorldDirection? suggestion = this.GetSuggestion(heading) ?? this.DefaultSuggestion.GetSuggestion();

				if (suggestion.HasValue) {
					heading = suggestion.Value;
				}
			}
			
			return heading;
		}
		
	}

}

