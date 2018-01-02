﻿using _CUSTOM_CONTROLS._ChatListBox;
using _CUSTOM_CONTROLS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Client
{
    public delegate void Adopt(string m);
    public delegate void Adoptf(ChatListBox chatListBox);
    public delegate void CatPool(string m);
    public delegate bool AdoptB(string m);
    public static class ProcessingCenter
    {
        #region 委托集合

        /// <summary>
        /// 控制是否显示主窗体
        /// </summary>
        private static bool mainF = false;
        public static bool MainF { get { return mainF; } set { mainF = value; } }
        public static bool kongzhi = true;
        public static bool Kongzhi { get { return kongzhi; } set { kongzhi = value; } }
        public static ChatListBox agent { get; set; }

        /// <summary>
        /// 控制 登录 和 消息的委托
        /// </summary>
        private static Adopt adoptL;
        private static Adopt adoptM;
        private static Adoptf adoptF;
        private static Adopt Adopt;
        public static CatPool chatPool;
        public static AdoptB Jdage;

        /// <summary>
        /// 对外控制委托代理的索引
        /// </summary>
        public static Adopt ChageAdoptL { get { return adoptL; } set { adoptL = value; } }
        public static Adopt ChageAdoptM { get { return adoptM; } set { adoptM = value; } }
        public static Adoptf ChageAdoptF { get { return adoptF; } set { adoptF = value; } }
        public static Adopt ChageAdopt { get { return Adopt; } set { Adopt = value; } }
        
        
        
        #endregion

        #region 传送信息统一格式
        /// <summary>
        /// 将一个对象生成josn字符串
        /// </summary>
        /// <param 任意对象类型="obj"></param>
        /// <returns>返回一个json字符串</returns>
        public static string ObjectToJson(object obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            MemoryStream stream = new MemoryStream();
            serializer.WriteObject(stream, obj);
            byte[] dataBytes = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(dataBytes, 0, (int)stream.Length);
            return UTF8Encoding.UTF8.GetString(dataBytes);
        }

        /// <summary>
        /// 将一个josn串解析成对象
        /// </summary>
        /// <param json串="jsonString"></param>
        /// <param 对象类型="obj"></param>
        /// <returns></returns>
        public static object JsonToObject(string jsonString, object obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            MemoryStream mStream = new MemoryStream(UTF8Encoding.UTF8.GetBytes(jsonString));
            return serializer.ReadObject(mStream);
        }

        /// <summary>
        /// 通信方式发送消息
        /// </summary>
        /// <param 类型="T"></param>
        /// <param 账号="Nub"></param>
        /// <param 消息="M"></param>
        /// <param Adopt="Adopt"></param>
        /// <param Adopt="Remenb"></param>
        /// <returns></returns>
        public static string GetJson(string T, string Nub, string M, string Adopt, string Remenb)
        {
            return ProcessingCenter.ObjectToJson(new string[] { T, Nub, M, Adopt, Remenb });
        }

        /// <summary>
        /// 通信方式接受消息的格式转换
        /// </summary>
        /// <param 接受到的json="json"></param>
        /// <returns></returns>
        public static string[] GetType(string json)
        {
            return (string[])ProcessingCenter.JsonToObject(json, new string[] { });
        }
        #endregion

        /// <summary>
        /// 当接受到消息时，消息处理中心
        /// </summary>
        /// <param 接受到的消息="json"></param>
        public static void JodgeType(string json)
        {
            Debug.Print("客户端消息接受中心接收到消息" + json);
            Debug.Print(GetType(json)[0]);
            switch (GetType(json)[0])
            {
                case "L":
                    Debug.Print("接受到的登陆的信息");
                    adoptL(json);
                    if (GetType(json)[1] == "True")
                    {
                        if (GetType(json)[2] != null)
                        {
                            Debug.Print(GetType(json)[2]);
                            while (kongzhi) ;
                            Adopt(GetType(json)[2]);
                        }
                    }
                    break;

                case "M":

                    if (Jdage(GetType(json)[1]))
                    {
                        chatPool(json);
                    }
                    else
                    {
                        Write(json);
                        Debug.Print("已将接受到的消息" + json + "写入文档");
                        adoptM(json);
                    }
                    break;

                case "R":
                    Register(json);
                    break;
            }
        }

        /// <summary>
        /// 发送注册消息
        /// </summary>
        /// <param 注册的账号="id"></param>
        /// <param 注册的昵称="name"></param>
        /// <param 注册的密码="passworld"></param>
        public static void SendRegister(string id, string name, string passworld,string oa)
        {
            Connect.SendMessage(GetJson("R", id, name, passworld, DateTime.Now.ToString()));
        }

        /// <summary>
        /// 发送消息给好友
        /// </summary>
        /// <param 自己的账号="sendid"></param>
        /// <param 朋友的账号="Friendid"></param>
        /// <param 发送的信息="message"></param>
        public static void SendMessage(string sendid, string Friendid, string message)
        {
            Connect.SendMessage(GetJson("M", sendid, Friendid, message, null));
          
        }

        /// <summary>
        /// 发送登录信息
        /// </summary>
        /// <param 登录的账号="id"></param>
        /// <param 登录的密码="passworld"></param>
        /// <param 是否记住密码="Remb"></param>
        /// <param 记住密码相应的主机号="conputerNub"></param>
        public static void SendLand(string id, string passworld, string Remb, string conputerNub)
        {
            Connect.SendMessage(GetJson("L", id, passworld, Remb, conputerNub));
        }

        /// <summary>
        /// 绘制一个组所有成员
        /// </summary>
        /// <param 组名="gourpName"></param>
        /// <param 该组的每个成员="information"></param>
        /// <returns>information 包含的是{组名,成员账号,成员昵称,成员签名}的集合</returns>
        public static ChatListItem DropGourp(string gourpName, List<string[]> information)
        {
            Random rnd = new Random();
            ChatListItem item = new ChatListItem(gourpName);
            foreach (var i in information)
            {
                ChatListSubItem subItem = new ChatListSubItem(i[1], i[2], i[3]);
                subItem.HeadImage = Image.FromFile("head/1 (" + rnd.Next(0, 45) + ").png");
                subItem.Status = (ChatListSubItem.UserStatus)(1);
                item.SubItems.AddAccordingToStatus(subItem);
            }

                item.SubItems.Sort();
            return item;
        }

        /// <summary>
        ///读取文档 
        /// </summary>
        /// <returns>返回文档所有的信息</returns>
        public static string[] Read()
        {
            int J = 0;
            int N = 0;
            string[] strs2 = File.ReadAllLines(@"ReChat.txt", UTF8Encoding.UTF8);
            foreach (var i in strs2) 
            {
                if (i == ""){   J++; }
            }
            string[] strs1 = new string[strs2.Length-J];
            foreach (var item in strs2)
            {
                if (item!="")
                {
                    strs1[N] = item;
                    N++;
                }
            }
            return strs1 ;
        }
        
        /// <summary>
        /// 向文档写入一条数据
        /// </summary>
        /// <param 添加的数据="a"></param>
        public static void Write(string a)
        {
            FileStream fs = new FileStream("ReChat.txt", FileMode.Create);
            //获得字节数组
            byte[] data = System.Text.UTF8Encoding.UTF8.GetBytes(a);
            //开始写入
            fs.Write(data, 0, data.Length);
            //清空缓冲区、关闭流
            fs.Flush();
            fs.Close();
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <param name="a"></param>
        public static void Delegt(string a)
        {
            string[] read = Read();
            for (int i = 0; i < read.Length; i++)
            {
             
                if (read[i] == a)
                {
                    read[i] = "";
                }
            }
            File.WriteAllLines(@"ReChat.txt", read, UTF8Encoding.UTF8);

        }

        /// <summary>
        /// 当发生点击点击事件的时候  查询当前是否发生数据 并删除同步
        /// </summary>
        /// <param 查询的号码="clic"></param>
        /// <returns>如果有好友发来消息，则返回</returns>
        public static string[] ClikRm(string clic)
        {
            string[] test= Read();
            for (int i = 0; i < test.Length; i++)
            {
                Debug.Print("查询到消息记录"+test[i]);
                Debug.Print(test[i]+"和"+clic);
                if (GetType(test[i])[1] == clic)
                {
                    Delegt(test[i]);
                    Debug.Print(test[i]);
                    return GetType(test[i]);


                }
            }
            return null;
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param 接受到的消息 Json="json"></param>
        public static void Register(string json)
        {
            Land.Adopt(GetType(json)[1] == "True");
            ///这里有一个 当接受到系统返回的注册 成功或者失败的提醒
           
        }
            


    }
}