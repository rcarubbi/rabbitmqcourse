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

Write-Host "Creating Exchange"
$exchangeType = [RabbitMQ.Client.ExchangeType]::Fanout
$model.ExchangeDeclare("Demo7.Exchange", $exchangeType, $true, $false, $null)

Write-Host "Creating Queue 1"
$model.QueueDeclare("Demo7.Queue1", $true, $false, $false, $null)
$model.QueueBind("Demo7.Queue1", "Demo7.Exchange", $null, $null)

Write-Host "Creating Queue 2"
$model.QueueDeclare("Demo7.Queue2", $true, $false, $false, $null)
$model.QueueBind("Demo7.Queue2", "Demo7.Exchange", $null, $null)

Write-Host "Setup complete"
