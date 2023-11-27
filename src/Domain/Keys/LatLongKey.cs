namespace Domain.Keys
{
    public static class LatLongKey
    {
        public static string Key(string latitude, string longitude) => $"Latitude#{latitude}_Longitude#{longitude}";
    }
}
