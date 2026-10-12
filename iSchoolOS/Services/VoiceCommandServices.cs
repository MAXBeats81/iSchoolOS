namespace iSchoolOS.Services;

public class VoiceCommandService
{
    public string ProcessCommand(string command)
    {
        command = command.ToLower().Trim();
        if (command.Contains("campus") && command.Contains("status"))
        {
            return "STATUS";
        }
        if (command.Contains("parking"))
        {
            return "PARKING";
        }
        if (command.Contains("emergency"))
        {
            return "EMERGENCY";
        }
        return "UNKNOWN";
    }
}