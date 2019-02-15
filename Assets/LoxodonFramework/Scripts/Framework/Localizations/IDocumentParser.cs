using System.Collections.Generic;
using System.IO;

namespace Loxodon.Framework.Localizations
{
    public interface IDocumentParser
    {
        Dictionary<string, object> Parse(Stream input);
        
    }
}
