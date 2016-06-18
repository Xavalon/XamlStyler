using System;
using System.IO;
using MonoDevelop.Core;
using Newtonsoft.Json;
using Xavalon.XamlStyler.Core.Options;

namespace Xavalon.XamlStyler.XamarinStudio
{
	public static class StylerOptionsConfiguration
	{
		public static StylerOptions ReadFromUserProfile()
		{
			var filePath = GetOptionsFilePath().ToString();

			try
			{
				var text = File.ReadAllText(filePath);
				return JsonConvert.DeserializeObject<StylerOptions>(text);
			}
			catch (FileNotFoundException)
			{
			}
			catch (Exception)
			{
				// delete file on any other exception (malformed etc.)
				File.Delete(filePath);
			}

			return new StylerOptions()
			{
				IndentSize = 4
			};
		}

		public static void WriteToUserProfile(StylerOptions options)
		{
			try
			{
				var text = JsonConvert.SerializeObject(options);
				File.WriteAllText(GetOptionsFilePath().ToString(), text);
			}
			catch (Exception)
			{
			}
		}

		public static void Reset()
		{
			File.Delete(GetOptionsFilePath().ToString());
		}

		private static FilePath GetOptionsFilePath()
		{
			return UserProfile.Current.ConfigDir.Combine("xamlstyler.config");
		}
	}
}

