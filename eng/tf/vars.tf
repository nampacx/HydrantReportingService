variable "location" {
  type = string
  default = "West Europe"
}

variable "application_name" {
  type = string
  default = "hrs"
}

variable "stage"{
    type = string
    default = "dev"
}

variable "sql_admin_name" {
    type = string
    default = "serveradmin"
}

variable "sql_admin_password" {
    type = string
    default = "4-v3ry-53cr37-p455w0rd"
}

 variable "resource_group_name"{
    type = string
    default =  "rg-githubactions-tf-states"
 }
  
  varialbe   "storage_account_name"{
    type = string
    default ="mkgithubtfstates"
  }
  variable  "container_name"{
    type = string
    default ="tfstatedevops"
  } 

  variable "key" {
    type = string
    default ="terraformgithubexample.tfstate"
  }