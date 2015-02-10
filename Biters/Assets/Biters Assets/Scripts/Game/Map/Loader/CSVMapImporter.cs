using System;
using Biters;

namespace Biters.Game
{
	/*
	 * Default map importer that imports a CSV map.
	 */
	public class CSVBitersMapImporter : IMapTileImporter<BitersGameTile>
	{
		public CSVBitersMapImporter () {}

		public IImportedMap<BitersGameTile> Import(string map) {
			CSVBitersImportedMap import = null;

			//TODO: Load the map from a file. Return null if the file does not exist.

			return import;
		}

	}

	public class CSVBitersImportedMap : IImportedMap<BitersGameTile> {

		public World<BitersGameTile> Make() {
			return null; //TODO: Reload the map each time.
		}

	}

}

