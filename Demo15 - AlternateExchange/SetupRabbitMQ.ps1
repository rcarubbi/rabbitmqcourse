param([String]$RabbitDllPath = "not specified")
Set-ExecutionPolicy Unrestricted

Write-Host "Rabbit Dll Path: "
Write-Host $RabbitDllPath -ForegroundColor Green

$absoluteRabbitDllPath = Resolve-Path $RabbitDllPath

Write-Host "Absolute Rabbit Dll Path: "
Write-Host $absoluteRabbitDllPath -ForegroundColor Green

[Reflection.Assembly]::LoadFile($absoluteRabbitDllPath)

Write-Host "Setting up RabbitMQ Connection Factory"
$factory = New-Object -TypeName RabbitMQ.Client.ConnectionFactory
$hostNameProp = [RabbitMQ.Client.ConnectionFactory].GetProperty("HostName")
$hostNameProp.SetValue($factory, "localhost")

$usernameProp = [RabbitMQ.Client.ConnectionFactory].GetProperty("UserName")
$usernameProp.SetValue($factory, "guest")

$passwordProp = [RabbitMQ.Client.ConnectionFactory].GetProperty("Password")
$passwordProp.SetValue($factory, "guest")

$createConnectionMethod = [RabbitMQ.Client.ConnectionFactory].GetMethod("CreateConnection", [Type]::EmptyTypes)
$connection = $createConnectionMethod.Invoke($factory, "instance,public", $null, $null, $null)

Write-Host "Setting up RabbitMQ Model"
$model = $connection.CreateModel()

Write-Host "Creating Alternative Exchange"
$exchangeType = [RabbitMQ.Client.ExchangeType]::Fanout
$model.ExchangeDeclare("Demo15.FailuresExchange", $exchangeType, $true, $false, $null)

Write-Host "Creating Oranges Queue"
$model.QueueDeclare("Demo15.Failures", $true, $false, $false, $null)
$model.QueueBind("Demo15.Failures", "Demo15.FailuresExchange", $null, $null)

Write-Host "Creating Exchange"
$exchangeType = [RabbitMQ.Client.ExchangeType]::Topic
$args = New-Object "System.Collections.Generic.Dictionary``2[System.String,System.Object]"
$args.Add("alternate-exchange", "Demo15.FailuresExchange")
$model.ExchangeDeclare("Demo15.Exchange", $exchangeType, $true, $false, $args)

Write-Host "Creating Apples Queue"
$model.QueueDeclare("Demo15.Apples", $true, $false, $false, $null)
$model.QueueBind("Demo15.Apples", "Demo15.Exchange", "apples", $null)

Write-Host "Creating Oranges Queue"
$model.QueueDeclare("Demo15.Oranges", $true, $false, $false, $null)
$model.QueueBind("Demo15.Oranges", "Demo15.Exchange",  "oranges", $null)

Write-Host "Setup complete"
