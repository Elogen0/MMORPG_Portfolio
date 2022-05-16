using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using DummyClient;
using Google.Protobuf;
using Kame;
using ServerCore;
using UnityEngine;

public class NetworkManager : SingletonMono<NetworkManager>
{
    public int AccountId { get; set; }
    public int Token { get; set; }
    
    private ServerSession _session = new ServerSession();

    public void Send(IMessage packet)
    {
        _session.Send(packet);
    } 
    
    public void ConnectToGame(ServerInfo info)
    {
        IPAddress ipAddr = IPAddress.Parse(info.IpAddress);
        IPEndPoint endPoint = new IPEndPoint(ipAddr, info.Port);
        
        Connector connector = new Connector();
        connector.Connect(endPoint, () => { return _session; }, 1);
    }

    void Update()
    {
        List<PacketMessage> list = PacketQueue.Instance.PopAll();
        foreach (var packet in list)
        {
            var handler = PacketManager.Instance.GetPacketHandler(packet.Id);
            handler?.Invoke(_session, packet.message);
        }
    }
}
