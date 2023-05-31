using SafeZone.Models;

namespace SafeZone.Services
{
    public static class CoordinateCalculation
    {
        public static Coordinate GetCoordinate(string coordinate)
        {
            var position=coordinate.Split(',');
            return new()
            {
                Longitude = double.Parse(position[0]),
                Latitude = double.Parse(position[1])
            };
        }
        public static double CalculateDistance(Coordinate coordinate1, Coordinate coordinate2)
        {
            double earthRadius = 6371; // Radius of the Earth in kilometers

            double lat1Rad = DegreesToRadians(coordinate1.Latitude);
            double lon1Rad = DegreesToRadians(coordinate1.Longitude);
            double lat2Rad = DegreesToRadians(coordinate2.Latitude);
            double lon2Rad = DegreesToRadians(coordinate2.Longitude);

            double deltaLat = lat2Rad - lat1Rad;
            double deltaLon = lon2Rad - lon1Rad;

            double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                       Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                       Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            double distance = earthRadius * c;

            return distance;
        }

        public static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
