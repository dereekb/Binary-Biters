using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Biters.Utility;

namespace Biters 
{

	#region Movement

	/*
	 * Movement with an AutoPilot.
	 */
	public interface IMovement : ITransformableElement, IPositionalElement, IUpdatingElement {
		
		IAutoPilot AutoPilot { get; set; }

		bool CanSetMovement();

	}

	/*
	 * Acts as the movement of a game object. Is moved via autopilots.
	 * 
	 * Wraps a Transformable element to performe movements.
	 */
	public class Movement : IMovement {
		
		private IAutoPilot autoPilot;
		private ITransformableElement element;
		
		public Movement(ITransformableElement Element) : this (Element, null) {}
		
		protected Movement(ITransformableElement Element, IAutoPilot AutoPilot) {
			this.element = Element;
			this.autoPilot = AutoPilot;
		}

		public virtual IAutoPilot AutoPilot {

			get {
				return this.autoPilot;
			}
			
			set {
				if (this.autoPilot != null) {
					this.autoPilot.DestroyAutoPilot();
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

		public virtual bool CanSetMovement() {
			return true;
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

	/*
	 * Movement that has a single, locked AutoPilot. 
	 */
	public class ProtectedMovement : Movement {

		private AutoPilotQueue autoPilotQueue;

		public ProtectedMovement(ITransformableElement Element) : this (Element, new AutoPilotQueue()) {}

		protected ProtectedMovement(ITransformableElement Element, AutoPilotQueue AutoPilot) : base (Element, AutoPilot) {
			this.autoPilotQueue = AutoPilot;
		}
		
		public override IAutoPilot AutoPilot {

			set {
				//Prevent overriding of Auto Pilot. 
			}
			
		}
		
		public override bool CanSetMovement() {
			return false;
		}

		#region Update
		
		protected override void UpdateWithAutoPilot() {
			this.autoPilotQueue.AutoMove (this);
		}

		#endregion

		#region Queue
		
		public void Enqueue(IAutoPilot AutoPilot) {
			this.autoPilotQueue.Enqueue (AutoPilot);
		}
		
		public void Clear() {
			this.autoPilotQueue.Clear ();
		}
		
		#endregion

	}

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
		void DestroyAutoPilot();

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
			position += (direction * Time.deltaTime);
			return position;
		}

		public virtual bool IsComplete() {
			return false; //Never ends.
		}
		
		public virtual void DestroyAutoPilot() {
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
		
		public override string ToString ()
		{
			return string.Format ("[PositionalVectorWrapper: Position={0}]", position);
		}

	}
	
	/*
	 * Wrapper for offsetting an IPositionalElement.
	 */
	public struct PositionalElementOffset : IPositionalElement
	{
		public readonly IPositionalElement Element;
		public readonly Vector3 Offset;
		
		public PositionalElementOffset(IPositionalElement Element, Vector3 Offset) {
			this.Element = Element;
			this.Offset = Offset;
		}
		
		public Vector3 Position {
			get {
				return (this.Element.Position + this.Offset);
			}
		}

		public override string ToString ()
		{
			return string.Format ("[PositionalElementOffset: Element={0}, Offset={1}]", Element, Offset);
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
		
		public virtual void DestroyAutoPilot() {
			this.Target = null;
			this.Element = null;
		}
		
		public override string ToString ()
		{
			return string.Format ("[WalkToTargetAutoPilot: Target={0}, Element={1}], Speed={2}", this.Target, this.Element, this.Speed);
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

		public virtual IAutoPilot Current {
			get {
				return this.current;
			}
		}

		public bool HasCurrent {
			get {
				return this.current != null;
			}
		}

		public bool HasRemaining {
			get {
				return (this.HasCurrent || (this.IsEmpty == false));
			}
		}

		public bool IsEmpty {
			get {
				return (this.queue.Count == 0);
			}
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
			return (current == null || current.IsComplete ()) && this.IsEmpty;
		}
		
		public void DestroyAutoPilot() {
			this.Clear ();
		}

		#endregion

		#region Queue
		
		public virtual void Enqueue(IAutoPilot AutoPilot) {
			if (this.HasCurrent) {
				this.queue.Enqueue(AutoPilot);
			} else {
				this.SetCurrentPilot(AutoPilot);
			}
		}
		
		protected virtual IAutoPilot DequeueNextPilot() {
			IAutoPilot next = null;
			
			if (this.IsEmpty == false) {
				next = queue.Dequeue();
			}

			return this.SetCurrentPilot(next);
		}

		protected virtual IAutoPilot SetCurrentPilot(IAutoPilot Next) {
			if (this.current != null) {
				this.current.DestroyAutoPilot ();
			}

			this.current = Next;
			return Next;
		}

		public virtual void Clear() {

			if (this.current != null) {
				this.current.DestroyAutoPilot();
				this.current = null;
			}

			if (this.IsEmpty == false) {
				foreach (IAutoPilot pilot in this.queue) {
					pilot.DestroyAutoPilot ();
				}

				this.queue.Clear();
			}
		}

		#endregion

		public override string ToString ()
		{
			return string.Format ("[AutoPilotQueue: Current={0}, Count={1}, IsEmpty={2}]", Current, this.queue.Count, IsEmpty);
		}

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