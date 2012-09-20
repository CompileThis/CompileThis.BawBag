namespace CompileThis.BawBag.Handlers.Factoids
{
    internal class FactiodResponse
    {
        public string Trigger { get; set; }
        public string Response { get; set; }
        public FactoidResponseType ResponseType { get; set; }
        public bool IsCaseSensitive { get; set; }
    }
}