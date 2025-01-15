terraform {
  required_version = ">= 1.0.0, < 2.0.0"

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.83.1"
    }
  }
}

data "aws_vpc" "default" {
  default = true
}

data "aws_subnets" "default" {
  filter {
    name   = "vpc-id"
    values = [data.aws_vpc.default.id]
  }
}

resource "aws_db_subnet_group" "default" {
  name       = var.db_subnet_group_name
  subnet_ids = data.aws_subnets.default.ids
}

resource "aws_db_instance" "db-instance-postgresql" {
  allocated_storage                     = "20"
  auto_minor_version_upgrade            = "false"
  availability_zone                     = "ap-northeast-1d"
  backup_retention_period               = "0"
  backup_target                         = "region"
  backup_window                         = "15:48-16:18"
  ca_cert_identifier                    = "rds-ca-rsa2048-g1"
  copy_tags_to_snapshot                 = "true"
  customer_owned_ip_enabled             = "false"
  db_subnet_group_name                  = aws_db_subnet_group.default.name
  dedicated_log_volume                  = "false"
  deletion_protection                   = "false"
  engine                                = "postgres"
  engine_lifecycle_support              = "open-source-rds-extended-support-disabled"
  engine_version                        = "16.6"
  iam_database_authentication_enabled   = "false"
  identifier                            = var.db_identifier
  instance_class                        = "db.t4g.micro"
  iops                                  = "0"
  license_model                         = "postgresql-license"
  maintenance_window                    = "mon:00:00-mon:00:30"
  max_allocated_storage                 = "1000"
  monitoring_interval                   = "0"
  multi_az                              = "false"
  network_type                          = "IPV4"
  option_group_name                     = "default:postgres-16"
  parameter_group_name                  = "default.postgres16"
  performance_insights_enabled          = "true"
  performance_insights_retention_period = "7"
  port                                  = "5432"
  publicly_accessible                   = "true"
  storage_encrypted                     = "true"
  storage_throughput                    = "0"
  storage_type                          = "gp2"
  username                              = "postgres"
  vpc_security_group_ids                = [var.vpc_security_group_id]
  skip_final_snapshot                   = true
  password                              = var.db_password
  //kms_key_id                            = "arn:aws:kms:ap-northeast-1:194722443726:key/cab8aa98-c4a2-4ae1-9d72-5984004f6de2"
  //performance_insights_kms_key_id       = "arn:aws:kms:ap-northeast-1:194722443726:key/cab8aa98-c4a2-4ae1-9d72-5984004f6de2"
}
