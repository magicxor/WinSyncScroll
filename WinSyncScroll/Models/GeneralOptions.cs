using System.ComponentModel.DataAnnotations;

namespace WinSyncScroll.Models;

public class GeneralOptions
{
    [Required]
    public required bool IsStrictProcessIdCheckEnabled { get; set; }

    [Required]
    public required bool IsLegacyModeEnabled { get; set; }
}
