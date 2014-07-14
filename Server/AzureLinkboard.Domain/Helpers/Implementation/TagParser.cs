using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AzureLinkboard.Domain.Helpers.Implementation
{
    internal class TagParser : ITagParser
    {
        public IEnumerable<string> FromString(string value)
        {
            string[] tags = value.Split(' ');
            return tags.Where(x => x.Trim().Length > 0).Select(x => x.Trim()).ToList();
        }

        public string ToString(IEnumerable<string> tags)
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (string tag in tags)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.Append(" ");
                }
                sb.Append(tag);
            }
            return sb.ToString();
        }
    }
}
