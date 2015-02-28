using System.IO;
using System.Linq;
using System.Reflection;

namespace MyMagicCollection.Shared.Helper
{
	public static class EmbeddedResourceHelper
	{
		public static string FindEmbeddedResource(this Assembly assembly, string resourceName)
		{
			resourceName = resourceName.ToLowerInvariant();
			return assembly.GetManifestResourceNames()
				.FirstOrDefault(n => n.ToLowerInvariant().Contains(resourceName));
		}

		public static string LoadEmbeddedResourceTextFile(this Assembly assembly, string resourceName)
		{
			var fullName = assembly.FindEmbeddedResource(resourceName);
			if (string.IsNullOrWhiteSpace(fullName))
			{
				return null;
			}

			using (var stream = assembly.GetManifestResourceStream(fullName))
			{
				using (var reader = new StreamReader(stream))
				{
					return reader.ReadToEnd();
				}
			}
		}
	}
}