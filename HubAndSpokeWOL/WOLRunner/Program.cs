using System.Net;

namespace WOLRunner;

internal class Program {


    static async void Main(string[] args) {
        IPEndPoint endpoint = new(
            IPAddress.Any,
            7009
        );

        Server server = new(endpoint);

        await server.Start();
    }
}