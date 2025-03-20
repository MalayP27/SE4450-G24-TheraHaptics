# SE4450-G24-TheraHaptics

Group 24:

Members:  Jan Awad,   Rohan Datta,   Malay Patel,   Zaiyan Azeem

IDs:      jawad2,     rdatta8,       mpate432,      zazeem2

Project: TheraHaptics


To start the project, there are two servers that need to be started outside of the Unity application.

First is the Back-End Server which serves the API. In order to start this server you must have C# installed and the .NET Framework
- cd into the 'Server'
- Run the following command 'dotnet watch run'

Second is the Socket Server which handles the outputs of the Machine Learning model and sends it through the TCP Socket, you must have Python installed
- cd into the 'Hardware/Web_Socket'
- Run the following command 'python 2_1_websocket_server.py'

Now you are ready to run the Unity Build
- Extract the zipped folder 'Build Render'
- Double click on the folder to reveal its contents (may have to do this twice)
- Double click on the TheraHaptics file which will open in full screen