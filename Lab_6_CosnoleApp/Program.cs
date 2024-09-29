using Newtonsoft.Json.Linq;

public class Weather
{
    public string Country { get; set; }
    public string Name { get; set; }
    public double Temp { get; set; }
    public string Description { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        string apiKey = "dda56c3e8395d0ab518f9c2a25ee3f14";
        var weathers = new List<Weather>();

        using (HttpClient client = new HttpClient())
        {
            Random rand = new Random();

            while (weathers.Count < 50)
            {
                double latitude = rand.NextDouble() * 180 - 90; // Широта: от -90 до 90
                double longitude = rand.NextDouble() * 360 - 180; // Долгота: от -180 до 180
                string requestUrl = $"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={apiKey}&units=metric";

                try
                {
                    HttpResponseMessage response = client.GetAsync(requestUrl).Result; // Синхронный запрос
                    response.EnsureSuccessStatusCode();
                    string responseBody = response.Content.ReadAsStringAsync().Result;

                    JObject json = JObject.Parse(responseBody);
                    string country = (string)json["sys"]?["country"];
                    string name = (string)json["name"];
                    double temp = (double)json["main"]["temp"];
                    string description = (string)json["weather"][0]["description"];

                    if (!string.IsNullOrEmpty(country) && !string.IsNullOrEmpty(name))
                    {
                        weathers.Add(new Weather
                        {
                            Country = country,
                            Name = name,
                            Temp = temp,
                            Description = description
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        // 1. Страна с максимальной и минимальной температурой
        var maxTempWeather = weathers.OrderByDescending(w => w.Temp).FirstOrDefault();
        var minTempWeather = weathers.OrderBy(w => w.Temp).FirstOrDefault();
        Console.WriteLine($"Максимальная температура: {maxTempWeather.Country}, {maxTempWeather.Temp}°C");
        Console.WriteLine($"Минимальная температура: {minTempWeather.Country}, {minTempWeather.Temp}°C");

        // 2. Средняя температура в мире
        var averageTemp = weathers.Average(w => w.Temp);
        Console.WriteLine($"Средняя температура в мире: {averageTemp:F2}°C");

        // 3. Количество стран в коллекции
        var countryCount = weathers.Select(w => w.Country).Distinct().Count();
        Console.WriteLine($"Количество стран в коллекции: {countryCount}");

        // 4. Первая найденная страна и название местности, в которых Description принимает значение "clear sky", "rain", "few clouds"
        var descriptions = new[] { "clear sky", "rain", "few clouds" };
        var weatherWithDescription = weathers.FirstOrDefault(w => descriptions.Contains(w.Description));
        if (weatherWithDescription != null)
        {
            Console.WriteLine($"Первая страна с подходящим описанием погоды: {weatherWithDescription.Country}, {weatherWithDescription.Name} ({weatherWithDescription.Description})");
        }
        else
        {
            Console.WriteLine("Нет подходящего описания погоды.");
        }
    }
}



