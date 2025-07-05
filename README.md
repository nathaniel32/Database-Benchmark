config.json

```json
{
    "ConnectionStrings": {
        "DbConnection": "Server=___;Database=___;User Id=___;Password=___;"
    }
}
```

<!-- ## Table-Valued Functions (TVF) Setup

Before running the TVF benchmarks, you need to create the necessary Table-Valued Functions in your SQL Server database. Execute the `dataset/tvf.sql` script against your database.
 -->

## Run in Terminal

```shell
dotnet run
```

After running `dotnet run`, you will see new options in the menu for benchmarking with TVFs (options 6-9).

## Run in VS

```shell
dotnet new sln -n csdb
```


```shell
dotnet sln csdb.sln add csdb.csproj
```