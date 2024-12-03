using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PTV.Services.StreetAPI.Extensions
{
    public class LineStringJsonConverter : JsonConverter<LineString>
    {
        private readonly GeometryFactory _geometryFactory;

        public LineStringJsonConverter()
        {
            _geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        }

        public override LineString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var json = JsonDocument.ParseValue(ref reader);
            var coordinates = json.RootElement.GetProperty("coordinates")
                               .EnumerateArray()
                               .Select(coord => new Coordinate(
                                   coord[0].GetDouble(),
                                   coord[1].GetDouble()))
                               .ToArray();
            return _geometryFactory.CreateLineString(coordinates);
        }

        public override void Write(Utf8JsonWriter writer, LineString value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("type", "LineString");
            writer.WritePropertyName("coordinates");
            writer.WriteStartArray();
            foreach (var coord in value.Coordinates)
            {
                writer.WriteStartArray();
                writer.WriteNumberValue(coord.X);
                writer.WriteNumberValue(coord.Y);
                writer.WriteEndArray();
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }
    }
}
