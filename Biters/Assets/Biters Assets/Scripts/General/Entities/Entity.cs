using System;
using UnityEngine;

namespace Biters
{

	/*
	 * Base entity class.
	 */
	public abstract class Entity : IGameElement, IVisibleElement, ITransformableElement, IMovingElement, IUpdatingElement
	{
		protected GameObject gameObject;
		protected Movement movement;

		#region Constructors

		protected Entity (GameObject GameObject) {
			this.gameObject = GameObject;
			this.movement = new Movement (this);
		}

		#endregion

		#region Accessors 

		public GameObject GameObject {

			get {
				return this.gameObject;
			}

		}

		public Movement Movement {

			get {
				return this.movement;
			}

		}
		
		public Transform Transform {
			
			get {
				return this.GameObject.transform;
			}
			
		}
		
		public Vector3 Position {
			
			get {
				return this.Transform.position;
			}
			
		}

		public Material Material {

			get {
				return GameObject.renderer.material;
			}

			set {
				GameObject.renderer.material = value;
			}

		}

		#endregion

		#region Functions

		public abstract void Update ();

		#endregion

		#region System

		public override string ToString ()
		{
			return string.Format ("[Entity: Movement={1} ]", GameObject, Movement, Transform, Position, Material);
		}

		#endregion

	}

}

