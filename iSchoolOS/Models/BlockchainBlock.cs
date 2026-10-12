namespace iSchoolOS.Models;

public class BlockchainBlock
{
    public int Index { get; set; }
    public DateTime Timestamp { get; set; }
    public string Transaction { get; set; } = "";
    public string PreviousHash { get; set; } = "";
    public string Hash { get; set; } = "";
}