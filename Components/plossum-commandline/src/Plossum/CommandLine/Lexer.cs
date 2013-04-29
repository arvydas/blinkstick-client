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
 *  $Id: Lexer.cs 3 2007-07-29 13:32:10Z palotas $
 */

// Rev 105

using System;
using SCG = System.Collections.Generic;
using System.Text;
using C5;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using Plossum.Resources;

namespace Plossum.CommandLine
{
    /// <summary>
    /// Provides tokenization of command lines. Used by the <see cref="CommandLineParser" />.
    /// </summary>
    internal class Lexer
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Lexer"/> class.
        /// </summary>
        /// <param name="input">The <see cref="System.IO.TextReader"/> to use for input. This need not be seekable.</param>
        /// <param name="escapeCharacters">The escape characters that should be recognized.</param>
        /// <param name="quotations">The quotations that should be recognized.</param>
        /// <param name="assignmentCharacters">The assignment characters that should be recognized.</param>
        public Lexer(TextReader input, ICollection<char> escapeCharacters, IDictionary<char, QuotationInfo> quotations,
            IDictionary<char, OptionStyles> assignmentCharacters)
        {
            mInput = input;
            FillCharacterBuffer();
            mEscapeCharacters = new GuardedCollection<char>(escapeCharacters);
            mQuotations = new GuardedDictionary<char, QuotationInfo>(quotations);
            mAssignmentCharacters = new GuardedDictionary<char, OptionStyles>(assignmentCharacters);

            mEnabledOptionStyles = OptionStyles.ShortUnix | OptionStyles.Windows | OptionStyles.File;
        }

        public Lexer(TextReader input, Lexer parent)
            : this(input, parent.mEscapeCharacters,parent.mQuotations,
            parent.mAssignmentCharacters)
        {
            mEscapeCharacters = parent.mEscapeCharacters;
            mAssignmentCharacters = parent.mAssignmentCharacters;
            mQuotations = parent.mQuotations;

            // OptionStyles are immutable, so this is safe. Changes to one lexer wont propagate to another.
            mEnabledOptionStyles = parent.mEnabledOptionStyles;

        }
        #endregion 

        #region Public properties

        /// <summary>
        /// Gets or sets the enabled option styles.
        /// </summary>
        /// <value>The enabled option styles.</value>
        /// <remarks>The enabled option styles detemines what types of switch-characters the lexer will recognize
        /// as starting an option on the command line.</remarks>
        /// <seealso cref="OptionStyles"/>
        public OptionStyles EnabledOptionStyles
        {
            get { return mEnabledOptionStyles; }
            set { mEnabledOptionStyles = value; }
        }

        public int CurrentLine
        {
            get { return mLine; }
        }

        #endregion

        #region Public methods

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")] // Just in the debug-statement!
        public ValueToken GetNextValueToken()
        {
            OptionStyles oldStyles = mEnabledOptionStyles;
            mEnabledOptionStyles = OptionStyles.None;
            Token returnToken = GetNextToken();
            mEnabledOptionStyles = oldStyles;
            Debug.Assert(returnToken is ValueToken);
            return (ValueToken)returnToken;
        }

        /// <summary>
        /// Gets the next token from the input.
        /// </summary>
        /// <returns>the next token from the input or a null reference if no more tokens are available.</returns>
        public Token GetNextToken()
        {
            if (!mTokenQueue.IsEmpty)
                return mTokenQueue.Dequeue();

            // Cache LA(1), it will be used a lot
            int la1;
            
            // Skip any whitespace
            while ((la1 = LA(1)) != -1 && IsWhiteSpace(la1))
                ReadCharacter();

            if (la1 == -1)    // No more tokens (or characters) to read
                return null;
            
            if (la1 == '-' && OptionStyleManager.IsAnyEnabled(EnabledOptionStyles, OptionStyles.Unix))
            {
                if (LA(2) == '-')
                {
                    if (IsWhiteSpace(LA(3)))
                        return MatchEndOption();
                    else if (OptionStyleManager.IsAllEnabled(EnabledOptionStyles, OptionStyles.LongUnix))
                        return MatchLongUnixOption();
                }

                if (OptionStyleManager.IsAllEnabled(EnabledOptionStyles, OptionStyles.ShortUnix))
                    return MatchShortUnixOption();
            }

            if (la1 == '/' && OptionStyleManager.IsAllEnabled(EnabledOptionStyles, OptionStyles.Windows))
                return MatchWindowsOption();

            if (la1 == '+' && OptionStyleManager.IsAllEnabled(EnabledOptionStyles, OptionStyles.Plus))
                    return MatchPlusOption();

                if (la1 == '@' && OptionStyleManager.IsAllEnabled(EnabledOptionStyles, OptionStyles.File))
                    return MatchFileOption();

            if (IsAssignmentCharacter(la1))
                return MatchAssignmentCharacter();

            return MatchValue();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Matches an assignment character within the current context.
        /// </summary>
        /// <returns>An <see cref="AssignmentToken"/> representing the assignment character found.</returns>
        private Token MatchAssignmentCharacter()
        {
            Debug.Assert(IsAssignmentCharacter(LA(1)));
            int ch = ReadCharacter();
            return new AssignmentToken((char)ch);
        }

        private Token MatchFileOption()
        {
            Debug.Assert(LA(1) == '@');
            SkipCharacters(1);

            if (LA(1) == -1 || IsWhiteSpace(LA(1)))
            {
                return new OptionFileToken("");
            }
            else
            {
                ValueToken value = (ValueToken)MatchValue();
                return new OptionFileToken(value.Value);
            }
        }

        /// <summary>
        /// Matches a value.
        /// </summary>
        /// <returns>A <see cref="ValueToken"/> describing the value read.</returns>
        /// <remarks>This method determines whether the value to be read is quoted or not and 
        /// calls the appropriate method to do the parsing.</remarks>
        /// <exception cref="MissingClosingQuoteException">no closing quote was found before the command line ended.</exception>
        private Token MatchValue()
        {
            if (mQuotations.Contains((char)LA(1)))
                return MatchQuotedValue();
            else
                return MatchUnquotedValue();
        }

        /// <summary>
        /// Matches an unquoted value. An unquoted value is any string that is terminated by an unescaped white space
        /// or the end of input.
        /// </summary>
        /// <returns>A <see cref="ValueToken"/> representing the value parsed.</returns>
        private Token MatchUnquotedValue()
        {
            Debug.Assert(!IsWhiteSpace(LA(1)));
            mText.Length = 0;

            while (!IsWhiteSpace(LA(1)) && LA(1) != -1)
            {
                mText.Append((char)ReadPossiblyEscapedCharacter('\0'));
            }
            return new ValueToken(mText.ToString());
        }

        /// <summary>
        /// Matches a quoted value. 
        /// </summary>
        /// <returns>A <see cref="ValueToken"/> representing the value parsed.</returns>
        /// <remarks>A quoted value is any string that starts with a quote character</remarks>
        /// <exception cref="MissingClosingQuoteException">no closing quote was found before the command line ended.</exception>
        private Token MatchQuotedValue()
        {
            Debug.Assert(mQuotations.Contains((char)LA(1)) && LA(1) != '\0');

            int quote = LA(1);
            
            SkipCharacters(1);
            mText.Length = 0;

            int ch;
            while ((ch = LA(1)) != quote && ch != -1)
            {
                mText.Append((char)ReadPossiblyEscapedCharacter((char)quote));

            }

            if (ReadCharacter() != quote)
                throw new MissingClosingQuoteException(String.Format(CultureInfo.CurrentUICulture,
                    CommandLineStrings.MissingClosingQuoteForValue0, quote.ToString(CultureInfo.CurrentUICulture) + 
                    mText.ToString()));

            return new ValueToken(mText.ToString());
        }

        /// <summary>
        /// Matches one or more options in sequence with a one-character name.
        /// </summary>
        /// <returns>An <see cref="OptionNameToken"/> representing the name (and style) of the first option 
        /// in the sequence read.</returns>
        /// <remarks>This method is called by the other methods that start with "MatchShort".
        /// Even though this method only returns the first option read, the <see cref="mTokenQueue"/> is 
        /// filled up with the rest of them.</remarks>
        private Token MatchGroupedOptionNames()
        {
            if (LA(1) == -1 || IsWhiteSpace(LA(1)))
                throw new MissingOptionNameException();

            while (LA(1) != -1 && !IsWhiteSpace(LA(1)) && !IsAssignmentCharacter(LA(1)))
            {
                mTokenQueue.Enqueue(new OptionNameToken(((char)ReadCharacter()).ToString(), mCurrentOptionStyle));
            }

            return mTokenQueue.Dequeue();
        }

        /// <summary>
        /// Matches an option name (or names depending on the option styles enabled) preceeded by the '-' sign.
        /// </summary>
        /// <returns>An <see cref="OptionNameToken"/> representing the name (and style) of the first option 
        /// in the sequence read.</returns>
        /// <remarks>This method calls <see cref="MatchGroupedOptionNames"/> if the <see cref="OptionStyles.Group"/> 
        /// style is enabled, otherwise <see cref="MatchLongOptionName"/>.</remarks>
        private Token MatchShortUnixOption()
        {
            Debug.Assert(LA(1) == '-');
            SkipCharacters(1);

            mCurrentOptionStyle = OptionStyles.ShortUnix;

            if (OptionStyleManager.IsAllEnabled(mEnabledOptionStyles, OptionStyles.Group))
                return MatchGroupedOptionNames();
            else
                return MatchLongOptionName();
        }

        /// <summary>
        /// Matches an option name (or names depending on the option styles enabled) preceeded by the '+' sign.
        /// </summary>
        /// <returns>An <see cref="OptionNameToken"/> representing the name (and style) of the first option 
        /// in the sequence read.</returns>
        /// <remarks>This method calls <see cref="MatchGroupedOptionNames"/> if the <see cref="OptionStyles.Group"/> 
        /// style is enabled, otherwise <see cref="MatchLongOptionName"/>.</remarks>
        private Token MatchPlusOption()
        {
            Debug.Assert(LA(1) == '+');
            SkipCharacters(1);

            mCurrentOptionStyle = OptionStyles.Plus;

            if (OptionStyleManager.IsAllEnabled(EnabledOptionStyles, OptionStyles.Group))
                return MatchGroupedOptionNames();
            else
                return MatchLongOptionName();
        }

        /// <summary>
        /// Matches the end of options token, represented by a double-dash ('--').
        /// </summary>
        /// <returns>A token of type <see cref="EndToken"/></returns>
        /// <remarks>This method does <b>not</b> make the lexer stop parsing options. The calling application
        /// needs to set EnabledOptionStyles to <see cref="OptionStyles.None"/> for that to happen, which can
        /// be done in response to this token being returned.</remarks>
        private Token MatchEndOption()
        {
            Debug.Assert(LA(1) == '-' && LA(2) == '-' && (IsWhiteSpace(LA(3)) || LA(3) == -1));
            SkipCharacters(3);
            return new EndToken();
        }

        /// <summary>
        /// Matches a long option name with the current option style. 
        /// </summary>
        /// <returns>An <see cref="OptionNameToken"/> representing the name and style of the option.</returns>
        /// <exception cref="MissingOptionNameException">there was no name following the option switch character</exception>
        private Token MatchLongOptionName()
        {
            mText.Length = 0;

            if (LA(1) == -1 || IsWhiteSpace(LA(1)))
                throw new MissingOptionNameException();

            int ch = LA(1);
            while (!IsWhiteSpace(ch = LA(1)) && !IsAssignmentCharacter(ch) && ch != -1)
            {
                mText.Append((char)ReadCharacter());
            }

            return new OptionNameToken(mText.ToString(), mCurrentOptionStyle);
        }

        /// <summary>
        /// Matches a long option name with the <see cref="OptionStyles.Windows"/> style. This must be preceeded
        /// a single slash ('/').
        /// </summary>
        /// <returns>An <see cref="OptionNameToken"/> representing the name and style of the option.</returns>
        /// <exception cref="MissingOptionNameException">there was no name following the option switch character</exception>
        private Token MatchWindowsOption()
        {
            Debug.Assert(LA(1) == '/');
            mCurrentOptionStyle = OptionStyles.Windows;
            SkipCharacters(1);
            return MatchLongOptionName();
        }

        /// <summary>
        /// Matches a long option name with the <see cref="OptionStyles.LongUnix"/> style. This may be preceeded
        /// by either a single ('-') or a double ('--') dash.
        /// </summary>
        /// <returns>An <see cref="OptionNameToken"/> representing the name and style of the option.</returns>
        /// <exception cref="MissingOptionNameException">there was no name following the option switch character</exception>
        private Token MatchLongUnixOption()
        {
            Debug.Assert(LA(1) == '-');

            // This check is neccessary since we don't know if the option starts
            // with one or two dashes. (If two is there, it starts with two)
            if (LA(2) == '-')
                SkipCharacters(2);
            else
                SkipCharacters(1);

            mText.Length = 0;
            mCurrentOptionStyle = OptionStyles.LongUnix;
            return MatchLongOptionName();
        }


        /// <summary>
        /// Determines whether the specified character is a white space character.
        /// </summary>
        /// <param name="ch">The character to test.</param>
        /// <returns>
        /// 	<c>true</c> the specified character is a white space character; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsWhiteSpace(int ch)
        {
            if (ch == -1)
                return false;

            return Char.IsWhiteSpace((char)ch);
        }

        /// <summary>
        /// Skips ahead the specified number of characters in the input stream (or buffer).
        /// </summary>
        /// <param name="count">The number of characters to skip.</param>
        /// <returns><c>true</c> if all specified characters were skipped, or <c>false</c> if there was
        /// not enough input left.</returns>
        /// <remarks>This has the same effect as calling <see cref="ReadCharacter"/> <paramref name="count"/>
        /// number of times.</remarks>
        private bool SkipCharacters(int count)
        {
            while (count-- > 0)
                if (ReadCharacter() == -1)
                    return false;
            return true;
        }

        /// <summary>
        /// Reads the next character from the input and moves the position of the input stream (or buffer
        /// depending on from where the character was read) one step forward. 
        /// </summary>
        /// <returns>The next available character from the input, or -1 if no more characters are available.</returns>
        private int ReadCharacter()
        {
            if (mCharacterBuffer.IsEmpty)
                FillCharacterBuffer();

            if (LA(1) == '\n')
                mLine++;

            return mCharacterBuffer.IsEmpty ? -1 : mCharacterBuffer.Dequeue();
        }

        /// <summary>
        /// Returns the lookahead character for the specified offset.
        /// </summary>
        /// <param name="offset">The number of characters to peek ahead.</param>
        /// <returns>the lookahead character for the specified offset, or -1 if there is no characters available that
        /// for the specified lookahead.</returns>
        /// <remarks>This is the last character that will be read by <see cref="ReadCharacter"/> if
        /// it is called <paramref name="offset"/> number of times.</remarks>
        private int LA(int offset)
        {
            // Look AHEAD, not back!
            Debug.Assert(offset > 0);

            // Adjust offset to buffer index
            offset = offset - 1;
            
            if (CharacterBufferSize <= offset)
                FillCharacterBuffer();

            if (CharacterBufferSize <= offset)
                return -1;

            return mCharacterBuffer[offset];

        }

        /// <summary>
        /// Fills up the character buffer with as many character as fits or are available, whichever is the least.
        /// </summary>
        /// <returns>The number of characters added to the buffer.</returns>
        private int FillCharacterBuffer()
        {
            int readCount = mCharacterBufferCapacity - CharacterBufferSize;
            if (readCount > 0)
            {
                char[] buf = new char[readCount + 1];
                readCount = mInput.Read(buf, 0, readCount + 1);
                for (int i = 0; i < readCount; ++i)
                {
                    // Perform newline translation. All newlines will
                    // be represented by a single '\n'.
                    if (buf[i] == '\r' && buf[i + 1] == '\n')
                    {
                        mCharacterBuffer.Enqueue('\n');
                        ++i;
                    }
                    else if (buf[i] == '\r')
                    {
                        mCharacterBuffer.Enqueue('\n');
                    }
                    else
                    {
                        mCharacterBuffer.Enqueue(buf[i]);
                    }
                }
            }
            return readCount;
        }

        /// <summary>
        /// Gets the number of available characters in the internal character buffer.
        /// </summary>
        /// <value>the number of available characters in the internal character buffer.</value>        
        private int CharacterBufferSize
        {
            get
            {
                return mCharacterBuffer.Count;
            }
        }

        /// <summary>
        /// Determines whether the specified character is a character used to escape other characters, such
        /// as the backslash '\' in C# strings.
        /// </summary>
        /// <param name="ch">The character to test.</param>
        /// <returns>
        /// 	<c>true</c> if the specified character is a character used to escape other characters; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>The escape character in use is <i>not</i> context sensitive.</remarks>
        private bool IsEscapeCharacter(int ch)
        {
            return mEscapeCharacters.Contains((char)ch);
        }

        /// <summary>
        /// Determines whether the specified character is an assignment character for the <see cref="OptionStyles"/>
        /// currently being parsed.
        /// </summary>
        /// <param name="ch">The character to check.</param>
        /// <returns>
        /// 	<c>true</c> if the specified character is an assignment character for the <see cref="OptionStyles"/>
        /// currently being parsed; otherwise, <c>false</c>.
        /// </returns>
        private bool IsAssignmentCharacter(int ch)
        {
            OptionStyles optionStyle;
            return mAssignmentCharacters.Find((char)ch, out optionStyle) && OptionStyleManager.IsAnyEnabled(optionStyle, EnabledOptionStyles)
                && OptionStyleManager.IsAnyEnabled(optionStyle, mCurrentOptionStyle);
        }

        /// <summary>
        /// Reads a character from the input stream which may be escaped within the specified context.
        /// </summary>
        /// <param name="currentQuote">The current quote in use for parsing the current value..</param>
        /// <returns>The character read if it was not an escape character within the current context that
        /// in turn is followed by a valid escape code within the current context. </returns>
        /// <remarks>The context is determined by the <paramref name="currentQuote"/> argument, which 
        /// indicates the type of quotes that were used for quoting the value currently being parsed. The
        /// special value of -1 indicates that no quotes were used.  If an escape character is found this method
        /// will actually read two characters and convert it to the replacement specified.</remarks>
        private int ReadPossiblyEscapedCharacter(char currentQuote)
        {
            QuotationInfo quotationInfo;

            if (IsEscapeCharacter(LA(1)) && mQuotations.Find(currentQuote, out quotationInfo) &&
                LA(2) != -1 && quotationInfo.IsEscapeCode((char)LA(2)))
            {
                char replacement = quotationInfo.EscapeCharacter((char)LA(2)); 
                SkipCharacters(2);
                return replacement;
            }
            return ReadCharacter();
        }

        #endregion

        #region Private fields

        /// <summary>
        /// Stores text read by the lexer when more than one character needs to be stored
        /// </summary>
        StringBuilder mText = new StringBuilder();


        /// <summary>
        /// Indicates the option styles that are currently active.
        /// </summary>
        private OptionStyles mEnabledOptionStyles;

        /// <summary>
        /// The input stream used by the lexer. This must be readable, but need not be seekable.
        /// </summary>
        private TextReader mInput;

        /// <summary>
        /// Intermediate storage of the current option style being parsed, used by various MatchXXXX functions.
        /// </summary>
        private OptionStyles mCurrentOptionStyle = OptionStyles.None;

        /// <summary>
        /// A queue of tokens to be returned by <see cref="GetNextToken"/>. Filled up if a single call
        /// to GetNextToken generates more than one token, such as when reading an option of style
        /// <see cref="OptionStyles.ShortUnix"/>
        /// </summary>
        private CircularQueue<Token> mTokenQueue = new CircularQueue<Token>();

        /// <summary>
        /// Internal buffer of characters read from the input stream.
        /// </summary>
        private CircularQueue<char> mCharacterBuffer = new CircularQueue<char>(mCharacterBufferCapacity);

        /// <summary>
        /// Constant indicating the size of the internal character buffer.
        /// </summary>
        private const int mCharacterBufferCapacity = 128;

        /// <summary>
        /// Table of the valid quotes and what characters to escape for a value within those quotes. The special
        /// key of <c>-1</c> stores the escaped character table to use for unquoted values.
        /// </summary>
        
        private IDictionary<char, QuotationInfo> mQuotations;

        /// <summary>
        /// Set storing the valid escape characters. (Normally only a backslash '\' character, but could be 
        /// set to something else.
        /// </summary>
        private ICollection<char> mEscapeCharacters;

        /// <summary>
        /// Table mapping valid assignment characters to their respective option values.
        /// </summary>
        private IDictionary<char, OptionStyles> mAssignmentCharacters;

        private int mLine = 1;
        #endregion

    }
}
