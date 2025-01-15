variable "db_connectionstring" {
  type = string
  sensitive = true
}

variable "vpc_security_group_id" {
  type = string
}
