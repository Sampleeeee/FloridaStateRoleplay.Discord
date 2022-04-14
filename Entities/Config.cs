using FloridaStateRoleplay.Discord.Entities;
using Newtonsoft.Json;


namespace FloridaStateRoleplay.Discord;

public class Config
{
    public static Config Current { get; set; }

    private Dictionary<string, ulong> _channels = new();

    public Dictionary<string, ulong> Channels
    {
        get => _channels;
        
        set
        {
            _channels = value;
            Save();
        }
    }

    private string _token = "OTIzNzk3MTc3NzAzMjE5MjIx.YcVPKg.DSxQB8i74TvUyO1wlet-9vMTNOo";

    public string Token
    {
        get => _token;

        set
        {
            _token = value;
            Save();
        }
    }

    private float _blacklistedWordMaxRatio = 90;
    public float BlacklistedWordMaxRatio
    {
        get => _blacklistedWordMaxRatio;
        set
        {
            _blacklistedWordMaxRatio = value;
            Save();
        }
    }

    private string[] _blacklistedWords =
    {
        "nigger"
    };

    public string[] BlacklistedWords
    {
        get => _blacklistedWords;
        set
        {
            _blacklistedWords = value;
            Save();
        }
    }
    
    /// <summary>
    /// Threads to stop from archiving
    /// </summary>
    private ( ulong, ulong )[] _keepAlive =
    {
        ( 917912579312087142, 952446561210761237 ),
        ( 928449051945492530, 952450719519215636 )
    };

    public ( ulong ChannelId, ulong ThreadId )[] KeepAliveThreads
    {
        get => _keepAlive;
        set
        {
            _keepAlive = value;
            Save();
        }
    }

    private ( ulong, ulong )[] _mediaOnlyChannels =
    {
        ( 917912579312087142, 952446561210761237 ),
        ( 928449051945492530, 952450719519215636 )
    };
        
    public ( ulong ChannelId, ulong ThreadId )[] MediaOnlyChannels
    {
        get => _mediaOnlyChannels;
        set
        {
            _mediaOnlyChannels = value;
            Save();
        }
    }

    private ulong _levelUpChannelId = 917912579312087146;

    public ulong LevelUpChannelId
    {
        get => _levelUpChannelId;
        set
        {
            _levelUpChannelId = value;
            Save();
        }
    }

    private List<Sticky> _stickies = new();

    public List<Sticky> Stickies
    {
        get => _stickies;
        set
        {
            _stickies = value;
            Save();
        }
    }

    private bool _levelingEnabled = true;

    public bool LevelingEnabled
    {
        get => _levelingEnabled;
        set
        {
            _levelingEnabled = value;
            Save();
        }
    }

    private ulong[] _ranks =
    {
        0,
        100,
        255,
        475,
        770,
        1150,
        1625,
        2205,
        2900,
        3720,
        4675,
        5775,
        7030,
        8450,
        10045,
        11825,
        13800,
        15980,
        18375,
        20995,
        23850,
        26950,
        30305,
        33925,
        37820,
        42000,
        46475,
        51255,
        56350,
        61770,
        67525,
        73625,
        80080,
        86900,
        94095,
        101675,
        109650,
        118030,
        126825,
        136045,
        145700,
        155800,
        166355,
        177375,
        188870,
        200850,
        213325,
        226305,
        239800,
        253820,
        268375,
        283475,
        299130,
        315350,
        332145,
        349525,
        367500,
        386080,
        405275,
        425095,
        445550,
        466650,
        488405,
        510825,
        533920,
        557700,
        582175,
        607355,
        633250,
        659870,
        687225,
        715325,
        744180,
        773800,
        804195,
        835375,
        867350,
        900130,
        933725,
        968145,
        1003400,
        1039500,
        1076455,
        1114275,
        1152970,
        1192550,
        1233025,
        1274405,
        1316700,
        1359920,
        1404075,
        1449175,
        1495230,
        1542250,
        1590245,
        1639225,
        1689200,
        1740180,
        1792175,
        1845195,
        1899250
    };

    public ulong[] Ranks
    {
        get => _ranks;
        set
        {
            _ranks = value;
            Save();
        }
    }
   

    public static void Initalize()
    {
        const string path = "./data/config.json";
        
        string json = File.ReadAllText( path );
        var config = JsonConvert.DeserializeObject<Config>( json );
        
        if ( config is null ) return;
        Current = config;
    }

    public static void Save()
    {
        const string path = "./data/config.json";
        
        string json = JsonConvert.SerializeObject( Current );
        File.WriteAllText( path, json );
    }

    public static void AddChannel( string key, ulong value )
    {
        Current.Channels[key] = value;
        Save();
    }

    public static void RemoveChannel( string key )
    {
        Current.Channels.Remove( key );
        Save();
    }
}