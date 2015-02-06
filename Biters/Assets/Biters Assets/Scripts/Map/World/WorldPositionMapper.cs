using System;
using UnityEngine;

namespace Biters
{
	/*
	 * Acts as a delegate to convert between the game engine's space and a world's space.
	 */
	public interface IWorldPositionMapper {

		WorldPosition? PositionForVector(Vector3 Vector);

		Vector3 VectorForPosition(WorldPosition WorldPosition);

	}

}

