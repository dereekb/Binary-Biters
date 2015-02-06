using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Biters 
{

	#region Movement

	/*
	 * Acts as the movement of a game object. Is moved via autopilots.
	 * 
	 * Wraps a Transformable element to performe movements.
	 */
	public class Movement : ITransformableElement, IUpdatingElement {
		
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

		#region Movement Tweaks
		
		public void StepForward() {
			this.StepForward (Time.deltaTime);
		}
		
		public void StepForward(float timeDelta) {
			
		}
		
		public void StepBackwards() {
			this.StepBackwards (Time.deltaTime);
		}
		
		public void StepBackwards(float timeDelta) {
			
		}

		/*
		 * TODO: Add additional helper functions for moving an element around.
		 * - Set Heading/Direction
		 * - Set Position
		 * etc.
		 */

		#endregion
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

	}

	/*
	 * AutoPilot implementation that walks for a certain amount of time.
	 */
	public class WalkForTimeAutoPilot : WalkAutoPilot {

		public float WalkTime;
		private float walkedTime;

		public WalkForTimeAutoPilot(Vector3 Direction, float WalkTime) : base(Direction) {
			this.WalkTime = WalkTime;
		}

		public bool HasTimeLeft {
			get {
				return WalkTime > walkedTime;
			}
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
	
	/*
	 * AutoPilot implementation that walks towards a position until it reaches that position.
	 */
	public class WalkToPositionAutoPilot : WalkAutoPilot {
		
		public Vector3 Position;

		public WalkToPositionAutoPilot(Vector3 Position, Vector3 Speed) {
			this.Position = Position;
			this.SetSpeed (Speed);
		}

		public override Vector3 Direction {
			set {}	//Disallow setting direction. Direction is set through SetSpeed.
		}
		
		protected override Vector3 GetNextMovePosition(Movement Movement) {
			Vector3 move = base.GetNextMovePosition (Movement);

			//TODO: If move is past the target position, then move straight to the target position.

			return move;
		}

		private void SetSpeed(Vector3 Speed) {
			
			/*
			 * TODO: Complete this. 
			 * 
			 * Set base.Direction to aim towards the target position, but the magnitude is based off Speed.
			 * 
			 * Will probably need the current object's position for all the math, 
			 * so add it as an input variable, or calculate the new direction at first GetNextMovePosition.
			 */

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
					this.DequeueNextPilot();
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

	#region More Auto Pilots

	/*
	 * Units wander aimlessly.
	 */
	/*
	 * TODO: COmplete
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