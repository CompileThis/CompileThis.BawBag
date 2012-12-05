namespace CompileThis.BawBag.Extensibility.Internal
{
    using System;
    using System.Linq;

    using CompileThis.Text;

    internal class PluginResponseValueProvider : IStringFormatterValueProvider
    {
        private readonly IPluginContext _context;

        public PluginResponseValueProvider(IPluginContext context)
        {
            this._context = context;
        }

        public string ProvideValue(string token)
        {
            if (token.Equals("WHO", StringComparison.OrdinalIgnoreCase))
            {
                return "@" + this._context.User.Name;
            }

            if (token.Equals("SOMEONE", StringComparison.OrdinalIgnoreCase))
            {
                var users = (from u in this._context.Room.Users
                             where u.Name != this._context.BotName
                             select u).ToList();

                var index = this._context.RandomProvider.Next(users.Count);
                var userName = users[index].Name;

                return "@" + userName;
            }

            return "UNKNOWN";
        }
    }
}