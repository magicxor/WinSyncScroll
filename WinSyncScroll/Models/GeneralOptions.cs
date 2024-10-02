using System.ComponentModel.DataAnnotations;

namespace WinSyncScroll.Models;

public class GeneralOptions
{
    [Required]
    public required bool IsStrictProcessIdCheckEnabled { get; set; }

    [Required]
    [Range(0, 500)]
    public required int InputDelay { get; set; }
}
