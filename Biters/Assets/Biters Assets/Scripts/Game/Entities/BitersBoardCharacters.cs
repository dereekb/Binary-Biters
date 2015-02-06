using System;
using UnityEngine;
using Biters.Utility;

namespace Biters.Game
{
	public class Unii : BitersMapEntity {
		
		public static readonly string UNII_ID = "Unii";
		public static readonly Material UNII_MAT = ResourceLoader.Load["Unii"].Material;

		public Unii () : base () {
			this.gameObject.renderer.material = UNII_MAT;
		}
		
		#region Entity
		
		public override string BitersId {
			get {
				return UNII_ID;
			}
		}

		#endregion

	}

	public class Nili : BitersMapEntity {

		public static readonly string NILI_ID = "Nili";
		public static readonly Material NILI_MAT = ResourceLoader.Load["Nili"].Material;
		
		public Nili () : base () {
			this.gameObject.renderer.material = NILI_MAT;
		}
		
		#region Entity
		
		public override string BitersId {
			get {
				return NILI_ID;
			}
		}

		#endregion

	}

}

