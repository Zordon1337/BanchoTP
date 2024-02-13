using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using static Server;

namespace BanchoTroller.Structs
{
    public class bUserStats
    {
        public bUserStats(int userId, string username, long rankedScore, float accuracy, int playcount, long totalScore, int rank, string avatarFilename, bStatusUpdate status, int timezone, string location, Permissions permission)
        {
            this.userId = userId;
            this.username = username;
            this.rankedScore = rankedScore;
            this.accuracy = accuracy;
            this.playcount = playcount;
            this.totalScore = totalScore;
            this.rank = rank;
            this.avatarFilename = avatarFilename;
            this.status = status;
            this.timezone = timezone;
            this.location = location;
            this.permission = permission;
        }

        


        public void WriteToStream(MemoryStream ms)
        {
            this.WriteToStream(new Writer(ms), false);
        }

        public void WriteToStream(Writer sw, bool forceFull)
        {
            sw.Write(this.userId);
            sw.Write((byte)this.completeness);
            this.status.WriteToStream(sw);
            if (this.completeness > Completeness.StatusOnly || forceFull)
            {
                sw.Write(this.rankedScore);
                sw.Write(this.accuracy);
                sw.Write(this.playcount);
                sw.Write(this.totalScore);
                sw.Write((ushort)this.rank);
            }
            if (this.completeness == Completeness.Full || forceFull)
            {
                sw.Write(this.username);
                sw.Write(this.avatarFilename);
                sw.Write((byte)(this.timezone + 24));
                sw.Write(this.location);
                sw.Write((byte)this.permission);
            }
            
        }

        

        public float accuracy;

        public string avatarFilename;

        public Completeness completeness;

        public int level;

        public string location;

        private readonly Permissions permission;

        public int playcount;

        public int rank;

        public long rankedScore;

        public bStatusUpdate status;

        public int timezone;

        public long totalScore;

        public int userId;

        public string username;
    }
    public enum Permissions
    {
        None = 0,
        Normal = 1,
        BAT = 2,
        Subscriber = 4
    }
    public enum bStatus
    {
        Idle,
        Afk,
        Playing,
        Editing,
        Modding,
        Multiplayer,
        Watching,
        Unknown,
        Testing,
        Submitting,
        StatsUpdate,
        Paused,
        Lobby
    }
    public enum RequestType
    {
        Osu_SendUserStatus,
        Osu_SendIrcMessage,
        Osu_Exit,
        Osu_RequestStatusUpdate,
        Osu_Pong,
        Bancho_LoginReply,
        Bancho_CommandError,
        Bancho_SendIrcMessage,
        Bancho_Ping,
        Bancho_HandleIrcChangeUsername,
        Bancho_HandleIrcQuit,
        Bancho_HandleIrcJoin,
        Bancho_HandleOsuUpdate,
        Bancho_HandleOsuQuit,
        Bancho_SpectatorJoined,
        Bancho_SpectatorLeft,
        Bancho_SpectateFrames,
        Osu_StartSpectating,
        Osu_StopSpectating,
        Osu_SpectateFrames,
        Bancho_VersionUpdate,
        Osu_ErrorReport,
        Osu_CantSpectate,
        Bancho_SpectatorCantSpectate,
        Bancho_GetAttention,
        Bancho_Announce,
        Osu_SendIrcMessagePrivate,
        Bancho_MatchUpdate,
        Bancho_MatchNew,
        Bancho_MatchDisband,
        Osu_LobbyPart,
        Osu_LobbyJoin,
        Osu_MatchCreate,
        Osu_MatchJoin,
        Osu_MatchPart,
        Bancho_LobbyJoin,
        Bancho_LobbyPart,
        Bancho_MatchJoinSuccess,
        Bancho_MatchJoinFail,
        Osu_MatchChangeSlot,
        Osu_MatchReady,
        Osu_MatchLock,
        Osu_MatchChangeSettings,
        Bancho_FellowSpectatorJoined,
        Bancho_FellowSpectatorLeft,
        Osu_MatchStart,
        AllPlayersLoaded,
        Bancho_MatchStart,
        Osu_MatchScoreUpdate,
        Bancho_MatchScoreUpdate,
        Osu_MatchComplete,
        Bancho_MatchTransferHost,
        Osu_MatchChangeMods,
        Osu_MatchLoadComplete,
        Bancho_MatchAllPlayersLoaded,
        Osu_MatchNoBeatmap,
        Osu_MatchNotReady,
        Osu_MatchFailed,
        Bancho_MatchPlayerFailed,
        Bancho_MatchComplete,
        Osu_MatchHasBeatmap,
        Osu_MatchSkipRequest,
        Bancho_MatchSkip,
        Bancho_Unauthorised,
        Osu_ChannelJoin,
        Bancho_ChannelJoinSuccess,
        Bancho_ChannelAvailable,
        Bancho_ChannelRevoked,
        Bancho_ChannelAvailableAutojoin,
        Osu_BeatmapInfoRequest,
        Bancho_BeatmapInfoReply,
        Osu_MatchTransferHost,
        Bancho_LoginPermissions,
        Bancho_FriendsList,
        Osu_FriendAdd,
        Osu_FriendRemove,
        Bancho_ProtocolNegotiation
    }
    public enum Mods
    {

        None,
        NoFail,
        Easy,
        NoVideo = 4,
        Hidden = 8,
        HardRock = 16,
        SuddenDeath = 32,
        DoubleTime = 64,
        Relax = 128,
        HalfTime = 256,
        Taiko = 512
    }
    public enum Completeness
    {
        StatusOnly,
        Statistics,
        Full
    }
    public enum SlotStatus
    {
        Open = 1,
        Locked = 2,
        NotReady = 4,
        Ready = 8,
        NoMap = 16,
        Playing = 32,
        Complete = 64,
        HasPlayer = 124
    }
    public enum MatchTypes
    {
        Standard,
        Powerplay
    }
    public enum PlayModes
    {
        OsuStandard,
        Taiko,
        CatchTheBeat
    }
    public class bStatusUpdate
    {
        public bStatusUpdate(bStatus status, bool beatmapUpdate, string statusText, string songChecksum, int beatmapId, Mods mods, PlayModes playMode)
        {
            this.status = status;
            this.beatmapUpdate = beatmapUpdate;
            this.beatmapChecksum = songChecksum;
            this.statusText = statusText;
            this.currentMods = mods;
            this.playMode = playMode;
            this.beatmapId = beatmapId;
        }

        public bStatusUpdate(Stream s)
        {
            if (s.CanRead)
            {
                try
                {
                    BinaryReader sr = new BinaryReader(s);
                    this.status = (bStatus)sr.ReadByte();
                    this.beatmapUpdate = sr.ReadBoolean();
                    if (!this.beatmapUpdate)
                    {
                        return;
                    }
                    this.statusText = sr.ReadString();
                    this.beatmapChecksum = sr.ReadString();
                    this.currentMods = (Mods)sr.ReadUInt16();
                    this.playMode = (PlayModes)sr.ReadByte();
                    this.beatmapId = sr.ReadInt32();
                }
                catch (Exception e)
                {

                }
            }
        }
        public void WriteToStream(Writer sw)
        {
            sw.Write((byte)this.status);
            sw.Write(this.beatmapUpdate);
            if (!this.beatmapUpdate)
            {
                return;
            }
            sw.Write(this.statusText);
            sw.Write(this.beatmapChecksum);
            sw.Write((ushort)this.currentMods);
            sw.Write((byte)this.playMode);
            sw.Write(this.beatmapId);
            
        }

        
        public string beatmapChecksum;

        public int beatmapId;

        public bool beatmapUpdate;

        public Mods currentMods;

        public PlayModes playMode;

        public bStatus status;

        public string statusText;
    }
    
}
