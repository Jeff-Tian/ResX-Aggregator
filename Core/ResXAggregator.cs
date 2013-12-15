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
    public class ResXAggregator
    {
        private readonly Dictionary<string, Dictionary<string, string>> _dictionary;
        private string _myFullPath;

        public Dictionary<string, Dictionary<string, string>> Dictionary
        {
            get
            {
                return _dictionary;
            }
        }

        public ResXAggregator(string resxAggregatorFullPath)
        {
            _myFullPath = resxAggregatorFullPath;
            var directory = Path.GetDirectoryName(resxAggregatorFullPath);
            var myName = Path.GetFileNameWithoutExtension(resxAggregatorFullPath);

            _dictionary = new Dictionary<string, Dictionary<string, string>>();
            IList<string> resxFileNames = FileSystemHelper.SearchFileNames(directory, "^{0}.resx$".FormatWith(myName)).ToList();
            resxFileNames = resxFileNames.Select(e => Path.Combine(directory, e)).ToList();

            foreach (var fileName in resxFileNames)
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

        public void Save()
        {
            var dic = GetAllCultureResxFileNames();

            foreach (var item in dic)
            {
                var fullPath = Path.Combine(Path.GetDirectoryName(_myFullPath), item.Value);
                if (!File.Exists(fullPath))
                {
                    File.Create(fullPath).Close();
                }

                Save(item.Key, fullPath);
            }
        }

        private void Save(string culture, string fullPath)
        {
            using (var fileStream = File.OpenWrite(fullPath))
            {
                using (var resxWriter = new ResXResourceWriter(fileStream))
                {
                    foreach (var item in Dictionary)
                    {
                        var key = item.Key;
                        if (!item.Value.ContainsKey(culture)) continue;

                        var value = item.Value[culture];
                        resxWriter.AddResource(key, value);
                    }

                    resxWriter.Generate();
                    resxWriter.Close();
                }

                fileStream.Close();
            }
        }

        private Dictionary<string,string> GetAllCultureResxFileNames()
        {
            var dic = new Dictionary<string, string>();
            var myName = Path.GetFileNameWithoutExtension(_myFullPath);

            foreach (var key in Dictionary.SelectMany(item => item.Value.Keys.Where(key => !dic.ContainsKey(key))))
            {
                dic.Add(key,
                    "{0}.{1}resx".FormatWith(myName,
                        key.Equals("default", StringComparison.InvariantCultureIgnoreCase)
                            ? string.Empty
                            : "{0}.".FormatWith(key)));
            }

            return dic;
        }
    }
}
