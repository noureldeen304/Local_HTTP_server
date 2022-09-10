using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public string  relativeURI;
         
        Dictionary<string, string> headerLines = new Dictionary<string, string>();

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

       HTTPVersion httpVersion;
        string Request_String;

        public Request(string Request_String)
        {
            this.Request_String = Request_String;
        }
        
        public bool Check_Request()
        {
            string[] separator = new string[1] { "\r\n" };  
            string[] Request_String_As_Lines;                 
            string sub;
            int count = 0;
            for (int i = 0; i <= this.Request_String.Length - 2; i++)
            {
                sub = this.Request_String.Substring(i, 2);
                if (sub == "\r\n")
                    count++;
            }
            Request_String_As_Lines = this.Request_String.Split(separator, count, StringSplitOptions.None);
             
            if (Request_String_As_Lines.Length < 3)
            {
                return false;
            }


          
            requestLines = Request_String_As_Lines[0].Split(' ');  
            string[] pureURL = requestLines[1].Split('/'); 
            
            if (Check_method(requestLines[0]) != true)
            {
                return false;
            }

            if (ValidateIsURI(requestLines[1]) != true)
            {                
                return false;
            }        
            
            if (validatehttpversion(requestLines[2]) != true)
            { return false; }

            // check blank line

            if (check_Blank_Line(Request_String_As_Lines) != true) 
            { return false; }

             

            string[] Header_Lines = new string[2];  
            for(int i=1;i<=Request_String_As_Lines.Length-2;i++)
            {
                Header_Lines = Request_String_As_Lines[i].Split(' ');
                this.headerLines.Add(Header_Lines[0], Header_Lines[1]);
            }

            if(LoadHeaderLines(HeaderLines)!=true)
            {
                return false;
            }

            return true;
        }

        private bool Check_method(string requestline)
        {
             
            if (requestline == RequestMethod.GET.ToString())
            {
                this.method = RequestMethod.GET;
                return true;
            }
            if (requestline == RequestMethod.HEAD.ToString())
            {
                this.method = RequestMethod.HEAD;
                return true;

            }
            if (requestline == RequestMethod.POST.ToString())
            {
                this.method = RequestMethod.POST;
                return true;
            }             
            return false;
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool validatehttpversion(string version)
        {
            if (version == "HTTP/1.1")
            {
                this.httpVersion = HTTPVersion.HTTP11;
                return true;
            }
            return false;
        }

        private bool LoadHeaderLines(Dictionary<string,string> header)
        {
            
            if (header.Count > 0 && header.ElementAt(0).Key =="Host:")
                return true;
            return false;
        }

        private bool check_Blank_Line(string[] requeststring)
        {
            if (requeststring[requeststring.Length - 1] == "\r\n")
                return true;
            return false;
        }

    }
}
