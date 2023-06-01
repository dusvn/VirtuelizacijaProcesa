using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public enum MessageType { Info, Warning, Error }
    [Serializable]
    public class Audit
    {
        static int count = 0;
        int id;
        DateTime timestamp;
        MessageType messageType;
        string message;

        public int Id { get => id; set => id = value; }
        public DateTime Timestamp { get => timestamp; set => timestamp = value; }
        public MessageType MessageType { get => messageType; set => messageType = value; }
        public string Message { get => message; set => message = value; }

        public Audit(int id, DateTime timestamp, MessageType messageType, string message)
        {
            Id = id;
            Timestamp = timestamp;
            MessageType = messageType;
            Message = message;
        }
        public Audit(DateTime timeStamp, MessageType messageType, string message)
        {
            Id = ++count;
            Timestamp = timeStamp;
            MessageType = messageType;
            Message = message;
        }

        public Audit()
        {

        }
    }
}
