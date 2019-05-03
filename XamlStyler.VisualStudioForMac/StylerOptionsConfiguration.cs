using System;
using System.IO;
using MonoDevelop.Core;
using Newtonsoft.Json;
using Xavalon.XamlStyler.Core.Options;

namespace Xavalon.XamlStyler.VisualStudioForMac
{
	public static class StylerOptionsConfiguration
	{
		public static StylerOptions ReadFromUserProfile()
		{
			var filePath = GetOptionsFilePath().ToString();
            try
            {
                var optionsJsonString = File.ReadAllText(filePath);
				var stylerOptions = JsonConvert.DeserializeObject<StylerOptions>(optionsJsonString);
			}
			catch (FileNotFoundException)
			{
			}
			catch (Exception ex)
			{
				LoggingService.LogError("Exception when saving user XamlStyler options", ex);
				File.Delete(filePath);
			}

			// Xamarin Forms defaults
			var options = new StylerOptions()
			{
				IndentSize = 4,
                KeepFirstAttributeOnSameLine = true
            };

			try
			{
				// update attribute ordering to include Forms attrs
				options.AttributeOrderingRuleGroups[6] += ", WidthRequest, HeightRequest";
				options.AttributeOrderingRuleGroups[7] += ", HorizontalOptions, VerticalOptions, XAlign, VAlign";
			}
			catch (Exception ex)
			{
				LoggingService.LogError("Exception when updating default options to include Xamarin Forms attributes", ex);
			}

			return options;
		}

		public static void WriteToUserProfile(StylerOptions options)
		{
			try
			{
				var text = JsonConvert.SerializeObject(options);
				File.WriteAllText(GetOptionsFilePath().ToString(), text);
			}
			catch (Exception ex)
			{
				LoggingService.LogError("Exception when saving user XamlStyler options", ex);
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

