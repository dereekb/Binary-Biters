using System;
using UnityEngine;

namespace Biters.Utility
{
	/*
	 * Basic timer. Counts up until it reaches or passes a certain duration value.
	 */
	public class Timer
	{
		//Length of time to reach.
		public float Length = 1.0f;
		
		//Current amount of time elapsed since the last spawn.
		private float count = 0.0f;

		public Timer () {}
		
		public Timer (float Length) {
			this.Length = Length;
		}

		public float Remaining {
			get {
				return Length - count;
			}
		}

		public float Elapsed {
			get {
				return this.count;
			}
		}

		public virtual bool Done {
			get {
				return this.Length < this.count;
			}
		}

		public virtual bool HasTimeRemaining {
			get {
				return !this.Done;
			}
		}
		
		public virtual void Update() {
			this.Increase (Time.deltaTime);
		}
		
		public virtual bool UpdateAndCheck() {
			this.Increase (Time.deltaTime);
			return this.Done;
		}

		public virtual void Increase(float time) {
			this.count += time;
		}

		public virtual bool IncreaseAndCheck(float time) {
			this.Increase(time);
			return this.Done;
		}

		public virtual void Reset() {
			this.count = 0.0f;
		}

	}

}

