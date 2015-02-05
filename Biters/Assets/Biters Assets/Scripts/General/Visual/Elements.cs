using System;
using UnityEngine;

namespace Biters 
{
	/*
	 * A game element.
	 */
	public interface GameElement 
	{
		GameObject GameObject { get; }
	}

	/*
	 * A visible game element. 
	 * 
	 * Allows access to material.
	 */
	public interface VisibleElement
	{
		Material Material { get; set; }
	}

	#region Moving Element
	
	/*
	 * Acts as the movement of a game object.
	 * 
	 * Wraps a Transformable element to performe movements.
	 */
	public class Movement : TransformableElement {
		
		private TransformableElement element;

		public Movement(TransformableElement Element) {
			this.element = Element;
		}
		
		public Transform Transform { 
			
			get {
				return element.Transform;
			}

		}
		
		/*
		 * TODO: Add helper functions for moving an element around.
		 * - Step Forwards
		 * - Step Backwards
		 * - Set Heading/Direction
		 * - Set Position
		 * etc.
		 */

	}

	/*
	 * Element that exposes it's transform directly.
	 */
	public interface TransformableElement
	{

		Transform Transform { get; }

	}
	
	/*
	 * A moving game element. 
	 * 
	 * Allows access to it's current X and Y positions on whatever plane of existence it currently exists on.
	 */
	public interface MovingElement
	{

		Movement Movement { get; }

	}

	#endregion

}

