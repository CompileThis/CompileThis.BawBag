namespace CompileThis.BawBag.Plugins.Quotes
{
    using System;

    internal class Quote
    {
        public string CreatedBy { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public string Nick { get; set; }

        public string Text { get; set; }
    }
}