﻿/*
using System;
using System.Net.Sockets;
using System.IO;
namespace ServerApplication
{
    class Client
    {
        public int Index;
        public string IP;
        public TcpClient Socket;
        public NetworkStream myStream;
        private byte[] readbuff;

        public void Start()
        {
            string path = "..\\server\\video1.mp4";

            using (NetworkStream netstream = Socket.GetStream())
            {
                
                using (FileStream fs = new FileStream(path, FileMode.Create))
                    netstream.CopyTo(fs);
            }
        }
        /*
        public void Start()
        {
            Socket.SendBufferSize = 4096;
            Socket.ReceiveBufferSize = 4096;
            myStream = Socket.GetStream();
            Array.Resize(ref readbuff, Socket.ReceiveBufferSize);
            myStream.BeginRead(readbuff, 0, Socket.ReceiveBufferSize, OnReceiveData, null);
        }

        void CloseConnection()
        {
            Socket.Close();
            Socket = null;
            Console.WriteLine("Player   :" + IP);

        }
        void OnReceiveData(IAsyncResult result)
        {
            try
            {
                int readBytes = myStream.EndRead(result);
                if (Socket == null)
                    return;
                if (readBytes <= 0)
                {
                    CloseConnection();
                    return;
                }
                byte[] newBytes = null;
                Array.Resize(ref newBytes, readBytes);
                Buffer.BlockCopy(readbuff, 0, newBytes, 0, readBytes);
                //HandleData
                //********Very Important***********
                //FileStream fs = File.Open("", FileMode.Open);
                if (Socket == null)
                {
                    return;
                }
                myStream.BeginRead(readbuff, 0, Socket.ReceiveBufferSize, OnReceiveData, null);
            }
            catch (Exception ex)
            {
                CloseConnection();
            }
        }
    
    }

}
*/