# How to make and use a packet

Packets are for sending data around between clients and server, and do stuff with this data.
For that, the WebmilioCommons mod has useful structures for it we can use to simplify netcode.

A packet is an object that contains properties that will be sent over the net. These properties can be used as fields though.
To create such a packet you extend either one of `NetworkPacket`, `PlayerNetworkPacket` or `ModPlayerNetworkPacket<RORPlayer>`.
The first one is preferred. The two player packets are always tied to the local player instance and have situational uses.
When you make such a class you have to override the `Behavior` field. It can be assigned 4 possible values:

```csharp
public enum NetworkPacketBehavior
    {
    SendToClient = 0, //Sends to a single specified client
    SendToAllClients = 1, //Sends to all clients
    SendToServer = 2, //Sends to the server
    SendToAll = 3 //Sends to the server, which then broadcasts to other clients
}
```

Then you specify any "fields" (properties) via this syntax: `public type Name  { get; set; }` 

Then you start by making (atleast) 2 constructors, an empty one and one that takes in all the data you want to sync.

```csharp
public class MyPacket : NetworkPacket
{
	public override NetworkPacketBehavior Behavior => NetworkPacketBehavior.SendToAll;

	public int SomeInt { get; set; }

	public MyPacket() { }

	public MyPacket(int someInt)
	{
		SomeInt = someInt;
	}
}
```

Now this packet won't do anything by itself because it just contains data. You want to do something on receiving, so we add this:

```csharp
public class MyPacket : NetworkPacket
{
	//previous stuff

	protected override bool PostReceive(BinaryReader reader, int fromWho)
	{
		string rec = "received: " + SomeInt;
		Console.WriteLine(rec); //Will print to server console
		Main.NewText(rec); //Will print to chat
		//Obviously an example, here use both because we have a SendToAll type packet
		//You can have netMode checks here if you need to, or none at all if you know that the receiver is only one side
		return base.PostReceive(reader, fromWho);
	}
}
```

Now we have the functionality defined, and all we have to do is send the packet in the appropriate place in our code like this:
```csharp
//We use the constructor with our data (here, just a single int)
new MyPacket(4).Send();
```

Depending on the `Behavior`, you need netMode checks. But that's the gist of it.

# Sidenote
This requires the below code in the `Mod` class to work. You aren't allowed to use any other code in this hook that reads packet data.
```csharp
public override void HandlePacket(BinaryReader reader, int whoAmI)
{
	NetworkPacketLoader.Instance.HandlePacket(reader, whoAmI);
}
```
