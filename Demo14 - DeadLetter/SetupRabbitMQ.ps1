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

Write-Host "Creating Dead Letter Exchange"
$exchangeType = [RabbitMQ.Client.ExchangeType]::Fanout
$model.ExchangeDeclare("Demo14.DeadLetterExchange", $exchangeType, $true, $false, $null)

Write-Host "Creating Dead Letter Queue "
$model.QueueDeclare("Demo14.DeadLetter", $true, $false, $false, $null)
$model.QueueBind("Demo14.DeadLetter", "Demo14.DeadLetterExchange", "", $null)

Write-Host "Creating Queue"
$args = New-Object "System.Collections.Generic.Dictionary``2[System.String,System.Object]"
$args.Add("x-dead-letter-exchange", "Demo14.DeadLetterExchange")
$model.QueueDeclare("Demo14.Normal", $true, $false, $false, $args)

Write-Host "Setup complete"
