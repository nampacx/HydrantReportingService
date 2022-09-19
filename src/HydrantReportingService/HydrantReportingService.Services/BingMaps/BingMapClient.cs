using BingMapsRESTToolkit;

using Microsoft.Extensions.Configuration;

namespace HydrantReportingService.Services.BingMaps

{
    public class BingMapClient
    {
        private readonly BingMapConfig _config;

        public BingMapClient(IConfiguration configuration)
        {
            _config = configuration.GetSection("BingMaps").Get<BingMapConfig>();
        }

        public async Task<Location> GetAddress(double latitude, double longitude)
        {
            Location? result = null;
            var coordinate = new Coordinate(latitude, longitude);
            var request = new ReverseGeocodeRequest()
            {
                BingMapsKey = _config.ApiKey,
                Point = coordinate
            };
            var response = await request.Execute();
            if (response != null &&
                response.ResourceSets != null &&
                response.ResourceSets.Length > 0 &&
                response.ResourceSets[0].Resources != null &&
                response.ResourceSets[0].Resources.Length > 0)
            {
                result = response.ResourceSets[0].Resources[0] as Location;
                return result;
            }
            return result;
        }
    }
}