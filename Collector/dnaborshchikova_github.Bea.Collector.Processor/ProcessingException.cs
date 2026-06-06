namespace dnaborshchikova_github.Bea.Collector.Processor
{
    public class ProcessingException : Exception
    {
        public ProcessingException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
