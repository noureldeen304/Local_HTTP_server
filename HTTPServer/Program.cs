using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: Call CreateRedirectionRulesFile() function to create the rules of redirection 
            
            CreateRedirectionRulesFile();

            
            Server server = new Server(1000, "redirectionRules.txt");
            server.StartServer();

            Logger.sr.Close();            
        }

        static void CreateRedirectionRulesFile()
        { 
            FileStream file = new FileStream("redirectionRules.txt", FileMode.Create, FileAccess.ReadWrite);
            file.Close();
            StreamWriter writer = new StreamWriter(file.Name);
            // each line in the file specify a redirection rule
            writer.WriteLine("aboutus.html" + " " + "aboutus2.html");             
            writer.Close();
            
        }
         
    }
}
