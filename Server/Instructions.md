# Install .NET 9.0.100
# Via website, google .NET installation

# Configure NUGET
# dotnet nuget list source // Check if nuget is configured/installed
# dotnet nuget add source https://api.nuget.org/v3/index.json --name nuget.org // Add source so project can pull dependencies from this website

# Install dependencies
# dotnet restore

# Commands to run
# dotnet new webapi -o Server // Used to create project files
# dotnet run // Build and run project after it is built

# dotnet watch run // Will monitor changes and deploy them as you are editing the project

# Make a .env file in the Server folder and write the following in the .env file
# MONGODB_CONNECTION_URI="mongodb+srv://<user>:<password>@therahaptics.al8ga.mongodb.net/?retryWrites=true&w=majority&appName=TheraHaptics"