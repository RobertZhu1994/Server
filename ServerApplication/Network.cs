using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
        

        //Socket[] Clients = new Socket[5];
        //private static byte[] result = new byte[1024];
        public void ServerStart()
        {
            //IPAddress ip = IPAddress.Parse("127.0.0.1");
            IPAddress ip = IPAddress.Parse("10.0.1.118");
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(ip, myProt));
            serverSocket.Listen(5);
            Console.WriteLine("Start monitoring {0} successfully", serverSocket.LocalEndPoint.ToString());
            ListenClientConnect();
        }
        private static void ListenClientConnect()
        {
            int Curr_StuID=-1;      //Current StuID;
            while (true)
            {
                Socket myClientSocket = serverSocket.Accept();
                try
                {
                    var result = new byte[1024];
                    var len = new byte[sizeof(int)];

                    string path;
                    string recv, StuId, Step;


                    ////**********************Receive file name in string*********************************/////////////
                    int length_received = myClientSocket.Receive(len, 4, SocketFlags.None);
                    int length = BitConverter.ToInt32(len, 0);
                    Console.WriteLine("Send {0} bytes", length);
                    int ReceiveNum = myClientSocket.Receive(result, length, SocketFlags.None);
                    string filename = Encoding.ASCII.GetString(result, 0, ReceiveNum);



                    //******************************"Will Change Later After Determine IP Address"***************************////
                    //Initialize IP address;
                    //StuID.Add("");
                    //StuID.Add("");
                    //Correspond StuID to ip Address
                    /*
                    switch("myClientSocket.RemoteEndPoint.ToString()")
                        case :
                        StuId
                        break;
                        case :
                        break;
                    //Curr_StuID = StuID.IndexOf(myClientSocket.RemoteEndPoint.ToString());
                    //Console.WriteLine("receive from client {0} message {1}", Curr_StuID,yEncoding.ASCII.GetString(result, 0, ReceiveNum));
                     //******************************"Will Change Later After Determine IP Address"***************************////


                    Console.WriteLine("receive from client {0} message {1}", myClientSocket.RemoteEndPoint.ToString(), Encoding.ASCII.GetString(result, 0, ReceiveNum));

                    ///************************Receive Comment Msg**********************************//////
                    if (filename.Contains("M:"))
                    {
                        Console.WriteLine("This connection is trasmitting pure message");
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
                        Console.WriteLine("FileName" + path);
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
                                book.ReplyComment(recv,index, stuid);
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

                    //////**********************Receive Resource File and Put them in Seperate Folders*************************************************///                    
                    else if (filename.Contains("wav") || filename.Contains("jpg") || filename.Contains("obj")|| filename.Contains("xml") || filename.Contains("mp4")|| filename.Contains("png"))
                    {
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
                        while ((bytesRead = myClientSocket.Receive(buffer)) > 0)
                        {
                            fs.Write(buffer, 0, bytesRead);
                            fs.Flush();
                        }

                        fs.Close();
                        //Console.WriteLine("Sucessfully write file");}
                    }

                    //**********************Send  Mp4 / Pic/ Voice File and comment back to the client*************************************************///             
                    else if (filename.Contains("Request"))
                    {
                        path = "..\\server\\xml\\Stu.xml";
                        Console.WriteLine(filename);
                        //**********************************Client Ask For Comment*********************************//
                        if (filename.Contains("_C:"))
                        {
                            if (!File.Exists(path))
                            {
                                System.Xml.Serialization.XmlSerializer writer;
                                var wfile = new System.IO.StreamWriter(path);
                                var book = new Book();
                                writer = new System.Xml.Serialization.XmlSerializer(typeof(Book));
                                writer.Serialize(wfile, book);
                                wfile.Close();
                                Console.WriteLine("created file");
                            }
                            myClientSocket.SendFile(path);
                            Console.WriteLine("successfully sent");
                        }

                        //*********************************Client Ask For file*********************************///
                        else if (filename.Contains("_F:"))
                        {

                        }

                    }
                    

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    myClientSocket.Shutdown(SocketShutdown.Both);
                    myClientSocket.Close();
                }
                myClientSocket.Close();
                Console.WriteLine("disconnected");
            }
        }
    }
}
