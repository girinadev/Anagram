using System.Collections.Generic;

namespace Anagram.Web.Services
{
    public interface ICombinationsService
    {
        IList<string> GetByString(string str);

        IList<string> FilterByDictionary(IList<string> list, HashSet<string> dict);
    }
}
