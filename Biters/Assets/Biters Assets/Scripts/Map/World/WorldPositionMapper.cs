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
		 * Top-Center position. Equivalent to (width/2, 0)
		 */
		Top,

		/*
		 * Left-Center position. Equivalent to (0,height/2)
		 */
		Left,
		
		/*
		 * Right-Center position. Equivalent to (width,height/2)
		 */
		Right,
		
		/*
		 * Center of the World Position. Equivalent to (width/2,height/2)
		 */
		Center,
		
		/*
		 * Bottom-Center position. Equivalent to (width/2, height)
		 */
		Bottom
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
			int X = Convert.ToInt32 (Math.Floor(Vector.x / PositionWidth));
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

