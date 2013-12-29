using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Linq;
using System.Reflection;
using System.Xml;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Diagnostics;

namespace ZiZhuJY.Helpers
{
    public static class Extensions
    {
        /// <summary>
        /// Defines the simple types that is directly writeable to XML.
        /// </summary>
        private static readonly Type[] writableTypes = new[] { typeof(string), typeof(DateTime), typeof(Enum), typeof(decimal), typeof(Guid) };

        #region object extensions
        /// <summary>
        /// Convert an object to a boolean value, if failed, return the specified value of Boolean type.
        /// </summary>
        /// <param name="o">The object.</param>
        /// <param name="defaultValue">The specified value to return when convertion failed.</param>
        /// <returns>A boolean value.</returns>
        public static bool ToBooleanOrDefault(this object o, bool defaultValue)
        {
            bool result;
            if (o != null && bool.TryParse(o.ToString(), out result))
            {
                return result;
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Convert an object to a boolean value, if failed, return the default value of Boolean type.
        /// </summary>
        /// <param name="o">The object.</param>
        /// <returns>A boolean value.</returns>
        public static bool ToBoolean(this object o)
        {
            return o.ToBooleanOrDefault(default(bool));
        }

        /// <summary>
        /// Convert an object to a integer value, if failed, return the specified value of Integer type.
        /// </summary>
        /// <param name="o">The object.</param>
        /// <param name="defaultValue">The specified value to return when convertion failed.</param>
        /// <returns>A integer value.</returns>
        public static int ToIntOrDefault(this object o, int defaultValue)
        {
            int result;
            if (o != null && int.TryParse(o.ToString(), out result))
            {
                return result;
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Convert an object to a integer value, if failed, return the default value of Integer type.
        /// </summary>
        /// <param name="o">The object.</param>
        /// <returns>A integer value</returns>
        public static int ToInt(this object o)
        {
            return o.ToIntOrDefault(default(int));
        }

        /// <summary>
        /// Convert an object to a DateTime value, if failed, return the specified value of DateTime type.
        /// </summary>
        /// <param name="o">The object to be converted.</param>
        /// <param name="defaultValue">The specified value to return when convertion failed.</param>
        /// <returns>A DateTime value.</returns>
        public static DateTime ToDateTimeOrDefault(this object o, DateTime defaultValue)
        {
            DateTime result;
            if (o != null && DateTime.TryParse(o.ToString(), out result))
            {
                return result;
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Convert an object to a DateTime value, if failed, return the default value of DateTime type.
        /// </summary>
        /// <param name="o">The object.</param>
        /// <returns>A DateTime value.</returns>
        public static DateTime ToDateTime(this object o)
        {
            return o.ToDateTimeOrDefault(default(DateTime));
        }

        /// <summary>
        /// To the serialized XML string.
        /// </summary>
        /// <param name="o">The object.</param>
        /// <returns>A XML string</returns>
        public static string ToSerializedXmlString(this object o)
        {
            XmlSerializer x = new XmlSerializer(o.GetType());
            System.Xml.XmlDocument xdoc = new System.Xml.XmlDocument();
            using (MemoryStream ms = new MemoryStream())
            {
                x.Serialize(ms, o);
                ms.Position = 0;
                xdoc.Load(ms);
                ms.Close();
            }

            return xdoc.InnerXml;
        }

        /// <summary>
        /// Determines whether [is simple type] [the specified type].
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        ///     <c>true</c> if [is simple type] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSimpleType(this Type type)
        {
            return type.IsPrimitive || writableTypes.Contains(type);
        }

        /// <summary>
        /// Converts an anonymous type to an XElement.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Returns the object as it's XML representation in an XElement.</returns>
        public static XElement ToXml(this object input)
        {
            return input.ToXml(null);
        }

        /// <summary>
        /// Converts an anonymous type to an XElement.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="element">The element name.</param>
        /// <returns>Returns the object as it's XML representation in an XElement.</returns>
        public static XElement ToXml(this object input, string element)
        {
            if (input == null)
            {
                return null;
            }

            if (String.IsNullOrEmpty(element))
            {
                element = "object";
            }

            element = XmlConvert.EncodeName(element);
            var ret = new XElement(element);

            if (input != null)
            {
                var type = input.GetType();
                var props = type.GetProperties();

                var elements = from prop in props
                               let name = XmlConvert.EncodeName(prop.Name)
                               let val = prop.PropertyType.IsArray ? "array" : prop.GetValue(input, null)
                               let value = prop.PropertyType.IsArray ? GetArrayElement(prop, (Array)prop.GetValue(input, null)) : (prop.PropertyType.IsSimpleType() ? new XElement(name, val) : val.ToXml(name))
                               where value != null
                               select value;

                ret.Add(elements);
            }

            return ret;
        }

        /// <summary>
        /// Gets the array element.
        /// </summary>
        /// <param name="info">The property info.</param>
        /// <param name="input">The input object.</param>
        /// <returns>Returns an XElement with the array collection as child elements.</returns>
        private static XElement GetArrayElement(PropertyInfo info, Array input)
        {
            var name = XmlConvert.EncodeName(info.Name);

            XElement rootElement = new XElement(name);

            var arrayCount = input.GetLength(0);

            for (int i = 0; i < arrayCount; i++)
            {
                var val = input.GetValue(i);
                XElement childElement = val.GetType().IsSimpleType() ? new XElement(name + "Child", val) : val.ToXml();

                rootElement.Add(childElement);
            }

            return rootElement;
        }
        #endregion

        #region string extensions
        /// <summary>
        /// A short cut for string.Format()
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The target string.</returns>
        public static string FormatWith(this string format, params object[] parameters)
        {
            return string.Format(format, parameters);
        }

        /// <summary>
        /// Extract a sub string from the parent string. For example, "||target|abcd".Substring("||", "|") = "target"
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="startAfter">The string that the sub string starts after.</param>
        /// <param name="endBefore">The string that the sub string ends before.</param>
        /// <param name="comparison">The comparison method.</param>
        /// <returns>The sub string.</returns>
        public static string Substring(this string s, string startAfter, string endBefore, StringComparison comparison)
        {
            int index1 = s.IndexOf(startAfter, comparison);
            if (index1 >= 0) index1 += startAfter.Length;
            else return "";

            int index2 = s.IndexOf(endBefore, index1, comparison);
            if (index2 >= 0) return s.Substring(index1, index2 - index1);
            else return s.Substring(index1);
        }

        /// <summary>
        /// Determines whether the source string contains a specified sub string.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="toCheck">The specified string to check.</param>
        /// <param name="comp">The comparison option.</param>
        /// <returns>
        ///   <c>true</c> if [contains] [the specified string to check]; otherwise, <c>false</c>.
        /// </returns>
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }
        #endregion

        #region StringCollection extensions
        /// <summary>
        /// Convert the StringCollection to an array.
        /// </summary>
        /// <param name="stringCollection">The string collection.</param>
        /// <returns>The array</returns>
        public static string[] ToArray(this System.Collections.Specialized.StringCollection stringCollection)
        {
            List<string> result = new List<string>();
            foreach (string s in stringCollection)
            {
                result.Add(s);
            }

            return result.ToArray();
        }
        #endregion StringCollection extensions

        #region SqlParameter extensions
        /// <summary>
        /// Format the SQL parameter values.
        /// </summary>
        /// <param name="sp">The SQL Parameter</param>
        /// <returns>The formatted SQL parameter value.</returns>
        public static String ParameterValueForSQL(this SqlParameter sp)
        {
            String retval = "";

            switch (sp.SqlDbType)
            {
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.Time:
                case SqlDbType.VarChar:
                case SqlDbType.Xml:
                case SqlDbType.Date:
                case SqlDbType.DateTime:
                case SqlDbType.DateTime2:
                case SqlDbType.DateTimeOffset:
                    retval = "'" + sp.Value.ToString().Replace("'", "''") + "'";
                    break;

                case SqlDbType.Bit:
                    retval = (sp.Value.ToBooleanOrDefault(false)) ? "1" : "0";
                    break;

                default:
                    retval = sp.Value.ToString().Replace("'", "''");
                    break;
            }

            return retval;
        }

        /// <summary>
        /// Get the text of SQL Command with the parameters resolved.
        /// </summary>
        /// <param name="sc">The SQL Command object.</param>
        /// <returns>The SQL Command text with the parameters resolved.</returns>
        public static String CommandAsSql(this SqlCommand sc)
        {
            StringBuilder sql = new StringBuilder();
            Boolean FirstParam = true;

            sql.AppendLine("use " + sc.Connection.Database + ";");
            switch (sc.CommandType)
            {
                case CommandType.StoredProcedure:
                    sql.AppendLine("declare @return_value int;");

                    foreach (SqlParameter sp in sc.Parameters)
                    {
                        if ((sp.Direction == ParameterDirection.InputOutput) || (sp.Direction == ParameterDirection.Output))
                        {
                            sql.Append("declare " + sp.ParameterName + "\t" + sp.SqlDbType.ToString() + "\t= ");

                            sql.AppendLine(((sp.Direction == ParameterDirection.Output) ? "null" : sp.ParameterValueForSQL()) + ";");

                        }
                    }

                    sql.AppendLine("exec [" + sc.CommandText + "]");

                    foreach (SqlParameter sp in sc.Parameters)
                    {
                        if (sp.Direction != ParameterDirection.ReturnValue)
                        {
                            sql.Append((FirstParam) ? "\t" : "\t, ");

                            if (FirstParam) FirstParam = false;

                            if (sp.Direction == ParameterDirection.Input)
                                sql.AppendLine(sp.ParameterName + " = " + sp.ParameterValueForSQL());
                            else

                                sql.AppendLine(sp.ParameterName + " = " + sp.ParameterName + " output");
                        }
                    }
                    sql.AppendLine(";");

                    sql.AppendLine("select 'Return Value' = convert(varchar, @return_value);");

                    foreach (SqlParameter sp in sc.Parameters)
                    {
                        if ((sp.Direction == ParameterDirection.InputOutput) || (sp.Direction == ParameterDirection.Output))
                        {
                            sql.AppendLine("select '" + sp.ParameterName + "' = convert(varchar, " + sp.ParameterName + ");");
                        }
                    }
                    break;
                case CommandType.Text:
                    string query = sc.CommandText;
                    foreach (SqlParameter p in sc.Parameters)
                    {
                        switch (p.SqlDbType)
                        {
                            case SqlDbType.Bit:
                                query = query.Replace("@" + p.ParameterName, string.Format("{0}", (bool)p.Value ? 1 : 0));
                                break;
                            case SqlDbType.Int:
                                query = query.Replace("@" + p.ParameterName, string.Format("{0}", p.Value));
                                break;
                            case SqlDbType.VarChar:
                                query = query.Replace("@" + p.ParameterName, string.Format("'{0}'", p.Value));
                                break;
                            default:
                                query = query.Replace("@" + p.ParameterName, string.Format("'{0}'", p.Value));
                                break;
                        }
                    }
                    sql.AppendLine(query);
                    break;
            }

            return sql.ToString();
        }
        #endregion

        #region Exception extensions
        /// <summary>
        /// Get the inner most exception from a exception object.
        /// </summary>
        /// <param name="ex">The exception object.</param>
        /// <returns>The inner most exception.</returns>
        public static Exception InnerMostException(this Exception ex)
        {
            Exception innerException = ex;
            while (innerException.InnerException != null) innerException = innerException.InnerException;
            return innerException;
        }

        public static string GetDebugString(this Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            int level = 0;

            do
            {
                sb.AppendLine(string.Format("{0}", ex.Message).PadLeft(4 * level, ' '));
                sb.AppendLine(string.Format("Source: {0}", ex.Source).PadLeft(4 * level, ' '));
                sb.AppendLine(string.Format("Type: {0}", ex.GetType().Name).PadLeft(4 * level, ' '));
                sb.AppendLine(string.Format("StackTrace: {0}", ex.StackTrace).PadLeft(4 * level, ' '));
                sb.AppendLine(string.Format("Data: {0}", ex.Data).PadLeft(4 * level, ' '));
                sb.AppendLine(string.Format("HelpLink: {0}", ex.HelpLink).PadLeft(4 * level, ' '));

                ex = ex.InnerException;
                level++;
            } while (ex != null);

            return sb.ToString();
        }
        #endregion

        #region Dictionary extensions
        /// <summary>
        /// Update if already exist or insert if new an item into a dictionary object.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dic">The dictionary object.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void Upsert<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, TValue value)
        {
            if (dic.ContainsKey(key))
            {
                dic[key] = value;
            }
            else
            {
                try
                {
                    dic.Add(key, value);
                }
                catch (Exception ex)
                {
                    throw new Exception("Exception caught when Add (key = {0}, value = {1})!".FormatWith(key, value), ex);
                }
            }
        }

        /// <summary>
        /// Gets the value or default.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dic">The dic.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The value associated with the key. If the key not found, then return the default value of the type of the value.</returns>
        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, TValue defaultValue = default(TValue))
        {
            TValue value;
            if (dic.TryGetValue(key, out value)) return value;
            else return defaultValue;
        }

        /// <summary>
        /// Gets the value or default.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dic">The dic.</param>
        /// <param name="key">The key.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The value associated with the key. If the key not found, then return the default value of the type of the value.</returns>
        public static TValue GetValueOrDefault<TValue>(this Dictionary<string, TValue> dic, string key, StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase, TValue defaultValue = default(TValue))
        {
            foreach (KeyValuePair<string, TValue> pair in dic)
            {
                if (pair.Key.Equals(key, stringComparison))
                    return pair.Value;
            }
            return defaultValue;
        }

        /// <summary>
        /// Gets the value or default.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dic">The dic.</param>
        /// <param name="pairDelimiter">The pair delimiter.</param>
        /// <param name="keyValueDelimiter">The key value delimiter.</param>
        /// <param name="nullValue">The null value.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// The value associated with the key. If the key not found, then return the default value of the type of the value.
        /// </returns>
        public static string ToString<TKey, TValue>(this Dictionary<TKey, TValue> dic, string pairDelimiter = ";", string keyValueDelimiter = "=", string nullValue = "[null]")
        {
            List<string> pairs = new List<string>();
            foreach (KeyValuePair<TKey, TValue> pair in dic)
            {
                pairs.Add("{0}{1}{2}".FormatWith(pair.Key.ToString(), keyValueDelimiter, pair.Value != null ? pair.Value.ToString() : nullValue));
            }

            return string.Join(pairDelimiter, pairs.ToArray());
        }
        #endregion

        #region XElement extensions
        /// <summary>
        /// The inner xml of an XElement
        /// </summary>
        /// <remarks>This method would append xml namespace for each nodes of that element if that element has a special xmlns attribute.</remarks>
        /// <param name="element">The element.</param>
        /// <returns>The inner xml string.</returns>
        public static string InnerXml(this XElement element)
        {
            StringBuilder innerXml = new StringBuilder();
            foreach (XNode node in element.Nodes())
            {
                innerXml.Append(node.ToString());
            }

            return innerXml.ToString();
        }

        /// <summary>
        /// The inner xml string of an XElement
        /// </summary>
        /// <remarks>This method would not append the element's special xmlns to each its child nodes.</remarks>
        /// <param name="element">The element.</param>
        /// <returns>The inner xml string.</returns>
        public static string InnerXmlString(this XElement element)
        {
            //return element.Descendants().Select(x => x.ToString()).Aggregate(String.Concat);
            string text = element.ToXmlNode().InnerXml;
            text = Regex.Replace(text, "^<[^>]+>", "");
            text = Regex.Replace(text, "<[^>]+>$", "");
            return text;
        }

        /// <summary>
        /// Convert the XElement object to the equivalent XmlNode object
        /// </summary>
        /// <param name="element">The XElement object.</param>
        /// <returns>The equivalent XmlNode object.</returns>
        public static XmlNode ToXmlNode(this XElement element)
        {
            using (XmlReader xmlReader = element.CreateReader())
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlReader);
                return xmlDoc;
            }
        }
        #endregion XElement extensions

        #region Random's Extension
        /// <summary>
        /// Returns a random character between the start and end characters specified
        /// </summary>
        /// <param name="rnd">
        /// <param name="start">The start of the range that the next random character will be generated from
        /// <param name="end">The end of the range that the next random character will be generated from
        /// <returns>A character whose ASCII code greater than or equal to the start's and less than or equal to the end's</returns>
        public static char NextChar(this Random rnd, char start = 'a', char end = 'z')
        {
            int startCode = (int)start;
            int endCode = (int)end + 1;
            if (startCode <= endCode)
            {
                int code = rnd.Next(startCode, endCode);
                return (char)code;
            }
            else
            {
                throw new ArgumentException("The 'start' character can NOT be greater than the 'end' charcater", "start");
            }
        }

        /// <summary>
        /// Returns a random character among a set of specified characters
        /// </summary>
        /// <param name="rnd">
        /// <param name="candidates">A set of the characters that the new random character will be generated from
        /// <returns>A character from the specified character set</returns>
        public static char NextChar(this Random rnd, char[] candidates)
        {
            if (candidates.Length > 0)
                return candidates[rnd.Next(0, candidates.Length)];
            else
                throw new ArgumentException("Must specify at least 1 character in the array (char[] candidates).", "candidates");
        }

        /// <summary>
        /// Returns a random letter character ({'a' - 'z'} + {'A' - 'Z'})
        /// </summary>
        /// <param name="rnd">
        /// <returns>A character of the 26 English letters ignoring case.</returns>
        public static char NextLetter(this Random rnd)
        {
            return rnd.NextChar(new char[] { rnd.NextChar('a', 'z'), rnd.NextChar('A', 'Z') });
        }

        /// <summary>
        /// Returns a random letter string (a string contains only letters, no other special characters) with customized length
        /// </summary>
        /// <param name="rnd">
        /// <param name="length">The length that the random string will be in
        /// <returns>A string contains only letters.</returns>
        public static string NextLetterString(this Random rnd, int length = 10)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append(rnd.NextLetter());
            }
            return sb.ToString();
        }
        #endregion

        #region Trace Listener extensions
        public static bool Contains(this TraceListenerCollection traceListeners, Type traceListenerTypeToCheck)
        {
            foreach (TraceListener traceListener in traceListeners)
            {
                if (traceListener.GetType().Equals(traceListenerTypeToCheck))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool Contains(this TraceListenerCollection traceListeners, Type traceListenerTypeToCheck, string traceListenerNameToCheck)
        {
            foreach (TraceListener traceListener in traceListeners)
            {
                if (traceListener.GetType().Equals(traceListenerTypeToCheck) && traceListener.Name.Equals(traceListenerNameToCheck, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
        #endregion
    }

    public class ExtendedMath
    {
        /// <summary>
        /// 将弧度值转换为角度值
        /// </summary>
        /// <param name="radian">弧度值.</param>
        /// <returns>角度值</returns>
        public static double Radian2Angle(double radian)
        {
            return radian * 180 / Math.PI;
        }

        public static double Distance(PointF p1, PointF p2)
        {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }
    }
}
