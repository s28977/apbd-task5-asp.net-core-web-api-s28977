using System.ComponentModel.DataAnnotations;

namespace apbd_task5_asp.net_core_web_api_s28977.Models;

public class Room
{
    public int Id { get; set; }
    [Required]
    [MinLength(1)]
    public string Name { get; set; } = string.Empty;
    [Required]
    [MinLength(1)]
    public string BuildingCode { get; set; } = string.Empty;
    public int Floor { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0")]
    public int Capacity { get; set; }
    public bool HasProjector { get; set; }
    public bool IsActive { get; set; }
}