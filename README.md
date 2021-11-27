# SocketFlow

An event-oriented protocol aggregator (tcp/WebSocket)  
_Light_ and _simple_ for use

#### Supported languages

|      | client | server |
|:-----|:------:|:------:|
| C#   | [client](https://www.nuget.org/packages/SocketFlow.Client/) | [server](https://www.nuget.org/packages/SocketFlow.Server/) |
| Js   | [client](https://www.npmjs.com/package/socketflow.client) | -      |
| Java | -      | -      |

### Getting started
#### Server
```cs
var server = new FlowServer(FlowOptions.Lazy);            // Lazy - auto use unknown types as json
server.UsingModule(new TcpModule(IPAddress.Any, 3333));   // To connect via TCP
server.UsingModule(new WebSocketModule("0.0.0.0:3334"));  // To connect via WebSocket (From browser)
server.UsingWrapper(new Utf8DataWrapper());               // To use String as value for transfer

server.Bind<string>(1, (client, value) => {         // Bind handler for id 1.
    Console.WriteLine($"User message: {value}");    // When client send message as string with id 1
});                                                 // server print this message to console.

server.ClientConnected += destinationClient => {    // Subscribe to client connect.
    destinationClient.Send(1, "Hello! I'm server"); // When client was be connect, send message for him.
};                                                  // Pay attention. Id from client to server and id from
                                                    // server to client may be equals. It's not a problem.

server.Start();
```
#### Client
C# client works only via TCP
```cs
var client = new FlowClient(IPAddress.Parse("127.0.0.1"), 3333, FlowOptions.Lazy);
client.UsingWrapper(new Utf8DataWrapper());

client.Bind<string>(1, value => {
    Console.WriteLine($"Server say: {value}");
});

client.Connect();

client.Send(1, "Hello server. I am connect to you to receive realtime values");
```

### Protocol

* Have a simple overhead (8 bytes for every event)
* Ability to transfer 2 GB of event (2147483639 bytes)
* Ability to use your data structure (Named **DataWrapper**)  
  such as:
    - Json
    - Xml
    - Raw bytes
    - Your own structure
* May be created 2 billion different events

Every event looks as:

<table>
    <thead>
        <tr>
            <th></th>
            <th colspan=4>length</th>
            <th colspan=4>type</th>
            <th colspan=6>event data</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>bytes:</td>
            <td>0</td>
            <td>1</td>
            <td>2</td>
            <td>3</td>
            <td>4</td>
            <td>5</td>
            <td>6</td>
            <td>7</td>
            <td>8</td>
            <td>9</td>
            <td>10</td>
            <td>11</td>
            <td>12</td>
            <td>...</td>
        </tr>
    </tbody>
</table>

First 4 bytes (int) it is length of data, no more.  
Second 4 bytes (int) it is type of event, determines which event occurred.  
Then followed by _data_, which will be converted to _value_ using the **data wrapper**.
