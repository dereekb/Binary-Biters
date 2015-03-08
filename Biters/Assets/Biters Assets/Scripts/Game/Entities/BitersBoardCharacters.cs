using System;
using UnityEngine;
using Biters.Utility;

namespace Biters.Game
{
	public class Unii : BitersGameEntity {
		
		public static readonly string UniiId = "Entity.Unii";
		public static readonly Vector3 UniiScale = new Vector3 (2.5f, 2.5f, 2.5f);
		public static readonly Material UniiMat = ResourceLoader.Load["Entity_Unii"].Material;

		public Unii () : base () {}
		
		#region Initialize
		
		public override void Initialize ()
		{
			this.gameObject.GetComponent<Renderer>().material = UniiMat;
			this.gameObject.transform.localScale = UniiScale;
			base.Initialize ();
		}
		
		#endregion

		#region Entity
		
		public override string EntityId {
			get {
				return UniiId;
			}
		}

		#endregion
		
		public override string ToString ()
		{
			return string.Format ("[Unii: Position={0}]", this.Position);
		}

	}

	public class Nili : BitersGameEntity {

		public static readonly string NiliId = "Entity.Nili";
		public static readonly Vector3 NiliScale = new Vector3 (2.5f, 2.5f, 2.5f);
		public static readonly Material NiliMat = ResourceLoader.Load["Entity_Nili"].Material;
		
		public Nili () : base () {}

		#region Initialize
		
		public override void Initialize ()
		{
			this.gameObject.GetComponent<Renderer>().material = NiliMat;
			this.gameObject.transform.localScale = NiliScale;
			base.Initialize ();
		}

		#endregion

		#region Entity

		public override string EntityId {
			get {
				return NiliId;
			}
		}

		#endregion

		public override string ToString ()
		{
			return string.Format ("[Nili: Position={0}]", this.Position);
		}

	}

}

