using System;
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
		
		public DirectionalGameTile () : this (DirectionalGameTileType.Up, DefaultMoveSpeed) {}
		
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
		Up,
		Down,

		//Horizontal
		Left,
		Right,

		/*
		 * T Intersections. The cross is the directional output.
		 */
		T_Up,
		T_Down,
		T_Left,
		T_Right,

		//TODO: Corners!!!

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
			case DirectionalGameTileType.Up:
			case DirectionalGameTileType.Down:
				material = VerticalTileMat;
				break;

			case DirectionalGameTileType.Left:
			case DirectionalGameTileType.Right: 
				material = HorizontalTileMat;
				break;
				
			case DirectionalGameTileType.T_Up:
			case DirectionalGameTileType.T_Down:
			case DirectionalGameTileType.T_Left:
			case DirectionalGameTileType.T_Right:
				material = TIntersectionTileMat;
				break;

			default: 
				material = DefaultTileMat;
				break;
			}

			return material;
		}

		//Tile Cube Rotations to make sure the tiles go the correct way with their material.
		public static void RotateTileObject(this DirectionalGameTileType Type, ITransformableElement Element) {
			
			switch (Type) {
			case DirectionalGameTileType.T_Up:
				Element.Transform.Rotate(90,0,0);
				break;
			case DirectionalGameTileType.T_Left:
				Element.Transform.Rotate(0,180,0);
				break;
			case DirectionalGameTileType.T_Right:
				//No Rotation.
				break;
			case DirectionalGameTileType.T_Down:
				Element.Transform.Rotate(0,-90,0);
				break;
			default:
				break;
			}
		}

		//Direction the tile will point elements.
		public static Vector3 TileDirection(this DirectionalGameTileType Type) {

			Vector3 direction;

			switch (Type) {
			case DirectionalGameTileType.Up:
			case DirectionalGameTileType.T_Up:
				direction = WorldDirection.North.Vector();
				break;
				
			case DirectionalGameTileType.Left:
			case DirectionalGameTileType.T_Left:
				direction = WorldDirection.West.Vector();
				break;
			case DirectionalGameTileType.Right: 
			case DirectionalGameTileType.T_Right:
				direction = WorldDirection.East.Vector();
				break;

			case DirectionalGameTileType.Down:
			case DirectionalGameTileType.T_Down:
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

	public class DirectionalTileAutoPilotFactory : IAutoPilotFactory {

		public float MoveSpeed = 1.0f;
		public Vector3 Offset = new Vector3 (0, 0, -BitersGameTile.BitersGameTileZOffset);

		private Vector3 direction;
		private DirectionalGameTileType type;

		public DirectionalGameTileType Type {
			get {
				return this.type;
			} 

			set {
				this.type = value;
				this.direction = value.TileDirection();
			}
		}

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
			queue.Add (this.MoveOutOfSquare ());
			return queue;
		}

		public virtual IAutoPilot MoveToCenter(IPositionalElement Target, IPositionalElement Element) {
			PositionalElementOffset middle = new PositionalElementOffset (Target, Offset);
			return new WalkToTargetAutoPilot(middle, Element, this.MoveSpeed);
		}
		
		public virtual IAutoPilot MoveOutOfSquare() {
			Vector3 moveDirection = (this.direction * this.MoveSpeed);
			return new WalkAutoPilot(moveDirection);
		}

	}

	#endregion

}

