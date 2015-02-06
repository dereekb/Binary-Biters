using System;
using UnityEngine;

namespace Biters 
{
	/*
	 * A game element.
	 */
	public interface IGameElement 
	{
		GameObject GameObject { get; }
	}

	/*
	 * A visible game element. 
	 * 
	 * Allows access to material.
	 */
	public interface IVisibleElement
	{
		Material Material { get; set; }

	}

	/*
	 * A game element that can update itself.
	 */
	public interface IUpdatingElement
	{
		void Update(Time time);
	}

	#region Moving Element
	
	/*
	 * Acts as the movement of a game object.
	 * 
	 * Wraps a Transformable element to performe movements.
	 */
	public class Movement : ITransformableElement {
		
		private ITransformableElement element;

		public Movement(ITransformableElement Element) {
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
	public interface ITransformableElement
	{

		Transform Transform { get; }

	}
	
	/*
	 * A moving game element. 
	 * 
	 * Allows access to it's current X and Y positions on whatever plane of existence it currently exists on.
	 */
	public interface IMovingElement
	{

		Movement Movement { get; }

	}

	#endregion

}

