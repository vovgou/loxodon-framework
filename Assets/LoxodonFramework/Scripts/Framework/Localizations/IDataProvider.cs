using System;
using System.Collections.Generic;
using System.Globalization;

namespace Loxodon.Framework.Localizations
{
    public interface IDataProvider
    {
        void Load(CultureInfo cultureInfo, Action<Dictionary<string, object>> onCompleted);
    }
}
