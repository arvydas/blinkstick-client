/* Copyright (c) Peter Palotas 2007
 *  
 *  All rights reserved.
 *  
 *  Redistribution and use in source and binary forms, with or without
 *  modification, are permitted provided that the following conditions are
 *  met:
 *  
 *      * Redistributions of source code must retain the above copyright 
 *        notice, this list of conditions and the following disclaimer.    
 *      * Redistributions in binary form must reproduce the above copyright 
 *        notice, this list of conditions and the following disclaimer in 
 *        the documentation and/or other materials provided with the distribution.
 *      * Neither the name of the copyright holder nor the names of its 
 *        contributors may be used to endorse or promote products derived 
 *        from this software without specific prior written permission.
 *  
 *  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 *  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 *  LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 *  A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
 *  CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 *  EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 *  PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 *  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 *  LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 *  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 *  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *  
 *  $Id: StringFormatter.cs 7 2007-08-04 12:02:15Z palotas $
 */
using System;
using SCG = System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using C5;

namespace Plossum
{
    /// <summary>
    /// Class performing various formatting operations on strings based on fixed width characters.
    /// </summary>
    public static class StringFormatter
    {
        #region Public methods

        /// <summary>
        /// Aligns the specified string within a field of the desired width, cropping it if it doesn't fit, and expanding it otherwise.
        /// </summary>
        /// <param name="str">The string to align.</param>
        /// <param name="width">The width of the field in characters in which the string should be fitted.</param>
        /// <param name="alignment">The aligmnent that will be used for fitting the string in the field in case it is shorter than the specified field width.</param>
        /// <returns>
        /// A string exactly <paramref name="width"/> characters wide, containing the specified string <paramref name="str"/> fitted
        /// according to the parameters specified.
        /// </returns>
        /// <remarks>
        /// <para>If the string consists of several lines, each line will be aligned according to the specified parameters.</para>
        /// <para>The padding character used will be the normal white space character (' '), and the ellipsis used will be the
        /// string <b>"..."</b>. Cropping will be done on the right hand side of the string.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> was a null reference (<b>Nothing</b> in Visual Basic)</exception>
        /// <exception cref="ArgumentOutOfRangeException">The specified <paramref name="width"/> was less than, or equal to zero.</exception>
        /// <exception cref="ArgumentException"><paramref name="width"/> is less than the length of the specified <paramref name="ellipsis"/>, or, 
        /// the cropping specified is <see cref="Cropping.Both"/> and <paramref name="width"/> was less than <i>twice</i> the 
        /// length of the <paramref name="ellipsis"/></exception>
        public static string Align(string str, int width, Alignment alignment)
        {
            return Align(str, width, alignment, Cropping.Right, "...");
        }

        /// <summary>
        /// Aligns the specified string within a field of the desired width, cropping it if it doesn't fit, and expanding it otherwise.
        /// </summary>
        /// <param name="str">The string to align.</param>
        /// <param name="width">The width of the field in characters in which the string should be fitted.</param>
        /// <param name="alignment">The aligmnent that will be used for fitting the string in the field in case it is shorter than the specified field width.</param>
        /// <param name="cropping">The method that will be used for cropping if the string is too wide to fit in the specified width.</param>
        /// <returns>
        /// A string exactly <paramref name="width"/> characters wide, containing the specified string <paramref name="str"/> fitted
        /// according to the parameters specified.
        /// </returns>
        /// <remarks>
        /// <para>If the string consists of several lines, each line will be aligned according to the specified parameters.</para>
        /// <para>The padding character used will be the normal white space character (' '), and the ellipsis used will be the
        /// string <b>"..."</b>.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> was a null reference (<b>Nothing</b> in Visual Basic)</exception>
        /// <exception cref="ArgumentOutOfRangeException">The specified <paramref name="width"/> was less than, or equal to zero.</exception>
        /// <exception cref="ArgumentException"><paramref name="width"/> is less than the length of the specified <paramref name="ellipsis"/>, or, 
        /// the cropping specified is <see cref="Cropping.Both"/> and <paramref name="width"/> was less than <i>twice</i> the 
        /// length of the <paramref name="ellipsis"/></exception>
        public static string Align(string str, int width, Alignment alignment, Cropping cropping)
        {
            return Align(str, width, alignment, cropping, "...");
        }

        /// <summary>
        /// Aligns the specified string within a field of the desired width, cropping it if it doesn't fit, and expanding it otherwise.
        /// </summary>
        /// <param name="str">The string to align.</param>
        /// <param name="width">The width of the field in characters in which the string should be fitted.</param>
        /// <param name="alignment">The aligmnent that will be used for fitting the string in the field in case it is shorter than the specified field width.</param>
        /// <param name="cropping">The method that will be used for cropping if the string is too wide to fit in the specified width.</param>
        /// <param name="ellipsis">A string that will be inserted at the cropped side(s) of the string to denote that the string has been cropped.</param>
        /// <returns>
        /// A string exactly <paramref name="width"/> characters wide, containing the specified string <paramref name="str"/> fitted
        /// according to the parameters specified.
        /// </returns>
        /// <remarks>
        /// <para>If the string consists of several lines, each line will be aligned according to the specified parameters.</para>
        /// <para>The padding character used will be the normal white space character (' ').</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> was a null reference (<b>Nothing</b> in Visual Basic)</exception>
        /// <exception cref="ArgumentOutOfRangeException">The specified <paramref name="width"/> was less than, or equal to zero.</exception>
        /// <exception cref="ArgumentException"><paramref name="width"/> is less than the length of the specified <paramref name="ellipsis"/>, or, 
        /// the cropping specified is <see cref="Cropping.Both"/> and <paramref name="width"/> was less than <i>twice</i> the 
        /// length of the <paramref name="ellipsis"/></exception>
        public static string Align(string str, int width, Alignment alignment, Cropping cropping, string ellipsis)
        {
            return Align(str, width, alignment, cropping, ellipsis, ' ');
        }

        /// <summary>
        /// Aligns the specified string within a field of the desired width, cropping it if it doesn't fit, and expanding it otherwise.
        /// </summary>
        /// <param name="str">The string to align.</param>
        /// <param name="width">The width of the field in characters in which the string should be fitted.</param>
        /// <param name="alignment">The aligmnent that will be used for fitting the string in the field in case it is shorter than the specified field width.</param>
        /// <param name="cropping">The method that will be used for cropping if the string is too wide to fit in the specified width.</param>
        /// <param name="ellipsis">A string that will be inserted at the cropped side(s) of the string to denote that the string has been cropped.</param>
        /// <param name="padCharacter">The character that will be used for padding the string in case it is shorter than the specified field width.</param>
        /// <returns>
        /// A string exactly <paramref name="width"/> characters wide, containing the specified string <paramref name="str"/> fitted
        /// according to the parameters specified.
        /// </returns>
        /// <remarks>If the string consists of several lines, each line will be aligned according to the specified parameters.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> was a null reference (<b>Nothing</b> in Visual Basic)</exception>
        /// <exception cref="ArgumentOutOfRangeException">The specified <paramref name="width"/> was less than, or equal to zero.</exception>
        /// <exception cref="ArgumentException"><paramref name="width"/> is less than the length of the specified <paramref name="ellipsis"/>, or, 
        /// the cropping specified is <see cref="Cropping.Both"/> and <paramref name="width"/> was less than <i>twice</i> the 
        /// length of the <paramref name="ellipsis"/></exception>
        public static string Align(string str, int width, Alignment alignment, Cropping cropping, string ellipsis, char padCharacter)
        {
            if (str == null)
                throw new ArgumentNullException("str");

            if (width <= 0)
                throw new ArgumentOutOfRangeException("width");

            if (ellipsis != null)
            {
                if (cropping != Cropping.Both && width < ellipsis.Length)
                    throw new ArgumentException("width must not be less than the length of ellipsis");
                else if (cropping == Cropping.Both && width < ellipsis.Length * 2)
                    throw new ArgumentException("width must not be less than twice the length of the ellipsis when cropping is set to Both");
            }

            IIndexed<string> lines = SplitAtLineBreaks(str);
            StringBuilder result = new StringBuilder();

            for (int j = 0; j < lines.Count; j++)
            {
                if (j != 0)
                    result.Append(Environment.NewLine);

                string s = lines[j];
                int length = s.Length;
                if (length <= width)
                {
                    switch (alignment)
                    {
                        case Alignment.Left:
                            result.Append(s);
                            result.Append(padCharacter, width - length);
                            continue;
                        case Alignment.Right:
                            result.Append(padCharacter, width - length);
                            result.Append(s);
                            continue;
                        case Alignment.Center:
                            result.Append(padCharacter, (width - length) / 2);
                            result.Append(s);
                            result.Append(padCharacter, (width - length) - ((width - length) / 2));
                            continue;                            
                        case Alignment.Justified:
                            string trimmed = s.Trim();
                            length = trimmed.Length;

                            int spaceCount = GetWordCount(s) - 1;

                            Debug.Assert(spaceCount >= 0);

                            if (spaceCount == 0) // string only contain a single word
                            {
                                result.Append(trimmed);
                                result.Append(padCharacter, width - length);
                            }

                            StringBuilder localResult = new StringBuilder();
                            int remainingSpace = width - length;
                            bool readingWord = true;

                            for (int i = 0; i < length; i++)
                            {
                                if (!char.IsWhiteSpace(trimmed[i]))
                                {
                                    readingWord = true;
                                    localResult.Append(trimmed[i]);
                                }
                                else if (readingWord)
                                {
                                    localResult.Append(trimmed[i]);
                                    int spacesToAdd = remainingSpace / spaceCount--;
                                    remainingSpace -= spacesToAdd;
                                    localResult.Append(padCharacter, spacesToAdd);
                                    readingWord = false;

                                }
                            }
                            result.Append(localResult);
                            continue;
                        default:
                            throw new InvalidOperationException("Internal error; Unimplemented Justification specified");
                    }
                }

                // The string is too long and need to be cropped
                switch (cropping)
                {
                    case Cropping.Right:
                        return s.Substring(0, width - ellipsis.Length) + ellipsis;
                    case Cropping.Left:
                        return ellipsis + s.Substring(length - width + ellipsis.Length);
                    case Cropping.Both:
                        return ellipsis + s.Substring(length / 2 - width / 2, width - ellipsis.Length * 2) + ellipsis;
                    default:
                        break;
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Performs word wrapping on the specified string, making it fit within the specified width.
        /// </summary>
        /// <param name="str">The string to word wrap.</param>
        /// <param name="width">The width of the field in which to fit the string.</param>
        /// <returns>A word wrapped version of the original string aligned and padded as specified.</returns>
        /// <remarks><para>No padding will be performed on strings that are shorter than the specified width, and each line will be 
        /// left aligned.</para>
        /// <para>This method uses <see cref="WordWrappingMethod.Optimal"/> to perform word wrapping.</para></remarks>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> was a null reference (<b>Nothing</b> in Visual Basic)</exception>
        /// <exception cref="ArgumentOutOfRangeException">The specified <paramref name="width"/> was less than, or equal to zero.</exception>
        public static string WordWrap(string str, int width)
        {
            return WordWrap(str, width, WordWrappingMethod.Optimal);
        }

        /// <summary>
        /// Performs word wrapping on the specified string, making it fit within the specified width.
        /// </summary>
        /// <param name="str">The string to word wrap.</param>
        /// <param name="width">The width of the field in which to fit the string.</param>
        /// <param name="method">The method to use for word wrapping.</param>
        /// <returns>A word wrapped version of the original string aligned and padded as specified.</returns>
        /// <remarks>No padding will be performed on strings that are shorter than the specified width, and each line will be 
        /// left aligned.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> was a null reference (<b>Nothing</b> in Visual Basic)</exception>
        /// <exception cref="ArgumentOutOfRangeException">The specified <paramref name="width"/> was less than, or equal to zero.</exception>
        public static string WordWrap(string str, int width, WordWrappingMethod method)
        {
            if (str == null)
                throw new ArgumentNullException("str");

            if (width <= 0)
                throw new ArgumentOutOfRangeException("width");

            if (method == WordWrappingMethod.Optimal)
                return new OptimalWordWrappedString(str, width).ToString();

            // Simple word wrapping method that simply fills lines as 
            // much as possible and then breaks. Creates a not so nice
            // layout. 
            StringBuilder dest = new StringBuilder(str.Length);
            StringBuilder word = new StringBuilder();
            
            int spaceLeft = width;
            StringReader reader = new StringReader(str);
            int ch;
            do
            {
                while ((ch = reader.Read()) != -1 && ch != ' ')
                {
                    word.Append((char)ch);
                    if (ch == '\n')
                        spaceLeft = width;
                }

                if (word.Length > spaceLeft)
                {
                    while (word.Length > width)
                    {
                        dest.Append('\n');
                        dest.Append(word.ToString(0, width));
                        word.Remove(0, width);
                        spaceLeft = width;
                    }

                    dest.Append('\n');
                    dest.Append(word);
                    spaceLeft = width - word.Length;
                    word.Length = 0;
                }
                else
                {
                    dest.Append(word);
                    spaceLeft -= word.Length;
                    word.Length = 0;
                }
                if (ch != -1 && spaceLeft > 0)
                {
                    dest.Append(' ');
                    spaceLeft--;
                }
            }
            while (ch != -1);
            reader.Close();
            return dest.ToString();
        }

        /// <summary>
        /// Performs word wrapping on the specified string, making it fit within the specified width and additionally aligns each line
        /// according to the <see cref="Alignment"/> specified.
        /// </summary>
        /// <param name="str">The string to word wrap and align.</param>
        /// <param name="width">The width of the field in which to fit the string.</param>
        /// <param name="method">The method to use for word wrapping.</param>
        /// <param name="alignment">The alignment to use for each line of the resulting string.</param>
        /// <returns>A word wrapped version of the original string aligned and padded as specified.</returns>
        /// <remarks>If padding is required, the normal simple white space character (' ') will be used.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> was a null reference (<b>Nothing</b> in Visual Basic)</exception>
        /// <exception cref="ArgumentOutOfRangeException">The specified <paramref name="width"/> was less than, or equal to zero.</exception>
        public static string WordWrap(string str, int width, WordWrappingMethod method, Alignment alignment)
        {
            return WordWrap(str, width, method, alignment, ' ');
        }

        /// <summary>
        /// Performs word wrapping on the specified string, making it fit within the specified width and additionally aligns each line
        /// according to the <see cref="Alignment"/> specified.
        /// </summary>
        /// <param name="str">The string to word wrap and align.</param>
        /// <param name="width">The width of the field in which to fit the string.</param>
        /// <param name="method">The method to use for word wrapping.</param>
        /// <param name="alignment">The alignment to use for each line of the resulting string.</param>
        /// <param name="padCharacter">The character to use for padding lines that are shorter than the specified width.</param>
        /// <returns>A word wrapped version of the original string aligned and padded as specified.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> was a null reference (<b>Nothing</b> in Visual Basic)</exception>
        /// <exception cref="ArgumentOutOfRangeException">The specified <paramref name="width"/> was less than, or equal to zero.</exception>
        public static string WordWrap(string str, int width, WordWrappingMethod method, Alignment alignment, char padCharacter)
        {
            return StringFormatter.Align(WordWrap(str, width, method), width, alignment, Cropping.Left, "", padCharacter);
        }


        /// <summary>
        /// Splits the specified strings at line breaks, resulting in an indexed collection where each item represents one line of the 
        /// original string.
        /// </summary>
        /// <param name="str">The string to split.</param>
        /// <returns>an indexed collection where each item represents one line of the 
        /// original string.</returns>
        /// <remarks>This might seem identical to the String.Split method at first, but it is not exactly, since this method 
        /// recognizes line breaks in the three formats: "\n", "\r" and "\r\n". Note that any newline characters will not be present
        /// in the returned collection.</remarks>
        public static IIndexed<string> SplitAtLineBreaks(string str)
        {
            return SplitAtLineBreaks(str, false);
        }

        /// <summary>
        /// Splits the specified strings at line breaks, resulting in an indexed collection where each item represents one line of the
        /// original string.
        /// </summary>
        /// <param name="str">The string to split.</param>
        /// <param name="removeEmptyLines">if set to <c>true</c> any empty lines will be removed from the resulting collection.</param>
        /// <returns>
        /// an indexed collection where each item represents one line of the
        /// original string.
        /// </returns>
        /// <remarks>This might seem identical to the String.Split method at first, but it is not exactly, since this method
        /// recognizes line breaks in the three formats: "\n", "\r" and "\r\n". Note that any newline characters will not be present
        /// in the returned collection.</remarks>
        public static IIndexed<string> SplitAtLineBreaks(string str, bool removeEmptyLines)
        {
            IIndexed<string> result = new ArrayList<string>();

            StringBuilder temp = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                if (i < str.Length - 1 && str[i] == '\r' && str[i + 1] == '\n')
                    i++;

                if (str[i] == '\n' || str[i] == '\r')
                {
                    if (!removeEmptyLines || temp.Length > 0)
                        result.Add(temp.ToString());
                    temp.Length = 0;
                }
                else
                {
                    temp.Append(str[i]);
                }
            }

            if (temp.Length > 0)
                result.Add(temp.ToString());
            return result;
        }

        /// <summary>
        /// Retrieves the number of words in the specified string.
        /// </summary>
        /// <param name="str">The string in which to count the words.</param>
        /// <returns>The number of words in the specified string.</returns>
        /// <remarks>A <i>word</i> here is defined as any number (greater than zero) of non-whitespace characters, separated from 
        /// other words by one or more white space characters.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> was a null reference (<b>Nothing</b> in Visual Basic)</exception>
        public static int GetWordCount(string str)
        {
            if (str == null)
                throw new ArgumentNullException("str");

            int count = 0;
            bool readingWord = false;
            for (int i = 0; i < str.Length; i++)
            {
                if (!char.IsWhiteSpace(str[i]))
                    readingWord = true;
                else if (readingWord)
                {
                    count++;
                    readingWord = false;
                }
            }
            if (readingWord)
                count++;

            return count;
        }

        /// <summary>
        /// Formats several fixed width strings into columns of the specified widths, performing word wrapping and alignment as specified.
        /// </summary>
        /// <param name="indent">The indentation (number of white space characters) to use before the first column.</param>
        /// <param name="columnSpacing">The spacing to use in between columns.</param>
        /// <param name="columns">An array of the <see cref="ColumnInfo"/> objects representing the columns to use.</param>
        /// <returns>A single string that when printed will represent the original strings formatted as specified in each <see cref="ColumnInfo"/>
        /// object.</returns>
        /// <exception cref="ArgumentNullException">A <see cref="ColumnInfo.Content"/> for a column was a null reference (<b>Nothing</b> in Visual Basic)</exception>
        /// <exception cref="ArgumentOutOfRangeException">The specified <see cref="ColumnInfo.Width"/> for a column was less than, or equal to zero.</exception>
        /// <exception cref="ArgumentException"><paramref name="columnSpacing"/> was less than zero, or, <paramref name="indent"/> was less than 
        /// zero, or, no columns were specified.</exception>
        public static string FormatInColumns(int indent, int columnSpacing, params ColumnInfo[] columns)
        {
            if (columnSpacing < 0)
                throw new ArgumentException("columnSpacing must not be less than zero", "columnSpacing");

            if (indent < 0)
                throw new ArgumentException("indent must not be less than zero", "indent");

            if (columns.Length == 0)
                return "";

            IIndexed<string>[] strings = new IIndexed<string>[columns.Length];
            int totalLineCount = 0;

            // Calculate the total number of lines that needs to be printed
            for (int i = 0; i < columns.Length; i++)
            {
                strings[i] = SplitAtLineBreaks(WordWrap(columns[i].Content, columns[i].Width, columns[i].WordWrappingMethod, columns[i].Alignment, ' '), false);
                totalLineCount = Math.Max(strings[i].Count, totalLineCount);
            }

            // Calculate the first line on which each column should start to print, based
            // on its vertical alignment.
            int[] startLine = new int[columns.Length];
            for (int col = 0; col < columns.Length; col++)
            {
                switch (columns[col].VerticalAlignment)
                {
                    case VerticalAlignment.Top:
                        startLine[col] = 0;
                        break;
                    case VerticalAlignment.Bottom:
                        startLine[col] = totalLineCount - strings[col].Count;
                        break;
                    case VerticalAlignment.Middle:
                        startLine[col] = (totalLineCount - strings[col].Count) / 2;
                        break;
                    default:
                        throw new InvalidOperationException(Resources.CommandLineStrings.InternalErrorUnimplementedVerticalAlignmentUsed);
                }
            }

            StringBuilder result = new StringBuilder();
            for (int line = 0; line < totalLineCount; line++)
            {
                result.Append(' ', indent);
                for (int col = 0; col < columns.Length; col++)
                {
                    if (line >= startLine[col] && line - startLine[col] < strings[col].Count)
                    {
                        result.Append(strings[col][line - startLine[col]]);
                    }
                    else
                    {
                        result.Append(' ', columns[col].Width);
                    }
                    if (col < columns.Length - 1)
                        result.Append(' ', columnSpacing);
                }
                if (line != totalLineCount - 1)
                    result.Append(Environment.NewLine);
            }
            return result.ToString();
        }

        #endregion 

        #region Private classes

        /// <summary>
        /// Class performing an "optimal solution" word wrapping creating a somewhat more estetically pleasing layout. 
        /// </summary>
        /// <remarks><para>This is based on the 
        /// "optimal solution" as described on the Wikipedia page for "Word Wrap" (http://en.wikipedia.org/wiki/Word_wrap).
        /// The drawback of this method compared to the simple "greedy" technique is that this is much, much slower. However for 
        /// short strings to print as console messages this will not be a problem, but using it in a WYSIWYG word processor is probably
        /// not a very good idea.</para></remarks>
        private class OptimalWordWrappedString
        {
            #region Constructors

            public OptimalWordWrappedString(string s, int lineWidth)
            {
                string[] lines = s.Split('\n');
                for (int c = 0; c < lines.Length; c++)
                {
                    mStr = lines[c].Trim();
                    mLineWidth = lineWidth;

                    BuildWordList(mStr, mLineWidth);
                    mCostCache = new int[mWordList.Length, mWordList.Length];
                    for (int x = 0; x < mWordList.Length; x++)
                        for (int y = 0; y < mWordList.Length; y++)
                            mCostCache[x, y] = -1;

                    mfCache = new LineBreakResult[mWordList.Length];

                    IStack<int> stack = new ArrayList<int>();

                    LineBreakResult last = new LineBreakResult(0, mWordList.Length - 1);
                    stack.Push(last.K);
                    while (last.K >= 0)
                    {
                        last = FindLastOptimalBreak(last.K);
                        if (last.K >= 0)
                            stack.Push(last.K);
                    }

                    int start = 0;
                    while (!stack.IsEmpty)
                    {
                        int next = stack.Pop();
                        mResult.Append(GetWords(start, next));
                        if (!stack.IsEmpty)
                            mResult.Append(Environment.NewLine);
                        start = next + 1;
                    }

                    if (c != lines.Length - 1)
                        mResult.Append(Environment.NewLine);
                }

                mWordList = null;
                mfCache = null;
                mStr = null;
                mCostCache = null;
            }

            #endregion

            #region Public methods

            public override string ToString()
            {
                return mResult.ToString();
            }

            #endregion

            #region Private methods

            private string GetWords(int i, int j)
            {
                int start = mWordList[i].pos;
                int end = (j + 1 >= mWordList.Length) ? mStr.Length : mWordList[j + 1].pos - (mWordList[j + 1].spacesBefore - mWordList[j].spacesBefore);
                return mStr.Substring(start, end - start);
            }

            private struct WordInfo
            {
                public int spacesBefore;
                public int pos;
                public int length;
                public int totalLength;
            }

            private void BuildWordList(string s, int lineWidth)
            {
                Debug.Assert(!s.Contains("\n"));

                ArrayList<WordInfo> mWordListAL = new ArrayList<WordInfo>();

                bool lookingForWs = false;
                WordInfo we = new WordInfo();

                we.pos = 0;
                int spaces = 0;
                int totalLength = 0;
                for (int i = 0; i < s.Length; i++)
                {
                    char ch = s[i];
                    if (lookingForWs && ch == ' ')
                    {
                        spaces++;
                        if (we.pos != i)
                            mWordListAL.Add(we);
                        we = new WordInfo();
                        we.spacesBefore = spaces;
                        we.pos = i + 1;
                        lookingForWs = false;
                        continue;
                    }
                    else if (ch != ' ')
                    {
                        lookingForWs = true;
                    }

                    we.length++;
                    totalLength++;
                    we.totalLength = totalLength;

                    if (we.length == lineWidth)
                    {
                        mWordListAL.Add(we);
                        we = new WordInfo();
                        we.spacesBefore = spaces;
                        we.pos = i + 1;
                    }
                }
                mWordListAL.Add(we);
                mWordList = mWordListAL.ToArray();
            }

            private int SumWidths(int i, int j)
            {
                return i == 0 ? mWordList[j].totalLength : mWordList[j].totalLength - mWordList[i - 1].totalLength;
            }

            private int GetCost(int i, int j)
            {
                int cost = mCostCache[i, j];

                if (cost == -1)
                {
                    cost = mLineWidth - (mWordList[j].spacesBefore - mWordList[i].spacesBefore) - SumWidths(i, j);
                    cost = cost < 0 ? mInfinity : cost * cost;
                    mCostCache[i, j] = cost;
                }
                return cost;
            }

            private LineBreakResult FindLastOptimalBreak(int j)
            {
                if (mfCache[j] != null)
                {
                    return mfCache[j];
                }

                int cost = GetCost(0, j);
                if (cost < mInfinity)
                {
                    return new LineBreakResult(cost, -1);
                }

                LineBreakResult min = new LineBreakResult();
                for (int k = 0; k < j; k++)
                {
                    int result = FindLastOptimalBreak(k).Cost + GetCost(k + 1, j);
                    if (result < min.Cost)
                    {
                        min.Cost = result;
                        min.K = k;
                    }
                }

                mfCache[j] = min;
                return min;
            }

            #endregion

            #region Private types

            private class LineBreakResult
            {
                public LineBreakResult()
                {
                    Cost = mInfinity;
                    K = -1;
                }

                public LineBreakResult(int cost, int k)
                {
                    this.Cost = cost;
                    this.K = k;
                }

                public int Cost;
                public int K;
            }

            #endregion

            #region Private fields

            private WordInfo[] mWordList;
            private StringBuilder mResult = new StringBuilder();
            private LineBreakResult[] mfCache;
            private string mStr;
            private int mLineWidth;
            private const int mInfinity = int.MaxValue / 2;

            // We need a rectangular array here, so this warning is unwarranted.
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Member")]
            private int[,] mCostCache;

            #endregion
        }

        #endregion

    }
}
