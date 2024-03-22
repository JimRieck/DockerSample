docker pull mcr.microsoft.com/mssql/server:2022-latest

# Create the DB volume if it doesn't exist
if (-not (docker volume ls -q --filter "name=darkside-leasing-volume" 2>$null)) {
    docker volume create darkside-leasing-volume
}

docker run -d `
    --name Darkside_Leasing_DB `
    -e 'ACCEPT_EULA=Y' `
    -e 'SA_PASSWORD=myPassword123' `
    -e 'AzureWebJobsScriptRoot=/home/site/wwwroot' `
    -e 'AzureFunctionsJobHost__Logging__Console__IsEnabled=true' `
    -p 3627:1433 `
    --volume darkside-leasing-volume:/var/opt/mssql/darksideleasingAPI `
    --hostname Darkside_Leasing_DB `
    mcr.microsoft.com/mssql/server:2022-latest

Start-Sleep -Seconds 5

# Create the database on the container.
$SqlServer    = 'localhost, 3627' # SQL Server instance (HostName\InstanceName for named instance)
$Database     = 'master'      # SQL database to connect to 
$SqlAuthLogin = 'sa'            # SQL Authentication login
$SqlAuthPw    = 'myPassword123'     # SQL Authentication login password

$NewDatabaseName = "Darkside_Leasing_db" # Specify the new database name
$ConnectionString = "Data Source=$SqlServer; User Id=$SqlAuthLogin; Password =$SqlAuthPw; Persist Security Info=True;"

$connection = New-Object System.Data.SqlClient.SqlConnection
$connection.ConnectionString = $connectionString

try {
    # Open the connection
    $connection.Open()
    
    # Create a new SqlCommand object
    $command = $connection.CreateCommand()

    # Check if the database already exists
    $checkDatabaseQuery = "SELECT COUNT(*) FROM sys.databases WHERE name = '$NewDatabaseName'"
    $command.CommandText = $checkDatabaseQuery
    $databaseExists = $command.ExecuteScalar()

    if ($databaseExists -eq 0) {
        # Create the database if it doesn't exist
        $createDatabaseQuery = "CREATE DATABASE $NewDatabaseName"
        $command.CommandText = $createDatabaseQuery
        $command.ExecuteNonQuery()
        Write-Host "Database '$NewDatabaseName' created successfully."
    } else {
        Write-Host "Database '$NewDatabaseName' already exists."
    }
}
catch {
    Write-Host "Error: $_"
}
finally {
    # Close the connection
    $connection.Close()
}
