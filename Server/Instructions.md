Make sure that you have the pre-requisites installed before-hand:

# Install .NET 9.0.100
(Via website, google ".NET installation 9.0.100")

# Configure NUGET, use the commands below in the terminal
dotnet nuget list source // Check if nuget is configured/installed, if the url below is there, then ignore the command below
dotnet nuget add source https://api.nuget.org/v3/index.json --name nuget.org // Add source so project can pull dependencies from this website

# Install dependencies, run the following command in terminal
dotnet restore

# Manually write in private variables (for your eyes only, dont push this .env file into Github)
# Make a ".env" file in the Server folder and write the following in the .env file:
MONGODB_CONNECTION_URI="mongodb+srv://<user>:<password>@therahaptics.al8ga.mongodb.net/?retryWrites=true&w=majority&appName=TheraHaptics"
JWT_SECRET_KEY=""

# Replace <user> and <password> with your credentials made in MongoDB
# In the quotation marks for the JWT_SECRET_KEY variable above, enter the output after running the following command ran in your terminal:
node -e "console.log(require('crypto').randomBytes(32).toString('hex'))"
# Save the file

# Start server, choose one of the two options, either one will work
dotnet run // Build and run project after it is built
dotnet watch run // Will monitor changes and deploy them as you are editing the project


# Notes for the developers**
dotnet new webapi -o Server // Used to create project files
dotnet add package MongoDB.Driver //  Used to install package MongoDB.Driver into project, it saves the package into the Server.csproj file so it can be recalled when running "dotnet restor"