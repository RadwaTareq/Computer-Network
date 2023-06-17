﻿using System;
using System.Collections.Generic;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines = new Dictionary<string, string>();
        int blankLineIndex;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            // throw new NotImplementedException();

            //TODO: parse the receivedRequest using the \r\n delimeter   
            string[] requestStringSeparator = new string[] { "\r\n" };
            requestLines = requestString.Split(requestStringSeparator, System.StringSplitOptions.None);

            
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            if (!ParseRequestLine())
            {
                return false;
            }
            if (requestLines.Length > 3)
            {
                return false;
            }
            // Parse Request line        
            // Validate blank line exists
            // Load header lines into HeaderLines dictionary
            if (!LoadHeaderLines() || !ValidateBlankLine())
            {
                return false;
            }
            return true;
        }

        private bool ParseRequestLine()
        {
            //throw new NotImplementedException();
            string[] line = requestLines[0].Split(' ');

            if (line[0].Equals("GET"))
            {
                method = RequestMethod.GET;
                relativeURI = line[1];
                if (!ValidateIsURI(relativeURI))
                {
                    return false;
                }

                if (line[2].Equals("HTTP/0.9"))
                {
                    httpVersion = HTTPVersion.HTTP09;
                }
                else if (line[2].Equals("HTTP/1.0"))
                {
                    httpVersion = HTTPVersion.HTTP10;
                }
                else if (line[2].Equals("HTTP/1.1"))
                {
                    httpVersion = HTTPVersion.HTTP11;
                }
                else
                {
                    return false;
                }
                return true;
            }
            return false;

        }

        private bool ValidateIsURI(string uri)
        {

            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            // throw new NotImplementedException();
            string[] headerLinesSeparator = { ": " };
            int i = 1;
            while (!requestLines[i].Equals(""))
            {
                string headerContent = requestLines[i];
                string[] data = headerContent.Split(headerLinesSeparator, System.StringSplitOptions.None);
                headerLines.Add(data[0], data[1]);
                i++;
            }

            blankLineIndex = i;
            return true;
        }

        private bool ValidateBlankLine()
        {
            //throw new NotImplementedException();
            if (!String.IsNullOrEmpty(requestLines[blankLineIndex]))
            {
                return false;
            }

            return true;
        }

    }
}