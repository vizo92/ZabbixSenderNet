# ZabbixSenderNet
Zabbix Sender Library for .Net Framework

# Overview
ZabbixSenderNet implements [Zabbix Sender Protocol 2.0](https://www.zabbix.org/wiki/Docs/protocols/zabbix_sender/2.0).

# Usage

```C#
var zabbixServer = "192.168.0.1";
var sender = new Ysq.Zabbix.Sender(zabbixServer);
var response = sender.Send("Host1", "trapper.item1", "10");
Console.WriteLine(reponse.Response); // "success" or "fail"
Console.WriteLine(response.Info); // e.g. "Processed 1 Failed 0 Total 1 Seconds spent 0.000253"
```
