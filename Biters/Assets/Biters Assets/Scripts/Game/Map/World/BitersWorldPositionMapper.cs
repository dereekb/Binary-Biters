using System;
using UnityEngine;

namespace Biters.Game
{
	/*
	 * Default Implementation of IWorldPositionMapper
	 */
	public sealed class BitersWorldPositionMapper : IWorldPositionMapper
	{
		public BitersWorldPositionMapper () {}
	
		public WorldPosition? PositionForVector(Vector3 Vector) {
			WorldPosition position = new WorldPosition ();

			//TODO: Transform Vector to Position;

			return position;
		}
		
		public Vector3 VectorForPosition(WorldPosition WorldPosition) {
			Vector3 vector = new Vector3 ();

			//TODO: Transform position to vector.

			return vector;
		}

	}
}

