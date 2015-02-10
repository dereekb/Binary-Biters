using System;
using System.Collections;
using System.Collections.Generic;
using Biters.Game;
using Biters.Utility;
using UnityEngine;

namespace Biters.Debugging
{

	public class DebugGameMapWatcher : GameMapWatcher<BitersGameTile, BitersMapEntity>
	{	
		public bool PreventEntityOOBRemoval = false;

		#region Handling
		
		public override void HandleEntityOutsideworld(GameMapEventInfo info) {
			if (this.PreventEntityOOBRemoval) {
				Vector2 position = this.Map.PositionMapper.VectorForPosition (WorldPosition.Origin);
				IGameMapEntity entity = info.Entity;
				entity.Transform.position = position;
			} else {
				base.HandleEntityOutsideworld(info);
			}
		}
		
		#endregion

	}
		

}

