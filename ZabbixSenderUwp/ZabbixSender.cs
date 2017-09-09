using Newtonsoft.Json;
using System;
using System.Dynamic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Ysq.Zabbix
{
    public class Sender
    {
        private readonly int _port;
        private readonly string _zabbixServer;

        public Sender(string zabbixServer, int port = 10051)
        {
            _zabbixServer = zabbixServer;
            _port = port;
        }

        public async Task<SenderResponse> SendAsync(string host, string itemKey, string value, int timeout = 500)
        {
            dynamic req = new ExpandoObject();
            req.request = "sender data";
            dynamic d = new ExpandoObject();
            d.host = host;
            d.key = itemKey;
            d.value = value;
            req.data = new[] { d };
            var jsonReq = JsonConvert.SerializeObject(req);
            using (var tcpClient = new TcpClient())
            {
                await tcpClient.ConnectAsync(_zabbixServer, _port);
                using (var networkStream = tcpClient.GetStream())
                {
                    var data = Encoding.ASCII.GetBytes(jsonReq);
                    networkStream.Write(data, 0, data.Length);
                    networkStream.Flush();
                    var counter = 0;
                    while (!networkStream.DataAvailable)
                    {
                        if (counter < timeout / 50)
                        {
                            counter++;
                            await Task.Delay(50);
                        }
                        else
                        {
                            throw new TimeoutException();
                        }
                    }

                    var resbytes = new Byte[1024];
                    networkStream.Read(resbytes, 0, resbytes.Length);
                    var s = Encoding.UTF8.GetString(resbytes);
                    var jsonRes = s.Substring(s.IndexOf('{'));
                    return JsonConvert.DeserializeObject<SenderResponse>(jsonRes);
                }
            }       
        }

        public SenderResponse Send(string host, string itemKey, string value, int timeout = 500)
        {
            return SendAsync(host, itemKey, value, timeout).Result;
        }
    }
}