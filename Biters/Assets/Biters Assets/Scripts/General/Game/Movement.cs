using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Biters.Utility;

namespace Biters 
{

	#region Movement

	/*
	 * Acts as the movement of a game object. Is moved via autopilots.
	 * 
	 * Wraps a Transformable element to performe movements.
	 */
	public class Movement : ITransformableElement, IPositionalElement, IUpdatingElement {
		
		private IAutoPilot autoPilot;
		private ITransformableElement element;
		
		public Movement(ITransformableElement Element) {
			this.element = Element;
		}
		
		public IAutoPilot AutoPilot {

			get {
				return this.autoPilot;
			}
			
			set {
				if (this.autoPilot != null) {
					this.autoPilot.CancelAutoPilot();
				}

				this.autoPilot = value;
			}

		}
		
		public Transform Transform { 
			
			get {
				return element.Transform;
			}
			
		}

		public Vector3 Position { 
			
			get {
				return element.Position;
			}
			
		}
		
		public virtual void Update() {
			this.UpdateWithAutoPilot ();
		}

		protected virtual void UpdateWithAutoPilot() {
			if (this.autoPilot != null) {
				if (this.autoPilot.AutoMove (this)) {
					this.autoPilot = null;
				}
			}
		}

		public override string ToString ()
		{
			return string.Format ("[Movement: AutoPilot={0}, Position={2}]", AutoPilot, Transform, Position);
		}

	}

	//TODO: Consider extending Movement with a class that supports a queue of auto pilots.
	
	#endregion

	#region Auto Pilot

	/*
	 * Auto Pilot that tells an element how to move, and for how long or at what coordinate to stop at.
	 */
	public interface IAutoPilot {

		/*
		 * Moves using the given Movement. 
		 * 
		 * Return true if complete.
		 */
		bool AutoMove(Movement Movement);

		/*
		 * Returns true if the 
		 */
		bool IsComplete();
		
		/*
		 * Called to cancel/end the current AutoPilot.
		 */
		void CancelAutoPilot();

	}

	/*
	 * Continuous Movement AutoPilot. 
	 */
	public class WalkAutoPilot : IAutoPilot {

		//Direction and speed vector.
		protected Vector3 direction;

		public virtual Vector3 Direction {

			get {
				return direction;
			}
			
			set {
				this.direction = value;
			}

		}

		protected WalkAutoPilot() : this(new Vector3(1, 1)) {}

		public WalkAutoPilot(Vector3 Direction) {
			this.direction = Direction;
		}

		public virtual bool AutoMove(Movement Movement) {
			this.MakeMovement (Movement);
			return this.IsComplete ();
		}
		
		protected virtual void MakeMovement(Movement Movement) {
			Movement.Transform.position = this.GetNextMovePosition (Movement);
		}

		protected virtual Vector3 GetNextMovePosition(Movement Movement) {
			Vector3 position = Movement.Transform.position;
			position.x += direction.x * Time.deltaTime;
			position.y += direction.y * Time.deltaTime;
			position.z += direction.z * Time.deltaTime;
			return position;
		}

		public virtual bool IsComplete() {
			return false; //Never ends.
		}
		
		public virtual void CancelAutoPilot() {
			//Nothing to do.
		}

		public override string ToString ()
		{
			return string.Format ("[WalkAutoPilot: Direction={0}]", this.direction);
		}

	}

	/*
	 * AutoPilot implementation that walks for a certain amount of time.
	 */
	public class WalkForTimeAutoPilot : WalkAutoPilot {

		public Timer WalkTimer;

		public WalkForTimeAutoPilot(Vector3 Direction, float WalkTime) : base(Direction) {
			this.WalkTimer = new Timer(WalkTime);
		}

		public bool HasTimeLeft {
			get {
				return !WalkTimer.Done;
			}
		}

		public override bool AutoMove(Movement Movement) {
			bool complete = !this.HasTimeLeft;

			if (!complete) {
				this.MakeMovement(Movement);
				WalkTimer.Update();
			}

			return complete;
		}
		
		public override bool IsComplete() {
			return !this.HasTimeLeft;
		}

		public override string ToString ()
		{
			return string.Format ("[WalkForTimeAutoPilot: Timer={0}]", this.WalkTimer);
		}

	}

	/*
	 * Wrapper for a Vector3 to implement IPositionalElement
	 */
	public struct PositionalVectorWrapper : IPositionalElement
	{
		private readonly Vector3 position;
		
		public PositionalVectorWrapper(Vector3 Position) {
			this.position = Position;
		}
		
		public Vector3 Position {
			get {
				return this.position;
			}
		}
		
	}
	
	/*
	 * Wrapper for offsetting an IPositionalElement.
	 */
	public struct PositionalElementOffset : IPositionalElement
	{
		private readonly IPositionalElement element;
		private readonly Vector3 offset;
		
		public PositionalElementOffset(IPositionalElement Element, Vector3 Offset) {
			this.element = Element;
			this.offset = Offset;
		}
		
		public Vector3 Position {
			get {
				return (this.element.Position + this.offset);
			}
		}
		
	}

	/*
	 * AutoPilot implementation that walks towards a position until it reaches that position.
	 */
	public class WalkToTargetAutoPilot : IAutoPilot {

		public IPositionalElement Target;
		public IPositionalElement Element;
		public float Speed;
		
		public WalkToTargetAutoPilot(Vector3 Target, IPositionalElement Element) : this(new PositionalVectorWrapper(Target), Element, 1.0f) {}
		
		public WalkToTargetAutoPilot(Vector3 Target, IPositionalElement Element, float Speed) : this(new PositionalVectorWrapper(Target), Element, Speed) {}

		public WalkToTargetAutoPilot(IPositionalElement Target, IPositionalElement Element) : this(Target, Element, 1.0f) {}
		
		public WalkToTargetAutoPilot(IPositionalElement Target, IPositionalElement Element, float Speed) {
			this.Target = Target;
			this.Element = Element;
			this.Speed = Speed;
		}
		
		public virtual bool AutoMove(Movement Movement) {
			this.MakeMovement (Movement);
			return this.IsComplete ();
		}
		
		protected virtual void MakeMovement(Movement Movement) {
			Vector3 move = Vector3.MoveTowards (Element.Position, Target.Position, Speed * Time.deltaTime);
			Movement.Transform.position = move;
		}

		public virtual bool IsComplete() {
			return Element.Position == Target.Position; 
		}
		
		public virtual void CancelAutoPilot() {
			//Nothing to do.
		}

	}

	#endregion
	
	#region Auto Pilot Queue
	
	/*
	 * AutoPilot that has a queue of AutoPilots to run.
	 */
	public class AutoPilotQueue : IAutoPilot {

		protected Queue<IAutoPilot> queue = new Queue<IAutoPilot>();
		protected IAutoPilot current;

		public AutoPilotQueue() {}

		public IAutoPilot Current {
			get {
				return this.current;
			}
		}

		public bool Empty {
			get {
				return (this.queue.Count == 0);
			}
		}

		private IAutoPilot DequeueNextPilot() {
			IAutoPilot next = null;

			if (this.Empty == false) {
				next = queue.Dequeue();
			}

			current = next;
			return next;
		}

		#region Auto Pilot

		public virtual bool AutoMove(Movement Movement) {
			bool complete = this.IsComplete ();

			if (!complete) {
				IAutoPilot pilot = this.current;

				if (pilot == null) {
					//Always a next pilot at this point, due to checking isComplete.
					pilot = this.DequeueNextPilot();
				}

				bool pilotFinished = pilot.AutoMove(Movement);

				if (pilotFinished) {
					current = null;
					complete = this.IsComplete();
				}
			}

			return complete;
		}

		public virtual bool IsComplete() {
			return (current == null || current.IsComplete ()) && (queue.Count == 0);
		}
		
		public void CancelAutoPilot() {
			this.queue.Clear();
		}

		#endregion

		#region Queue

		public void Add(IAutoPilot AutoPilot) {
			queue.Enqueue (AutoPilot);
		}

		#endregion

	}

	#endregion
	
	#region Auto Pilot Factory

	/*
	 * Factory for making IAutoPilot instances. Make is given two optional inputs.
	 */
	public interface IAutoPilotFactory : IFactory<IAutoPilot> {

		IAutoPilot Make(IPositionalElement Target, IPositionalElement Element);

	}

	#endregion

	#region More Auto Pilots

	/*
	 * Units wander aimlessly.
	 */
	/*
	 * TODO: Complete
	public class WanderAutoPilot : WalkAutoPilot {

		public WanderAutoPilot(Vector3 Direction, float WalkTime) : base(Direction) {
		}
		
		public Vector3 RandomDirection() {
			
		}

		public Vector3 RandomWalkTime() {
			
		}
		
		public override bool AutoMove(Movement Movement) {
			bool complete = !this.HasTimeLeft;
			
			if (!complete) {
				this.MakeMovement(Movement);
				walkedTime += Time.deltaTime;
			}
			
			return complete;
		}
		
		public override bool IsComplete() {
			return !this.HasTimeLeft;
		}
		
	}
	*/

	#endregion

	/*
	 * TODO: Consider adding more types of autopilots as needed.
	 * - One that waits x seconds before moving again.
	 */

}