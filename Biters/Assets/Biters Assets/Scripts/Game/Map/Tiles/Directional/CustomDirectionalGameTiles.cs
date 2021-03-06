using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Biters;
using Biters.World;
using Biters.Utility;

namespace Biters.Game
{	

	#region Custom

	/*
	 * Custom type that uses a "Script" built from a Dictionary of IDirectionSuggestions.
	 */
	public abstract class CustomTileAutoPilotFactoryDelegate : IDirectionalTileAutoPilotFactoryDelegate {

		public abstract Dictionary<DirectionalGameTileType, IDirectionSuggestion> Script { get; }
		
		public readonly DirectionalGameTileType Type;
		
		public CustomTileAutoPilotFactoryDelegate(DirectionalGameTileType Type) {
			this.Type = Type;
		}
		
		public virtual WorldDirection HeadingForElement(IPositionalElement Target, IPositionalElement Element) {
			IDirectionSuggestion suggestion = this.Script [this.Type];
			
			WorldPositionAlignment side = WorldPositionAlignmentInfo.GetAlignment(Target.Position, Element.Position);
			WorldDirection heading = suggestion.GetSuggestion(side).Value;
			return heading;
		}
		
		public virtual Vector3 DirectionForElement(IPositionalElement Target, IPositionalElement Element) {
			Vector3 direction = this.HeadingForElement(Target, Element).Vector();
			return direction;
		}
		
	}

	#endregion
	
	#region Maze
	
	/*
	 * Keep elements moving forward as best as possible, and from passing over tile "boundaries".
	 * 
	 * When elements cannot move forward, they choose a new direction randomly that is acceptable to the tile.
	 */
	public class MazeTileAutoPilotFactoryDelegate : CustomTileAutoPilotFactoryDelegate {
		
		internal readonly static Dictionary<DirectionalGameTileType, IDirectionSuggestion> Suggestions = DefaultSuggestions;
		
		//Static AI Logic
		internal static Dictionary<DirectionalGameTileType, IDirectionSuggestion> DefaultSuggestions {
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
		
		public override Dictionary<DirectionalGameTileType, IDirectionSuggestion> Script { get { return Suggestions; } }
		
		public MazeTileAutoPilotFactoryDelegate(DirectionalGameTileType Type) : base(Type) {}
		
	}
	
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
	
	#endregion

	#region Rotate
	
	/*
	 * Distributes elements evenly across the possible paths.
	 */
	public class RotateTileAutoPilotFactoryDelegate : CustomTileAutoPilotFactoryDelegate {

		//Static AI Logic
		internal static Dictionary<DirectionalGameTileType, IDirectionSuggestion> DefaultSuggestions {
			get {
				//Start with the MazeTileAutoPilotFactoryDelegate Script.
				Dictionary<DirectionalGameTileType, IDirectionSuggestion> s = MazeTileAutoPilotFactoryDelegate.DefaultSuggestions;

				//T_Sections
				//TODO: Update script later to instead always rotate between other two exists, not only when at the crossroads.
				s.Remove(DirectionalGameTileType.T_Up);
				s.Add(DirectionalGameTileType.T_Up, new HeadingSuggestion(WorldDirection.North)
				      .AllowAllBut(WorldDirection.South)		//If facing south, go east or west.
				      .Suggest(IfThenSuggestion.Rotate(WorldDirection.South, WorldDirection.East, WorldDirection.West)));

				s.Remove(DirectionalGameTileType.T_Down);
				s.Add(DirectionalGameTileType.T_Down, new HeadingSuggestion(WorldDirection.South)
				      .AllowAllBut(WorldDirection.North)		//If facing north, go east or west.
				      .Suggest(IfThenSuggestion.Rotate(WorldDirection.North, WorldDirection.East, WorldDirection.West)));

				s.Remove(DirectionalGameTileType.T_Left);
				s.Add(DirectionalGameTileType.T_Left, new HeadingSuggestion(WorldDirection.West)
				      .AllowAllBut(WorldDirection.East)		//If facing east, go north or south.
				      .Suggest(IfThenSuggestion.Rotate(WorldDirection.East, WorldDirection.North, WorldDirection.South)));

				s.Remove(DirectionalGameTileType.T_Right);
				s.Add(DirectionalGameTileType.T_Right, new HeadingSuggestion(WorldDirection.East)
				      .AllowAllBut(WorldDirection.West)		//If facing west, go north or south.
				      .Suggest(IfThenSuggestion.Rotate(WorldDirection.West, WorldDirection.North, WorldDirection.South)));

				return s;
			}
		}
		
		private readonly Dictionary<DirectionalGameTileType, IDirectionSuggestion> Suggestions = DefaultSuggestions;

		//Script is not a static scrip, since it relys on Rotate.
		public override Dictionary<DirectionalGameTileType, IDirectionSuggestion> Script { get { return this.Suggestions; } }
		
		public RotateTileAutoPilotFactoryDelegate(DirectionalGameTileType Type) : base(Type) {}
		
	}
	
	public static class DirectionalGameTileRotatingExtension
	{
		
		/*
		 * Changes the delegate to be a Maze Tile intead.
		 */
		public static T MakeRotationTile<T>(this T Tile) where T : DirectionalGameTile {
			Tile.DirectionDelegate = new RotateTileAutoPilotFactoryDelegate(Tile.TileType);
			return Tile;
		}
		
	}
	
	#endregion

}

