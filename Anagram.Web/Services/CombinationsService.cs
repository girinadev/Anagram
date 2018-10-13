using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Anagram.Web.Services
{
    public class CombinationsService : ICombinationsService
    {
        public IList<string> GetByString(string str)
        {
            #region Validation

            if (string.IsNullOrEmpty(str))
            {
                throw new System.Exception("Input string value can't be empty");
            }

            str = Regex.Match(str, @"\b[A-Za-zа-яА-Я]+\b", RegexOptions.Singleline)
                .Value.Trim().ToLower();

            if (string.IsNullOrEmpty(str))
            {
                throw new System.Exception("Invalid string value");
            }

            #endregion

            var list = new List<string>();

            for (int i = 0; i < str.Length; i++)
            {
                char[] arr = str.Substring(0, str.Length - i).ToCharArray();
                list.AddRange(GetCombinations(arr));
            }

            return list;
        }

        public IList<string> FilterByDictionary(IList<string> list, HashSet<string> dict)
        {
            return list.AsParallel().Where(x => dict.Contains(x)).ToList();
        }

        private HashSet<string> GetCombinations(IList<char> arr, HashSet<string> hashSet = null, List<char> current = null)
        {
            if (hashSet == null) hashSet = new HashSet<string>();
            if (current == null) current = new List<char>();

            if (arr.Count == 0)
            {
                hashSet.Add(new string(current.ToArray()));
                return hashSet;
            }

            for (int i = 0; i < arr.Count; i++)
            {
                var lst = new List<char>(arr);
                lst.RemoveAt(i);
                var newCurrent = new List<char>(current) { arr[i] };
                GetCombinations(lst, hashSet, newCurrent);
            }

            return hashSet;
        }
    }
}
