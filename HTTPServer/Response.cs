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
        
        string statusLine;
        List<string> headerLines = new List<string>();
        string content;
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            this.statusLine=this.GetStatusLine(code);
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            this.headerLines.Add("Content_Type:"+contentType+ "\r\n");
            this.headerLines.Add("Content_Length:" + content.Length+ "\r\n");
            this.headerLines.Add("Date:" + DateTime.Now+ "\r\n");
            
            if(code == StatusCode.Redirect)
            {
                this.headerLines.Add("Location:" + redirectoinPath);
                this.headerLines.Add("\r\n");
            }
            else
                this.headerLines.Add("\r\n");

             
            // TODO: Create the response string
            if(this.headerLines.Count==4)
            {
                this.responseString = this.statusLine + "\r\n" + headerLines[0] + headerLines[1] + headerLines[2] + headerLines[3] +  content;
            }
            else if(this.headerLines.Count == 5)
            {
                this.responseString = this.statusLine + "\r\n" + headerLines[0] + headerLines[1] + headerLines[2] + headerLines[3] + headerLines[4] +  content;
            }
            /////////////////////////////////////////////////
            Console.WriteLine("Response: \n"+this.responseString);
            ////////////////////////////////////////////////
        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = Configuration.ServerHTTPVersion +" "+ ((int)code) +" "+ code;
            return statusLine;
        }
    }
}
