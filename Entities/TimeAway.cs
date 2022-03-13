using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using FloridaStateRoleplay.Discord.Extensions;

namespace FloridaStateRoleplay.Discord.Entities;

public class TimeAway
{
    public bool Permanenet { get; set; }
    
    public int Seconds { get; set; }
    public int Minutes { get; set; }
    public int Hours { get; set; }
    public int Days { get; set; }
    public int Weeks { get; set; }
    public int Months { get; set; }
    public int Years { get; set; }

    public TimeAway( string time )
    {
        if ( time == "-1" || time == "perm" || time == "*" )
        {
            Permanenet = true;
            return;
        }

        var regex = new Regex( @"^(?<years>\d+y\s*)?(?<months>\d+mo\s*)?(?<weeks>\d+w\s*)?(?<days>\d+d\s*)?(?<hours>\d+h\s*)?(?<minutes>\d+m\s*)?(?<seconds>\d+s\s*)?$", RegexOptions.ECMAScript );

        var match = regex.Match( time );

        var years = match.Groups["years"];
        var months = match.Groups["months"];
        var weeks = match.Groups["weeks"];
        var days = match.Groups["days"];
        var hours = match.Groups["hours"];
        var minutes = match.Groups["minutes"];
        var seconds = match.Groups["seconds"];

        if ( years.Success )
            Years = Convert.ToInt32( years.Value.Remove( years.Length - 1, 1 ) );
        
        if ( match.Groups["months"].Success )
            Months = Convert.ToInt32( months.Value.Remove( months.Length - 1, 1 ) );
        
        if ( match.Groups["weeks"].Success )
            Weeks = Convert.ToInt32( weeks.Value.Remove( weeks.Length - 1, 1 ) );
        
        if ( match.Groups["days"].Success )
            Days = Convert.ToInt32( days.Value.Remove( days.Length - 1, 1 ) );
        
        if ( match.Groups["hours"].Success )
            Hours = Convert.ToInt32( hours.Value.Remove( hours.Length - 1, 1 ) );
        
        if ( match.Groups["minutes"].Success )
            Minutes = Convert.ToInt32( minutes.Value.Remove( minutes.Length - 1, 1 ) );
        
        if ( match.Groups["seconds"].Success )
            Seconds = Convert.ToInt32( seconds.Value.Remove( seconds.Length - 1, 1 ) );
    }

    public string FormattedTime()
    {
        var str = string.Empty;

        if ( Permanenet )
            return "Permanent";

        if ( Years > 0 )
            str += $"{Months} {Years.Pluralize( "year", "years" )} ";
        if ( Months > 0 )
            str += $"{Months} {Months.Pluralize( "month", "months" )} ";
        if ( Weeks > 0 )
            str += $"{Weeks} {Weeks.Pluralize( "week", "weeks" )} ";
        if ( Days > 0 )
            str += $"{Days} {Days.Pluralize( "day", "days" )} ";
        if ( Hours > 0 )
            str += $"{Hours} {Hours.Pluralize( "hour", "hours" )} ";
        if ( Minutes > 0 )
            str += $"{Minutes} {Minutes.Pluralize( "minute", "minutes" )} ";
        if ( Seconds > 0 )
            str += $"{Seconds} {Seconds.Pluralize( "second", "seconds" )} ";

        return str.TrimEnd();
    }

    public TimeSpan ToTimeSpan()
    {
        if ( Permanenet )
            return TimeSpan.FromDays( 1000 * 365 );
        
        return TimeSpan.FromDays( Years * 365 ).Add( TimeSpan.FromDays( Months * 30 ) )
            .Add( TimeSpan.FromDays( Weeks * 7 ) ).Add( TimeSpan.FromDays( Days ) ).Add( TimeSpan.FromHours( Hours ) )
            .Add( TimeSpan.FromMinutes( Minutes ) ).Add( TimeSpan.FromSeconds( Seconds ) );
    }

    public override string ToString()
    {
        return FormattedTime();
    }

    public static explicit operator TimeSpan( TimeAway x ) =>
        x.ToTimeSpan();

    public static explicit operator DateTimeOffset( TimeAway x )
    {
        var dto = new DateTimeOffset();

        if ( x.Permanenet )
        {
            dto = dto.AddYears( 1000 );
        }
        else
        {
            dto = dto.AddYears( x.Years );
            dto = dto.AddMonths( x.Months );
            dto = dto.AddDays( x.Weeks * 7 );
            dto = dto.AddDays( x.Days );
            dto = dto.AddHours( x.Hours );
            dto = dto.AddMinutes( x.Minutes );
            dto = dto.AddSeconds( x.Seconds );
        }

        return dto;
    }
}