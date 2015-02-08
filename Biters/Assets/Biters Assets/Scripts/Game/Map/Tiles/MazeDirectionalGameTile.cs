using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Biters;
using Biters.World;
using Biters.Utility;

namespace Biters.Game
{	

	public static class DirectionalGameTileMazeExtension
	{

		/*
		 * Changes the delegate to be a Maze Tile intead.
		 */
		public static T MakeMazeTile<T>(this T Tile) where T : DirectionalGameTile {
			Tile.DirectionDelegate = new MazeTileAutoPilotFactoryDelegate(Tile.TileType);
			return Tile;
		}

	}

	/*
	 * More complex implementation of IDirectionalTileAutoPilotFactoryDelegate that will keep elements moving forward as best as possible.
	 * 
	 * When elements cannot move forward, they choose a new direction randomly that is acceptable to the tile.
	 */
	public struct MazeTileAutoPilotFactoryDelegate : IDirectionalTileAutoPilotFactoryDelegate {
		
		private readonly static Dictionary<DirectionalGameTileType, IDirectionSuggestion> Suggestions = DefaultSuggestions;

		//Static AI Logic
		private static Dictionary<DirectionalGameTileType, IDirectionSuggestion> DefaultSuggestions {
			get {
				Dictionary<DirectionalGameTileType, IDirectionSuggestion> s
					= new Dictionary<DirectionalGameTileType, IDirectionSuggestion>();
				
				//Straight
				s.Add(DirectionalGameTileType.Horizontal, new HeadingSuggestion(WorldDirection.East).AvoidUpDown());
				s.Add(DirectionalGameTileType.Vertical, new HeadingSuggestion(WorldDirection.North).AvoidEastWest());
				
				//T_Sections
				s.Add(DirectionalGameTileType.T_Up, new HeadingSuggestion(WorldDirection.North)
				      .AllowAllBut(WorldDirection.South)		//If facing south, go east or west.
				      .Suggest(IfThenSuggestion.Random(WorldDirection.South, WorldDirection.East, WorldDirection.West)));

				s.Add(DirectionalGameTileType.T_Down, new HeadingSuggestion(WorldDirection.South)
				      .AllowAllBut(WorldDirection.North)		//If facing north, go east or west.
				      .Suggest(IfThenSuggestion.Random(WorldDirection.North, WorldDirection.East, WorldDirection.West)));
				
				s.Add(DirectionalGameTileType.T_Left, new HeadingSuggestion(WorldDirection.West)
				      .AllowAllBut(WorldDirection.East)		//If facing east, go north or south.
				      .Suggest(IfThenSuggestion.Random(WorldDirection.East, WorldDirection.North, WorldDirection.South)));
				
				s.Add(DirectionalGameTileType.T_Right, new HeadingSuggestion(WorldDirection.East)
				      .AllowAllBut(WorldDirection.West)		//If facing west, go north or south.
				      .Suggest(IfThenSuggestion.Random(WorldDirection.West, WorldDirection.North, WorldDirection.South)));

				//Corner_Sections
				s.Add(DirectionalGameTileType.Corner_Top_Left, new HeadingSuggestion()
				      .Avoid(WorldDirection.South, WorldDirection.East)
				      .Suggest(new IfThenSuggestion(WorldDirection.South, WorldDirection.West),		//If facing down, go left
				     				new IfThenSuggestion(WorldDirection.East, WorldDirection.North)));	//If facing right, go up
				
				s.Add(DirectionalGameTileType.Corner_Top_Right, new HeadingSuggestion()
				      .Avoid(WorldDirection.South, WorldDirection.West)
				      .Suggest(new IfThenSuggestion(WorldDirection.South, WorldDirection.East),
				         		new IfThenSuggestion(WorldDirection.West, WorldDirection.North)));	//If facing left, go up
				
				s.Add(DirectionalGameTileType.Corner_Bottom_Left, new HeadingSuggestion()
				      .Avoid(WorldDirection.North, WorldDirection.East)
				      .Suggest(new IfThenSuggestion(WorldDirection.North, WorldDirection.West),
				         		new IfThenSuggestion(WorldDirection.East, WorldDirection.South)));	//If facing right, go down
				
				s.Add(DirectionalGameTileType.Corner_Bottom_Right, new HeadingSuggestion()
				      .Avoid(WorldDirection.North, WorldDirection.West)
				      .Suggest(new IfThenSuggestion(WorldDirection.North, WorldDirection.East),
				         		new IfThenSuggestion(WorldDirection.West, WorldDirection.South)));	//If facing left, go up

				return s;
			}
		}

		public readonly DirectionalGameTileType Type;

		public MazeTileAutoPilotFactoryDelegate(DirectionalGameTileType Type) {
			this.Type = Type;
		}

		public Vector3 DirectionForElement(IPositionalElement Target, IPositionalElement Element) {
			IDirectionSuggestion suggestion = Suggestions [this.Type];

			WorldPositionAlignment side = WorldPositionAlignmentInfo.GetAlignment(Target.Position, Element.Position);
			
			Debug.Log(String.Format("Get Suggestion for element at Side {0} with type {1} using [{2}].", side, Type, suggestion));
			WorldDirection heading = suggestion.GetSuggestion(side).Value;
			Debug.Log(String.Format("Suggestion for element at Side {0} with type {1} -> {2}.", side, Type, heading));

			Vector3 direction = heading.Vector ();
			return direction;
		}
		
	}
	

}

