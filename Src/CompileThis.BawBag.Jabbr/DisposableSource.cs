namespace CompileThis.BawBag.Jabbr
{
    using System;

    internal class DisposableSource
    {
        public IDisposable Disposable { get; set; }

        public void Dispose()
        {
            var disposable = Disposable;

            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}
