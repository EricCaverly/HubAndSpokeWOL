using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace WOL;

public class Packet {
    const int c_wolPort = 9;
    const int c_headerLenth = 6;
    const int c_payloadLength = 6 * 16;

    // Information that will be used to send this packet
    public IPAddress BroadcastAddress { get; set; }
    public PhysicalAddress MACAddress { get; set; }


    /// <summary>
    /// Constructor from actual types
    /// </summary>
    /// <param name="targetBroadcastNetwork"></param>
    /// <param name="macAddress"></param>
    public Packet(IPAddress targetBroadcastNetwork, PhysicalAddress macAddress) {
        BroadcastAddress = targetBroadcastNetwork;
        MACAddress = macAddress;
    }

    
    /// <summary>
    /// Gets this packet in the byte form (0xFF,MAC)
    /// </summary>
    /// <returns>byte[102] of the actual raw packet data</returns>
    public byte[] GetBytes() {
        byte[] bytes = new byte[c_headerLenth + c_payloadLength];
        
        for (int i = 0; i < c_headerLenth; ++i) {
            bytes[i] = 0xff;
        }

        byte[] mac = MACAddress.GetAddressBytes();
        for (int i = 0; i < c_payloadLength / 6; i++) {
            mac.CopyTo(bytes, i + c_headerLenth);
        }

        return bytes;
    }

    
    /// <summary>
    /// Sends this WOL packet using UDP
    /// </summary>
    /// <exception cref="Exception">Typically happens when unexpected number of bytes sent</exception>
    /// <exception cred="SocketException">A socket exception thrown by the UDP client</exception>
    public async void SendPacket() {
        UdpClient client = new();
        IPEndPoint endPoint = new(BroadcastAddress, c_wolPort);

        int bytesSent = await client.SendAsync(GetBytes(), endPoint);
        if (bytesSent != c_headerLenth+c_payloadLength) {
            throw new Exception("Unexpected number of bytes sent");
        }
    }
}
