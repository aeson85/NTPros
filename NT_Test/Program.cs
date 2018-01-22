using System;
using System.Xml.Linq;
using System.IO;
using System.Text;
using System.Linq;

namespace NT_Test
{
    class Program
    {
        static readonly string a = @"<xml>
            <ToUserName>
                <![CDATA[gh_7cc4bf189586]]>
            </ToUserName>\n
            <FromUserName>
                <![CDATA[orW9-jh1BQUU40t5yqnPH7SDHhzs]]>
            </FromUserName>\n
            <CreateTime>1516538818</CreateTime>\n
            <MsgType>
                <![CDATA[text]]>
            </MsgType>\n
            <Content>
                <![CDATA[包包]]>
            </Content>\n
            <MsgId>6513484626862316315</MsgId>\n
        </xml>";
        static void Main(string[] args)
        {
            var bytes = Encoding.UTF8.GetBytes(a);
            var sr = new MemoryStream(bytes);
            var xDoc = XDocument.Load(sr);
            var root = xDoc.Element("xml");
            var toUserName = GetValueFrom(root, "ToUserName");
            var fromUserName = GetValueFrom(root, "FromUserName");
            var createTime = GetValueFrom(root, "CreateTime");
            var msgType = GetValueFrom(root, "MsgType");
            var content = GetValueFrom(root, "Content");
            var msgId = GetValueFrom(root, "MsgId");

            var b = @"<xml> <ToUserName>< ![CDATA[toUser] ]></ToUserName> <FromUserName>< ![CDATA[fromUser] ]></FromUserName> <CreateTime>12345678</CreateTime> <MsgType>< ![CDATA[text] ]></MsgType> <Content>< ![CDATA[你好] ]></Content> </xml>";
        }

        private static string GetValueFrom(XElement root, string eleName)
        {
            return root.Element(eleName)?.Value;
        }
    }
}
