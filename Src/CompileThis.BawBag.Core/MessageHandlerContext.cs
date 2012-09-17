namespace CompileThis.BawBag
{
    using Raven.Client;

    public class MessageHandlerContext
    {
        private readonly IDocumentSession _session;

        internal MessageHandlerContext(IDocumentSession session)
        {
            _session = session;
        }

        public IDocumentSession RavenSession
        {
            get { return _session; }
        }
    }
}
