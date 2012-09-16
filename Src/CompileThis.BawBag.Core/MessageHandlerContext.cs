using Raven.Client;
namespace CompileThis.BawBag
{
    using System;

    public class MessageHandlerContext
    {
        internal MessageHandlerContext()
        {
            
        }

        public IDocumentSession RavenSession
        {
            get { throw new NotImplementedException(); }
        }
    }
}
