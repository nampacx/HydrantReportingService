variable "location" {
  type    = string
  default = "West Europe"
}

variable "application_name" {
  type    = string
  default = "hrsvc"
}

variable "stage" {
  type    = string
  default = "dev"
}

variable "bingmaps_api_key" {
  type      = string
  sensitive = true
}