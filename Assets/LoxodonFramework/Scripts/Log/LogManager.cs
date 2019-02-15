using System;

namespace Loxodon.Log
{
    public static class LogManager
    {
        private static readonly DefaultLogFactory _defaultFactory = new DefaultLogFactory();
        private static ILogFactory _factory;

        public static DefaultLogFactory Default { get { return _defaultFactory; } }

        public static ILog GetLogger(Type type)
        {
            if (_factory != null)
                return _factory.GetLogger(type);

            return _defaultFactory.GetLogger(type);
        }

        public static ILog GetLogger(string name)
        {
            if (_factory != null)
                return _factory.GetLogger(name);

            return _defaultFactory.GetLogger(name);
        }

        public static void Registry(ILogFactory factory)
        {
            if (_factory != null && _factory != factory)
                throw new Exception("Don't register log factory many times");

            _factory = factory;
        }

    }
}
