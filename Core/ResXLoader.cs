using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using ZiZhuJY.FileSystemHelpers;
using ZiZhuJY.Helpers;

namespace ZiZhuJY.ResX_Aggregator.Core
{
    public class ResXLoader
    {
        private Dictionary<string, Dictionary<string, string>> _dictionary;
        private IList<string> _resxFileNames; 

        public Dictionary<string, Dictionary<string, string>> Dictionary
        {
            get
            {
                return _dictionary;
            }
        }

        public ResXLoader(string resxAggregatorFullPath)
        {
            var directory = Path.GetDirectoryName(resxAggregatorFullPath);
            var myName = Path.GetFileNameWithoutExtension(resxAggregatorFullPath);

            _dictionary = new Dictionary<string, Dictionary<string, string>>();
            _resxFileNames = FileSystemHelper.SearchFileNames(directory, "^{0}.resx$".FormatWith(myName)).ToList();
            _resxFileNames = _resxFileNames.Select(e => Path.Combine(directory, e)).ToList();

            foreach (var fileName in _resxFileNames)
            {
                var culture = Path.GetFileNameWithoutExtension(fileName);
                if (!culture.StartsWith(myName)) continue;

                culture = culture.Length > myName.Length ? culture.Substring(myName.Length + 1) : "default";

                using (var fileStream = File.OpenRead(fileName))
                {
                    using (var resXReader = new ResXResourceReader(fileStream))
                    {
                        foreach (DictionaryEntry d in resXReader)
                        {
                            var key = d.Key.ToString();
                            var value = d.Value.ToString();

                            if (Dictionary.ContainsKey(key))
                            {
                                if (Dictionary[key].ContainsKey(culture))
                                {
                                    Dictionary[key][culture] = value;
                                }
                                else
                                {
                                    Dictionary[key].Add(culture, value);
                                }
                            }
                            else
                            {
                                Dictionary.Add(key, new Dictionary<string, string>() {{culture, value}});
                            }
                        }

                        resXReader.Close();
                    }

                    fileStream.Close();
                }
            }
        }
    }
}
