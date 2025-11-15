using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace WOLRunner;
    
internal class Server {
    private readonly UdpClient udpClient;

    public Server(IPEndPoint endpoint) {
        udpClient = new UdpClient(endpoint);
    }

    private static WOL.Command? ParseCommand(UdpReceiveResult msg) {
        WOL.Command? cmd = new();

        int size = Marshal.SizeOf(cmd);

        if (msg.Buffer.Length < size) {
            Console.Error.WriteLine("Length of received data does not match struct size. Malformed or missing data.");
            return null;
        }

        nint ptr = Marshal.AllocHGlobal(size);
        Marshal.Copy(msg.Buffer, 0, ptr, size);
        if (Marshal.PtrToStructure(ptr, cmd.GetType()) is WOL.Command c)
            return c;

        Console.Error.WriteLine("Unexpected type marshaled. Failed");
        return null;
    }

    public async Task Start() {
        while (true) {
            try {
                var msg = await udpClient.ReceiveAsync();
                WOL.Command? cmd = ParseCommand(msg);
                if (cmd is null)
                    continue;

                WOL.Packet p = new(
                    new(cmd.Value.MacAddress),
                    new(cmd.Value.IpAddress)
                );

                p.SendPacket();

            } catch (Exception ex) {
                Console.Error.WriteLine(ex.Message);
            }
        }
    }
}