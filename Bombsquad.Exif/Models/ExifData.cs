using System;
using System.Runtime.Serialization;

namespace Bombsquad.Exif.Models
{
    [DataContract(Name = "exifData")]
	public class ExifData
	{
        [DataMember(Name = "dateTaken", IsRequired = false, EmitDefaultValue = false)]
		public DateTime? DateTaken { get; set; }

        [DataMember(Name = "cameraManufacturer", IsRequired = false, EmitDefaultValue = false)]
		public string CameraManufacturer { get; set; }

        [DataMember(Name = "cameraModel", IsRequired = false, EmitDefaultValue = false)]
		public string CameraModel { get; set; }

        [DataMember(Name = "creationSoftware", IsRequired = false, EmitDefaultValue = false)]
		public string CreationSoftware { get; set; }

        [DataMember(Name = "imageWidth")]
		public int ImageWidth { get; set; }

        [DataMember(Name = "imageHeight")]
		public int ImageHeight { get; set; }

        [DataMember(Name = "horizontalResolution", IsRequired = false, EmitDefaultValue = false)]
		public double? HorizontalResolution { get; set; }

        [DataMember(Name = "verticalResolution", IsRequired = false, EmitDefaultValue = false)]
		public double? VerticalResolution { get; set; }

        [DataMember(Name = "imageOrientation")]
		public ExifOrientation ImageOrientation { get; set; }

        [DataMember(Name = "colorRepresentation")]
		public ExifColorRepresentation ColorRepresentation { get; set; }

        [DataMember(Name = "isoSpeed", IsRequired = false, EmitDefaultValue = false)]
		public uint? ISOSpeed { get; set; }

        [DataMember(Name = "fNumber", IsRequired = false, EmitDefaultValue = false)]
		public double? FNumber { get; set; }

        [DataMember(Name = "exposureTime", IsRequired = false, EmitDefaultValue = false)]
		public double? ExposureTime { get; set; }

        [DataMember(Name = "exposureCompensation", IsRequired = false, EmitDefaultValue = false)]
		public double? ExposureCompensation { get; set; }

        [DataMember(Name = "lensAperture", IsRequired = false, EmitDefaultValue = false)]
		public double? LensAperture { get; set; }

        [DataMember(Name = "focalLength", IsRequired = false, EmitDefaultValue = false)]
		public double? FocalLength { get; set; }

        [DataMember(Name = "flashMode")]
		public ExifFlashMode FlashMode { get; set; }

        [DataMember(Name = "exposureMode")]
		public ExifExposureMode ExposureMode { get; set; }

        [DataMember(Name = "whiteBalanceMode")]
		public ExifWhiteBalanceMode WhiteBalanceMode { get; set; }

        [DataMember(Name = "gpsData")]
		public ExifGpsData GpsData { get; set; }
	}
}