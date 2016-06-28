function OnResourceStart()
{
	Console.Print("Starting gamemode...");

	var Vehicle1 = Vehicle("Infernus", Vector3(2785.87, 426.42, 5.82));
	var Vehicle2 = Vehicle("Feltzer", Vector3(2787.87, 429.42, 5.82));
	var Vehicle3 = Vehicle("Turismo", Vector3(2789.87, 431.42, 5.82));

	Console.Print("Gamemode started.");
}

Event("OnResourceStart").AddHandler(OnResourceStart);

function OnPlayerConnected(Player)
{
	Console.Print("Player connected.");
	Player.Spawn(Vector3(2783.87, 426.42, 5.82), 45.0);
	Player.FadeScreenIn(1000);
}

Event("OnPlayerConnected").AddHandler(OnPlayerConnected);

function OnPlayerDisconnected(Player)
{
	Console.Print("Player disconnected.");
}
Event("OnPlayerDisconnected").AddHandler(OnPlayerDisconnected);

function OnElementCreated(Element)
{
}
Event("OnElementCreated").AddHandler(OnElementCreated)

function OnElementDestroyed(Element)
{
}
Event("OnElementDestroyed").AddHandler(OnElementDestroyed);