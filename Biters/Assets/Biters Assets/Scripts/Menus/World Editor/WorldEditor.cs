using UnityEngine;
using System.Collections;

namespace Biters.Menus.WorldEditor {

	public class BitersWorldEditor : MonoBehaviour {

		public static readonly string MAP_EDITOR_SCENE_NAME = "WorldEditorScene";
		
		public static void LoadLevel() {
			Application.LoadLevel (MAP_EDITOR_SCENE_NAME);
		}

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

	}

}