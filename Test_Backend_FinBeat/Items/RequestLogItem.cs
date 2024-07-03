namespace Test_Backend_FinBeat.Items
{
    public class RequestLogItem
    {
        public int Id { get; set; }
        public string Method { get; set; }
        public string Path { get; set; }
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
        public int StatusCode { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
