# Reddit API

Web API to get random images from reddit
https://redditapitask.azurewebsites.net/swagger/

## Features

- Get random image from `/r/pics` subreddit
- Get history of random images

## Tech

- ASP.NET Core 5.0 Web API
- Entity Framework
- MSSQL

## How to run
Change `server-name` in `appsettings.json` to your local MSSQL server name
```sh
"ConnectionStrings": {
 "DefaultConnection": "Server=server-name;Database=RedditDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

If you are using Visual Studio simply run application, otherwise use following dotnet cli commands to run application

```sh
dotnet restore
dotnet build
dotnet run
```

