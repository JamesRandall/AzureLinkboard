using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureLinkboard.Domain.Helpers
{
    public interface ITagParser
    {
        IEnumerable<string> FromString(string value);
        string ToString(IEnumerable<string> tags);
    }
}
