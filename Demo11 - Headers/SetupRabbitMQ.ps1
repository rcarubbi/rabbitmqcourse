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
$exchangeType = [RabbitMQ.Client.ExchangeType]::Headers
$model.ExchangeDeclare("Demo11.Exchange", $exchangeType, $true, $false, $null)

Write-Host "Creating Queue 1"
$model.QueueDeclare("Demo11.Queue1", $true, $false, $false, $null)

$subscription1 = New-Object "System.Collections.Generic.Dictionary``2[System.String,System.Object]"
$subscription1.Add("material", "wood")
$subscription1.Add("customertype", "b2b")
 
$model.QueueBind("Demo11.Queue1", "Demo11.Exchange", "", $subscription1)

Write-Host "Creating Queue 2"
$model.QueueDeclare("Demo11.Queue2", $true, $false, $false, $null)

$subscription2 = New-Object "System.Collections.Generic.Dictionary``2[System.String,System.Object]"
$subscription2.Add("material", "metal")
$subscription2.Add("customertype", "b2c")

$model.QueueBind("Demo11.Queue2", "Demo11.Exchange", "", $subscription2)

Write-Host "Creating Queue 3"
$model.QueueDeclare("Demo11.Queue3", $true, $false, $false, $null)

$subscription3 = New-Object "System.Collections.Generic.Dictionary``2[System.String,System.Object]"
$subscription3.Add("x-match", "any")
$subscription3.Add("material", "wood")
$subscription3.Add("customertype", "b2b")

$model.QueueBind("Demo11.Queue3", "Demo11.Exchange",  "", $subscription3)

Write-Host "Creating Queue 4"
$model.QueueDeclare("Demo11.Queue4", $true, $false, $false, $null)

$subscription4 = New-Object "System.Collections.Generic.Dictionary``2[System.String,System.Object]"
$subscription4.Add("x-match", "any")
$subscription4.Add("material", "metal")
$subscription4.Add("customertype", "b2c")

$model.QueueBind("Demo11.Queue4", "Demo11.Exchange",  "", $subscription4)


Write-Host "Setup complete"
