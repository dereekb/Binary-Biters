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
		void Update();
	}

	#region Moving Element

	/*
	 * Element that has a position.
	 */
	public interface IPositionalElement
	{

		Vector3 Position { get; }

	}

	/*
	 * Element that exposes it's transform directly.
	 */
	public interface ITransformableElement : IPositionalElement
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

