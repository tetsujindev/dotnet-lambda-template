variable "db_subnet_group_name" {
  type = string
}

variable "db_password" {
  type      = string
  sensitive = true
}

variable "vpc_security_group_id" {
  type = string
}

variable "db_identifier" {
  type = string
}
