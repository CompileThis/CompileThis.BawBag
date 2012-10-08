namespace CompileThis.BawBag.Jabbr
{
    using System;

    internal class DisposableWrapper : DisposableBase
    {
        public IDisposable Disposable { get; set; }

        protected override void OnDisposing(bool isDisposing)
        {
            if (isDisposing)
            {
                var disposable = Disposable;

                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}
