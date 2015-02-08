using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Biters;
using Biters.Utility;

namespace Biters.Game
{
	/*
	 * Extension of game tile that randomly sets entity's movement as they move through passible tiles.
	 * 
	 * This differs from DirectionalGameTile, which just has a type and direction.
	 */
	public class MazeDirectionalGameTile : DirectionalGameTile
	{

		public MazeDirectionalGameTile () {}
		
	}
	
	/*
	 * Default Implementation. Uses an IDirectionalTileMoveSuggestion.
	 */
	public class MazeTileAutoPilotFactoryDelegate : IDirectionalTileAutoPilotFactoryDelegate {
		
		private readonly static Dictionary<DirectionalGameTileType, IDirectionSuggestion> Suggestions = DefaultSuggestions;
		
		private static Dictionary<DirectionalGameTileType, IDirectionSuggestion> DefaultSuggestions {
			get {
				Dictionary<DirectionalGameTileType, IDirectionSuggestion> s 
					= new Dictionary<DirectionalGameTileType, IDirectionSuggestion>();
				
				//Straight
				s.Add(DirectionalGameTileType.Horizontal, new HeadingSuggestion(WorldDirection.East).AvoidEastWest());
				s.Add(DirectionalGameTileType.Vertical, new HeadingSuggestion(WorldDirection.North).AvoidEastWest());
				
				//T_Sections
				s.Add(DirectionalGameTileType.T_Up, new HeadingSuggestion(WorldDirection.North)
				      .AllowAllBut(WorldDirection.South)		//If south, go east or west.
				      .Suggest(IfThenSuggestion.Random(WorldDirection.South, WorldDirection.East, WorldDirection.West)));
				
				s.Add(DirectionalGameTileType.T_Down, new HeadingSuggestion(WorldDirection.South)
				      .AllowAllBut(WorldDirection.North)		//If north, go east or west.
				      .Suggest(IfThenSuggestion.Random(WorldDirection.North, WorldDirection.East, WorldDirection.West)));
				
				s.Add(DirectionalGameTileType.T_Left, new HeadingSuggestion(WorldDirection.East)
				      .AllowAllBut(WorldDirection.East)		//If east, go north or south.
				      .Suggest(IfThenSuggestion.Random(WorldDirection.South, WorldDirection.East, WorldDirection.West)));
				
				
				
				//TODO: Consider adding randomization to suggest other direction incase it entered from the I in T.
				s.Add(DirectionalGameTileType.T_Up, new DirectionSuggestion(WorldDirection.North, WorldDirection.East).AllowAllBut(WorldDirection.South));
				s.Add(DirectionalGameTileType.T_Down, new DirectionSuggestion(WorldDirection.South, WorldDirection.West).AllowAllBut(WorldDirection.North));
				s.Add(DirectionalGameTileType.T_Left, new DirectionSuggestion(WorldDirection.West, WorldDirection.North).AllowAllBut(WorldDirection.East));
				s.Add(DirectionalGameTileType.T_Right, new DirectionSuggestion(WorldDirection.East, WorldDirection.South).AllowAllBut(WorldDirection.West));
				
				//Corner_Sections
				s.Add(DirectionalGameTileType.Corner_Top_Left, new DirectionSuggestion(WorldDirection.South, WorldDirection.West).Avoid(WorldDirection.South, WorldDirection.East));
				s.Add(DirectionalGameTileType.Corner_Top_Right, new DirectionSuggestion(WorldDirection.South, WorldDirection.East).Avoid(WorldDirection.South, WorldDirection.West));
				s.Add(DirectionalGameTileType.Corner_Bottom_Left, new DirectionSuggestion(WorldDirection.North, WorldDirection.West).Avoid(WorldDirection.North, WorldDirection.East));
				s.Add(DirectionalGameTileType.Corner_Bottom_Right, new DirectionSuggestion(WorldDirection.North, WorldDirection.East).Avoid(WorldDirection.North, WorldDirection.West));
				
				return s;
			}
		}
		
		public MazeTileAutoPilotFactoryDelegate() {}
		
		public Vector3 DirectionForElement(IPositionalElement Target, IPositionalElement Element, DirectionalGameTileType type) {
			IDirectionSuggestion suggestion = Suggestions [type];
			WorldPositionAlignment side = WorldPositionAlignmentInfo.GetAlignment(Target.Position, Element.Position);
			
			WorldDirection heading = suggestion.GetSuggestion (side);
			Vector3 direction = heading.Vector ();
			return direction;
		}
		
	}
	

}

