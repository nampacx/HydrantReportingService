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

resource "azurerm_app_service_plan" "svc_plan" {
  name                = "${var.application_name}-${var.stage}-svc-plan"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  kind                = "FunctionApp"

  sku {
    tier = "Dynamic"
    size = "Y1"
  }
}

resource "azurerm_application_insights" "ai" {
  name                = "${var.application_name}-${var.stage}-ai"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  application_type    = "web"
}

resource "azurerm_sql_server" "sql_server" {
  name                         = "${var.application_name}-${var.stage}-sql-server"
  resource_group_name          = azurerm_resource_group.rg.name
  location                     = azurerm_resource_group.rg.location
  version                      = "12.0"
  administrator_login          = var.sql_admin_name
  administrator_login_password = var.sql_admin_password
}

resource "azurerm_sql_firewall_rule" "allowAzureServices" {
  name                = "Allow_Azure_Services"
  resource_group_name = azurerm_resource_group.rg.name
  server_name         = azurerm_sql_server.sql_server.name
  start_ip_address    = "0.0.0.0"
  end_ip_address      = "0.0.0.0"
}

resource "azurerm_sql_database" "database" {
  name                = "${var.application_name}${var.stage}db"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  server_name         = azurerm_sql_server.sql_server.name
}

resource "azurerm_linux_function_app" "example" {
  name                       = "${var.application_name}-${var.stage}-fnc"
  location                   = azurerm_resource_group.rg.location
  resource_group_name        = azurerm_resource_group.rg.name
  service_plan_id            = azurerm_app_service_plan.svc_plan.id
  storage_account_name       = azurerm_storage_account.sa.name
  storage_account_access_key = azurerm_storage_account.sa.primary_access_key

  site_config {
    application_insights_key = azurerm_application_insights.ai.instrumentation_key
  }

  app_settings = {
    SqlConnectionString = "Server=tcp:${azurerm_sql_database.database.name}.database.windows.net,1433;Initial Catalog=${azurerm_sql_database.database.name};Persist Security Info=False;User ID=${var.sql_admin_name};Password=${var.sql_admin_password};MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
