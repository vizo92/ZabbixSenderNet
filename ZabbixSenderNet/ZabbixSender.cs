using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Ysq.Zabbix
{
    public class Sender
    {
        private int _port;
        private string _zabbixServer;

        public Sender(string zabbixServer, int port = 10051)
        {
            _zabbixServer = zabbixServer;
            _port = port;
        }

        public SenderResponse Send(string host, string itemKey, string value)
        {
            dynamic req = new ExpandoObject();
            req.request = "sender data";
            dynamic d = new ExpandoObject();
            d.host = host;
            d.key = itemKey;
            d.value = value;
            req.data = new[] { d };
            var jsonReq = JsonConvert.SerializeObject(req);
            using (var tcpClient = new TcpClient(_zabbixServer, _port))
            using(var networkStream = tcpClient.GetStream())
            {
                var writer = new BinaryWriter(networkStream);
                writer.Write(jsonReq);
                writer.Flush();
                var res = new BinaryReader(networkStream).ReadString();
                return JsonConvert.DeserializeObject<SenderResponse>(res);
            }       
        }
    }
}