namespace iSchoolOS.Models;

public class CampusData
{
    public int StudentsPresent { get; set; }
    public int RoomOccupancy { get; set; }
    public int AvailableParking { get; set; }
    public double Temperature { get; set; }
    public DateTime Timestamp { get; set; }
}