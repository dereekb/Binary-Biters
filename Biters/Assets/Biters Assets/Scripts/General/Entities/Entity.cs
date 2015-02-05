using System;
using UnityEngine;

namespace Biters
{

	/*
	 * Base entity class.
	 */
	public abstract class Entity : GameElement, VisibleElement, TransformableElement, MovingElement, UpdatingElement
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

		public Material Material {

			get {
				return GameObject.renderer.material;
			}

			set {
				GameObject.renderer.material = value;
			}

		}

		public Transform Transform {

			get {
				return this.GameObject.transform;
			}

		}

		#endregion

		#region Functions

		public abstract void Update (Time time);

		#endregion
	}

}

