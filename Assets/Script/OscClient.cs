using System.Net;
using System.Net.Sockets;

public class OscClient
{
    private UdpClient _udp;
    private IPEndPoint _endPoint;

    public void Configure(string ip, int port)
    {
        _udp?.Close();
        _udp = new UdpClient();
        _endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
    }

    public void Send(byte[] data)
    {
        _udp?.Send(data, data.Length, _endPoint);
    }
}