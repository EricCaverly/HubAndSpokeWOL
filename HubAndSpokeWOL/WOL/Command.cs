using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace WOL;

public enum Operation {
    Wake,
    Ping
}

public struct Command {
    public byte[] MacAddress;
    public byte[] IpAddress;
    public Operation Operation;
}
