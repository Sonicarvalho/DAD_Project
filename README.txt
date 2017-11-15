Configuration file:
	Since that functionality is not yet implemented all testing must be done through the console, for now.

Functionality example:
	Launch PCS,
	Launch PM,
	Launch a server with StartServer server1 localhost localhost:11001 1 1 (the last two arguments are not being used for now)
	Launch a client with StartClient client1 localhost localhost:9000 1 1 (the last two arguments are not being used for now)
	



Missing functionalities:
	Server- Correct intersects with ghosts and walls. 
	Client- There are some problems when trying to use multiple clients in the same machine. Chat is not fully implemented. 
	PuppetMaster- Freeze, Unfreeze and InjectDelay are not yet implemented. The script reading functionality is not yet implemented. There are still some difficulties when launching clients throught the PM.
