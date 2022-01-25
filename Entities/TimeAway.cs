using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using FloridaStateRoleplay.Discord.Extensions;

namespace FloridaStateRoleplay.Discord.Entities;

public class TimeAway
{
    public int Hours { get; set; }
    public int Days { get; set; }
    public int Weeks { get; set; }
    public int Months { get; set; }
    public int Years { get; set; }

    public TimeAway( string time )
    {
        const string YEARS = @"\d+(?=y)";
        const string MONTHS = @"\d+(?=m)";
        const string WEEKS = @"\d+(?=w)";
        const string DAYS = @"\d+(?=y)";
        const string HOURS = @"\d+(?=h)";

        var yearsMatch = Regex.Match( time, YEARS );
        var monthsMatch = Regex.Match( time, MONTHS );
        var weeksMatch = Regex.Match( time, WEEKS );
        var daysMatch = Regex.Match( time, DAYS );
        var hoursMatch = Regex.Match( time, HOURS );

        Years = GetNumberFromMatch( yearsMatch );
        Months = GetNumberFromMatch( monthsMatch );
        Weeks = GetNumberFromMatch( weeksMatch );
        Days = GetNumberFromMatch( daysMatch );
        Hours = GetNumberFromMatch( hoursMatch );
    }

    public string FormattedTime()
    {
        var str = string.Empty;

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

        return str.TrimEnd();
    }
    
    private static int GetNumberFromMatch( Match match ) =>
        match.Value == string.Empty ? 0 : Convert.ToInt32( match.Value );

    public TimeSpan ToTimeSpan()
    {
        return TimeSpan.FromDays( Years * 365 ).Add( TimeSpan.FromDays( Months * 30 ) )
            .Add( TimeSpan.FromDays( Weeks * 7 ) ).Add( TimeSpan.FromDays( Days ) ).Add( TimeSpan.FromHours( Hours ) );
    }

    public override string ToString()
    {
        return FormattedTime();
    }
}