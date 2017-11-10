﻿# ----------------------------------------------------------------------------------
#
# Copyright Microsoft Corporation
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
# http://www.apache.org/licenses/LICENSE-2.0
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.
# ----------------------------------------------------------------------------------

<#
.SYNOPSIS
Test New-AzureRmContainerRegistry, Get-AzureRmContainerRegistry, Update-AzureRmContainerRegistry, and Remove-AzureRmContainerRegistry.
#>
function Test-AzureContainerRegistry
{
    # Setup
    $resourceGroupName = Get-RandomResourceGroupName
    $classicRegistryName = Get-RandomRegistryName
    $location = Get-ProviderLocation "Microsoft.ContainerRegistry/registries"

	try
	{
		New-AzureRmResourceGroup -Name $resourceGroupName -Location $location

		# Creating a container registry with a default new storage account
		$classicRegistry = New-AzureRmContainerRegistry -ResourceGroupName $resourceGroupName -Name $classicRegistryName -Sku "Classic"
		Verify-ContainerRegistry $classicRegistry $resourceGroupName $classicRegistryName "Classic" $null $false

		# Check if the registry name already exists
		$nameStatus = Test-AzureRmContainerRegistryNameAvailability -Name $classicRegistryName
		Assert-True {!$nameStatus.nameAvailable}
		Assert-AreEqual "AlreadyExists" $nameStatus.Reason
		Assert-AreEqual "The registry $($classicRegistryName) is already in use." $nameStatus.Message

		# Create different sku registries
		$storageAccountName = $classicRegistry.StorageAccountName
		$retrievedRegistry = Get-AzureRmContainerRegistry -ResourceGroupName $resourceGroupName -Name $classicRegistryName
		Verify-ContainerRegistry $retrievedRegistry $resourceGroupName $classicRegistryName "Classic" $storageAccountName $false

		$basicRegistryName = Get-RandomRegistryName
		$basicRegistry = New-AzureRmContainerRegistry -ResourceGroupName $resourceGroupName -Name $basicRegistryName -Sku "Basic" -EnableAdminUser
		Verify-ContainerRegistry $basicRegistry $resourceGroupName $basicRegistryName "Basic" $null $true

		$standardRegistryName = Get-RandomRegistryName
		$standardRegistry = New-AzureRmContainerRegistry -ResourceGroupName $resourceGroupName -Name $standardRegistryName -Sku "Standard"
		Verify-ContainerRegistry $standardRegistry $resourceGroupName $standardRegistryName "Standard" $null $false

		$premiumRegistryName = Get-RandomRegistryName
		$premiumRegistry = New-AzureRmContainerRegistry -ResourceGroupName $resourceGroupName -Name $premiumRegistryName -Sku "Premium"
		Verify-ContainerRegistry $premiumRegistry $resourceGroupName $premiumRegistryName "Premium" $null $false

		# Check registry usage
		$usage = Get-AzureRmContainerRegistry -ResourceGroupName $resourceGroupName -Name $premiumRegistryName -ShowUsage
		Assert-AreEqual "Size" $usage[0].Name
		Assert-AreEqual "Webhooks" $usage[0].Name

		# Get list of container registries under a resource group
		$registries = Get-AzureRmContainerRegistry -ResourceGroupName $resourceGroupName
		Assert-AreEqual 4 $registries.Count
		foreach($r in $registries)
		{
			switch($r.SkuName)
			{
				"Classic" { Verify-ContainerRegistry $r $resourceGroupName $classicRegistryName "Classic" $storageAccountName $false }
				"Basic" { Verify-ContainerRegistry $r $resourceGroupName $basicRegistryName "Basic" $null $true }
				"Standard" { Verify-ContainerRegistry $r $resourceGroupName $standardRegistryName "Standard" $null $false }
				"Premium" { Verify-ContainerRegistry $r $resourceGroupName $premiumRegistryName "Premium" $null $false }
			}
		}

		# Delete container registry
		Remove-AzureRmContainerRegistry -ResourceGroupName $resourceGroupName -Name $classicRegistryName
		Remove-AzureRmContainerRegistry -ResourceGroupName $resourceGroupName -Name $basicRegistryName
		Remove-AzureRmContainerRegistry -ResourceGroupName $resourceGroupName -Name $standardRegistryName
		Remove-AzureRmContainerRegistry -ResourceGroupName $resourceGroupName -Name $premiumRegistryName
		$registries = Get-AzureRmContainerRegistry -ResourceGroupName $resourceGroupName
		Assert-AreEqual 0 $registries.Count

		# Creating a container registry with an existing storage account
		$classicRegistryName = Get-RandomRegistryName
		$classicRegistry = New-AzureRmContainerRegistry -ResourceGroupName $resourceGroupName -Name $classicRegistryName -Sku "Classic" -StorageAccountName $storageAccountName
		Verify-ContainerRegistry $classicRegistry $resourceGroupName $classicRegistryName "Classic" $storageAccountName $false

		# Creating a premium sku container registry with an existing storage account, storage account should be ignored
		$premiumRegistryName = Get-RandomRegistryName
		$premiumRegistry = New-AzureRmContainerRegistry -ResourceGroupName $resourceGroupName -Name $premiumRegistryName -Sku "Premium" -StorageAccountName $storageAccountName
		Verify-ContainerRegistry $premiumRegistry $resourceGroupName $premiumRegistryName "Premium" $null $false
    
		# update classic sku container registry
		$updatedClassicRegistry = Update-AzureRmContainerRegistry -ResourceGroupName $resourceGroupName -Name $classicRegistryName -EnableAdminUser -StorageAccountName $storageAccountName
		Verify-ContainerRegistry $updatedClassicRegistry $resourceGroupName $classicRegistryName "Classic" $storageAccountName $true
	
		# update premium sku container registry
		Assert-Error {Update-AzureRmContainerRegistry -ResourceGroupName $resourceGroupName -Name $premiumRegistryName -EnableAdminUser -StorageAccountName $storageAccountName} "Storage account cannot be updated in SKU Premium"

		Remove-AzureRmContainerRegistry -ResourceGroupName $resourceGroupName -Name $classicRegistryName	
	}
	finally
	{
		Remove-AzureRmResourceGroup -Name $resourceGroupName -Force
	}
}

<#
.SYNOPSIS
verify registry object has right property values
#>
function Verify-ContainerRegistry
{
	param([Microsoft.Azure.Commands.ContainerRegistry.PSContainerRegistry] $registry, [string] $resourceGroupName, [string] $registryName, [string] $sku, [string] $storageAccountName, [bool] $adminUserEnabled) 

	Assert-NotNull $registry
	Assert-AreEqual $resourceGroupName $registry.ResourceGroupName
    Assert-AreEqual $registryName  $registry.Name
    Assert-AreEqual "Microsoft.ContainerRegistry/registries" $registry.Type
    Assert-AreEqual $sku $registry.SkuName
    Assert-AreEqual $sku $registry.SkuTier 
    Assert-AreEqual "$($registryName.ToLower()).azurecr.io" $registry.LoginServer
    Assert-AreEqual "Succeeded" $registry.ProvisioningState
    Assert-AreEqual $adminUserEnabled $registry.AdminUserEnabled
	If($sku -eq 'Classic')
	{
		If(!$storageAccountName)
		{
			Assert-NotNull $registry.StorageAccountName
		}
		Else
		{
			Assert-AreEqual $storageAccountName $registry.StorageAccountName
		}
	}
	Else
	{
		Assert-Null $registry.StorageAccountName
	}
}

<#
.SYNOPSIS
Test Get-AzureRmContainerRegistryCredential and Update-AzureRmContainerRegistryCredential.
#>
function Test-AzureContainerRegistryCredential
{
    # Setup
    $resourceGroupName = Get-RandomResourceGroupName    
    $location = Get-ProviderLocation "Microsoft.ContainerRegistry/registries"

    New-AzureRmResourceGroup -Name $resourceGroupName -Location $location
	
	Test-AzureContainerRegistryCredentialBySku $resourceGroupName $location "Classic"

	Test-AzureContainerRegistryCredentialBySku $resourceGroupName $location "Basic"	

	Test-AzureContainerRegistryCredentialBySku $resourceGroupName $location "Standard"	

	Test-AzureContainerRegistryCredentialBySku $resourceGroupName $location "Premium"	

    Remove-AzureRmResourceGroup -Name $resourceGroupName -Force
}

function Test-AzureContainerRegistryCredentialBySku
{
	param([string] $resourceGroupName, [string] $location, [string] $sku) 

	$registryName = Get-RandomRegistryName
    # Creating a container registry with a default new storage account
    $registry = New-AzureRmContainerRegistry -ResourceGroupName $resourceGroupName -Name $registryName -Sku $sku -EnableAdminUser
	Verify-ContainerRegistry $registry $resourceGroupName $registryName $sku $null $true

    $credential = Get-AzureRmContainerRegistryCredential -ResourceGroupName $resourceGroupName -Name $registryName
    Assert-AreEqual $registryName $credential.Username
    Assert-NotNull $credential.Password
    Assert-NotNull $credential.Password2

    $newCredential1 = Update-AzureRmContainerRegistryCredential -ResourceGroupName $resourceGroupName -Name $registryName -PasswordName Password
    Assert-AreEqual $registryName $newCredential1.Username
    Assert-AreNotEqual $credential.Password $newCredential1.Password
    Assert-AreEqual $credential.Password2 $newCredential1.Password2

    $newCredential2 = Update-AzureRmContainerRegistryCredential -ResourceGroupName $resourceGroupName -Name $registryName -PasswordName Password2
    Assert-AreEqual $registryName $newCredential2.Username
    Assert-AreEqual $newCredential1.Password $newCredential2.Password
    Assert-NotNull $newCredential1.Password2 $newCredential2.Password2
}

<#
.SYNOPSIS
Test Test-AzureRmContainerRegistryNameAvailability
#>
function Test-AzureContainerRegistryNameAvailability
{
    # The chance of this randomly generated name has a duplication is rare
    $nameStatus = Test-AzureRmContainerRegistryNameAvailability -Name $(Get-RandomRegistryName)
    Assert-True {$nameStatus.nameAvailable}
    Assert-Null $nameStatus.Reason
    Assert-Null $nameStatus.Message

    $nameStatus = Test-AzureRmContainerRegistryNameAvailability -Name "Microsoft"
    Assert-True {!$nameStatus.nameAvailable}
    Assert-AreEqual "Invalid" $nameStatus.Reason
    Assert-AreEqual "The specified resource name is disallowed" $nameStatus.Message
}


function Test-AzureContainerRegistryReplication
{
	# Setup
    $resourceGroupName = Get-RandomResourceGroupName    
    $location = Get-ProviderLocation "Microsoft.ContainerRegistry/registries"

	try
	{
		$replicationLocation = "centralus"
		$replicationLocation2 = "westus2"
		New-AzureRmResourceGroup -Name $resourceGroupName -Location $location

		# Classic sku doesn't support replication
		$classicRegistryName = Get-RandomRegistryName
		$classicRegistry = New-AzureRmContainerRegistry -ResourceGroupName $resourceGroupName -Name $classicRegistryName -Sku "Classic" -Location $location
		Assert-Error {New-AzureRmContainerRegistryReplication -Registry $classicRegistry -Location $replicationLocation} "The resource type replications is not supported for the registry"

		# Basic sku doesn't support replication
		$basicRegistryName = Get-RandomRegistryName
		$basicRegistry = New-AzureRmContainerRegistry -ResourceGroupName $resourceGroupName -Name $basicRegistryName -Sku "Basic" -Location $location
		Assert-Error {New-AzureRmContainerRegistryReplication -Registry $basicRegistry -Location $replicationLocation} "The resource type replications is not supported for the registry"

		# Standard sku doesn't support replication
		$standardRegistryName = Get-RandomRegistryName
		$standardRegistry = New-AzureRmContainerRegistry -ResourceGroupName $resourceGroupName -Name $standardRegistryName -Sku "Standard" -Location $location
		Assert-Error {New-AzureRmContainerRegistryReplication -Registry $standardRegistry -Location $replicationLocation} "The resource type replications is not supported for the registry"

		# Create replication for premium sku
		$premiumRegistryName = Get-RandomRegistryName
		$premiumRegistry = New-AzureRmContainerRegistry -ResourceGroupName $resourceGroupName -Name $premiumRegistryName -Sku "Premium" -Location $location
		$replication = New-AzureRmContainerRegistryReplication -ResourceGroupName $premiumRegistry.ResourceGroupName -RegistryName $premiumRegistry.Name -Location $replicationLocation  -Tag @{key='val'}
		Verify-AzureContainerRegistryReplication $replication $replicationLocation @{key='val'}
		
		# update replication and compare it with retrieved one
		$updatedReplication = Update-AzureRmContainerRegistryReplication -ResourceGroupName $premiumRegistry.ResourceGroupName -RegistryName $premiumRegistry.Name -Name $replication.Name -Tag @{key123='123'}
		$retrievedReplication = Get-AzureRmContainerRegistryReplication -Registry $premiumRegistry -Name $replication.Name
		Verify-AzureContainerRegistryReplication $updatedReplication $retrievedReplication.Location @{key123='123'} $retrievedReplication.Name

		$replication2 = New-AzureRmContainerRegistryReplication -ResourceGroupName $premiumRegistry.ResourceGroupName -RegistryName $premiumRegistry.Name -Location $replicationLocation2
		Verify-AzureContainerRegistryReplication $replication2 $replicationLocation2

		# Get multiple replications from a registry
		$replications = Get-AzureRmContainerRegistryReplication -Registry $premiumRegistry
		Assert-AreEqual 2 $replications.Count

		# Delete one replication
		Remove-AzureRmContainerRegistryReplication -ResourceGroupName $premiumRegistry.ResourceGroupName -RegistryName $premiumRegistry.Name -Name $replication2.Name
		$replications = Get-AzureRmContainerRegistryReplication -ResourceGroupName $premiumRegistry.ResourceGroupName -RegistryName $premiumRegistry.Name
		Assert-AreEqual 1 $replications.Count
	}
	finally
	{
		Remove-AzureRmResourceGroup -Name $resourceGroupName -Force
	}
}

function Verify-AzureContainerRegistryReplication
{
	param([Microsoft.Azure.Commands.ContainerRegistry.PSContainerRegistryReplication] $replication, [string] $location, [System.Collections.Hashtable] $tags = $null, [string] $name = $location)

	Assert-NotNull $replication
	Assert-AreEqual $name $replication.Name
	Assert-AreEqual $location $replication.Location
	Assert-AreEqual "Microsoft.ContainerRegistry/registries/replications" $replication.Type
	Assert-AreEqual "Succeeded"	$replication.ProvisioningState
	Assert-NotNull $replication.StatusTimestamp
	Assert-True { ($replication.Status -eq "Syncing") -or ($replication.Status -eq "Ready") }
	if($tags)
	{
		Verify-Dictionary $tags $replication.Tags
	}
}

function Verify-Dictionary
{
	param([System.Collections.Hashtable] $expected, [System.Collections.Generic.Dictionary`2[System.String,System.String]] $actual)

	Assert-AreEqualArray $expected.Keys $actual.Keys
	Assert-AreEqualArray $expected.Values $actual.Values
}

function Test-AzureContainerRegistryWebhook
{
	# Setup
    $resourceGroupName = Get-RandomResourceGroupName    
    $location = Get-ProviderLocation "Microsoft.ContainerRegistry/registries"

	try
	{
		$replicationLocation = "centralus"
		$replicationLocation2 = "westus2"
		$webhookUri = "http://bing.com"
		$webhookUri2 = "http://microsoft.com"
		New-AzureRmResourceGroup -Name $resourceGroupName -Location $location
		$webhookName = Get-RandomRegistryName

		# Classic sku doesn't support webhook
		$classicRegistryName = Get-RandomRegistryName
		$classicRegistry = New-AzureRmContainerRegistry -ResourceGroupName $resourceGroupName -Name $classicRegistryName -Sku "Classic" -Location $location
		Assert-Error {New-AzureRmContainerRegistryWebhook -Registry $classicRegistry -Location $location -Name $webhookName -Actions "push","delete" -Uri $webhookUri} "The resource type webhooks is not supported for the registry"

		# Create webhook for basic sku
		$basicRegistryName = Get-RandomRegistryName
		$basicRegistry = New-AzureRmContainerRegistry -ResourceGroupName $resourceGroupName -Name $basicRegistryName -Sku "Basic" -Location $location
		$webhook = New-AzureRmContainerRegistryWebhook -Registry $basicRegistry -Name $webhookName -Actions "push","delete" -Uri $webhookUri
		Verify-AzureContainerRegistryWebhook $webhook $webhookName $location "push","delete"

		# Standard sku doesn't support webhook
		$standardRegistryName = Get-RandomRegistryName
		$standardRegistry = New-AzureRmContainerRegistry -ResourceGroupName $resourceGroupName -Name $standardRegistryName -Sku "Standard" -Location $location
		$webhook = New-AzureRmContainerRegistryWebhook -Registry $standardRegistry -Location $location -Name $webhookName -Actions "push","delete" -Uri $webhookUri
		Verify-AzureContainerRegistryWebhook $webhook $webhookName $location "push","delete"

		# Create webhook for premium sku
		$premiumRegistryName = Get-RandomRegistryName		
		$premiumRegistry = New-AzureRmContainerRegistry -ResourceGroupName $resourceGroupName -Name $premiumRegistryName -Sku "Premium" -Location $location
		$webhook = New-AzureRmContainerRegistryWebhook -Registry $premiumRegistry -Name $webhookName -Actions "push","delete" -Uri $webhookUri -Tag @{key='val'} -Scope "foo:*"
		Verify-AzureContainerRegistryWebhook $webhook $webhookName $location "push","delete" "key=val" "enabled" "foo:*"

		# Create another one for same location
		$webhookName2 = Get-RandomRegistryName
		$webhook2 = New-AzureRmContainerRegistryWebhook  -ResourceGroupName $resourceGroupName -RegistryName $premiumRegistryName -Name $webhookName2 -Actions "push" -Uri $webhookUri -Status "Disabled"
		Verify-AzureContainerRegistryWebhook $webhook2 $webhookName2 $location "push" "" "disabled"
		$webhookConfig = Get-AzureRmContainerRegistryWebhook  -ResourceGroupName $resourceGroupName -RegistryName $premiumRegistryName -Name $webhookName2 -GetConfig
		Assert-AreEqual $webhookUri $webhookConfig.ServiceUri

		# Update existing webhook
		$updatedWebhook = Update-AzureRmContainerRegistryWebhook -Registry $premiumRegistry  -Name $webhookName2 -Actions "push","delete" -Uri $webhookUri2 -Status "Enabled" -Tag @{key='val'} -Scope "foo:*" -Headers @{customheader="abc";testheader="123"}
		Verify-AzureContainerRegistryWebhook $updatedWebhook $webhookName2 $location "push","delete" "key=val" "enabled" "foo:*"
		$webhookConfig2 = Get-AzureRmContainerRegistryWebhook  -ResourceGroupName $resourceGroupName -RegistryName $premiumRegistryName -Name $webhookName2 -GetConfig
		Assert-AreEqual $webhookUri2 $webhookConfig2.ServiceUri
		Verify-Dictionary @{customheader="abc";testheader="123"} $webhookConfig2.CustomHeaders

		# Fail to create webhook on location which doesn't have replication
		$webhookName3 = Get-RandomRegistryName
		Assert-Error {New-AzureRmContainerRegistryWebhook  -ResourceGroupName $resourceGroupName -RegistryName $premiumRegistryName -Name $webhookName3 -Actions "push" -Uri $webhookUri -Location $replicationLocation} "The registry resource $($premiumRegistryName) could not be found"

		# Create replication on a new location and create webhook
		New-AzureRmContainerRegistryReplication -ResourceGroupName $resourceGroupName -RegistryName $premiumRegistryName -Location $replicationLocation
		$webhook3 = New-AzureRmContainerRegistryWebhook -ResourceGroupName $resourceGroupName -RegistryName $premiumRegistryName -Name $webhookName3 -Actions "push" -Uri $webhookUri -Location $replicationLocation
		Verify-AzureContainerRegistryWebhook $webhook3 $webhookName3 $replicationLocation "push"

		# Get list of webhooks
		$webhooks = Get-AzureRmContainerRegistryWebhook -Registry $premiumRegistry -List
		Assert-AreEqual 3 $webhooks.Count

		# Ping webhook and check events
		Ping-AzureRmContainerRegistryWebhook -ResourceGroupName $resourceGroupName -RegistryName $premiumRegistryName -Name $webhookName3
		Ping-AzureRmContainerRegistryWebhook -ResourceGroupName $resourceGroupName -RegistryName $premiumRegistryName -Name $webhookName3		
		$pingEvents = Get-AzureRmContainerRegistryWebhook  -ResourceGroupName $resourceGroupName -RegistryName $premiumRegistryName -Name $webhookName3 -ListEvents
		Assert-AreEqual 2 $pingEvents

		$pingEvents = Get-AzureRmContainerRegistryWebhook  -ResourceGroupName $resourceGroupName -RegistryName $premiumRegistryName -Name $webhookName2 -ListEvents
		Assert-AreEqual 0 $pingEvents


		# Delete Webhook
		Remove-AzureRmContainerRegistryWebhook -ResourceGroupName $resourceGroupName -RegistryName $premiumRegistryName -Name $webhookName1
		Remove-AzureRmContainerRegistryWebhook -ResourceGroupName $resourceGroupName -RegistryName $premiumRegistryName -Name $webhookName2
		$webhooks = Get-AzureRmContainerRegistryWebhook -Registry $premiumRegistry -List
		Assert-AreEqual 1 $webhooks.Count
	}
	finally
	{
		Remove-AzureRmResourceGroup -Name $resourceGroupName -Force
	}
}

function Verify-AzureContainerRegistryWebhook
{
	param([Microsoft.Azure.Commands.ContainerRegistry.PSContainerRegistryWebhook] $webhook, [string] $name, [string] $location, [Array] $actions, [string] $tags = "", [string] $status="enabled", [string] $scope = "")

	Assert-NotNull $webhook
	Assert-AreEqual $webhook.Name $name
	Assert-AreEqual $webhook.Type "Microsoft.ContainerRegistry/registries/webhooks"
	Assert-AreEqual $webhook.Location $location
	Assert-AreEqual $webhook.Status $status
	Assert-AreEqual $webhook.ProvisioningState "Succeeded"
	Assert-AreEqualArray $webhook.Actions $actions
	if($scope)
	{
		Assert-AreEqual $webhook.Scope $scope
	}
	if($tags)
	{
		Verify-Dictionary $webhook.Tags $tags
	}
}