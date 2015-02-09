using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Biters
{
	#region Position Queriable

	/*
	 * Object that contains elements within a space.
	 */
	public interface IEntityPositionQueriable<T>
		where T : class, IPositionalElement {

		ICollection<T> QueriableEntities { get; }

	}
	
	public interface IEntityPositionQuery<T>
		where T : class, IPositionalElement {

		//Queriable Element
		IEntityPositionQueriable<T> Queriable { get; set; }

		//The Center/Target Position
		Vector3 TargetPosition { get; set; }

		//Result Limit
		int? Limit { get; set; }

		//Min Distance from the Target Position
		float? MinDistance { get; set; }

		//Max Distance from the Target Position
		float? MaxDistance { get; set; }

		//Results to Exclude
		HashSet<T> Exclude { get; set; }

		/*
		 * Returns true if the entity matches the given query conditions.
		 */
		bool MatchesQuery(T Entity);

		/*
		 * Returns true if all entities match the given query conditions.
		 */
		bool MatchesQuery(IEnumerable<T> Entities);
		
		/*
		 * Returns all entities that match the given query.
		 */
		List<T> EntitiesThatMatchQuery(ICollection<T> Entities);
		
		/*
		 * Returns the first result that matches the query.
		 */
		T SearchFirstEntity();

		/*
		 * Returns the entity that is closest and matches the query.
		 */
		T SearchClosestEntity();
		
		/*
		 * Returns all entities that match the query.
		 */
		List<T> SearchEntities();

		/*
		 * Returns a random entity that matches the query.
		 */
		T RandomEntity();

		/*
		 * Returns a random entity. Searches up to the limit before picking a random element.
		 */ 
		T RandomEntity(int Limit);

		/*
		 * Returns random entities.
		 */
		List<T> RandomEntities(int Limit);

	}

	/*
	 * Default implementation.
	 */
	public class EntityPositionQuery<T> : IEntityPositionQuery<T>
		where T : class, IPositionalElement
	{
		
		//Queriable Element
		public IEntityPositionQueriable<T> Queriable { get; set; }
		
		//The Center/Target Position
		public Vector3 TargetPosition { get; set; }
		
		//Result Limit
		public int? Limit { get; set; }

		//Min Distance from the Target Position
		public float? MinDistance { get; set; }
		
		//Max Distance from the Target Position
		public float? MaxDistance { get; set; }

		//Results to Exclude
		protected HashSet<T> exclude = new HashSet<T> ();
		
		public virtual HashSet<T> Exclude {
			get {
				return this.exclude;
			} 

			set {
				if (value != null) {
					this.exclude = value;
				} else {
					this.exclude.Clear();
				}
			}
		}

		public EntityPositionQuery () {}
		
		public EntityPositionQuery(IEntityPositionQueriable<T> Queriable) {
			this.Queriable = Queriable;
		}

		public virtual float DistanceToTarget(T Entity) {
			float distance = (TargetPosition - Entity.Position).magnitude;
			return distance;
		}

		public virtual bool IsWithinRange(T Entity) {
			bool withinRange = true;
			float distance = this.DistanceToTarget (Entity);

			if (this.MinDistance.HasValue) {
				withinRange = distance <= this.MinDistance.Value;
			}

			if (this.MaxDistance.HasValue && withinRange) {
				withinRange = distance >= this.MaxDistance.Value;
			}

			return withinRange;
		}

		public virtual bool MatchesQuery(T Entity) {
			bool isMatch = false;

			if (exclude.Contains (Entity) == false) {
				isMatch = this.IsWithinRange(Entity);
			}

			return isMatch;
		}

		public virtual bool MatchesQuery(IEnumerable<T> Entities) {
			bool isMatch = true;
			
			foreach (T Entity in Entities) {
				isMatch = MatchesQuery(Entity);

				if (!isMatch) {
					break;
				}
			}

			return isMatch;
		}

		public virtual List<T> EntitiesThatMatchQuery(ICollection<T> Entities) {
			return this.EntitiesThatMatchQuery (Entities, (this.Limit.HasValue) ? this.Limit.Value : Entities.Count);
		}
		
		public virtual List<T> EntitiesThatMatchQuery(ICollection<T> Entities, int Limit) {
				List<T> entities = new List<T>();

				foreach (T Entity in Entities) {
					if (this.MatchesQuery(Entity)) {
						entities.Add (Entity);

						if (entities.Count >= Limit) {
							break;
						}
					}
				}

				return entities;
		}

		public virtual T SearchFirstEntity() {
			List<T> matches = this.EntitiesThatMatchQuery (this.Queriable.QueriableEntities, 1);
			T entity = null;

			if (matches.Count > 0) {
				entity = matches[0];
			}

			return entity;
		}

		public virtual T SearchClosestEntity() {
			List<T> matches = this.SearchEntities ();
			T entity = null;
			
			if (matches.Count > 0) {
				float best = float.MaxValue;

				foreach (T match in matches) {
					float distance = this.DistanceToTarget (match);

					if (distance < best) {
						entity = match;
						best = distance;
					}
				}
			}
			
			return entity;
		}

		public virtual List<T> SearchEntities() {
			return this.EntitiesThatMatchQuery (this.Queriable.QueriableEntities);
		}
		
		public virtual T RandomEntity() {
			return this.RandomEntity (10);
		}

		public virtual T RandomEntity(int Limit) {
			T result = null;

			List<T> results = this.RandomEntities (Limit, 1);

			if (results.Count > 0) {
				result = results[0];
			}

			return result;
		}
		
		public virtual List<T> RandomEntities(int Limit) {
			return this.RandomEntities (Math.Max(Limit * 2, 10), Limit);
		}
		
		public virtual List<T> RandomEntities(int RandomSearchLimit, int Limit) {
			return this.RandomEntities (this.Queriable.QueriableEntities, RandomSearchLimit, Limit);
		}
		
		protected virtual List<T> RandomEntities(ICollection<T> Entities, int RandomSearchLimit, int Limit) {
			List<T> results = new List<T> ();
			List<T> searchResults = this.EntitiesThatMatchQuery (Entities, RandomSearchLimit);
			System.Random random = new System.Random ();
			
			if (searchResults.Count <= Limit) {
				results = searchResults;
			} else {
				for (int i = 0; i < Limit; i += 1) {
					int index = random.Next (0, searchResults.Count);
					results.Add(searchResults[index]);
					searchResults.RemoveAt(index);
				}
			}
			
			return results;
		}

	}

	#endregion

	#region World Position Queryable
	
	public interface IEntityWorldPositionQueriable<T> : IEntityPositionQueriable<T>
	where T : class, IPositionalElement {

		bool EntityIsAtWorldPosition(T Entity, WorldPosition Position);

		List<T> EntitiesAtPosition (WorldPosition Position);
		
	}
	
	public interface IEntityWorldPositionQuery<T> : IEntityPositionQuery<T>
	where T : class, IPositionalElement {
		
		new IEntityWorldPositionQueriable<T> Queriable { get; set; }

		WorldPosition? TargetWorldPosition { get; set; }
		
	}
	
	public class EntityWorldPositionQuery<T> : EntityPositionQuery<T>, IEntityWorldPositionQuery<T>
		where T : class, IPositionalElement
	{

		private IEntityWorldPositionQueriable<T> WorldQueriable;

		public new IEntityWorldPositionQueriable<T> Queriable {

			get {
				return this.WorldQueriable;
			}

			set {
				this.WorldQueriable = value;
				base.Queriable = value;
			}

		}

		public EntityWorldPositionQuery(IEntityWorldPositionQueriable<T> Queriable) : base(Queriable) {
			this.WorldQueriable = Queriable;
		}

		public WorldPosition? TargetWorldPosition { get; set; }

		public override bool MatchesQuery(T Entity) {
			bool isMatch = true;

			if (this.TargetWorldPosition.HasValue) {
				isMatch = this.WorldQueriable.EntityIsAtWorldPosition (Entity, this.TargetWorldPosition.Value);
			}

			if (isMatch) {
				isMatch = base.MatchesQuery(Entity);
			}

			return isMatch;
		}
		
		public override List<T> SearchEntities() {
			List<T> results;
			
			if (this.TargetWorldPosition.HasValue) {
				List<T> localEntities = this.WorldQueriable.EntitiesAtPosition(this.TargetWorldPosition.Value);
				results = this.EntitiesThatMatchQuery(localEntities);
			} else {
				results = base.SearchEntities();
			}

			return results;
		}
		
		public override List<T> RandomEntities(int RandomSearchLimit, int Limit) {
			List<T> results;
			
			if (this.TargetWorldPosition.HasValue) {
				List<T> localEntities = this.WorldQueriable.EntitiesAtPosition(this.TargetWorldPosition.Value);
				results = base.RandomEntities(localEntities, RandomSearchLimit, Limit);
			} else {
				results = base.RandomEntities(RandomSearchLimit, Limit);
			}
			
			return results;
		}

	}

	#endregion

}

