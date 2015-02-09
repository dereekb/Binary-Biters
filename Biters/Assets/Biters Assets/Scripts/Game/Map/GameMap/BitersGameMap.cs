using System;
using UnityEngine;
using Biters;
using Biters.Game;
using Biters.Utility;

namespace Biters.Game
{
	/*
	 * Biters Game Map.
	 */
	public class BitersGameMap : SafeGameMap<BitersGameTile, BitersMapEntity>
	{

		public BitersGameMap(GameObject GameObject, IMapWorldFactory<BitersGameTile> MapWorldFactory) : base (GameObject, MapWorldFactory) {}

		#region Element Initialization

		protected override void InitializeEntity (BitersMapEntity Element, WorldPosition Position)
		{
			Element.Map = this;
			base.InitializeEntity (Element, Position);
		}

		protected override void InitializeTile (BitersGameTile Element, WorldPosition Position)
		{
			Element.Map = this;
			base.InitializeTile (Element, Position);
		}

		#endregion

	}

}

