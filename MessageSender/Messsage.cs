namespace MessageSender
{
    public class Message
    {
        public string From { get; set; }

        public string To { get; set; }

        public string Body { get; set; }

        public override string ToString() => $"From={From};To={To};Body={Body}";
    }
}
