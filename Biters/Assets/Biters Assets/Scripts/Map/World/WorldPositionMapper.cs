using System;
using UnityEngine;

namespace Biters
{
	/*
	 * Describes the alignment positions of a world position.
	 * 
	 * Each WorldPosition is considered to have a given width. 
	 */
	public enum WorldPositionAlignment {

		//TODO: Add four corners (Top-Left, etc.) if necessary.
		
		/*
		 * Right-Center position. Equivalent to (width,height/2)
		 */
		Right = 0,

		/*
		 * Top-Center position. Equivalent to (width/2, 0)
		 */
		Top = 1,

		/*
		 * Left-Center position. Equivalent to (0,height/2)
		 */
		Left = 2,
		
		/*
		 * Bottom-Center position. Equivalent to (width/2, height)
		 */
		Bottom = 3,
		
		/*
		 * Center of the World Position. Equivalent to (width/2,height/2)
		 */
		Center = 4

	}
	
	public static class WorldPositionAlignmentInfo
	{

		public static Vector2 Magnitude(this WorldPositionAlignment Alignment)
		{
			Vector2 vector = new Vector2();
			
			switch (Alignment) {
			case WorldPositionAlignment.Top: vector = new Vector2(0.5f, 0f); break;
			case WorldPositionAlignment.Left: vector = new Vector2(0f, 0.5f); break;
			case WorldPositionAlignment.Right: vector = new Vector2(0.5f, 1f); break;
			case WorldPositionAlignment.Center: vector = new Vector2(0.5f, 0.5f); break;
			case WorldPositionAlignment.Bottom: vector = new Vector2(1f, 0.5f); break;
			}
			
			return vector;
		}
		
		public static WorldPositionAlignment OppositeAlignment(this WorldPositionAlignment Alignment) {
			WorldPositionAlignment opposite = WorldPositionAlignment.Center;
			
			switch (Alignment) {
			case WorldPositionAlignment.Top: opposite = WorldPositionAlignment.Bottom; break;
			case WorldPositionAlignment.Left: opposite = WorldPositionAlignment.Right; break;
			case WorldPositionAlignment.Right: opposite = WorldPositionAlignment.Left; break;
			case WorldPositionAlignment.Bottom: opposite = WorldPositionAlignment.Top; break;
			case WorldPositionAlignment.Center: opposite = WorldPositionAlignment.Center; break; 
			}
			
			return opposite;
		}

		public static WorldDirection SuggestedDirection(this WorldPositionAlignment Alignment) {
			WorldDirection direction = WorldDirection.North;
			
			switch (Alignment) {
			case WorldPositionAlignment.Top: direction = WorldDirection.North; break;
			case WorldPositionAlignment.Left: direction = WorldDirection.West;  break;
			case WorldPositionAlignment.Right: direction = WorldDirection.East;  break;
			case WorldPositionAlignment.Bottom: direction = WorldDirection.South; break;
				
				//TODO: Add "None" direction later, potentially.
			case WorldPositionAlignment.Center: direction = WorldDirection.North; break; 
			}
			
			return direction;
		}

		public static WorldPositionAlignment GetAlignment(Vector3 Center, Vector3 Element) {
			double angle = GetAngleDegree (Center, Element);
			int quadrant = GetQuadrant (angle + 45.0);
			return (WorldPositionAlignment) quadrant;
		}
		
		public static int GetQuadrant(double angle) {
			int quadrant = (int) Math.Floor(angle / 90.0);
			return quadrant % 4;
		}
		
		public static double GetAngleDegree(Vector3 Center, Vector3 Target) {
			Vector3 relative = Center - Target;
			double n = (Math.Atan2(relative.y, relative.x) * 180.0 / Math.PI) + 180.0;
			return n;
		}

	}

	/*
	 * Acts as a delegate to convert between the game engine's space and a world's space.
	 */
	public interface IWorldPositionMapper {
		
		WorldPosition? PositionForVector(Vector3 Vector);
		
		Vector2 VectorForPosition(WorldPosition WorldPosition);
		
		Vector2 VectorForPosition(WorldPosition WorldPosition, WorldPositionAlignment Alignment);

	}
	
	/*
	 * Default Implementation of IWorldPositionMapper
	 * 
	 * World Positions are 
	 */
	public sealed class WorldPositionMapper : IWorldPositionMapper
	{
		public float PositionWidth = 1.0f;
		public float PositionHeight = 1.0f;
		
		public WorldPositionMapper () {}
		
		public WorldPositionMapper (float PositionWidth, float PositionHeight) {
			this.PositionWidth = PositionWidth;
			this.PositionHeight = PositionHeight;
		}
		
		public WorldPosition? PositionForVector(Vector3 Vector) {
			int X = Convert.ToInt32 (Math.Floor (Vector.x / PositionWidth));
			int Y = Convert.ToInt32 (Math.Floor (Vector.y / PositionHeight));
			
			//Returns the coordinate at the top-left corner of the position tile, if it exists.
			WorldPosition? position = WorldPosition.MakeValidPosition (X, Y);
			return position;
		}
		
		public Vector2 VectorForPosition(WorldPosition WorldPosition) {
			return this.VectorForPosition (WorldPosition, WorldPositionAlignment.Center);
		}
		
		public Vector2 VectorForPosition(WorldPosition WorldPosition, WorldPositionAlignment Alignment) {
			Vector2 vector = new Vector3 ();
			Vector2 magnitude = Alignment.Magnitude();

			//(0,0) will return the vector coordinates 0,0, so the bottom alignment of the square is 0+0.5, 0+1
			//(1,1) will return the vector coordinates 1,1
			vector.x = (magnitude.x + WorldPosition.X) * PositionWidth;
			vector.y = (magnitude.y + WorldPosition.Y) * PositionWidth;

			return vector;
		}

	}
}

