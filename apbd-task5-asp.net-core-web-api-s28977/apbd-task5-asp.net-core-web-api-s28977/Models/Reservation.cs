using System.ComponentModel.DataAnnotations;

namespace apbd_task5_asp.net_core_web_api_s28977.Models;

public class Reservation
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    [Required]
    [MinLength(1)]
    public string OrganizerName { get; set; } = string.Empty;
    [Required]
    [MinLength(1)]
    public string Topic { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string Status { get; set; } = string.Empty;
}