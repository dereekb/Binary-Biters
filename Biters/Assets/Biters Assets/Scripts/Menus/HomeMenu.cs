using UnityEngine;
using System.Collections;
using Biters.Menus.WorldEditor;

namespace Biters.Menus {

	public class HomeMenu : MonoBehaviour {

		public static readonly string MAIN_MENU_SCENE_NAME = "HomeScene";

		public static void LoadLevel() {
			Application.LoadLevel (MAIN_MENU_SCENE_NAME);
		}

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		public void SegueToMapEditor () {
			BitersWorldEditor.LoadLevel ();
		}

	}

}