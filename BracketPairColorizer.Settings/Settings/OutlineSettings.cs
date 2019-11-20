using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BracketPairColorizer.Settings.Settings
{
    public class OutlineSettings : ISettingsObject
    {
        public string Name => "outlines";
        public List<Tuple<int, int>> Regions { get; private set; } = new List<Tuple<int, int>>();

        public void Read(JsonTextReader reader)
        {
            Regions.Clear();
            if (!reader.ReadStartObject()) return;
            if (reader.ReadPropertyName() != "regions") return;
            if (!reader.ReadStartArray()) return;
        }

        public void Save(JsonTextWriter writer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("regions");
            writer.WriteStartArray();
            foreach (var region in Regions)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("start");
                writer.WriteValue(region.Item1);
                writer.WritePropertyName("length");
                writer.WriteValue(region.Item2);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
            writer.WriteEndObject();
        }
    }
}
