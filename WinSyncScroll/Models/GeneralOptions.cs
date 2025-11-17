using System.ComponentModel.DataAnnotations;
using WinSyncScroll.Enums;

namespace WinSyncScroll.Models;

public class GeneralOptions
{
    [Required]
    public required bool IsStrictProcessIdCheckEnabled { get; set; }

    [Required]
    public required bool IsLegacyModeEnabled { get; set; }

    [Required]
    public required LegacyModeBehaviour LegacyModeBehaviour { get; set; }
}
