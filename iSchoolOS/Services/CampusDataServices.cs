using iSchoolOS.Models;

namespace iSchoolOS.Services;

public class CampusDataService
{
    private readonly Random random = new();

    public async Task<CampusData> GetCampusDataAsync()
    {
        // Simulate network latency (IoT Remote Data Retrieval)
        await Task.Delay(1000);
        return new CampusData
        {
            StudentsPresent = random.Next(50, 150),
            RoomOccupancy = random.Next(30, 100),
            AvailableParking = random.Next(5, 50),
            Temperature = Math.Round(25 + random.NextDouble() * 10, 1),
            Timestamp = DateTime.Now
        };
    }
}