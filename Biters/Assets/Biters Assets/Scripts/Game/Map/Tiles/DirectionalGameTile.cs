using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Biters;
using Biters.Utility;

namespace Biters.Game
{

	/*
	 * Basic game tile that moves entities that enter it's tiles towards the center, and then in a new direction.
	 */
	public class DirectionalGameTile : BitersGameTile {

		public const float DefaultMoveSpeed = 1.0f;
		public const string DirectionalTileId = "Entity.Direction";

		//Factory which builds AutoPilots for entities that enter this tile.
		public IAutoPilotFactory MovementFactory;

		#region Constructors
		
		public DirectionalGameTile () : this (DirectionalGameTileType.Vertical, DefaultMoveSpeed) {}
		
		public DirectionalGameTile (DirectionalGameTileType Type) : this (Type,  DefaultMoveSpeed) {}

		public DirectionalGameTile (DirectionalGameTileType Type, float MoveSpeed) : base () {
			this.SetTileType (Type, MoveSpeed);
		}

		#endregion

		#region Entity
		
		public override string EntityId {
			get {
				return DirectionalTileId;
			}
		}
		
		#endregion

		#region Accessors

		public void SetTileType(DirectionalGameTileType Type, float MoveSpeed) {
			this.MovementFactory = new DirectionalTileAutoPilotFactory (Type, MoveSpeed);
			this.GameObject.renderer.material = Type.TileMaterial();
			Type.RotateTileObject(this);
		}

		#endregion
		
		#region Events

		public override void RegisterForEvents() {
			this.Map.RegisterForGameMapEvent (this, GameMapEvent.EntityEnteredTile);
			//this.Map.RegisterForEvent (this, GameMapEvent.EntityExitedTile);
		}
		
		protected override void HandleGameMapEvent(GameMapEventInfo Info) {
			
			switch (Info.GameMapEvent) {
			case GameMapEvent.EntityEnteredTile:
				if (Info.Position.Equals(this.MapTilePosition)) {
					this.ChangeEntityMovement(Info);
				}
				break;
			}
			
		}
		
		#endregion

		#region Redirection

		protected void ChangeEntityMovement(GameMapEventInfo Info) {
			IGameMapEntity entity = Info.Entity;
			entity.Movement.AutoPilot = this.MovementFactory.Make(this, entity);
		}

		#endregion

	}

	public enum DirectionalGameTileType : int {

		//Vertical
		Vertical,

		//Horizontal
		Horizontal,

		/*
		 * T Intersections. The cross is the directional output.
		 * 
		 * Movement is computed based on where the element is entering from.
		 */
		T_Up,
		T_Down,
		T_Left,
		T_Right,

		/*
		 * Corner Pieces.
		 */
		Corner_Top_Right,
		Corner_Top_Left,
		Corner_Bottom_Right,
		Corner_Bottom_Left

		/*
		//Ends
		E_Up,
		E_Down,
		E_Left,
		E_Right

		//Custom
		Custom
		*/
	}

	//Vector Extension 
	public static class WorldDirectionalGameTileInfo
	{

		//TODO: Will later ditch this for a Material Factory that can load Tilesets, but that will be later.
		
		public static Material DefaultTileMat = ResourceLoader.Load["Tiles_Concrete"].Material;
		public static Material HorizontalTileMat = ResourceLoader.Load["Tiles_Horizontal"].Material;
		public static Material VerticalTileMat = ResourceLoader.Load["Tiles_Vertical"].Material;
		public static Material TIntersectionTileMat = ResourceLoader.Load["Tiles_T_Intersection"].Material;
		public static Material CornerTileMat = ResourceLoader.Load["Tiles_Corner"].Material;

		public static Vector3 Vector(this WorldDirection Direction)
		{
			WorldPositionChange change = Direction.PositionChange ();
			return change.Vector();
		}

		public static Vector3 Vector(this WorldPositionChange Change)
		{
			Vector3 direction = new Vector3 ();
			direction.x = Change.dX;
			direction.y = Change.dY;
			return direction;
		}

		public static Material TileMaterial(this DirectionalGameTileType Type) {
			Material material = null;

			switch (Type) {
			case DirectionalGameTileType.Vertical:
				material = VerticalTileMat;
				break;
				
			case DirectionalGameTileType.Horizontal:
				material = HorizontalTileMat;
				break;
			
			case DirectionalGameTileType.Corner_Top_Left:
			case DirectionalGameTileType.Corner_Top_Right:
			case DirectionalGameTileType.Corner_Bottom_Left:
			case DirectionalGameTileType.Corner_Bottom_Right:
				material = CornerTileMat;
				break;

			case DirectionalGameTileType.T_Up:
			case DirectionalGameTileType.T_Down:
			case DirectionalGameTileType.T_Left:
			case DirectionalGameTileType.T_Right:
				//And other T Intersections...
			default: 
				material = TIntersectionTileMat;
				break;
			}

			return material;
		}

		/*
		 * Tile Cube Rotations to make sure the tiles go the correct way with their material.
		 */
		public static void RotateTileObject(this DirectionalGameTileType Type, ITransformableElement Element) {
			
			switch (Type) {
				
			case DirectionalGameTileType.Corner_Top_Left:
			case DirectionalGameTileType.T_Up:
				Element.Transform.Rotate(0,0,90);
				break;
				
			case DirectionalGameTileType.Corner_Top_Right:
			case DirectionalGameTileType.T_Left:
				//No Rotation.
				break;
			case DirectionalGameTileType.Corner_Bottom_Left:
			case DirectionalGameTileType.T_Right:
				Element.Transform.Rotate(0,90,0);
				break;
			case DirectionalGameTileType.Corner_Bottom_Right:
			case DirectionalGameTileType.T_Down:
				Element.Transform.Rotate(0,0,270);
				break;
			default:
				break;
			}
		}

		/*
		 * Direction the tile will point elements.
		 * 
		 * T elements default to where the | points.
		 * 
		 * Corner elements point up and down.
		 */

		//TODO: Remove this. Use DirectionSuggestion instead.
		public static Vector3 DefaultTileDirection(this DirectionalGameTileType Type) {

			Vector3 direction;

			switch (Type) {
			case DirectionalGameTileType.Vertical:
			case DirectionalGameTileType.T_Up:
			case DirectionalGameTileType.Corner_Top_Left:
			case DirectionalGameTileType.Corner_Top_Right:
				direction = WorldDirection.North.Vector();
				break;
				
			case DirectionalGameTileType.Horizontal:
			case DirectionalGameTileType.T_Left:
				direction = WorldDirection.West.Vector();
				break;
			case DirectionalGameTileType.T_Right:
				direction = WorldDirection.East.Vector();
				break;

			case DirectionalGameTileType.T_Down:
			case DirectionalGameTileType.Corner_Bottom_Right:
			case DirectionalGameTileType.Corner_Bottom_Left:
				direction = WorldDirection.South.Vector();
				break;

			default: 
				direction = WorldDirection.South.Vector();
				break;
			}

			return direction;
		}

	}

	#region Auto Pilot Factory

	/*
	 * Default Implementation.
	 */
	public class DirectionalTileAutoPilotFactory : IAutoPilotFactory {

		public float MoveSpeed = 1.0f;
		public Vector3 Offset = new Vector3 (0, 0, -BitersGameTile.BitersGameTileZOffset);

		private DirectionalGameTileType Type;
		public IDirectionalTileAutoPilotFactoryDelegate Delegate = new DirectionalTileAutoPilotFactoryDelegate();

		public DirectionalTileAutoPilotFactory(DirectionalGameTileType Type, float MoveSpeed) {
			this.Type = Type;
			this.MoveSpeed = MoveSpeed;
		}

		public IAutoPilot Make() {
			throw new InvalidOperationException ("Factory requires a target and element.");
		}
		
		public IAutoPilot Make(IPositionalElement Target, IPositionalElement Element) {
			AutoPilotQueue queue = new AutoPilotQueue ();
			queue.Add (this.MoveToCenter (Target, Element));
			queue.Add (this.MoveOutOfSquare (Target, Element));
			return queue;
		}

		public virtual IAutoPilot MoveToCenter(IPositionalElement Target, IPositionalElement Element) {
			PositionalElementOffset middle = new PositionalElementOffset (Target, Offset);
			return new WalkToTargetAutoPilot(middle, Element, this.MoveSpeed);
		}
		
		public virtual IAutoPilot MoveOutOfSquare(IPositionalElement Target, IPositionalElement Element) {
			Vector3 direction = this.Delegate.DirectionForElement(Target, Element, this.Type);
			Vector3 moveDirection = (direction * this.MoveSpeed);

			/*
			 * Handle corner-type tiles and stuff.
			 */

			return new WalkAutoPilot(moveDirection);
		}
	}
	
	public interface IDirectionalTileAutoPilotFactoryDelegate {
		
		/*
		 * Returns the direction.
		 */
		Vector3 DirectionForElement(IPositionalElement Target, IPositionalElement Element, DirectionalGameTileType type);
		
	}

	/*
	 * Default Implementation. Uses an IDirectionalTileMoveSuggestion.
	 */
	public class DirectionalTileAutoPilotFactoryDelegate : IDirectionalTileAutoPilotFactoryDelegate {

		private readonly static Dictionary<DirectionalGameTileType, IDirectionSuggestion> Suggestions = DefaultSuggestions;

		private static Dictionary<DirectionalGameTileType, IDirectionSuggestion> DefaultSuggestions {
			get {
				Dictionary<DirectionalGameTileType, IDirectionSuggestion> s 
					= new Dictionary<DirectionalGameTileType, IDirectionSuggestion>();
				
				s.Add(DirectionalGameTileType.Vertical, new DirectionSuggestion().AvoidEastWest());
				s.Add(DirectionalGameTileType.Horizontal, new DirectionSuggestion().AvoidUpDown());

				//TODO: Complete for each tile type.

				return s;
			}
		}
		
		public DirectionalTileAutoPilotFactoryDelegate() {}
		
		public Vector3 DirectionForElement(IPositionalElement Target, IPositionalElement Element, DirectionalGameTileType type) {
			WorldPositionAlignment side = WorldPositionAlignmentInfo.GetAlignment(Target.Position, Element.Position);
			Vector3 direction = new Vector3 ();

			//TODO: Get Type/Suggestion form static.
			
			//TODO: Get Alignment.
			
			//TODO: Get Vector for direction.

			return direction;
		}

	}


	#endregion

}

