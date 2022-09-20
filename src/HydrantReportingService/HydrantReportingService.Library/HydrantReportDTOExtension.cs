using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydrantReportingService.Library
{
    public static class HydrantReportDTOExtension
    {
        public static FeatureCollection ConvertToGeoJsonFeature(this HydrantReportDTO report)
        {

            
                var coordinates = new Position(report.Latitude, report.Longitude);
                var geom = new Point(coordinates);
                var properties = new Dictionary<string, object>
                {
                    {"type", report.Type },
                    {"address", report.Address },
                    {"approved", report.Approved },
                    {"id", report.Id }
                };
                var feature = new Feature(geom, properties);
                var features = new List<Feature>() { feature};
                return new FeatureCollection(features);
        }
    }
}
