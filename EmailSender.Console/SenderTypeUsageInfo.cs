namespace EmailSender.Console
{
    public class SenderTypeUsageInfo
    {
        public string Name { get; set; }
        public string[] RequiredParameters { get; set; }
        public string[] OptionalParameters { get; set; }
    }
}