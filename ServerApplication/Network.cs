using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.IO.Compression;
using System.Threading;

namespace ServerApplication
{
    public class Network
    {
        public class Book
        {

            public List<string> comments = new List<string>();
            //public int StuId;
            //public List<int> Step = new List<int>();
            public List<string> StuId = new List<string>();
            public List<string> Step = new List<string>();          //just for log of infromation record comment related to each step
            public List<int> Upvote = new List<int>();
            public int len;
            public int start;
            //******************Reply Part**************************..
            public List<List<String>> Reply = new List<List<String>>();
            //public List<List<int>> Upvote_Reply = new List<List<int>>();
            public List<int> len_reply = new List<int>();
            //public int start_reply;
            public Book() { start = 0; }
            public void BookAdd(string comment, string stuId, string step)
            {
                comments.Add(comment);
                StuId.Add(stuId);
                Step.Add(step);
                Upvote.Add(0);
                len = comments.Count;
                Reply.Add(new List<string>());
                //Upvote_Reply.Add(new List<int>());
                len_reply.Add(0);
            }

            public void BookUpvote(int index)
            {

                Upvote[index]++;
            }

            public void Pin(int index)
            {
                string tmp_stuid, tmp_step, tmp_cmt;
                List<string> tmp_reply;
                int tmp_upvote,temp_lenreply;
                if (index == start)
                {
                    start++;
                }
                //****************Swap process**********************//
                tmp_stuid = this.StuId[index];
                tmp_step = this.Step[index];
                tmp_cmt = this.comments[index];
                tmp_upvote = this.Upvote[index];
                temp_lenreply = this.len_reply[index];
                tmp_reply = this.Reply[index];


                for (int i = index; i > start; i--)
                {
                    this.StuId[i] = this.StuId[i - 1];
                    this.Step[i] = this.Step[i - 1];
                    this.comments[i] = this.comments[i - 1];
                    this.Upvote[i] = this.Upvote[i - 1];
                    this.len_reply[i] = this.len_reply[i - 1];
                    this.Reply[i]= this.Reply[i-1];
                }

                this.StuId[start] = tmp_stuid;
                this.Step[start] = tmp_step;
                this.comments[start] = tmp_cmt;
                this.Upvote[start] = tmp_upvote;
                this.len_reply[start] = temp_lenreply;
                this.Reply[start] = tmp_reply;

                start++;
            }

            public void ReplyComment(string reply, int comment_index, int stuid)
            {
                reply = "Student " + stuid + " replied: "+reply;
                Reply[comment_index].Add(reply);
                len_reply[comment_index]++;
            }
            public void print()
            {
                for (int i = 0; i < len; i++)
                    Console.WriteLine("comment: {0} corresponding to step {1} sent from student {2}", comments[i], Step[i], StuId);
                for (int i = 0; i < len_reply[0]; i++)
                    Console.WriteLine("Reply to comment {0} are {1}", comments[0], Reply[i]);
            }

        }


        public class Step
        {
            public List<string> StuId = new List<string>();
            public List<string> StepNum = new List<string>();
            Dictionary<string, int> Upvote = new Dictionary<string, int>();
            public int len;
            public Step() { }
            public void StepAdd(string stuid, string stepnum)
            {
                if (Upvote.ContainsKey(stepnum))
                    Upvote[stepnum]++;
                else
                    Upvote.Add(stepnum, 1);

                StuId.Add(stuid);
                StepNum.Add(stepnum);
                len = StepNum.Count;
            }
        }
        //public TcpListener ServerSocket;
        static Socket serverSocket;
        private static int myProt = 5500;   
        public static Network instance = new Network();
        public static List<string> StuID;     //Initialized Later;

        //Dictionary<int, TcpClient> list_clients = new Dictionary<int, TcpClient>();
        static Dictionary<string, TcpClient> list_clients = new Dictionary<string, TcpClient>();
        int count=0;
        static private readonly object _lock = new object();        //Lock to protect list_client
        static private readonly object _lockClient = new object();  //Lock to protect each TCPClient socket


        //Socket[] Clients = new Socket[5];
        //private static byte[] result = new byte[1024];
        public void ServerStart()
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            TcpListener serverSocket=new TcpListener(ip,myProt);
            serverSocket.Start();
            //int counter=0;
            Console.WriteLine("Start monitoring {0} successfully", ((IPEndPoint)serverSocket.LocalEndpoint).Address.ToString());
            while(true)
            {
                /*
                while(!ListenClientConnect.Pending())
                {
                    Thread.Sleep(500);

                }
                */
                TcpClient tcpClient=serverSocket.AcceptTcpClient();
                Console.WriteLine("I am listening for connections from " + IPAddress.Parse(((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString()) +"on port number " + ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Port.ToString());
                ThreadPool.QueueUserWorkItem(new WaitCallback(HandleConnection), tcpClient);

                /*
                switch (((IPEndPoint)tcpClient.Client.RemoteEndPoint).Port.ToString())
                {
                    case "5000":        //Contribution
                        ThreadPool.QueueUserWorkItem(new WaitCallback(HandleFileConnection),tcpClient);
                        break;
                    case "8000":        //Message
                        ThreadPool.QueueUserWorkItem(new WaitCallback(HandleMsgConnection),tcpClient);
                        break;
                    default:
                        break;
                }
                */
            }
       
        }
        /*
        class ConnectionThread
        {
            private TcpListener  threadListener;
            public ConnectionThread(TcpListener Lis)
            {
                threadListener=Lis;
                ThreadPool.QueueUserWorkItem(new WaitCallback(HandleFileConnection));
                ThreadPool.QueueUserWorkItem(new WaitCallback(HandleMsgConnection));
            }
        }
        */
        private static void HandleFileConnection(object myclient)
        {



        }
        private void WriteBook()
        {
            System.Xml.Serialization.XmlSerializer writer;
            System.Xml.Serialization.XmlSerializer reader;
        }

        private static void Broadcast()                 //Broadcast to all connected client in list_clients
        {
            byte[] ReadBuffer = new byte[1024];
            string ACK = "ACK";
            byte[] writeBuffer = Encoding.ASCII.GetBytes("Refresh" + Environment.NewLine);
            {
                foreach (TcpClient c in list_clients.Values)
                {
                    try
                    {
                        NetworkStream stream = c.GetStream();
                        stream.Write(writeBuffer, 0, writeBuffer.Length);

                        stream.ReadTimeout = 3000;
                        stream.Read(ReadBuffer, 0, ACK.Length);
                        stream.Close();
                    }
                    catch (IOException E)
                    {
                        Console.WriteLine("This Client Disconnected with some error and "+E.Message);
                        string IPIndex = ((IPEndPoint)c.Client.RemoteEndPoint).Address.ToString();
                        lock(_lock)
                            list_clients.Remove(IPIndex);
                    }
                }
            }
        }

        private static void HandleConnection(object obj)
        {
            int Curr_StuID=-2;      //Current StuID;
            TcpClient myclient = (TcpClient)obj;
            NetworkStream ns = myclient.GetStream();
            //counter += 1;
            //Socket myClientSocket = serverSocket.Accept();
            try
            {
                var result = new byte[1024];
                var len = new byte[sizeof(int)];

                string path;
                string recv, StuId, Step;


                //NetworkStream ns= (TcpClient)myclient.GetStream();
                ////**********************Receive file name in string*********************************/////////////
                int length_received = ns.Read(len, 0,4);
                int length = BitConverter.ToInt32(len, 0);
                Console.WriteLine("Send {0} bytes", length);
                int ReceiveNum = ns.Read(result, 0,length);
                string filename = Encoding.ASCII.GetString(result, 0, ReceiveNum);
                Console.WriteLine("receive from client {0} message {1}", ((IPEndPoint)myclient.Client.RemoteEndPoint).Address.ToString(), Encoding.ASCII.GetString(result, 0, ReceiveNum));


                //******************************"WILL CHANGE LATER AFTER DETERMINE IP ADDRESS"***************************////
                //INITIALIZE IP ADDRESS;
                //STUID.ADD("");
                //STUID.ADD("");
                //CORRESPOND STUID TO IP ADDRESS
                /*
                SWITCH (MYCLIENTSOCKET.REMOTEENDPOINT.ADDRESS.TOSTRING())
                {
                    CASE "10.0.1.129"://ZIYI
                        CURR_STUID = -1;
                        CONSOLE.WRITELINE("RECEIVE FROM INSTRCTOR");
                        BREAK;
                    CASE "10.0.1.27"://SURFACE 1
                        CURR_STUID = 0;
                        CONSOLE.WRITELINE("RECEIVE FROM STUDENT WITH ID 0");
                        BREAK;
                    CASE "10.0.1.34": //SURFACE 2 ZIYI'S SURFACE
                        CURR_STUID = 1;
                        CONSOLE.WRITELINE("RECEIVE FROM STUDENT WITH ID 1");
                        BREAK;
                    CASE "10.0.1.17":
                        CURR_STUID = 2;
                        CONSOLE.WRITELINE("RECEIVE FROM STUDENT WITH ID 2");
                        BREAK;
                    DEFAULT:
                        BREAK;
                }
                */
                //CURR_STUID = STUID.INDEXOF(MYCLIENTSOCKET.REMOTEENDPOINT.TOSTRING());
                //CONSOLE.WRITELINE("RECEIVE FROM CLIENT {0} MESSAGE {1}", CURR_STUID,YENCODING.ASCII.GETSTRING(RESULT, 0, RECEIVENUM));
                //******************************"WILL CHANGE LATER AFTER DETERMINE IP ADDRESS"***************************////
                if (filename.Contains("Enter:"))
                {
                    string IPIndex;
                    string Message;
                    lock (_lock)
                    {

                        IPIndex = ((IPEndPoint)myclient.Client.RemoteEndPoint).Address.ToString();
                        if (list_clients.ContainsKey(IPIndex))
                        {
                            Console.WriteLine("Previous connection not properly closed");
                            //list_clients.Remove(IPIndex);
                        }
                        else
                            list_clients.Add(IPIndex, myclient);
                    }


                    while (true)
                    {
                        lock (_lock)                //constantly check if the client is still online
                        {
                            if (!list_clients.ContainsKey(IPIndex))
                            {
                                Console.WriteLine("Already Disconnected");
                                myclient.Client.Shutdown(SocketShutdown.Both);
                                ns.Close();
                                myclient.Close();
                                break;
                            }
                        }
                        try
                        {
                            var len_array = new byte[sizeof(int)];
                            var Msg = new byte[1024];
                            int length_msg = ns.Read(len_array, 0, 4);
                            int Msg_length = BitConverter.ToInt32(len_array, 0);
                            Console.WriteLine("Message size is", Msg_length);
                            int ReceiveMsg = ns.Read(Msg, 0, Msg_length);
                            Message = Encoding.ASCII.GetString(Msg, 0, ReceiveMsg);
                            Console.WriteLine("receive Message: " + Message);
                            //Broadcast();
                            if (Message == "Leave")
                            {
                                lock (_lock)
                                {
                                    IPIndex = ((IPEndPoint)myclient.Client.RemoteEndPoint).Address.ToString();
                                    if (!list_clients.ContainsKey(IPIndex))
                                        Console.WriteLine("Trying to close a session improperly");
                                    else
                                        list_clients.Remove(IPIndex);
                                    myclient.Client.Shutdown(SocketShutdown.Both);
                                    ns.Close();
                                    myclient.Close();
                                    Console.WriteLine(IPIndex + "Left the Chat room");
                                }
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Message Exception" + ex.Message);
                            myclient.Client.Shutdown(SocketShutdown.Both);
                            ns.Close();
                            myclient.Close();
                            break;
                        }

                    }


                }
                /*
                if (filename.Contains("Leave:"))
                {
                    lock (_lock)
                    {
                        string IPIndex = ((IPEndPoint)myclient.Client.RemoteEndPoint).Address.ToString();
                        if (!list_clients.ContainsKey(IPIndex))
                            Console.WriteLine("Trying to close a session improperly");
                        else
                            list_clients.Remove(IPIndex);
                        myclient.Client.Shutdown(SocketShutdown.Both);
                        ns.Close();
                        myclient.Close();
                        Console.WriteLine(IPIndex+"Left the Chat room");
                    }

                }
                */



                //***********************************Long Connection*****************************//
                ///************************Receive Comment Msg**********************************//////
                else if (filename.Contains("M:"))
                {
                    Console.WriteLine("Server is Receiving Message");  //Ana
                    //string[] sArray = (new char[2] { ':', '_' });
                    char[] delimiter = { ':', '_' };
                    string[] sArray = filename.Split(delimiter);
                    //Console.WriteLine("after split, {0}, {1}, {2}, {3}",sArray[0],sArray[1],sArray[2], sArray[3]);
                    StuId = sArray[1];
                    Step = sArray[2];
                    recv = sArray[3];
                    //upvote = sArray[4];
                    //path = "..\\xml\\Stu" + sArray[1] + ".xml";
                    //path = "..\\xml\\Stu.xml";
                    path = "..\\server\\xml\\Stu.xml";
                    string rootpath= "..\\server\\xml";
                    if (!Directory.Exists(rootpath))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(rootpath);
                    }

                    System.Xml.Serialization.XmlSerializer writer;
                    System.Xml.Serialization.XmlSerializer reader;

                    //********************Write to seperate students files********************************//
                    if (!File.Exists(path))
                    {
                        var wfile = new System.IO.StreamWriter(path);
                        var book = new Book();

                        book.BookAdd(recv, StuId, Step);
                        writer = new System.Xml.Serialization.XmlSerializer(typeof(Book));
                        writer.Serialize(wfile, book);
                        wfile.Close();
                        //book.print();
                    }
                    else
                    {
                        /////***********************Read************************///////////
                        reader = new System.Xml.Serialization.XmlSerializer(typeof(Book));
                        System.IO.StreamReader file = new System.IO.StreamReader(path);
                        var book = (Book)reader.Deserialize(file);
                        file.Close();

                        ////************************Write comment/reply***********************///////////
                        int index = Convert.ToInt32(Step);
                        int stuid = Convert.ToInt32(StuId);
                        if (filename.Contains("Reply"))
                            book.ReplyComment(recv, index, stuid);
                        else
                            book.BookAdd(recv, StuId, Step);

                        writer = new System.Xml.Serialization.XmlSerializer(typeof(Book));
                        var wfile = new System.IO.StreamWriter(path);
                        writer.Serialize(wfile, book);
                        wfile.Close();
                        //book.print();
                    }
                    //myClientSocket.Close();

                }

                ///************************Receive Upvote**********************************//////
                else if (filename.Contains("Up:"))
                {

                    Console.WriteLine("Server is Receiving Message");  //Ana
                    char[] delimiter = { ':' };   //Up:Index
                    string[] sArray = filename.Split(delimiter);
                    int index = Convert.ToInt32(sArray[1]);
                    System.Xml.Serialization.XmlSerializer writer;
                    System.Xml.Serialization.XmlSerializer reader;
                    path = "..\\server\\xml\\Stu.xml";

                    /////***********************Read************************///////////
                    reader = new System.Xml.Serialization.XmlSerializer(typeof(Book));
                    System.IO.StreamReader file = new System.IO.StreamReader(path);
                    var book = (Book)reader.Deserialize(file);
                    file.Close();

                    ////************************Write***********************///////////
                    book.BookUpvote(index);
                    writer = new System.Xml.Serialization.XmlSerializer(typeof(Book));
                    var wfile = new System.IO.StreamWriter(path);
                    writer.Serialize(wfile, book);
                    wfile.Close();
                    //book.print();

                }

                //////********************** Receive Pin Msg *************************************************///  
                else if (filename.Contains("PIN:"))
                {
                    Console.WriteLine("Server is Receiving Pin Message Request");  //Ana
                    char[] delimiter = { ':' };   //Up:Index
                    string[] sArray = filename.Split(delimiter);
                    int index = Convert.ToInt32(sArray[1]);
                    System.Xml.Serialization.XmlSerializer writer;
                    System.Xml.Serialization.XmlSerializer reader;
                    path = "..\\server\\xml\\Stu.xml";

                    /////***********************Read************************///////////
                    reader = new System.Xml.Serialization.XmlSerializer(typeof(Book));
                    System.IO.StreamReader file = new System.IO.StreamReader(path);
                    var book = (Book)reader.Deserialize(file);
                    file.Close();
                    ////************************Write***********************///////////
                    book.Pin(index);
                    //Console.WriteLine("Index=" + index);
                    writer = new System.Xml.Serialization.XmlSerializer(typeof(Book));
                    var wfile = new System.IO.StreamWriter(path);
                    writer.Serialize(wfile, book);
                    wfile.Close();
                    //book.print();
                }
                
                //***********************************Long Connection*****************************//



                //////**********************Receive Resource File from Instrcutor and Put them in Seperate Folders*************************************************///                    
                else if (filename.Contains("wav") || filename.Contains("jpg") || filename.Contains("obj") || filename.Contains("xml") || filename.Contains("mp4") || filename.Contains("png"))
                {
                    Console.WriteLine("Server is Receiving Project Files Created by the Instructor");  //Ana
                    string StuRepo = "Resource_" + Curr_StuID.ToString();
                    string PATH = "..\\server\\" + StuRepo;
                    //Directory.CreateDirectory(PATH);
                    string[] FileFolders = { "Audio", "CAD", "Data", "Image", "Video" };
                    string[] SubPath = { "", "", "", "", "" };
                    for (int i = 0; i < FileFolders.Length; i++)
                        SubPath[i] = Path.Combine(PATH, FileFolders[i]);

                    if (!Directory.Exists(PATH))
                    {
                        Directory.CreateDirectory(PATH);
                        for (int i = 0; i < FileFolders.Length; i++)
                        {
                            Directory.CreateDirectory(SubPath[i]);
                        }
                    }

                    FileStream fs;

                    switch (Path.GetExtension(filename))
                    {
                        case ".wav":
                            path = Path.Combine(SubPath[0], filename);
                            break;
                        case ".obj":
                            path = Path.Combine(SubPath[1], filename);
                            break;
                        case ".xml":
                            path = Path.Combine(SubPath[2], filename);
                            break;
                        case ".png":
                            path = Path.Combine(SubPath[3], filename);
                            break;
                        case ".jpg":
                            path = Path.Combine(SubPath[3], filename);
                            break;
                        case ".mp4":
                            path = Path.Combine(SubPath[4], filename);
                            break;
                        default:
                            path = Path.Combine(SubPath[4], "Error");
                            break;
                    }
                    fs = new FileStream(path, FileMode.Create);
                    var buffer = new byte[1024];
                    int bytesRead;
                    while ((bytesRead = ns.Read(buffer,0,buffer.Length)) > 0)
                    {
                        fs.Write(buffer, 0, bytesRead);
                        fs.Flush();
                    }

                    fs.Close();
                    //Console.WriteLine("Sucessfully write file");}
                    myclient.Client.Shutdown(SocketShutdown.Both);
                    ns.Close();
                    myclient.Close();
                    Console.WriteLine("disconnected");
                }

                //**********************Send  Mp4 / Pic/ Voice File and comment back to the client*************************************************///             
                else if (filename.Contains("Request"))
                {

                    //Console.WriteLine(filename);
                    //**********************************Client Ask For Comment*********************************//

                    if (filename.Contains("_C:"))
                    {
                        Console.WriteLine("Student {0} is Requesting for Recently Posted Messages", Curr_StuID);  //Ana
                        path = "..\\server\\xml\\Stu.xml";
                        if (!File.Exists(path))
                        {
                            System.Xml.Serialization.XmlSerializer writer;
                            var wfile = new System.IO.StreamWriter(path);
                            var book = new Book();
                            writer = new System.Xml.Serialization.XmlSerializer(typeof(Book));
                            writer.Serialize(wfile, book);
                            wfile.Close();
                            //Console.WriteLine("created file");
                        }
                        myclient.Client.SendFile(path);
                        //Console.WriteLine("successfully sent");
                    }

                    //*********************************Client Ask For File*********************************///
                    //*********************Compress all the files in the directory******************************//
                    else if (filename.Contains("_F:"))
                    {
                        Console.WriteLine("Student {0} is Requesting for Uploaded AR Prject", Curr_StuID);  //Ana
                        //Zip File folder first
                        string startPath = "..\\server\\Resource_-1";
                        string zipPath = "..\\server\\TransmitToClient.zip";
                        if (File.Exists(zipPath))  //Delete Previous ZipFile
                            File.Delete(zipPath);
                        ZipFile.CreateFromDirectory(startPath, zipPath);
                        path = zipPath;
                        myclient.Client.SendFile(path);
                        File.Delete(zipPath);
                        //Console.WriteLine("successfully sent");
                    }
                    myclient.Client.Shutdown(SocketShutdown.Both);
                    ns.Close();
                    myclient.Close();
                    Console.WriteLine("disconnected");
                }
                //**********************Receive  commited Files from the client*************
                else if (filename.Contains("zip"))
                {
                    Console.WriteLine("Receiving commited File {0} from Student", Curr_StuID);  //Ana
                    //path = "..\\server\\" + filename;
                    string FolderName = "..\\server\\Contribution\\";
                    if (!Directory.Exists(FolderName))
                        Directory.CreateDirectory(FolderName);
                    path = FolderName + "Contribution_" + Curr_StuID.ToString()+".zip";
                    string zipPath = path;
                    //string extractPath = Path.Combine("..\\server", "Contribution_" + Path.GetFileNameWithoutExtension(path));
                    //Console.WriteLine("Receive Contribution File {0} from Student", extractPath);
                    if (File.Exists(zipPath))
                        File.Delete(zipPath);

                    FileStream fss = new FileStream(path, FileMode.Create);
                    var buffer = new byte[1024];
                    int bytesRead;
                    while ((bytesRead = ns.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fss.Write(buffer, 0, bytesRead);
                        fss.Flush();
                    }
                    fss.Close();
                    myclient.Client.Shutdown(SocketShutdown.Both);
                    ns.Close();
                    myclient.Close();
                    Console.WriteLine("disconnected");
                    /*
                    if (Directory.Exists(extractPath))
                        Directory.Delete(extractPath, true);
                    ZipFile.ExtractToDirectory(zipPath, extractPath);
                    if (File.Exists(zipPath))
                        File.Delete(zipPath);
                    */
                }

                //*****Instructor is fetching the files form the cloud****************//
                else if (filename.Contains("InFetch"))
                {
                    Console.WriteLine("Instructor Request to see the Student's contribution"); //Ana
                    //The instructor has to decompress itself
                    //Send All the compressed files
                    string FolderName = "..\\server\\Contribution\\";
                    string[] fileNames = Directory.GetFiles(FolderName);
                        
                    //Zip File folder first
                    string startPath = FolderName;
                    string zipPath = "..\\server\\StuCommit.zip";
                    if (File.Exists(zipPath))  //Delete Previous ZipFile
                        File.Delete(zipPath);
                    ZipFile.CreateFromDirectory(startPath, zipPath);
                    path = zipPath;
                    myclient.Client.SendFile(path);
                    File.Delete(zipPath);
                    //Console.WriteLine("successfully sent");
                    myclient.Client.Shutdown(SocketShutdown.Both);
                    ns.Close();
                    myclient.Close();
                    Console.WriteLine("disconnected");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Total Exception"+ex.Message);
                myclient.Client.Shutdown(SocketShutdown.Both);
                ns.Close();
                myclient.Close();
            }
            /*
            myclient.Client.Shutdown(SocketShutdown.Both);
            ns.Close();
            myclient.Close();
            Console.WriteLine("disconnected");
            */
            
        }
    }
}
