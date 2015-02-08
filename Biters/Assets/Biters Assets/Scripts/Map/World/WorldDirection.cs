using System;
using System.Collections;
using System.Collections.Generic;

namespace Biters
{
	
	/*
	 * Interface for suggesting what directions to avoid.
	 */
	public interface IDirectionSuggestion {
		
		WorldDirection GetSuggestion(WorldPositionAlignment Alignment);
		
	}
	
	/*
	 * Tells a possible movement whether or not the current direction is acceptable or not.
	 * 
	 * If not, it will use the suggested direction.
	 */
	public sealed class DirectionSuggestion : IDirectionSuggestion {
		
		public WorldDirection WeakSuggestion; //Suggested Direction. If it is the same as the current entity, use the backup.
		public WorldDirection StrongSuggestion; //Backup Suggested Direction.
		private HashSet<WorldDirection> avoid = new HashSet<WorldDirection>();
		
		public DirectionSuggestion() : this (WorldDirection.North) {}
		
		public DirectionSuggestion(WorldDirection Suggestion) {
			this.WeakSuggestion = Suggestion;
			this.StrongSuggestion = Suggestion;
		}
		
		public DirectionSuggestion(WorldDirection WeakSuggestion, WorldDirection StrongSuggestion) {
			this.WeakSuggestion = WeakSuggestion;
			this.StrongSuggestion = StrongSuggestion;
		}

		public DirectionSuggestion Avoid(WorldDirection Direction) {
			this.avoid.Add(Direction);
			return this;
		}
		
		public DirectionSuggestion Avoid(params WorldDirection[] Directions) {

			foreach (WorldDirection direction in Directions) {
				this.avoid.Add(direction);
			}

			return this;
		}
		
		public DirectionSuggestion AvoidUpDown() {
			this.avoid.Add(WorldDirection.North);
			this.avoid.Add(WorldDirection.South);
			return this;
		}
		
		public DirectionSuggestion AvoidEastWest() {
			this.avoid.Add(WorldDirection.East);
			this.avoid.Add(WorldDirection.West);
			return this;
		}

		public DirectionSuggestion AvoidAllBut(WorldDirection Direction) {
			this.avoid = new HashSet<WorldDirection> (Direction.AllExcept());
			return this;
		}
		
		public DirectionSuggestion Allow(WorldDirection Direction) {
			this.avoid.Remove (Direction);
			return this;
		}
		
		public DirectionSuggestion AllowAllBut(WorldDirection Direction) {
			this.avoid = new HashSet<WorldDirection> ();
			this.Avoid (Direction);
			return this;
		}

		public WorldDirection GetSuggestion(WorldPositionAlignment Alignment) {
			WorldDirection heading = Alignment.OppositeAlignment().SuggestedDirection();
			
			if (this.avoid.Contains(heading)) {
				if (heading.Opposite() == this.WeakSuggestion) {
					heading = this.StrongSuggestion;
				} else {
					heading = this.WeakSuggestion;
				}
			}
			
			return heading;
		}
		
	}

}

