using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Biters;
using Biters.Utility;

namespace Biters.Game
{

	/*
	 * Standard game map tile that will send units in the target direction.
	 * 
	 * Uses a DirectionalTileAutoPilotFactory for handling movement.
	 */
	public class DirectionalGameTile : MoveEntityGameTile
	{
		public const string DirectionalTileId = "Entity.Direction";
		
		private DirectionalGameTileType tileType;
		
		public DirectionalGameTileType TileType {
			get {
				return this.tileType;
			}
			
			set {
				this.SetTileType(value);
			}
			
		}

		//Internal element to avoid need to cast to MovementFactory.
		private DirectionalTileAutoPilotFactory tileDirectionFactory;
		
		public virtual DirectionalTileAutoPilotFactory TileDirectionFactory {
			
			get {
				return this.tileDirectionFactory;
			}

		}

		public virtual IDirectionalTileAutoPilotFactoryDelegate DirectionDelegate {

			get {
				return this.tileDirectionFactory.Delegate;
			}

			set {
				this.tileDirectionFactory.Delegate = value;
			}

		}

		#region Constructor
		
		public DirectionalGameTile () : this(new DirectionalTileAutoPilotFactory()) {}
		
		public DirectionalGameTile (DirectionalGameTileType Type)
		: this(new DirectionalTileAutoPilotFactory()) {
			this.SetTileType (Type);
		}

		public DirectionalGameTile (DirectionalGameTileType Type, WorldDirection Direction)
			: this(new DirectionalTileAutoPilotFactory()) {
			this.SetTileType (Type);
			this.SetTileDirection (Direction);
		}
		
		public DirectionalGameTile (IDirectionalTileAutoPilotFactoryDelegate Delegate)
			: this(new DirectionalTileAutoPilotFactory(), Delegate) {}

		public DirectionalGameTile (DirectionalTileAutoPilotFactory Factory) : this(Factory, null) {
			this.tileDirectionFactory = Factory;
		}
		
		public DirectionalGameTile (DirectionalTileAutoPilotFactory Factory, IDirectionalTileAutoPilotFactoryDelegate Delegate)
			: base (Factory) {
			this.tileDirectionFactory = Factory;
			this.DirectionDelegate = Delegate;
		}

		#endregion

		#region Entity
		
		public override string EntityId {
			get {
				return DirectionalTileId;
			}
		}
		
		#endregion
		
		#region Initialization
		
		public void SetTileType(DirectionalGameTileType Type) {
			this.tileType = Type;
			this.GameObject.renderer.material = Type.TileMaterial();
			Type.RotateTileObject(this);
		}

		public virtual void SetTileDirection(WorldDirection Direction) {
			//Override to prevent in sub-classes if this behavior is unwanted.
			DirectionalTileAutoPilotFactoryDelegate NewDelegate = new DirectionalTileAutoPilotFactoryDelegate (Direction);
			this.tileDirectionFactory.Delegate = NewDelegate;
		}
		
		#endregion
	}

	#region Game Tile Type
	
	public enum DirectionalGameTileType : int {

		//Vertical
		Vertical = 0,
		
		//Horizontal
		Horizontal = 1,
		
		/*
		 * T Intersections. The cross is the directional output.
		 * 
		 * Movement is computed based on where the element is entering from.
		 */
		T_Up = 10,
		T_Down = 11,
		T_Left = 12,
		T_Right = 13,
		
		/*
		 * Corner Pieces.
		 */
		Corner_Top_Right = 20,
		Corner_Top_Left = 21,
		Corner_Bottom_Right = 22,
		Corner_Bottom_Left = 23
		
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
	public static class DirectionalGameTileInfo
	{
		
		//TODO: Will later ditch this for a Material Factory that can load Tilesets, but that will be later.
		
		public static Material DefaultTileMat = ResourceLoader.Load["Tiles_Concrete"].Material;
		public static Material HorizontalTileMat = ResourceLoader.Load["Tiles_Horizontal"].Material;
		public static Material VerticalTileMat = ResourceLoader.Load["Tiles_Vertical"].Material;
		public static Material TIntersectionTileMat = ResourceLoader.Load["Tiles_T_Intersection"].Material;
		public static Material CornerTileMat = ResourceLoader.Load["Tiles_Corner"].Material;

		public static readonly DirectionalGameTileType[] All = new DirectionalGameTileType[] {
			DirectionalGameTileType.Vertical, DirectionalGameTileType.Horizontal,

			DirectionalGameTileType.T_Up, DirectionalGameTileType.T_Down, 
			DirectionalGameTileType.T_Left, DirectionalGameTileType.T_Right,

			DirectionalGameTileType.Corner_Top_Right, DirectionalGameTileType.Corner_Top_Left, 
			DirectionalGameTileType.Corner_Bottom_Right, DirectionalGameTileType.Corner_Bottom_Left
		};

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
				
			case DirectionalGameTileType.Corner_Bottom_Right:
			case DirectionalGameTileType.T_Down:
				Element.Transform.Rotate(0,0,90);
				break;
				
			case DirectionalGameTileType.Corner_Bottom_Left:
			case DirectionalGameTileType.T_Left:
				//No Rotation.
				break;
			case DirectionalGameTileType.Corner_Top_Right:
			case DirectionalGameTileType.T_Right:
				Element.Transform.Rotate(0,90,0);
				break;
			case DirectionalGameTileType.Corner_Top_Left:
			case DirectionalGameTileType.T_Up:
				Element.Transform.Rotate(0,0,270);
				break;
			default:
				break;
			}
		}
		
	}

	#endregion

	#region Directional Auto Pilot Factory
	
	/*
	 * Default Implementation.
	 */
	public class DirectionalTileAutoPilotFactory : IAutoPilotFactory {
		
		public float MoveSpeed = 1.0f;
		public Vector3 Offset = new Vector3 (0, 0, -BitersGameTile.BitersGameTileZOffset);
		public IDirectionalTileAutoPilotFactoryDelegate Delegate;

		public DirectionalTileAutoPilotFactory() {}

		public DirectionalTileAutoPilotFactory(float MoveSpeed, IDirectionalTileAutoPilotFactoryDelegate Delegate) {
			this.MoveSpeed = MoveSpeed;
			this.Delegate = Delegate;
		}
		
		public IAutoPilot Make() {
			throw new InvalidOperationException ("Factory requires a target and element.");
		}
		
		public IAutoPilot Make(IPositionalElement Target, IPositionalElement Element) {
			AutoPilotQueue queue = new AutoPilotQueue ();
			queue.Enqueue (this.MoveToCenter (Target, Element));
			queue.Enqueue (this.MoveOutOfSquare (Target, Element));
			return queue;
		}
		
		public virtual IAutoPilot MoveToCenter(IPositionalElement Target, IPositionalElement Element) {
			PositionalElementOffset middle = new PositionalElementOffset (Target, Offset);
			return new WalkToTargetAutoPilot(middle, Element, this.MoveSpeed);
		}
		
		public virtual IAutoPilot MoveOutOfSquare(IPositionalElement Target, IPositionalElement Element) {
			Vector3 direction = this.Delegate.DirectionForElement(Target, Element);
			Vector3 moveDirection = (direction * this.MoveSpeed);
			return new WalkAutoPilot(moveDirection);
		}
		
	}
	
	public interface IDirectionalTileAutoPilotFactoryDelegate {
		
		/*
		 * Returns the direction to send the element after reaching the middle of the tile.
		 */
		Vector3 DirectionForElement(IPositionalElement Target, IPositionalElement Element);
		
	}
	
	/*
	 * Basic implementation that has a direction to send elements.
	 */
	public class DirectionalTileAutoPilotFactoryDelegate : IDirectionalTileAutoPilotFactoryDelegate {
		
		public WorldDirection Direction;
		
		public DirectionalTileAutoPilotFactoryDelegate(WorldDirection Direction) {
			this.Direction = Direction;
		}
		
		public Vector3 DirectionForElement(IPositionalElement Target, IPositionalElement Element) {
			return this.Direction.Vector ();
		}
		
	}
	
	#endregion

}

