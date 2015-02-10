using System;
using UnityEngine;

namespace Biters.Utility
{

	/*
	 * Supporting class for loading files.
	 */
	public sealed class ResourceLoader
	{
		public static ResourceLoader Load {
			get {
				return new ResourceLoader ();
			}
		}

		public ResourceLoader () {}
		
		public ResourceLoaderItem this[string Filename]
		{
			get {
				return new ResourceLoaderItem(Filename);
			}
		}

	}

	public struct ResourceLoaderItem
	{
		private string filename;

		public ResourceLoaderItem(string Filename) {
			this.filename = Filename;
		}

		//Loads the element as a material.
		public Material Material {
			get {
				return this.Load(typeof(Material)) as Material;
			}
		}

		public object Resource {
			get {
				return Resources.Load(this.filename);
			}
		}
		
		public object Load(Type filter) {
			return Resources.Load(this.filename, filter);
		}

	}

}

