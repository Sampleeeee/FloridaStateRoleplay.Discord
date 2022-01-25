using System.Text.RegularExpressions;

namespace FloridaStateRoleplay.Discord.Extensions;

public static class StringX
{
    public static string Pluralize( this int count, string singular, string plural )
    {
        return count == 1 ? singular : plural;
    }
}