// © Xavalon. All rights reserved.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using Xavalon.XamlStyler.Core.Options;

namespace Xavalon.XamlStyler.Mac.Converters
{
    public class GlobalOptionsJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(IStylerOptions).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jStylerOptions = JObject.Load(reader);
            var stylerOptions = (IStylerOptions)jStylerOptions.ToObject(objectType);

            // TODO Discuss about these properties, why they're with JsonIgnore and how to handle it correct
            stylerOptions.SearchToDriveRoot = (bool?)jStylerOptions[nameof(IStylerOptions.SearchToDriveRoot)] ?? false;
            stylerOptions.ConfigPath = (string)jStylerOptions[nameof(IStylerOptions.ConfigPath)];
            stylerOptions.IndentWithTabs = (bool?)jStylerOptions[nameof(IStylerOptions.IndentWithTabs)] ?? false;

            return stylerOptions;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var stylerOptions = (IStylerOptions)value;
            var jStylerOptions = JToken.FromObject(stylerOptions);

            // TODO Discuss about these properties, why they're with JsonIgnore and how to handle it correct
            jStylerOptions[nameof(IStylerOptions.SearchToDriveRoot)] = stylerOptions.SearchToDriveRoot;
            jStylerOptions[nameof(IStylerOptions.ConfigPath)] = stylerOptions.ConfigPath;
            jStylerOptions[nameof(IStylerOptions.IndentWithTabs)] = stylerOptions.IndentWithTabs;

            serializer.Serialize(writer, jStylerOptions);
        }
    }
}