namespace mxGraph.online
{

	public class Constants
	{

		/// <summary>
		/// Maximum size (in bytes) for request payloads. Default is 52428800 (50MB).
		/// </summary>
		public const int MAX_REQUEST_SIZE = 52428800;

		/// <summary>
		/// Maximum are for exports. Default assumes the area taken by a 
		/// 10000px by 10000px image.
		/// </summary>
		public const int MAX_AREA = 10000 * 10000;

		/// <summary>
		/// The domain where legacy images are stored.
		/// </summary>
		public const string IMAGE_DOMAIN = "http://img.diagramly.com/";

	}

}