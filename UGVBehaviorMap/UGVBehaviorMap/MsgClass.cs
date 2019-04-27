using MessagePack;

namespace UGVBehaviorMap
{
     // *** Name all float/long types to double

    [MessagePackObject]
    public class Msg
    {
        [Key("type")]
        public string Type;
        [Key("id")]
        public int Id;
        [Key("sid")]
        public int Sid;
        [Key("tid")]
        public int Tid;
        [Key("time")]
        public double Time;
    }

    //////////////messages to be sent/////////////
    public class ConnectMsg : Msg
    {
        [Key("jobsAvailable")]
        public string[] JobsAvailable;
        public ConnectMsg()
        {
            Type = "connect";
        }
    }

    public class UpdateMsg : Msg
    {
        [Key("lat")]
        public double Lat;
        [Key("lng")]
        public double Lng;
        [Key("status")]
        public string Status;
        [Key("heading")]
        public double Heading;
        public UpdateMsg()
        {
            Type = "update";
        }
    }

    public class POIMsg : Msg
    {
        [Key("lat")]
        public string Lat;
        [Key("lng")]
        public string Lng;
        public POIMsg()
        {
            Type = "poi";
        }
    }

    public class CompleteMsg : Msg
    {
        public CompleteMsg()
        {
            Type = "complete";
        }
    }

    //////////////messages to be received/////////////

    public class ConnAckMsg : Msg
    {
        public ConnAckMsg()
        {
            Type = "connectionAck";
        }
    }

    public class StartMsg : Msg
    {
        [Key("jobType")]
        public string JobType;
        public StartMsg()
        {
            Type = "start";
        }
    }

    public class AddMissionMsg : Msg
    {
        [Key("missionInfo")]
        public MissionInfo MissionInfo; // either retrieveTarget or deliverTarget; same values required
        public AddMissionMsg()
        {
            Type = "addMission";
        }
    }

    public class MissionInfo
    {
        [Key("taskType")]
        public string TaskType;
        [Key("lat")]
        public double Lat;
        [Key("lng")]
        public double Lng;
    }

    public class PauseMsg : Msg
    {
        public PauseMsg()
        {
            Type = "pause";
        }
    }

    public class ResumeMsg : Msg
    {
        public ResumeMsg()
        {
            Type = "resume";
        }
    }

    public class StopMsg : Msg
    {
        public StopMsg()
        {
            Type = "stop";
        }
    }

    public class AckMsg : Msg
    {
        [Key("ackid")]
        public int AckId;
        public AckMsg()
        {
            Type = "ack";
        }
    }

    public class BadMsg : Msg
    {
        [Key("error")]
        public string Error;
        public BadMsg()
        {
            Type = "badMessage";
        }
    }
}
