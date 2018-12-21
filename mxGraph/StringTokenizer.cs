using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxGraph
{
    public class StringTokenizer : IEnumerator<object>
    {
        private int currentPosition; //当前位置
        private int newPosition; //新位置
        private int maxPosition; //最大位置
        private string str; //操作的字符串
        private string delimiters; //分隔符
        private bool retDelims;
        private bool delimsChanged;
        private int maxDelimCodePoint;
        private bool hasSurrogates = false;
        private int[] delimiterCodePoints;

        public object Current => throw new NotImplementedException();

        private void setMaxDelimCodePoint()
        {
            if (string.ReferenceEquals(delimiters, null))
            {
                maxDelimCodePoint = 0;
                return;
            }
            int m = 0;
            int c;
            int count = 0;
            for (int i = 0; i < delimiters.Length; i += Common.charCount(c))
            {
                c = delimiters[i];
                char MIN_HIGH_SURROGATE = '\uD800';
                char MAX_LOW_SURROGATE = '\uDFFF';
                if (c >= MIN_HIGH_SURROGATE && c <= MAX_LOW_SURROGATE)
                {
                    c = char.ConvertToUtf32(delimiters, i);
                    hasSurrogates = true;
                }
                if (m < c)
                {
                    m = c;
                }
                count++;
            }
            maxDelimCodePoint = m;
            if (hasSurrogates)
            {
                delimiterCodePoints = new int[count];
                for (int i = 0, j = 0; i < count; i++, j += Common.charCount(c))
                {
                    c = char.ConvertToUtf32(delimiters, j);
                    delimiterCodePoints[i] = c;
                }
            }
        }
        public StringTokenizer(string str, string delim, bool returnDelims)
        {
            currentPosition = 0;
            newPosition = -1;
            delimsChanged = false;
            this.str = str;
            maxPosition = str.Length;
            delimiters = delim;
            retDelims = returnDelims;
            setMaxDelimCodePoint();
        }
        public StringTokenizer(string str, string delim) : this(str, delim, false)
        {
        }
        public StringTokenizer(string str) : this(str, " \t\n\r\f", false)
        {
        }
        /// <summary>
        ///跳过分隔符 </summary>
        private int skipDelimiters(int startPos)
        {
            if (string.ReferenceEquals(delimiters, null))
            {
                throw new System.NullReferenceException();
            }
            int position = startPos;
            while (!retDelims && position < maxPosition)
            {
                if (!hasSurrogates)
                {
                    char c = str[position];
                    if ((c > maxDelimCodePoint) || (delimiters.IndexOf(c) < 0))
                    {
                        break;
                    }
                    position++;
                }
                else
                {
                    int c = char.ConvertToUtf32(str, position);
                    if ((c > maxDelimCodePoint) || !isDelimiter(c))
                    {
                        break;
                    }
                    position += Common.charCount(c);
                }
            }
            return position;
        }
        /// <summary>
        ///从某个位置开始遍历token </summary>
        private int scanToken(int startPos)
        {
            int position = startPos;
            while (position < maxPosition)
            {
                if (!hasSurrogates)
                {
                    char c = str[position];
                    if ((c <= maxDelimCodePoint) && (delimiters.IndexOf(c) >= 0))
                    {
                        break;
                    }
                    position++;
                }
                else
                {
                    int c = char.ConvertToUtf32(str, position);
                    if ((c <= maxDelimCodePoint) && isDelimiter(c))
                    {
                        break;
                    }
                    position += Common.charCount(c);
                }
            }
            if (retDelims && (startPos == position))
            {
                if (!hasSurrogates)
                {
                    char c = str[position];
                    if ((c <= maxDelimCodePoint) && (delimiters.IndexOf(c) >= 0))
                    {
                        position++;
                    }
                }
                else
                {
                    int c = char.ConvertToUtf32(str, position);
                    if ((c <= maxDelimCodePoint) && isDelimiter(c))
                    {
                        position += Common.charCount(c);
                    }
                }
            }
            return position;
        }
        /// <summary>
        ///判断该位置字符是否是分隔符 </summary>
        private bool isDelimiter(int codePoint)
        {
            for (int i = 0; i < delimiterCodePoints.Length; i++)
            {
                if (delimiterCodePoints[i] == codePoint)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        ///是否还有token </summary>
        public virtual bool hasMoreTokens()
        {
            newPosition = skipDelimiters(currentPosition);
            return (newPosition < maxPosition);
        }
        public virtual string nextToken()
        {
            currentPosition = (newPosition >= 0 && !delimsChanged) ? newPosition : skipDelimiters(currentPosition);
            delimsChanged = false;
            newPosition = -1;
            if (currentPosition >= maxPosition)
            {
                throw new Exception("NoSuchElementException");
            }
            int start = currentPosition;
            currentPosition = scanToken(currentPosition);
            return str.Substring(start, currentPosition - start);
        }
        /// <summary>
        ///下一个token </summary>
        public virtual string nextToken(string delim)
        {
            delimiters = delim;
            delimsChanged = true;
            setMaxDelimCodePoint();
            return nextToken();
        }
        /// <summary>
        ///是否还有元素 </summary>
        public virtual bool hasMoreElements()
        {
            return hasMoreTokens();
        }
        /// <summary>
        ///下一个元素 </summary>
        public virtual object nextElement()
        {
            return nextToken();
        }
        /// <summary>
        ///计算Tokens的数量 </summary>
        public virtual int countTokens()
        {
            int count = 0;
            int currpos = currentPosition;
            while (currpos < maxPosition)
            {
                currpos = skipDelimiters(currpos);
                if (currpos >= maxPosition)
                {
                    break;
                }
                currpos = scanToken(currpos);
                count++;
            }
            return count;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool MoveNext()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }

}
