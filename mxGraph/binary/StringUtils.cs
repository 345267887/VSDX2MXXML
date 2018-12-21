using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxGraph.binary
{
    // Referenced classes of package org.apache.commons.lang:
    //            ArrayUtils, ObjectUtils, CharSetUtils, StringEscapeUtils, 
    //            WordUtils, CharUtils

    public class StringUtils
    {

        public StringUtils()
        {
        }

        public static bool isEmpty(string str)
        {
            return string.ReferenceEquals(str, null) || str.Length == 0;
        }

        public static bool isNotEmpty(string str)
        {
            return !isEmpty(str);
        }

        public static bool isBlank(string str)
        {
            int strLen;
            if (string.ReferenceEquals(str, null) || (strLen = str.Length) == 0)
            {
                return true;
            }
            for (int i = 0; i < strLen; i++)
            {
                if (!char.IsWhiteSpace(str[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool isNotBlank(string str)
        {
            return !isBlank(str);
        }

        /// @deprecated Method clean is deprecated 

        public static string clean(string str)
        {
            return !string.ReferenceEquals(str, null) ? str.Trim() : "";
        }

        public static string trim(string str)
        {
            return !string.ReferenceEquals(str, null) ? str.Trim() : null;
        }

        public static string trimToNull(string str)
        {
            string ts = trim(str);
            return isEmpty(ts) ? null : ts;
        }

        public static string trimToEmpty(string str)
        {
            return !string.ReferenceEquals(str, null) ? str.Trim() : "";
        }

        public static string strip(string str)
        {
            return strip(str, null);
        }

        public static string stripToNull(string str)
        {
            if (string.ReferenceEquals(str, null))
            {
                return null;
            }
            else
            {
                str = strip(str, null);
                return str.Length != 0 ? str : null;
            }
        }

        public static string stripToEmpty(string str)
        {
            return !string.ReferenceEquals(str, null) ? strip(str, null) : "";
        }

        public static string strip(string str, string stripChars)
        {
            if (isEmpty(str))
            {
                return str;
            }
            else
            {
                str = stripStart(str, stripChars);
                return stripEnd(str, stripChars);
            }
        }

        public static string stripStart(string str, string stripChars)
        {
            int strLen;
            if (string.ReferenceEquals(str, null) || (strLen = str.Length) == 0)
            {
                return str;
            }
            int start = 0;
            if (string.ReferenceEquals(stripChars, null))
            {
                for (; start != strLen && char.IsWhiteSpace(str[start]); start++)
                {
                    ;
                }
            }
            else
            {
                if (stripChars.Length == 0)
                {
                    return str;
                }
                for (; start != strLen && stripChars.IndexOf(str[start]) != -1; start++)
                {
                    ;
                }
            }
            return str.Substring(start);
        }

        public static string stripEnd(string str, string stripChars)
        {
            int end;
            if (string.ReferenceEquals(str, null) || (end = str.Length) == 0)
            {
                return str;
            }
            if (string.ReferenceEquals(stripChars, null))
            {
                for (; end != 0 && char.IsWhiteSpace(str[end - 1]); end--)
                {
                    ;
                }
            }
            else
            {
                if (stripChars.Length == 0)
                {
                    return str;
                }
                for (; end != 0 && stripChars.IndexOf(str[end - 1]) != -1; end--)
                {
                    ;
                }
            }
            return str.Substring(0, end);
        }

        public static string[] stripAll(string[] strs)
        {
            return stripAll(strs, null);
        }

        public static string[] stripAll(string[] strs, string stripChars)
        {
            int strsLen;
            if (strs == null || (strsLen = strs.Length) == 0)
            {
                return strs;
            }
            string[] newArr = new string[strsLen];
            for (int i = 0; i < strsLen; i++)
            {
                newArr[i] = strip(strs[i], stripChars);
            }

            return newArr;
        }

        public static bool Equals(string str1, string str2)
        {
            return !string.ReferenceEquals(str1, null) ? str1.Equals(str2) : string.ReferenceEquals(str2, null);
        }

        public static bool equalsIgnoreCase(string str1, string str2)
        {
            return !string.ReferenceEquals(str1, null) ? str1.Equals(str2, StringComparison.CurrentCultureIgnoreCase) : string.ReferenceEquals(str2, null);
        }

        public static int indexOf(string str, char searchChar)
        {
            if (isEmpty(str))
            {
                return -1;
            }
            else
            {
                return str.IndexOf(searchChar);
            }
        }

        public static int indexOf(string str, char searchChar, int startPos)
        {
            if (isEmpty(str))
            {
                return -1;
            }
            else
            {
                return str.IndexOf(searchChar, startPos);
            }
        }

        public static int indexOf(string str, string searchStr)
        {
            if (string.ReferenceEquals(str, null) || string.ReferenceEquals(searchStr, null))
            {
                return -1;
            }
            else
            {
                return str.IndexOf(searchStr, StringComparison.Ordinal);
            }
        }

        public static int ordinalIndexOf(string str, string searchStr, int ordinal)
        {
            return ordinalIndexOf(str, searchStr, ordinal, false);
        }

        private static int ordinalIndexOf(string str, string searchStr, int ordinal, bool lastIndex)
        {
            if (string.ReferenceEquals(str, null) || string.ReferenceEquals(searchStr, null) || ordinal <= 0)
            {
                return -1;
            }
            if (searchStr.Length == 0)
            {
                return lastIndex ? str.Length : 0;
            }
            int found = 0;
            int index = lastIndex ? str.Length : -1;
            do
            {
                if (lastIndex)
                {
                    index = str.LastIndexOf(searchStr, index - 1, StringComparison.Ordinal);
                }
                else
                {
                    index = str.IndexOf(searchStr, index + 1, StringComparison.Ordinal);
                }
                if (index < 0)
                {
                    return index;
                }
            } while (++found < ordinal);
            return index;
        }

        public static int indexOf(string str, string searchStr, int startPos)
        {
            if (string.ReferenceEquals(str, null) || string.ReferenceEquals(searchStr, null))
            {
                return -1;
            }
            if (searchStr.Length == 0 && startPos >= str.Length)
            {
                return str.Length;
            }
            else
            {
                return str.IndexOf(searchStr, startPos, StringComparison.Ordinal);
            }
        }
        

        public static int lastIndexOf(string str, char searchChar)
        {
            if (isEmpty(str))
            {
                return -1;
            }
            else
            {
                return str.LastIndexOf(searchChar);
            }
        }

        public static int lastIndexOf(string str, char searchChar, int startPos)
        {
            if (isEmpty(str))
            {
                return -1;
            }
            else
            {
                return str.LastIndexOf(searchChar, startPos);
            }
        }

        public static int lastIndexOf(string str, string searchStr)
        {
            if (string.ReferenceEquals(str, null) || string.ReferenceEquals(searchStr, null))
            {
                return -1;
            }
            else
            {
                return str.LastIndexOf(searchStr, StringComparison.Ordinal);
            }
        }

        public static int lastOrdinalIndexOf(string str, string searchStr, int ordinal)
        {
            return ordinalIndexOf(str, searchStr, ordinal, true);
        }

        public static int lastIndexOf(string str, string searchStr, int startPos)
        {
            if (string.ReferenceEquals(str, null) || string.ReferenceEquals(searchStr, null))
            {
                return -1;
            }
            else
            {
                return str.LastIndexOf(searchStr, startPos, StringComparison.Ordinal);
            }
        }

        public static bool contains(string str, char searchChar)
        {
            if (isEmpty(str))
            {
                return false;
            }
            else
            {
                return str.IndexOf(searchChar) >= 0;
            }
        }

        public static bool contains(string str, string searchStr)
        {
            if (string.ReferenceEquals(str, null) || string.ReferenceEquals(searchStr, null))
            {
                return false;
            }
            else
            {
                return str.IndexOf(searchStr, StringComparison.Ordinal) >= 0;
            }
        }
        

        public static int indexOfAny(string str, char[] searchChars)
        {
            if (isEmpty(str) || (searchChars)==null|| searchChars.Length==0)
            {
                return -1;
            }
            for (int i = 0; i < str.Length; i++)
            {
                char ch = str[i];
                for (int j = 0; j < searchChars.Length; j++)
                {
                    if (searchChars[j] == ch)
                    {
                        return i;
                    }
                }

            }

            return -1;
        }

        public static int indexOfAny(string str, string searchChars)
        {
            if (isEmpty(str) || isEmpty(searchChars))
            {
                return -1;
            }
            else
            {
                return indexOfAny(str, searchChars.ToCharArray());
            }
        }

        public static bool containsAny(string str, char[] searchChars)
        {
            if (string.ReferenceEquals(str, null) || str.Length == 0 || searchChars == null || searchChars.Length == 0)
            {
                return false;
            }
            for (int i = 0; i < str.Length; i++)
            {
                char ch = str[i];
                for (int j = 0; j < searchChars.Length; j++)
                {
                    if (searchChars[j] == ch)
                    {
                        return true;
                    }
                }

            }

            return false;
        }

        public static bool containsAny(string str, string searchChars)
        {
            if (string.ReferenceEquals(searchChars, null))
            {
                return false;
            }
            else
            {
                return containsAny(str, searchChars.ToCharArray());
            }
        }

        public static int indexOfAnyBut(string str, char[] searchChars)
        {
            if (isEmpty(str) || (searchChars)==null|| searchChars.Length>0)
            {
                return -1;
            }
            int i = 0;
            do
            {
                {
                    if (i >= str.Length)
                    {
                        goto label0Break;
                    }
                    char ch = str[i];
                    for (int j = 0; j < searchChars.Length; j++)
                    {
                        if (searchChars[j] == ch)
                        {
                            goto label1Break;
                        }
                    }

                    return i;
                }
                label1Break:
                i++;
            } while (true);
            label0Continue:;
            label0Break:
            return -1;
        }

        public static int indexOfAnyBut(string str, string searchChars)
        {
            if (isEmpty(str) || isEmpty(searchChars))
            {
                return -1;
            }
            for (int i = 0; i < str.Length; i++)
            {
                if (searchChars.IndexOf(str[i]) < 0)
                {
                    return i;
                }
            }

            return -1;
        }

        public static bool containsOnly(string str, char[] valid)
        {
            if (valid == null || string.ReferenceEquals(str, null))
            {
                return false;
            }
            if (str.Length == 0)
            {
                return true;
            }
            if (valid.Length == 0)
            {
                return false;
            }
            else
            {
                return indexOfAnyBut(str, valid) == -1;
            }
        }

        public static bool containsOnly(string str, string validChars)
        {
            if (string.ReferenceEquals(str, null) || string.ReferenceEquals(validChars, null))
            {
                return false;
            }
            else
            {
                return containsOnly(str, validChars.ToCharArray());
            }
        }

        public static bool containsNone(string str, char[] invalidChars)
        {
            if (string.ReferenceEquals(str, null) || invalidChars == null)
            {
                return true;
            }
            int strSize = str.Length;
            int validSize = invalidChars.Length;
            for (int i = 0; i < strSize; i++)
            {
                char ch = str[i];
                for (int j = 0; j < validSize; j++)
                {
                    if (invalidChars[j] == ch)
                    {
                        return false;
                    }
                }

            }

            return true;
        }

        public static bool containsNone(string str, string invalidChars)
        {
            if (string.ReferenceEquals(str, null) || string.ReferenceEquals(invalidChars, null))
            {
                return true;
            }
            else
            {
                return containsNone(str, invalidChars.ToCharArray());
            }
        }

        public static int indexOfAny(string str, string[] searchStrs)
        {
            if (string.ReferenceEquals(str, null) || searchStrs == null)
            {
                return -1;
            }
            int sz = searchStrs.Length;
            int ret = 2147483647;
            int tmp = 0;
            for (int i = 0; i < sz; i++)
            {
                string search = searchStrs[i];
                if (string.ReferenceEquals(search, null))
                {
                    continue;
                }
                tmp = str.IndexOf(search, StringComparison.Ordinal);
                if (tmp != -1 && tmp < ret)
                {
                    ret = tmp;
                }
            }

            return ret != 2147483647 ? ret : -1;
        }

        public static int lastIndexOfAny(string str, string[] searchStrs)
        {
            if (string.ReferenceEquals(str, null) || searchStrs == null)
            {
                return -1;
            }
            int sz = searchStrs.Length;
            int ret = -1;
            int tmp = 0;
            for (int i = 0; i < sz; i++)
            {
                string search = searchStrs[i];
                if (string.ReferenceEquals(search, null))
                {
                    continue;
                }
                tmp = str.LastIndexOf(search, StringComparison.Ordinal);
                if (tmp > ret)
                {
                    ret = tmp;
                }
            }

            return ret;
        }

        public static string substring(string str, int start)
        {
            if (string.ReferenceEquals(str, null))
            {
                return null;
            }
            if (start < 0)
            {
                start = str.Length + start;
            }
            if (start < 0)
            {
                start = 0;
            }
            if (start > str.Length)
            {
                return "";
            }
            else
            {
                return str.Substring(start);
            }
        }

        public static string substring(string str, int start, int end)
        {
            if (string.ReferenceEquals(str, null))
            {
                return null;
            }
            if (end < 0)
            {
                end = str.Length + end;
            }
            if (start < 0)
            {
                start = str.Length + start;
            }
            if (end > str.Length)
            {
                end = str.Length;
            }
            if (start > end)
            {
                return "";
            }
            if (start < 0)
            {
                start = 0;
            }
            if (end < 0)
            {
                end = 0;
            }
            return str.Substring(start, end - start);
        }

        public static string left(string str, int len)
        {
            if (string.ReferenceEquals(str, null))
            {
                return null;
            }
            if (len < 0)
            {
                return "";
            }
            if (str.Length <= len)
            {
                return str;
            }
            else
            {
                return str.Substring(0, len);
            }
        }

        public static string right(string str, int len)
        {
            if (string.ReferenceEquals(str, null))
            {
                return null;
            }
            if (len < 0)
            {
                return "";
            }
            if (str.Length <= len)
            {
                return str;
            }
            else
            {
                return str.Substring(str.Length - len);
            }
        }

        public static string mid(string str, int pos, int len)
        {
            if (string.ReferenceEquals(str, null))
            {
                return null;
            }
            if (len < 0 || pos > str.Length)
            {
                return "";
            }
            if (pos < 0)
            {
                pos = 0;
            }
            if (str.Length <= pos + len)
            {
                return str.Substring(pos);
            }
            else
            {
                return str.Substring(pos, len);
            }
        }

        public static string substringBefore(string str, string separator)
        {
            if (isEmpty(str) || string.ReferenceEquals(separator, null))
            {
                return str;
            }
            if (separator.Length == 0)
            {
                return "";
            }
            int pos = str.IndexOf(separator, StringComparison.Ordinal);
            if (pos == -1)
            {
                return str;
            }
            else
            {
                return str.Substring(0, pos);
            }
        }

        public static string substringAfter(string str, string separator)
        {
            if (isEmpty(str))
            {
                return str;
            }
            if (string.ReferenceEquals(separator, null))
            {
                return "";
            }
            int pos = str.IndexOf(separator, StringComparison.Ordinal);
            if (pos == -1)
            {
                return "";
            }
            else
            {
                return str.Substring(pos + separator.Length);
            }
        }

        public static string substringBeforeLast(string str, string separator)
        {
            if (isEmpty(str) || isEmpty(separator))
            {
                return str;
            }
            int pos = str.LastIndexOf(separator, StringComparison.Ordinal);
            if (pos == -1)
            {
                return str;
            }
            else
            {
                return str.Substring(0, pos);
            }
        }

        public static string substringAfterLast(string str, string separator)
        {
            if (isEmpty(str))
            {
                return str;
            }
            if (isEmpty(separator))
            {
                return "";
            }
            int pos = str.LastIndexOf(separator, StringComparison.Ordinal);
            if (pos == -1 || pos == str.Length - separator.Length)
            {
                return "";
            }
            else
            {
                return str.Substring(pos + separator.Length);
            }
        }

        public static string substringBetween(string str, string tag)
        {
            return substringBetween(str, tag, tag);
        }

        public static string substringBetween(string str, string open, string close)
        {
            if (string.ReferenceEquals(str, null) || string.ReferenceEquals(open, null) || string.ReferenceEquals(close, null))
            {
                return null;
            }
            int start = str.IndexOf(open, StringComparison.Ordinal);
            if (start != -1)
            {
                int end = str.IndexOf(close, start + open.Length, StringComparison.Ordinal);
                if (end != -1)
                {
                    return StringHelperClass.SubstringSpecial(str, start + open.Length, end);
                }
            }
            return null;
        }

        public static string[] substringsBetween(string str, string open, string close)
        {
            if (string.ReferenceEquals(str, null) || isEmpty(open) || isEmpty(close))
            {
                return null;
            }
            int strLen = str.Length;
            if (strLen == 0)
            {
                return new string[] { };
            }
            int closeLen = close.Length;
            int openLen = open.Length;
            List<string> list = new List<string>();
            int pos = 0;
            do
            {
                if (pos >= strLen - closeLen)
                {
                    break;
                }
                int start = str.IndexOf(open, pos, StringComparison.Ordinal);
                if (start < 0)
                {
                    break;
                }
                start += openLen;
                int end = str.IndexOf(close, start, StringComparison.Ordinal);
                if (end < 0)
                {
                    break;
                }
                list.Add(str.Substring(start, end - start));
                pos = end + closeLen;
            } while (true);
            if (list.Count == 0)
            {
                return null;
            }
            else
            {
                return list.ToArray();//(string[])(string[])list.ToArray(typeof(string));
            }
        }

        /// @deprecated Method getNestedString is deprecated 

        public static string getNestedString(string str, string tag)
        {
            return substringBetween(str, tag, tag);
        }

        /// @deprecated Method getNestedString is deprecated 

        public static string getNestedString(string str, string open, string close)
        {
            return substringBetween(str, open, close);
        }

        public static string[] split(string str)
        {
            return split(str, null, -1);
        }

        public static string[] split(string str, char separatorChar)
        {
            return splitWorker(str, separatorChar, false);
        }

        public static string[] split(string str, string separatorChars)
        {
            return splitWorker(str, separatorChars, -1, false);
        }

        public static string[] split(string str, string separatorChars, int max)
        {
            return splitWorker(str, separatorChars, max, false);
        }

        public static string[] splitByWholeSeparator(string str, string separator)
        {
            return splitByWholeSeparatorWorker(str, separator, -1, false);
        }

        public static string[] splitByWholeSeparator(string str, string separator, int max)
        {
            return splitByWholeSeparatorWorker(str, separator, max, false);
        }

        public static string[] splitByWholeSeparatorPreserveAllTokens(string str, string separator)
        {
            return splitByWholeSeparatorWorker(str, separator, -1, true);
        }

        public static string[] splitByWholeSeparatorPreserveAllTokens(string str, string separator, int max)
        {
            return splitByWholeSeparatorWorker(str, separator, max, true);
        }

        private static string[] splitByWholeSeparatorWorker(string str, string separator, int max, bool preserveAllTokens)
        {
            if (string.ReferenceEquals(str, null))
            {
                return null;
            }
            int len = str.Length;
            if (len == 0)
            {
                return new string[] { };
            }
            if (string.ReferenceEquals(separator, null) || "".Equals(separator))
            {
                return splitWorker(str, null, max, preserveAllTokens);
            }
            int separatorLength = separator.Length;
            ArrayList substrings = new ArrayList();
            int numberOfSubstrings = 0;
            int beg = 0;
            for (int end = 0; end < len;)
            {
                end = str.IndexOf(separator, beg, StringComparison.Ordinal);
                if (end > -1)
                {
                    if (end > beg)
                    {
                        if (++numberOfSubstrings == max)
                        {
                            end = len;
                            substrings.Add(str.Substring(beg));
                        }
                        else
                        {
                            substrings.Add(str.Substring(beg, end - beg));
                            beg = end + separatorLength;
                        }
                    }
                    else
                    {
                        if (preserveAllTokens)
                        {
                            if (++numberOfSubstrings == max)
                            {
                                end = len;
                                substrings.Add(str.Substring(beg));
                            }
                            else
                            {
                                substrings.Add("");
                            }
                        }
                        beg = end + separatorLength;
                    }
                }
                else
                {
                    substrings.Add(str.Substring(beg));
                    end = len;
                }
            }

            return (string[])(string[])substrings.ToArray(typeof(string));
        }

        public static string[] splitPreserveAllTokens(string str)
        {
            return splitWorker(str, null, -1, true);
        }

        public static string[] splitPreserveAllTokens(string str, char separatorChar)
        {
            return splitWorker(str, separatorChar, true);
        }

        private static string[] splitWorker(string str, char separatorChar, bool preserveAllTokens)
        {
            if (string.ReferenceEquals(str, null))
            {
                return null;
            }
            int len = str.Length;
            if (len == 0)
            {
                return new string[] { };
            }
            List<string> list = new List<string>();
            int i = 0;
            int start = 0;
            bool match = false;
            bool lastMatch = false;
            while (i < len)
            {
                if (str[i] == separatorChar)
                {
                    if (match || preserveAllTokens)
                    {
                        list.Add(str.Substring(start, i - start));
                        match = false;
                        lastMatch = true;
                    }
                    start = ++i;
                }
                else
                {
                    lastMatch = false;
                    match = true;
                    i++;
                }
            }
            if (match || preserveAllTokens && lastMatch)
            {
                list.Add(str.Substring(start, i - start));
            }
            return list.ToArray();
        }

        public static string[] splitPreserveAllTokens(string str, string separatorChars)
        {
            return splitWorker(str, separatorChars, -1, true);
        }

        public static string[] splitPreserveAllTokens(string str, string separatorChars, int max)
        {
            return splitWorker(str, separatorChars, max, true);
        }

        private static string[] splitWorker(string str, string separatorChars, int max, bool preserveAllTokens)
        {
            if (string.ReferenceEquals(str, null))
            {
                return null;
            }
            int len = str.Length;
            if (len == 0)
            {
                return new string[] { };
            }
            List<string> list = new List<string>();
            int sizePlus1 = 1;
            int i = 0;
            int start = 0;
            bool match = false;
            bool lastMatch = false;
            if (string.ReferenceEquals(separatorChars, null))
            {
                while (i < len)
                {
                    if (char.IsWhiteSpace(str[i]))
                    {
                        if (match || preserveAllTokens)
                        {
                            lastMatch = true;
                            if (sizePlus1++ == max)
                            {
                                i = len;
                                lastMatch = false;
                            }
                            list.Add(str.Substring(start, i - start));
                            match = false;
                        }
                        start = ++i;
                    }
                    else
                    {
                        lastMatch = false;
                        match = true;
                        i++;
                    }
                }
            }
            else if (separatorChars.Length == 1)
            {
                char sep = separatorChars[0];
                while (i < len)
                {
                    if (str[i] == sep)
                    {
                        if (match || preserveAllTokens)
                        {
                            lastMatch = true;
                            if (sizePlus1++ == max)
                            {
                                i = len;
                                lastMatch = false;
                            }
                            list.Add(str.Substring(start, i - start));
                            match = false;
                        }
                        start = ++i;
                    }
                    else
                    {
                        lastMatch = false;
                        match = true;
                        i++;
                    }
                }
            }
            else
            {
                while (i < len)
                {
                    if (separatorChars.IndexOf(str[i]) >= 0)
                    {
                        if (match || preserveAllTokens)
                        {
                            lastMatch = true;
                            if (sizePlus1++ == max)
                            {
                                i = len;
                                lastMatch = false;
                            }
                            list.Add(str.Substring(start, i - start));
                            match = false;
                        }
                        start = ++i;
                    }
                    else
                    {
                        lastMatch = false;
                        match = true;
                        i++;
                    }
                }
            }
            if (match || preserveAllTokens && lastMatch)
            {
                list.Add(str.Substring(start, i - start));
            }
            return list.ToArray();
        }
        

        /// @deprecated Method concatenate is deprecated 

        public static string concatenate(object[] array)
        {
            return join(array, ((string)(null)));
        }

        public static string join(object[] array)
        {
            return join(array, ((string)(null)));
        }

        public static string join(object[] array, char separator)
        {
            if (array == null)
            {
                return null;
            }
            else
            {
                return join(array, separator, 0, array.Length);
            }
        }

        public static string join(object[] array, char separator, int startIndex, int endIndex)
        {
            if (array == null)
            {
                return null;
            }
            int bufSize = endIndex - startIndex;
            if (bufSize <= 0)
            {
                return "";
            }
            bufSize *= (array[startIndex] != null ? array[startIndex].ToString().Length : 16) + 1;
            StringBuilder buf = new StringBuilder(bufSize);
            for (int i = startIndex; i < endIndex; i++)
            {
                if (i > startIndex)
                {
                    buf.Append(separator);
                }
                if (array[i] != null)
                {
                    buf.Append(array[i]);
                }
            }

            return buf.ToString();
        }

        public static string join(object[] array, string separator)
        {
            if (array == null)
            {
                return null;
            }
            else
            {
                return join(array, separator, 0, array.Length);
            }
        }

        public static string join(object[] array, string separator, int startIndex, int endIndex)
        {
            if (array == null)
            {
                return null;
            }
            if (string.ReferenceEquals(separator, null))
            {
                separator = "";
            }
            int bufSize = endIndex - startIndex;
            if (bufSize <= 0)
            {
                return "";
            }
            bufSize *= (array[startIndex] != null ? array[startIndex].ToString().Length : 16) + separator.Length;
            StringBuilder buf = new StringBuilder(bufSize);
            for (int i = startIndex; i < endIndex; i++)
            {
                if (i > startIndex)
                {
                    buf.Append(separator);
                }
                if (array[i] != null)
                {
                    buf.Append(array[i]);
                }
            }

            return buf.ToString();
        }
        

        public static string deleteWhitespace(string str)
        {
            if (isEmpty(str))
            {
                return str;
            }
            int sz = str.Length;
            char[] chs = new char[sz];
            int count = 0;
            for (int i = 0; i < sz; i++)
            {
                if (!char.IsWhiteSpace(str[i]))
                {
                    chs[count++] = str[i];
                }
            }

            if (count == sz)
            {
                return str;
            }
            else
            {
                return new string(chs, 0, count);
            }
        }

        public static string removeStart(string str, string remove)
        {
            if (isEmpty(str) || isEmpty(remove))
            {
                return str;
            }
            if (str.StartsWith(remove, StringComparison.Ordinal))
            {
                return str.Substring(remove.Length);
            }
            else
            {
                return str;
            }
        }
        
        public static string removeEnd(string str, string remove)
        {
            if (isEmpty(str) || isEmpty(remove))
            {
                return str;
            }
            if (str.EndsWith(remove, StringComparison.Ordinal))
            {
                return str.Substring(0, str.Length - remove.Length);
            }
            else
            {
                return str;
            }
        }
        

        public static string remove(string str, string remove)
        {
            if (isEmpty(str) || isEmpty(remove))
            {
                return str;
            }
            else
            {
                return replace(str, remove, "", -1);
            }
        }

        public static string remove(string str, char remove)
        {
            if (isEmpty(str) || str.IndexOf(remove) == -1)
            {
                return str;
            }
            char[] chars = str.ToCharArray();
            int pos = 0;
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] != remove)
                {
                    chars[pos++] = chars[i];
                }
            }

            return new string(chars, 0, pos);
        }

        public static string replaceOnce(string text, string searchString, string replacement)
        {
            return replace(text, searchString, replacement, 1);
        }

        public static string replace(string text, string searchString, string replacement)
        {
            return replace(text, searchString, replacement, -1);
        }

        public static string replace(string text, string searchString, string replacement, int max)
        {
            if (isEmpty(text) || isEmpty(searchString) || string.ReferenceEquals(replacement, null) || max == 0)
            {
                return text;
            }
            int start = 0;
            int end = text.IndexOf(searchString, start, StringComparison.Ordinal);
            if (end == -1)
            {
                return text;
            }
            int replLength = searchString.Length;
            int increase = replacement.Length - replLength;
            increase = increase >= 0 ? increase : 0;
            increase *= max >= 0 ? max <= 64 ? max : 64 : 16;
            StringBuilder buf = new StringBuilder(text.Length + increase);
            do
            {
                if (end == -1)
                {
                    break;
                }
                buf.Append(text.Substring(start, end - start)).Append(replacement);
                start = end + replLength;
                if (--max == 0)
                {
                    break;
                }
                end = text.IndexOf(searchString, start, StringComparison.Ordinal);
            } while (true);
            buf.Append(text.Substring(start));
            return buf.ToString();
        }

        public static string replaceEach(string text, string[] searchList, string[] replacementList)
        {
            return replaceEach(text, searchList, replacementList, false, 0);
        }

        public static string replaceEachRepeatedly(string text, string[] searchList, string[] replacementList)
        {
            int timeToLive = searchList != null ? searchList.Length : 0;
            return replaceEach(text, searchList, replacementList, true, timeToLive);
        }

        private static string replaceEach(string text, string[] searchList, string[] replacementList, bool repeat, int timeToLive)
        {
            if (string.ReferenceEquals(text, null) || text.Length == 0 || searchList == null || searchList.Length == 0 || replacementList == null || replacementList.Length == 0)
            {
                return text;
            }
            if (timeToLive < 0)
            {
                throw new System.InvalidOperationException("TimeToLive of " + timeToLive + " is less than 0: " + text);
            }
            int searchLength = searchList.Length;
            int replacementLength = replacementList.Length;
            if (searchLength != replacementLength)
            {
                throw new System.ArgumentException("Search and Replace array lengths don't match: " + searchLength + " vs " + replacementLength);
            }
            bool[] noMoreMatchesForReplIndex = new bool[searchLength];
            int textIndex = -1;
            int replaceIndex = -1;
            int tempIndex = -1;
            for (int i = 0; i < searchLength; i++)
            {
                if (noMoreMatchesForReplIndex[i] || string.ReferenceEquals(searchList[i], null) || searchList[i].Length == 0 || string.ReferenceEquals(replacementList[i], null))
                {
                    continue;
                }
                tempIndex = text.IndexOf(searchList[i], StringComparison.Ordinal);
                if (tempIndex == -1)
                {
                    noMoreMatchesForReplIndex[i] = true;
                    continue;
                }
                if (textIndex == -1 || tempIndex < textIndex)
                {
                    textIndex = tempIndex;
                    replaceIndex = i;
                }
            }

            if (textIndex == -1)
            {
                return text;
            }
            int start = 0;
            int increase = 0;
            for (int i = 0; i < searchList.Length; i++)
            {
                if (string.ReferenceEquals(searchList[i], null) || string.ReferenceEquals(replacementList[i], null))
                {
                    continue;
                }
                int greater = replacementList[i].Length - searchList[i].Length;
                if (greater > 0)
                {
                    increase += 3 * greater;
                }
            }

            increase = Math.Min(increase, text.Length / 5);
            StringBuilder buf = new StringBuilder(text.Length + increase);
            while (textIndex != -1)
            {
                int i;
                for (i = start; i < textIndex; i++)
                {
                    buf.Append(text[i]);
                }

                buf.Append(replacementList[replaceIndex]);
                start = textIndex + searchList[replaceIndex].Length;
                textIndex = -1;
                replaceIndex = -1;
                tempIndex = -1;
                i = 0;
                while (i < searchLength)
                {
                    if (!noMoreMatchesForReplIndex[i] && !string.ReferenceEquals(searchList[i], null) && searchList[i].Length != 0 && !string.ReferenceEquals(replacementList[i], null))
                    {
                        tempIndex = text.IndexOf(searchList[i], start, StringComparison.Ordinal);
                        if (tempIndex == -1)
                        {
                            noMoreMatchesForReplIndex[i] = true;
                        }
                        else if (textIndex == -1 || tempIndex < textIndex)
                        {
                            textIndex = tempIndex;
                            replaceIndex = i;
                        }
                    }
                    i++;
                }
            }
            int textLength = text.Length;
            for (int i = start; i < textLength; i++)
            {
                buf.Append(text[i]);
            }

            string result = buf.ToString();
            if (!repeat)
            {
                return result;
            }
            else
            {
                return replaceEach(result, searchList, replacementList, repeat, timeToLive - 1);
            }
        }

        public static string replaceChars(string str, char searchChar, char replaceChar)
        {
            if (string.ReferenceEquals(str, null))
            {
                return null;
            }
            else
            {
                return str.Replace(searchChar, replaceChar);
            }
        }

        public static string replaceChars(string str, string searchChars, string replaceChars)
        {
            if (isEmpty(str) || isEmpty(searchChars))
            {
                return str;
            }
            if (string.ReferenceEquals(replaceChars, null))
            {
                replaceChars = "";
            }
            bool modified = false;
            int replaceCharsLength = replaceChars.Length;
            int strLength = str.Length;
            StringBuilder buf = new StringBuilder(strLength);
            for (int i = 0; i < strLength; i++)
            {
                char ch = str[i];
                int index = searchChars.IndexOf(ch);
                if (index >= 0)
                {
                    modified = true;
                    if (index < replaceCharsLength)
                    {
                        buf.Append(replaceChars[index]);
                    }
                }
                else
                {
                    buf.Append(ch);
                }
            }

            if (modified)
            {
                return buf.ToString();
            }
            else
            {
                return str;
            }
        }

        /// @deprecated Method overlayString is deprecated 

        public static string overlayString(string text, string overlay, int start, int end)
        {
            return (new StringBuilder(((start + overlay.Length + text.Length) - end) + 1)).Append(text.Substring(0, start)).Append(overlay).Append(text.Substring(end)).ToString();
        }

        public static string overlay(string str, string overlay, int start, int end)
        {
            if (string.ReferenceEquals(str, null))
            {
                return null;
            }
            if (string.ReferenceEquals(overlay, null))
            {
                overlay = "";
            }
            int len = str.Length;
            if (start < 0)
            {
                start = 0;
            }
            if (start > len)
            {
                start = len;
            }
            if (end < 0)
            {
                end = 0;
            }
            if (end > len)
            {
                end = len;
            }
            if (start > end)
            {
                int temp = start;
                start = end;
                end = temp;
            }
            return (new StringBuilder(((len + start) - end) + overlay.Length + 1)).Append(str.Substring(0, start)).Append(overlay).Append(str.Substring(end)).ToString();
        }

        public static string chomp(string str)
        {
            if (isEmpty(str))
            {
                return str;
            }
            if (str.Length == 1)
            {
                char ch = str[0];
                if (ch == '\r' || ch == '\n')
                {
                    return "";
                }
                else
                {
                    return str;
                }
            }
            int lastIdx = str.Length - 1;
            char last = str[lastIdx];
            if (last == '\n')
            {
                if (str[lastIdx - 1] == '\r')
                {
                    lastIdx--;
                }
            }
            else if (last != '\r')
            {
                lastIdx++;
            }
            return str.Substring(0, lastIdx);
        }

        public static string chomp(string str, string separator)
        {
            if (isEmpty(str) || string.ReferenceEquals(separator, null))
            {
                return str;
            }
            if (str.EndsWith(separator, StringComparison.Ordinal))
            {
                return str.Substring(0, str.Length - separator.Length);
            }
            else
            {
                return str;
            }
        }

        /// @deprecated Method chompLast is deprecated 

        public static string chompLast(string str)
        {
            return chompLast(str, "\n");
        }

        /// @deprecated Method chompLast is deprecated 

        public static string chompLast(string str, string sep)
        {
            if (str.Length == 0)
            {
                return str;
            }
            string sub = str.Substring(str.Length - sep.Length);
            if (sep.Equals(sub))
            {
                return str.Substring(0, str.Length - sep.Length);
            }
            else
            {
                return str;
            }
        }

        /// @deprecated Method getChomp is deprecated 

        public static string getChomp(string str, string sep)
        {
            int idx = str.LastIndexOf(sep, StringComparison.Ordinal);
            if (idx == str.Length - sep.Length)
            {
                return sep;
            }
            if (idx != -1)
            {
                return str.Substring(idx);
            }
            else
            {
                return "";
            }
        }

        /// @deprecated Method prechomp is deprecated 

        public static string prechomp(string str, string sep)
        {
            int idx = str.IndexOf(sep, StringComparison.Ordinal);
            if (idx == -1)
            {
                return str;
            }
            else
            {
                return str.Substring(idx + sep.Length);
            }
        }

        /// @deprecated Method getPrechomp is deprecated 

        public static string getPrechomp(string str, string sep)
        {
            int idx = str.IndexOf(sep, StringComparison.Ordinal);
            if (idx == -1)
            {
                return "";
            }
            else
            {
                return str.Substring(0, idx + sep.Length);
            }
        }

        public static string chop(string str)
        {
            if (string.ReferenceEquals(str, null))
            {
                return null;
            }
            int strLen = str.Length;
            if (strLen < 2)
            {
                return "";
            }
            int lastIdx = strLen - 1;
            string ret = str.Substring(0, lastIdx);
            char last = str[lastIdx];
            if (last == '\n' && ret[lastIdx - 1] == '\r')
            {
                return ret.Substring(0, lastIdx - 1);
            }
            else
            {
                return ret;
            }
        }

        /// @deprecated Method chopNewline is deprecated 

        public static string chopNewline(string str)
        {
            int lastIdx = str.Length - 1;
            if (lastIdx <= 0)
            {
                return "";
            }
            char last = str[lastIdx];
            if (last == '\n')
            {
                if (str[lastIdx - 1] == '\r')
                {
                    lastIdx--;
                }
            }
            else
            {
                lastIdx++;
            }
            return str.Substring(0, lastIdx);
        }
        

        public static string repeat(string str, int repeat)
        {
            if (string.ReferenceEquals(str, null))
            {
                return null;
            }
            if (repeat <= 0)
            {
                return "";
            }
            int inputLength = str.Length;
            if (repeat == 1 || inputLength == 0)
            {
                return str;
            }
            if (inputLength == 1 && repeat <= 8192)
            {
                return padding(repeat, str[0]);
            }
            int outputLength = inputLength * repeat;
            switch (inputLength)
            {
                case 1: // '\001'
                    char ch = str[0];
                    char[] output1 = new char[outputLength];
                    for (int i = repeat - 1; i >= 0; i--)
                    {
                        output1[i] = ch;
                    }

                    return new string(output1);

                case 2: // '\002'
                    char ch0 = str[0];
                    char ch1 = str[1];
                    char[] output2 = new char[outputLength];
                    for (int i = repeat * 2 - 2; i >= 0; i--)
                    {
                        output2[i] = ch0;
                        output2[i + 1] = ch1;
                        i--;
                    }

                    return new string(output2);
            }
            StringBuilder buf = new StringBuilder(outputLength);
            for (int i = 0; i < repeat; i++)
            {
                buf.Append(str);
            }

            return buf.ToString();
        }

       

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: private static String padding(int repeat, char padChar) throws IndexOutOfBoundsException
        private static string padding(int repeat, char padChar)
        {
            if (repeat < 0)
            {
                throw new System.IndexOutOfRangeException("Cannot pad a negative amount: " + repeat);
            }
            char[] buf = new char[repeat];
            for (int i = 0; i < buf.Length; i++)
            {
                buf[i] = padChar;
            }

            return new string(buf);
        }

        public static string rightPad(string str, int size)
        {
            return rightPad(str, size, ' ');
        }

        public static string rightPad(string str, int size, char padChar)
        {
            if (string.ReferenceEquals(str, null))
            {
                return null;
            }
            int pads = size - str.Length;
            if (pads <= 0)
            {
                return str;
            }
            if (pads > 8192)
            {
                return rightPad(str, size, padChar.ToString());
            }
            else
            {
                return str + padding(pads, padChar);
            }
        }

        public static string rightPad(string str, int size, string padStr)
        {
            if (string.ReferenceEquals(str, null))
            {
                return null;
            }
            if (isEmpty(padStr))
            {
                padStr = " ";
            }
            int padLen = padStr.Length;
            int strLen = str.Length;
            int pads = size - strLen;
            if (pads <= 0)
            {
                return str;
            }
            if (padLen == 1 && pads <= 8192)
            {
                return rightPad(str, size, padStr[0]);
            }
            if (pads == padLen)
            {
                return str + padStr;
            }
            if (pads < padLen)
            {
                return str + padStr.Substring(0, pads);
            }
            char[] padding = new char[pads];
            char[] padChars = padStr.ToCharArray();
            for (int i = 0; i < pads; i++)
            {
                padding[i] = padChars[i % padLen];
            }

            return str + new string(padding);
        }

        public static string leftPad(string str, int size)
        {
            return leftPad(str, size, ' ');
        }

        public static string leftPad(string str, int size, char padChar)
        {
            if (string.ReferenceEquals(str, null))
            {
                return null;
            }
            int pads = size - str.Length;
            if (pads <= 0)
            {
                return str;
            }
            if (pads > 8192)
            {
                return leftPad(str, size, padChar.ToString());
            }
            else
            {
                return padding(pads, padChar) + str;
            }
        }

        public static string leftPad(string str, int size, string padStr)
        {
            if (string.ReferenceEquals(str, null))
            {
                return null;
            }
            if (isEmpty(padStr))
            {
                padStr = " ";
            }
            int padLen = padStr.Length;
            int strLen = str.Length;
            int pads = size - strLen;
            if (pads <= 0)
            {
                return str;
            }
            if (padLen == 1 && pads <= 8192)
            {
                return leftPad(str, size, padStr[0]);
            }
            if (pads == padLen)
            {
                return padStr + str;
            }
            if (pads < padLen)
            {
                return padStr.Substring(0, pads) + str;
            }
            char[] padding = new char[pads];
            char[] padChars = padStr.ToCharArray();
            for (int i = 0; i < pads; i++)
            {
                padding[i] = padChars[i % padLen];
            }

            return (new string(padding)) + str;
        }

        public static int length(string str)
        {
            return !string.ReferenceEquals(str, null) ? str.Length : 0;
        }

        public static string center(string str, int size)
        {
            return center(str, size, ' ');
        }

        public static string center(string str, int size, char padChar)
        {
            if (string.ReferenceEquals(str, null) || size <= 0)
            {
                return str;
            }
            int strLen = str.Length;
            int pads = size - strLen;
            if (pads <= 0)
            {
                return str;
            }
            else
            {
                str = leftPad(str, strLen + pads / 2, padChar);
                str = rightPad(str, size, padChar);
                return str;
            }
        }

        public static string center(string str, int size, string padStr)
        {
            if (string.ReferenceEquals(str, null) || size <= 0)
            {
                return str;
            }
            if (isEmpty(padStr))
            {
                padStr = " ";
            }
            int strLen = str.Length;
            int pads = size - strLen;
            if (pads <= 0)
            {
                return str;
            }
            else
            {
                str = leftPad(str, strLen + pads / 2, padStr);
                str = rightPad(str, size, padStr);
                return str;
            }
        }

        public static string upperCase(string str)
        {
            if (string.ReferenceEquals(str, null))
            {
                return null;
            }
            else
            {
                return str.ToUpper();
            }
        }

        
        public static string lowerCase(string str)
        {
            if (string.ReferenceEquals(str, null))
            {
                return null;
            }
            else
            {
                return str.ToLower();
            }
        }

       

       
        

        public static string uncapitalize(string str)
        {
            int strLen;
            if (string.ReferenceEquals(str, null) || (strLen = str.Length) == 0)
            {
                return str;
            }
            else
            {
                return (new StringBuilder(strLen)).Append(char.ToLower(str[0])).Append(str.Substring(1)).ToString();
            }
        }

        /// @deprecated Method uncapitalise is deprecated 

        public static string uncapitalise(string str)
        {
            return uncapitalize(str);
        }

        
        public static int countMatches(string str, string sub)
        {
            if (isEmpty(str) || isEmpty(sub))
            {
                return 0;
            }
            int count = 0;
            for (int idx = 0; (idx = str.IndexOf(sub, idx, StringComparison.Ordinal)) != -1; idx += sub.Length)
            {
                count++;
            }

            return count;
        }

        public static bool isAlpha(string str)
        {
            if (string.ReferenceEquals(str, null))
            {
                return false;
            }
            int sz = str.Length;
            for (int i = 0; i < sz; i++)
            {
                if (!char.IsLetter(str[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool isAlphaSpace(string str)
        {
            if (string.ReferenceEquals(str, null))
            {
                return false;
            }
            int sz = str.Length;
            for (int i = 0; i < sz; i++)
            {
                if (!char.IsLetter(str[i]) && str[i] != ' ')
                {
                    return false;
                }
            }

            return true;
        }

        public static bool isAlphanumeric(string str)
        {
            if (string.ReferenceEquals(str, null))
            {
                return false;
            }
            int sz = str.Length;
            for (int i = 0; i < sz; i++)
            {
                if (!char.IsLetterOrDigit(str[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool isAlphanumericSpace(string str)
        {
            if (string.ReferenceEquals(str, null))
            {
                return false;
            }
            int sz = str.Length;
            for (int i = 0; i < sz; i++)
            {
                if (!char.IsLetterOrDigit(str[i]) && str[i] != ' ')
                {
                    return false;
                }
            }

            return true;
        }

        

        public static bool isNumeric(string str)
        {
            if (string.ReferenceEquals(str, null))
            {
                return false;
            }
            int sz = str.Length;
            for (int i = 0; i < sz; i++)
            {
                if (!char.IsDigit(str[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool isNumericSpace(string str)
        {
            if (string.ReferenceEquals(str, null))
            {
                return false;
            }
            int sz = str.Length;
            for (int i = 0; i < sz; i++)
            {
                if (!char.IsDigit(str[i]) && str[i] != ' ')
                {
                    return false;
                }
            }

            return true;
        }

        public static bool isWhitespace(string str)
        {
            if (string.ReferenceEquals(str, null))
            {
                return false;
            }
            int sz = str.Length;
            for (int i = 0; i < sz; i++)
            {
                if (!char.IsWhiteSpace(str[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool isAllLowerCase(string str)
        {
            if (string.ReferenceEquals(str, null) || isEmpty(str))
            {
                return false;
            }
            int sz = str.Length;
            for (int i = 0; i < sz; i++)
            {
                if (!char.IsLower(str[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool isAllUpperCase(string str)
        {
            if (string.ReferenceEquals(str, null) || isEmpty(str))
            {
                return false;
            }
            int sz = str.Length;
            for (int i = 0; i < sz; i++)
            {
                if (!char.IsUpper(str[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static string defaultString(string str)
        {
            return !string.ReferenceEquals(str, null) ? str : "";
        }

        public static string defaultString(string str, string defaultStr)
        {
            return !string.ReferenceEquals(str, null) ? str : defaultStr;
        }

        public static string defaultIfEmpty(string str, string defaultStr)
        {
            return isEmpty(str) ? defaultStr : str;
        }

        

        public static string abbreviate(string str, int maxWidth)
        {
            return abbreviate(str, 0, maxWidth);
        }

        public static string abbreviate(string str, int offset, int maxWidth)
        {
            if (string.ReferenceEquals(str, null))
            {
                return null;
            }
            if (maxWidth < 4)
            {
                throw new System.ArgumentException("Minimum abbreviation width is 4");
            }
            if (str.Length <= maxWidth)
            {
                return str;
            }
            if (offset > str.Length)
            {
                offset = str.Length;
            }
            if (str.Length - offset < maxWidth - 3)
            {
                offset = str.Length - (maxWidth - 3);
            }
            if (offset <= 4)
            {
                return str.Substring(0, maxWidth - 3) + "...";
            }
            if (maxWidth < 7)
            {
                throw new System.ArgumentException("Minimum abbreviation width with offset is 7");
            }
            if (offset + (maxWidth - 3) < str.Length)
            {
                return "..." + abbreviate(str.Substring(offset), maxWidth - 3);
            }
            else
            {
                return "..." + str.Substring(str.Length - (maxWidth - 3));
            }
        }

        public static string abbreviateMiddle(string str, string middle, int length)
        {
            if (isEmpty(str) || isEmpty(middle))
            {
                return str;
            }
            if (length >= str.Length || length < middle.Length + 2)
            {
                return str;
            }
            else
            {
                int targetSting = length - middle.Length;
                int startOffset = targetSting / 2 + targetSting % 2;
                int endOffset = str.Length - targetSting / 2;
                StringBuilder builder = new StringBuilder(length);
                builder.Append(str.Substring(0, startOffset));
                builder.Append(middle);
                builder.Append(str.Substring(endOffset));
                return builder.ToString();
            }
        }

        public static string difference(string str1, string str2)
        {
            if (string.ReferenceEquals(str1, null))
            {
                return str2;
            }
            if (string.ReferenceEquals(str2, null))
            {
                return str1;
            }
            int at = indexOfDifference(str1, str2);
            if (at == -1)
            {
                return "";
            }
            else
            {
                return str2.Substring(at);
            }
        }

        public static int indexOfDifference(string str1, string str2)
        {
            if (string.ReferenceEquals(str1, str2))
            {
                return -1;
            }
            if (string.ReferenceEquals(str1, null) || string.ReferenceEquals(str2, null))
            {
                return 0;
            }
            int i;
            for (i = 0; i < str1.Length && i < str2.Length && str1[i] == str2[i]; i++)
            {
                ;
            }
            if (i < str2.Length || i < str1.Length)
            {
                return i;
            }
            else
            {
                return -1;
            }
        }

        public static int indexOfDifference(string[] strs)
        {
            if (strs == null || strs.Length <= 1)
            {
                return -1;
            }
            bool anyStringNull = false;
            bool allStringsNull = true;
            int arrayLen = strs.Length;
            int shortestStrLen = 2147483647;
            int longestStrLen = 0;
            for (int i = 0; i < arrayLen; i++)
            {
                if (string.ReferenceEquals(strs[i], null))
                {
                    anyStringNull = true;
                    shortestStrLen = 0;
                }
                else
                {
                    allStringsNull = false;
                    shortestStrLen = Math.Min(strs[i].Length, shortestStrLen);
                    longestStrLen = Math.Max(strs[i].Length, longestStrLen);
                }
            }

            if (allStringsNull || longestStrLen == 0 && !anyStringNull)
            {
                return -1;
            }
            if (shortestStrLen == 0)
            {
                return 0;
            }
            int firstDiff = -1;
            int stringPos = 0;
            do
            {
                if (stringPos >= shortestStrLen)
                {
                    break;
                }
                char comparisonChar = strs[0][stringPos];
                int arrayPos = 1;
                do
                {
                    if (arrayPos >= arrayLen)
                    {
                        break;
                    }
                    if (strs[arrayPos][stringPos] != comparisonChar)
                    {
                        firstDiff = stringPos;
                        break;
                    }
                    arrayPos++;
                } while (true);
                if (firstDiff != -1)
                {
                    break;
                }
                stringPos++;
            } while (true);
            if (firstDiff == -1 && shortestStrLen != longestStrLen)
            {
                return shortestStrLen;
            }
            else
            {
                return firstDiff;
            }
        }

        public static string getCommonPrefix(string[] strs)
        {
            if (strs == null || strs.Length == 0)
            {
                return "";
            }
            int smallestIndexOfDiff = indexOfDifference(strs);
            if (smallestIndexOfDiff == -1)
            {
                if (string.ReferenceEquals(strs[0], null))
                {
                    return "";
                }
                else
                {
                    return strs[0];
                }
            }
            if (smallestIndexOfDiff == 0)
            {
                return "";
            }
            else
            {
                return strs[0].Substring(0, smallestIndexOfDiff);
            }
        }

        public static int getLevenshteinDistance(string s, string t)
        {
            if (string.ReferenceEquals(s, null) || string.ReferenceEquals(t, null))
            {
                throw new System.ArgumentException("Strings must not be null");
            }
            int n = s.Length;
            int m = t.Length;
            if (n == 0)
            {
                return m;
            }
            if (m == 0)
            {
                return n;
            }
            if (n > m)
            {
                string tmp = s;
                s = t;
                t = tmp;
                n = m;
                m = t.Length;
            }
            int[] p = new int[n + 1];
            int[] d = new int[n + 1];
            for (int i = 0; i <= n; i++)
            {
                p[i] = i;
            }

            for (int j = 1; j <= m; j++)
            {
                char t_j = t[j - 1];
                d[0] = j;
                for (int i = 1; i <= n; i++)
                {
                    int cost = s[i - 1] != t_j ? 1 : 0;
                    d[i] = Math.Min(Math.Min(d[i - 1] + 1, p[i] + 1), p[i - 1] + cost);
                }

                int[] _d = p;
                p = d;
                d = _d;
            }

            return p[n];
        }
        
       
        public const string EMPTY = "";
        public const int INDEX_NOT_FOUND = -1;
        private const int PAD_LIMIT = 8192;
    }


    /*
		DECOMPILATION REPORT
	
		Decompiled from: D:\Work\ebookpro\trunk\ebookpro\WebRoot\WEB-INF\lib\commons-lang-2.5.jar
		Total time: 245 ms
		Jad reported messages/errors:
		Exit status: 0
		Caught exceptions:
	*/
}

//-------------------------------------------------------------------------------------------
//	Copyright © 2007 - 2016 Tangible Software Solutions Inc.
//	This class can be used by anyone provided that the copyright notice remains intact.
//
//	This class is used to convert some aspects of the Java String class.
//-------------------------------------------------------------------------------------------
internal static class StringHelperClass
{
    //----------------------------------------------------------------------------------
    //	This method replaces the Java String.substring method when 'start' is a
    //	method call or calculated value to ensure that 'start' is obtained just once.
    //----------------------------------------------------------------------------------
    internal static string SubstringSpecial(this string self, int start, int end)
    {
        return self.Substring(start, end - start);
    }

    //------------------------------------------------------------------------------------
    //	This method is used to replace calls to the 2-arg Java String.startsWith method.
    //------------------------------------------------------------------------------------
    internal static bool StartsWith(this string self, string prefix, int toffset)
    {
        return self.IndexOf(prefix, toffset, System.StringComparison.Ordinal) == toffset;
    }

    //------------------------------------------------------------------------------
    //	This method is used to replace most calls to the Java String.split method.
    //------------------------------------------------------------------------------
    internal static string[] Split(this string self, string regexDelimiter, bool trimTrailingEmptyStrings)
    {
        string[] splitArray = System.Text.RegularExpressions.Regex.Split(self, regexDelimiter);

        if (trimTrailingEmptyStrings)
        {
            if (splitArray.Length > 1)
            {
                for (int i = splitArray.Length; i > 0; i--)
                {
                    if (splitArray[i - 1].Length > 0)
                    {
                        if (i < splitArray.Length)
                            System.Array.Resize(ref splitArray, i);

                        break;
                    }
                }
            }
        }

        return splitArray;
    }

    //-----------------------------------------------------------------------------
    //	These methods are used to replace calls to some Java String constructors.
    //-----------------------------------------------------------------------------
    internal static string NewString(sbyte[] bytes)
    {
        return NewString(bytes, 0, bytes.Length);
    }
    internal static string NewString(sbyte[] bytes, int index, int count)
    {
        return System.Text.Encoding.UTF8.GetString((byte[])(object)bytes, index, count);
    }
    internal static string NewString(sbyte[] bytes, string encoding)
    {
        return NewString(bytes, 0, bytes.Length, encoding);
    }
    internal static string NewString(sbyte[] bytes, int index, int count, string encoding)
    {
        return System.Text.Encoding.GetEncoding(encoding).GetString((byte[])(object)bytes, index, count);
    }

    //--------------------------------------------------------------------------------
    //	These methods are used to replace calls to the Java String.getBytes methods.
    //--------------------------------------------------------------------------------
    internal static sbyte[] GetBytes(this string self)
    {
        return GetSBytesForEncoding(System.Text.Encoding.UTF8, self);
    }
    internal static sbyte[] GetBytes(this string self, System.Text.Encoding encoding)
    {
        return GetSBytesForEncoding(encoding, self);
    }
    internal static sbyte[] GetBytes(this string self, string encoding)
    {
        return GetSBytesForEncoding(System.Text.Encoding.GetEncoding(encoding), self);
    }
    private static sbyte[] GetSBytesForEncoding(System.Text.Encoding encoding, string s)
    {
        sbyte[] sbytes = new sbyte[encoding.GetByteCount(s)];
        encoding.GetBytes(s, 0, s.Length, (byte[])(object)sbytes, 0);
        return sbytes;
    }

}
