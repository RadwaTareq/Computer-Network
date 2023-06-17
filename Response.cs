using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            this.code = code;
            //throw new NotImplementedException();
            string respDate = DateTime.Now.ToString("ddd, dd MMM yyy HH’:’mm’:’ss ‘EST’");
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            string headerLines =
                "Content-Type: " + contentType + "\r\n" +
                "Content-Length: " + content.Length.ToString() + "\r\n" +
                "Date: " + respDate + "\r\n";
            if (code == StatusCode.Redirect)
            {
                headerLines += "Location: " + redirectoinPath + "\r\n";
            }
            // TODO: Create the request string
            this.responseString = GetStatusLine(code) + "\r\n" + headerLines + "\r\n" + content + "\r\n";

        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            var enumDisplayStatus = (StatusCode)code;
            string stringValue = enumDisplayStatus.ToString();
            string statusLine = Configuration.ServerHTTPVersion + ' ' + code + stringValue;
            return statusLine;
        }
    }
}
