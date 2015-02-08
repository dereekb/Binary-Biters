using System;
using System.Collections;
using System.Collections.Generic;

namespace Biters
{
	#region World

	/*
	 * Taxicab World that has direction. Contains element T at a coordinate.
	 * 
	 * Used for tiles management.
	 */
	public class World<T> : IEnumerable {
		
		private Dictionary<WorldPosition, T> Everything;
		
		public World() {
			Everything = new Dictionary<WorldPosition, T> ();
		}

		#region Positions

		public IEnumerable<KeyValuePair<WorldPosition, T>> ElementPairs {

			get 
			{
				return this.Everything;
			}

		}

		public IEnumerable<WorldPosition> Positions {

			get 
			{
				return this.Everything.Keys;
			}

		}

		#endregion

		#region Elements

		public T this[WorldPosition position]
		{
			get
			{
				return this.GetAtPosition(position);
			}
			
			set
			{
				this.SetAtPosition(value, position);
			}
			
		}

		public T GetAtPosition(WorldPosition Position) {
			T result;
			Everything.TryGetValue (Position, out result);
			return result;
		}

		public T SetAtPosition(T Element, WorldPosition Position) {
			T replaced = this.RemoveFromPosition (Position);

			if (Element != null) {
				Everything[Position] = Element;
			}

			return replaced;
		}
		
		public T RemoveFromPosition (WorldPosition Position) {
			T removed = this.GetAtPosition (Position);
			
			if (removed != null) {
				Everything.Remove(Position);
			}
			
			return removed;
		}

		#endregion
		
		//Elements in Direction
		//TODO: Add function

		//Neighbors for position
		public Dictionary<WorldDirection, T> GetNeighbors(WorldPosition input) {
			Dictionary<WorldDirection, WorldPosition> positions = input.ValidNeighbors;
			Dictionary<WorldDirection, T> neighbors = new Dictionary<WorldDirection, T> ();
			
			foreach (KeyValuePair<WorldDirection, WorldPosition> pair in positions) {
				WorldDirection direction = pair.Key;
				WorldPosition position = pair.Value;

				T neighbor = this.GetAtPosition(position);
				neighbors[direction] = neighbor;
			}
			
			return neighbors;
		}
		
		//Vector
		//TODO: Add functions for World Vector usage.

		#region Enumerator

		public IEnumerator GetEnumerator() {
			return Everything.Values.GetEnumerator();
		}

		#endregion

	}
	
	//Complex Position
	public struct WorldVector {
		
		public int Magnitude;
		public WorldDirection Direction;
		public WorldPosition Position;
		
		public WorldVector(WorldDirection Direction, WorldPosition Position) : this(Direction, Position, 1) {}
		
		public WorldVector(WorldDirection Direction, WorldPosition Position, int Magnitude) {
			this.Direction = Direction;
			this.Position = Position;
			this.Magnitude = Magnitude;
		}
		
		public WorldVector? TakeStep() {
			//TODO: Return a WorldVector with the given direction and position. Return nil if the position is invalid.

			return null;
		}
		
	}

	#endregion

	#region Position
	//Position Coordinate (Positive only positions)
	public struct WorldPosition : IEquatable<WorldPosition> {
		
		//Max position available with Cantor Pairing on 32Bit. More than fine.
		private static int MIN_POSITION = 0;
		private static int MAX_POSITION = 65535;
		
		private static int SanitizePosition(int X) {
			return (X < MAX_POSITION) ? ((X > MIN_POSITION) ? X : MIN_POSITION) : MAX_POSITION;
		}
		
		public static bool IsValidPosition(int X, int Y) {
			return WorldPosition.IsValidPosition (X) && WorldPosition.IsValidPosition (Y);
		}
		
		public static bool IsValidPosition(int X) {
			return (X < MAX_POSITION) ? ((X > MIN_POSITION) ? true : false) : false;
		}
		
		internal int x;
		internal int y;
		
		public int X {
			get {
				return x;
			}
			
			set {
				this.x = SanitizePosition(value);
			}
		}
		
		public int Y {
			get {
				return y;
			}

			set {
				this.y = SanitizePosition(value);
			}
		}
		
		internal WorldPosition(int sqr) {
			this.x = sqr;
			this.y = sqr;
		}

		public WorldPosition(int X, int Y) {
			this.x = SanitizePosition (X);
			this.y = SanitizePosition (Y);
		}

		public static WorldPosition? MakeValidPosition(int X, int Y) {
			WorldPosition? position = null;

			if (WorldPosition.IsValidPosition (X, Y)) {
				WorldPosition temp = new WorldPosition(0);
				temp.x = X;
				temp.y = Y;
				position = temp;
			}

			return position;

		}
		
		//Direction
		public WorldPosition? MoveInDirection(WorldDirection Direction) {
			WorldPositionChange change = Direction.PositionChange();
			return change.Move(this);
		}
		
		//Neighbors
		public Dictionary<WorldDirection, WorldPosition> ValidNeighbors {
			get {
				return this.GetValidNeighbors();
			}
		}
		
		public Dictionary<WorldDirection, WorldPosition> GetValidNeighbors() {
			Dictionary<WorldDirection, WorldPosition> neighbors = new Dictionary<WorldDirection, WorldPosition> ();

			foreach (WorldDirection direction in WorldDirectionInfo.All) {
				WorldPosition? position = this.MoveInDirection(direction);

				if (position.HasValue) {
					neighbors[direction] = position.Value;
				}
			}
			
			return neighbors;
		}
		
		//Unique Hashcode 
		public override int GetHashCode() {
			//Cantor Pairing
			return (X + Y) * (X + Y + 1) / 2 + X;
		}

		public override string ToString ()
		{
			return string.Format ("[WorldPosition: X={0}, Y={0}]", X, Y);
		}

		public bool Equals(WorldPosition other)
		{
			return (other.X == this.X && other.Y == this.Y);
		}

	}

	public struct WorldPositionChange {

		public int dX;
		public int dY;
		
		public WorldPosition? Move(WorldPosition position) {
			
			int X = position.X + dX;
			int Y = position.Y + dY;
			
			WorldPosition? move = null;
			
			//Check if valid
			if (WorldPosition.IsValidPosition(X, Y)) {
				move = new WorldPosition(X, Y);
			}
			
			//If invalid, return null (or throw Out of Bounds exception?)
			return move;
		}
		
	}

	#endregion

	#region Direction

	//Directions on the board
	public enum WorldDirection {
		North, East, South, West
	}

	public static class WorldDirectionInfo
	{
		//STYLE: Use lowercase for private variables.
		private static WorldDirection[] all = new WorldDirection[4] { WorldDirection.North, WorldDirection.East, WorldDirection.South, WorldDirection.West };
		
		public static WorldDirection[] All {
			get {
				return all; //Public accessor.
			}
		}
		
		//Extension of WorldDirection enum with static functinon.
		public static WorldPositionChange PositionChange(this WorldDirection direction)
		{
			WorldPositionChange change = new WorldPositionChange();
			
			switch (direction) {
			case WorldDirection.North:
				change.dY = -1; 
				break;
			case WorldDirection.East:
				change.dX = 1;
				break;
			case WorldDirection.South:
				change.dY = 1;
				break;
			case WorldDirection.West:
				change.dX = -1;
				break;
			}
			
			return change;
		}
		
	}

	#endregion

}

