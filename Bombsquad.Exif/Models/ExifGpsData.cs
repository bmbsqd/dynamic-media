using System.Runtime.Serialization;

namespace Bombsquad.Exif.Models
{
    [DataContract]
	public class ExifGpsData
	{
        [DataMember(Name = "gpsVersion", IsRequired = false, EmitDefaultValue = false)]
		public decimal? GpsVersion { get; set; }

        [DataMember(Name = "processingMethod", IsRequired = false, EmitDefaultValue = false)]
		public string ProcessingMethod { get; set; }

        [DataMember(Name = "measureMode")]
		public ExifGpsMeasureMode MeasureMode { get; set; }

        [DataMember(Name = "latitude", IsRequired = false, EmitDefaultValue = false)]
		public double? Latitude { get; set; }

        [DataMember(Name = "longitude", IsRequired = false, EmitDefaultValue = false)]
		public double? Longitude { get; set; }

        [DataMember(Name = "altitude", IsRequired = false, EmitDefaultValue = false)]
		public double? Altitude { get; set; }
	}
}