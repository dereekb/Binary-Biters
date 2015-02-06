using System;
using UnityEngine;
using Biters;

namespace Biters.Game
{
	/*
	 * Base class for Biters game tiles. 
	 */
	public abstract class BitersGameTile : Entity, IMapTile
	{
		//Position in the map's world. Should only be changed by the Map.
		public WorldPosition MapTilePosition { get; set; }

		public BitersGameTile(GameObject GameObject) : base(GameObject) {}

		#region Map

		public virtual void AddedToMap(Map<IMapTile> Map, WorldPosition Position) {
			//TODO: Move into view.
		}
		
		public virtual void RemovedFromMap(Map<IMapTile> Map) {
			//TODO: Remove from view? Discard?
		}

		#endregion

	}

}

