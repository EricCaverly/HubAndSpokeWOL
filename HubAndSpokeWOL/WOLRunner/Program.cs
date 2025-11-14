using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// curl -v --header 'Content-Type: application/json' -d '{"BroadcastAddress": "192.168.50.3", "MACaddress": "00112233445566"}' http://127.0.0.1:5127/wake
app.MapPost("/wake", Results<Accepted, InternalServerError<string>, BadRequest<string>> (WOL.JSONPacket packet) => {
    WOL.Packet p;
    try {
        p = new(packet);
    } catch (ArgumentException ex) {
        return TypedResults.BadRequest(ex.Message);
    }

    try {
        p.SendPacket();
    } catch (Exception ex) {
        return TypedResults.InternalServerError(ex.Message);
    }

    return TypedResults.Accepted($"{packet.MACAddress}");
});

app.Run();
