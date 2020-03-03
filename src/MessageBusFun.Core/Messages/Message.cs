using System.Text;

namespace MessageBusFun.Core.Messages
{
    public abstract class Message
    {
        public MessageType MessageType { get; internal set; }
        public static char Delimiter = '|';
        public static string EOF = "<EOF>";
        public string User { get; set; }
        public string Password { get; set; }

        public abstract byte[] ToByteArray();

        internal byte[] ToByteArray(params string[] values)
        {
            var sb = new StringBuilder();
            foreach (var value in values)
            {
                sb.Append(Delimiter + value);
            }
            return Encoding.ASCII.GetBytes(MessageType.ToString() + Delimiter + User + Delimiter + Password + sb + EOF);
        }

        public virtual void ParseMessage(byte[] message)
        {
            var messageStringArray = GetMessageStringArray(message);
            User = messageStringArray[1];
            Password = messageStringArray[2];
        }

        public override string ToString()
        {
            return Encoding.ASCII.GetString(ToByteArray()).Replace(Message.EOF, string.Empty);
        }

        public string[] GetMessageStringArray(byte[] message)
        {
            return Encoding.ASCII.GetString(message).Replace(EOF, string.Empty).Split(Delimiter);
        }
    }
}
