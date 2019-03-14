using System.Collections.Concurrent;

namespace Bobend.LINQPadToolbox
{
    public class NamedMonitor
    {
        private readonly ConcurrentDictionary<string, object> _dictionary = new ConcurrentDictionary<string, object>();

        public object this[string name] => _dictionary.GetOrAdd(name, _ => new object());
    }
}
