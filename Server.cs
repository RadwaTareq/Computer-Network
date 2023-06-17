using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, portNumber);
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.serverSocket.Bind(ip);
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            this.serverSocket.Listen(1000);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket clientSocket = this.serverSocket.Accept();
                Thread thread = new Thread(new ParameterizedThreadStart(HandleConnection));
                thread.Start(clientSocket);
            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket
            Socket clientSocket = (Socket)obj;
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            clientSocket.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    // TODO: Receive request
                    byte[] data = new byte[1024 * 1024];
                    int receivedLen = clientSocket.Receive(data);
                    // TODO: break the while loop if receivedLen==0
                    if (receivedLen == 0)
                    {
                        Console.WriteLine("Didn't receive any data");
                        break;
                    }

                    // TODO: Create a Request object using received request string
                    Request request = new Request(Encoding.ASCII.GetString(data));

                    // TODO: Call HandleRequest Method that returns the response
                    Response response = this.HandleRequest(request);

                    // TODO: Send Response back to client
                    clientSocket.Send(Encoding.ASCII.GetBytes(response.ResponseString));

                }

                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                    break;
                }
            }

            // TODO: close client socket
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }

        Response HandleRequest(Request request)
        {
            //throw new NotImplementedException();
            string content = "";
            Response response;
            StatusCode code = StatusCode.OK;
            string pageName = "";
            string path = Configuration.RootPath;
            StreamReader sr;

            try
            {
                bool redirected = false;
                bool defaultLoaded = false;

                //TODO: check for bad request

                if (!request.ParseRequest())
                {
                    code = StatusCode.BadRequest;
                    pageName = Configuration.BadRequestDefaultPageName;
                }

                //TODO: map the relativeURI in request to get the physical path of the resource.
                string relativeURI = request.relativeURI.TrimStart('/');

                //TODO: check for redirect
                string redirectionPageName = "";
                redirectionPageName = GetRedirectionPagePathIFExist(relativeURI);

                if (!redirectionPageName.Equals(""))
                {
                    code = StatusCode.Redirect;
                    pageName = redirectionPageName;
                    redirected = true;
                }

                //TODO: check file exists
                if (!redirected)
                {
                    content = LoadDefaultPage(relativeURI);
                    if (content.Equals(""))
                    {
                        code = StatusCode.NotFound;
                        pageName = Configuration.NotFoundDefaultPageName;
                    }

                    else
                    {
                        defaultLoaded = true;
                    }
                }

                //TODO: read the physical file
                if (!defaultLoaded)
                {
                    sr = new StreamReader(Path.Combine(path, pageName));
                    content = sr.ReadToEnd();
                    sr.Close();
                }

                // Create OK response
                response = new Response(code, "Text/HTML", content, redirectionPageName);
                return response;
            }

            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);

                // TODO: in case of exception, return Internal Server Error.
                code = StatusCode.InternalServerError;
                pageName = Configuration.InternalErrorDefaultPageName;

                sr = new StreamReader(Path.Combine(path, pageName));
                content = sr.ReadToEnd();
                sr.Close();

                response = new Response(code, "Text/HTML", content, "");
                return response;
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            foreach (KeyValuePair<string, string> pair in Configuration.RedirectionRules)
            {
                if (pair.Key.Equals(relativePath))
                {
                    return pair.Value;
                }
            }

            return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            if (!File.Exists(filePath))
            {
                Logger.LogException(new Exception("File not found"));
                return string.Empty;
            }
            // else read file and return its content
            StreamReader sr = new StreamReader(filePath);
            string content = sr.ReadToEnd();
            sr.Close();

            return content;
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    string[] words = new string[2];

                    while ((line = sr.ReadLine()) != null)
                    {
                        words = line.Split(',');
                        Configuration.RedirectionRules.Add(words[0], words[1]);
                    }

                }
                // then fill Configuration.RedirectionRules dictionary 
            }

            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}