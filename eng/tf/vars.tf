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