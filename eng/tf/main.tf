terraform {
  backend "azurerm" {
    resource_group_name  = "rg-githubactions-tf-states"
    storage_account_name = "mkgithubtfstates"
    container_name       = "tfstatedevops"
    key                  = "terraformgithubexample.tfstate"
  }
}

provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "rg" {
  name     = "rg-${var.application_name}-${var.stage}"
  location = var.location
}

resource "azurerm_storage_account" "sa" {
  name                     = "${var.application_name}${var.stage}sa"
  resource_group_name      = azurerm_resource_group.rg.name
  location                 = azurerm_resource_group.rg.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_service_plan" "svc_plan" {
  name                = "${var.application_name}-${var.stage}-svc-plan"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  os_type             = "Linux"
  sku_name            = "Y1"
}

resource "azurerm_application_insights" "ai" {
  name                = "${var.application_name}-${var.stage}-ai"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  application_type    = "web"
}

resource "azurerm_cosmosdb_account" "cosmos_acc" {
  name                = "${var.application_name}-${var.stage}-cosmos-acc"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  offer_type          = "Standard"
  capabilities {
    name = "EnableServerless"
  }

  consistency_policy {
    consistency_level = "Session"
  }

  geo_location {
    location          = var.location
    failover_priority = 0
  }

  identity {
    type = "SystemAssigned"
  }
} 

resource "azurerm_cosmosdb_sql_database" "cosmos_db" {
  name                = "${var.application_name}-${var.stage}-cosmos-db"
  resource_group_name = azurerm_cosmosdb_account.cosmos_acc.resource_group_name
  account_name        = azurerm_cosmosdb_account.cosmos_acc.name
}

resource "azurerm_cosmosdb_sql_container" "container" {
  name                  = "reports"
  resource_group_name   = azurerm_resource_group.rg.name
  account_name          = azurerm_cosmosdb_account.cosmos_acc.name
  database_name         = azurerm_cosmosdb_sql_database.cosmos_db.name
  partition_key_path    = "/documentType"
  partition_key_version = 1
}

resource "azurerm_linux_function_app" "example" {
  name                       = "${var.application_name}-${var.stage}-fnc"
  location                   = azurerm_resource_group.rg.location
  resource_group_name        = azurerm_resource_group.rg.name
  service_plan_id            = azurerm_service_plan.svc_plan.id
  storage_account_name       = azurerm_storage_account.sa.name
  storage_account_access_key = azurerm_storage_account.sa.primary_access_key

  site_config {
    application_insights_key = azurerm_application_insights.ai.instrumentation_key
  }
  app_settings = {
    "CosmosDBConnection" = azurerm_cosmosdb_account.cosmos_acc.connection_strings[0]
    "CosmosDatabase"     = azurerm_cosmosdb_sql_database.cosmos_db.name
    "CosmosCollection"   = azurerm_cosmosdb_sql_container.container.name
    "BingMapsApiKey"     = var.bingmaps_api_key
  }
}
