using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket socket;
        string requeststring;
        public Server(int portNumber, string redirectionMatrixPath)
        {
             
            
            this.LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), portNumber);
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(iPEndPoint);
        }

        public void StartServer()
        {
             
            this.socket.Listen(200);
             
            while (true)
            {
                Socket serversocket = this.socket.Accept();
                Thread thread = new Thread(new ParameterizedThreadStart(HandleConnection));
                thread.Start(serversocket);
            }
        }

        public void HandleConnection(object obj)
        { 
            Socket serversocket = (Socket)obj;
            serversocket.ReceiveTimeout = 0;
             
            byte[] data;
            int length;
            while (true)
            {
                 data = new byte[1024 * 1024];
                
                try
                {
                     
                    length = serversocket.Receive(data);
                     
                    if(length==0)
                    {
                        break;
                    }

                    Console.WriteLine("Request: \n" + Encoding.ASCII.GetString(data, 0, length));//////////                    
                    
                    Request request = new Request(Encoding.ASCII.GetString(data, 0, length));
                    this.requeststring = Encoding.ASCII.GetString(data, 0, length);
                    
                    Response response = HandleRequest(request);
                     
                    serversocket.Send(Encoding.ASCII.GetBytes(response.ResponseString));
                }
                catch (Exception ex)
                { 
                     
                   Logger.LogException(ex.Message);
                }
            }

            
            serversocket.Close();
        }

        Response HandleRequest(Request request)
        { 
            Response response;


            string requestURL = GetURLFromRequestString(this.requeststring);
            
            string content;
            try
            {
                 

             if (request.Check_Request()!=true)
                {                
                    content = LoadDefaultPage(Configuration.BadRequestDefaultPageName);
                    response =new Response(StatusCode.BadRequest, "text/html", content, GetRedirectionPagePathIFExist(requestURL));
                    return response;
                }

             
            if (GetRedirectionPagePathIFExist(requestURL) !=string.Empty)
                {
                   content = LoadDefaultPage(Configuration.RedirectionDefaultPageName);
                   response =
                   new Response(StatusCode.Redirect, "text/html", content, GetRedirectionPagePathIFExist(requestURL));
                   return response;
                }


             
            
            if (CheckIfTheFileExist(requestURL) !=true)
                {
                        content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                        response =
                        new Response(StatusCode.NotFound, "text/html", content, GetRedirectionPagePathIFExist(requestURL));
                        return response;
                 }

             
                content = LoadDefaultPage(requestURL);
                response =
                new Response(StatusCode.OK, "text/html", content, GetRedirectionPagePathIFExist(requestURL));
                return response;
            }
            catch (Exception ex)
            {
                Logger.LogException("Internal server error");
                content = LoadDefaultPage(Configuration.InternalErrorDefaultPageName);
                response =
                new Response(StatusCode.InternalServerError, "text/html", content, GetRedirectionPagePathIFExist(requestURL));
                return response;
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
             
            if(relativePath == Configuration.RedirectionRules.ElementAt(0).Key)
                return Configuration.RedirectionRules.ElementAt(0).Value;
            return string.Empty;
        }

        private bool CheckIfTheFileExist(string filepath)
        {
            string abseloutePath = Path.Combine(Configuration.RootPath, filepath);
            FileInfo file = new FileInfo(abseloutePath);

            if (file.Exists)
            {
                //Logger.LogException("This file is not exist");
                return true;
            }
            return false;
        }
        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            StreamReader reader = new StreamReader(filePath);
            string content = reader.ReadToEnd();
            reader.Close();
            return content;
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            { 
                StreamReader reader = new StreamReader(filePath);
                string redirectionline = reader.ReadLine();
                reader.Close();
                string[] twopages = redirectionline.Split(' ');
                Configuration.RedirectionRules.Add(twopages[0], twopages[1]);
            }
            catch (Exception ex)
            {
                Environment.Exit(1);
            }
        }

        private string GetURLFromRequestString(string Entire_String)
        {
            string[] separator = new string[] { "\r\n" }; 

                
            string sub; 
            int count = 0; 
            for (int i = 0; i <= Entire_String.Length - 2; i++)
            {
                sub = Entire_String.Substring(i, 2);
                if (sub == "\r\n")
                    count++;
            }

            string[] Request_String_As_Lines;
            Request_String_As_Lines = Entire_String.Split(separator, count, StringSplitOptions.None);
            string[] Request_Lines = Request_String_As_Lines[0].Split(' ');  
            string[] pureURL = Request_Lines[1].Split('/');
            return pureURL[2];
        }
    }
}
