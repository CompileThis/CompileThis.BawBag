using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompileThis.BawBag
{
    public interface IDateTimeProvider
    {
        DateTime GetUtcNow();
    }

    public class DefaultDateTimeProvider : IDateTimeProvider
    {
        public DateTime GetUtcNow()
        {
            return DateTime.UtcNow;
        }
    }
}
