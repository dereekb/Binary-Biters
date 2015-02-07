using System;
using UnityEngine;
using Biters.Utility;

namespace Biters.Game
{
	public class Unii : BitersMapEntity {
		
		public static readonly string UniiId = "Unii";
		public static readonly Vector3 UniiScale = new Vector3 (0.1f, 0.1f, 0.1f);
		public static readonly Material UniiMat = ResourceLoader.Load["Entity_Unii"].Material;

		public Unii () : base () {
			this.gameObject.renderer.material = UniiMat;
			this.GameObject.transform.localScale = UniiScale;
		}
		
		#region Entity
		
		public override string BitersId {
			get {
				return UniiId;
			}
		}

		#endregion

	}

	public class Nili : BitersMapEntity {

		public static readonly string NiliId = "Entity_Nili";
		public static readonly Vector3 NiliScale = new Vector3 (0.1f, 0.1f, 0.1f);
		public static readonly Material NiliMat = ResourceLoader.Load["Entity_Nili"].Material;
		
		public Nili () : base () {}
		
		#region Entity

		public override void Initialize ()
		{
			this.gameObject.renderer.material = NiliMat;
			this.GameObject.transform.localScale = NiliScale;
			base.Initialize ();
		}

		public override string BitersId {
			get {
				return NiliId;
			}
		}

		#endregion

	}

}

