using System;
using System.IO;
using System.Reflection;

namespace MyMagicCollection.Shared
{
	public static class PathHelper
	{
		static PathHelper()
		{
			ExeFolder = new FileInfo(Assembly.GetEntryAssembly().Location).Directory.FullName;
			UserDataFolder = Path.Combine(ExeFolder, "App_Data");
			ImageCacheFolder = Path.Combine(UserDataFolder, "ImageCache");
		}

		public static string MakeAbsolutePath(this FileSystemInfo folder, string fileName)
		{
			Uri uri;
			if (!Uri.TryCreate(fileName, UriKind.RelativeOrAbsolute, out uri))
			{
				return fileName;
			}

			return uri.IsAbsoluteUri ? fileName : Path.Combine(folder.FullName, fileName);
		}

		public static string GetRelativePathFrom(this FileSystemInfo to, FileSystemInfo from)
		{
			return from.GetRelativePathTo(to);
		}

		public static string GetRelativePathTo(this FileSystemInfo from, FileSystemInfo to)
		{
			Func<FileSystemInfo, string> getPath = fsi =>
			{
				var d = fsi as DirectoryInfo;
				return d == null ? fsi.FullName : d.FullName.TrimEnd('\\') + "\\";
			};

			var fromPath = getPath(from);
			var toPath = getPath(to);

			var fromUri = new Uri(fromPath);
			var toUri = new Uri(toPath);

			var relativeUri = fromUri.MakeRelativeUri(toUri);
			var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

			return relativePath.Replace('/', Path.DirectorySeparatorChar);
		}

		public static string ExeFolder { get; private set; }

		public static string UserDataFolder { get; private set; }

		public static string ImageCacheFolder { get; private set; }
	}
}