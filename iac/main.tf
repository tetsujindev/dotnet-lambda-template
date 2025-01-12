provider "aws" {
  region = "ap-northeast-1"
}

terraform {
  required_providers {
    aws = {
      version = "~> 5.83.1"
    }
  }
}

data "aws_vpc" "default" {
  default = true
}

/*
data "aws_vpc" "custom" {
  filter {
    name   = "tag:Name"
    values = ["custom"]
  }
}
*/

data "aws_subnets" "default" {
  filter {
    name   = "vpc-id"
    values = [data.aws_vpc.default.id]
  }
}

data "aws_security_group" "default" {
  name = "default"
}

resource "aws_db_subnet_group" "default" {
  name       = "default-db-subnet-group"
  subnet_ids = data.aws_subnets.default.ids
}

variable "db_password" {
  type      = string
  sensitive = true
}

resource "aws_db_instance" "tfer--db-instance-postgresql" {
  allocated_storage                   = "20"
  auto_minor_version_upgrade          = "false"
  availability_zone                   = "ap-northeast-1d"
  backup_retention_period             = "0"
  backup_target                       = "region"
  backup_window                       = "15:48-16:18"
  ca_cert_identifier                  = "rds-ca-rsa2048-g1"
  copy_tags_to_snapshot               = "true"
  customer_owned_ip_enabled           = "false"
  db_subnet_group_name                = aws_db_subnet_group.default.name
  dedicated_log_volume                = "false"
  deletion_protection                 = "false"
  engine                              = "postgres"
  engine_lifecycle_support            = "open-source-rds-extended-support-disabled"
  engine_version                      = "16.6"
  iam_database_authentication_enabled = "false"
  identifier                          = "db-instance-postgresql"
  instance_class                      = "db.t4g.micro"
  iops                                = "0"
  //kms_key_id                            = "arn:aws:kms:ap-northeast-1:194722443726:key/cab8aa98-c4a2-4ae1-9d72-5984004f6de2"
  license_model                = "postgresql-license"
  maintenance_window           = "mon:00:00-mon:00:30"
  max_allocated_storage        = "1000"
  monitoring_interval          = "0"
  multi_az                     = "false"
  network_type                 = "IPV4"
  option_group_name            = "default:postgres-16"
  parameter_group_name         = "default.postgres16"
  performance_insights_enabled = "true"
  //performance_insights_kms_key_id       = "arn:aws:kms:ap-northeast-1:194722443726:key/cab8aa98-c4a2-4ae1-9d72-5984004f6de2"
  performance_insights_retention_period = "7"
  port                                  = "5432"
  publicly_accessible                   = "false"
  storage_encrypted                     = "true"
  storage_throughput                    = "0"
  storage_type                          = "gp2"
  username                              = "postgres"
  vpc_security_group_ids                = [data.aws_security_group.default.id]
  skip_final_snapshot                   = true

  password = var.db_password
}

/*
resource "null_resource" "lambda_build" {
  triggers = {
    md5 = "${md5(file("../src/Lambda/StudentLambda/src/StudentLambda/Function.cs"))}"
  }

  provisioner "local-exec" {
    command = "dotnet lambda package --project-location ../src/Lambda/StudentLambda/src/StudentLambda/ --output-package ../src/Lambda/StudentLambda/src/StudentLambda/bin/StudentLambda.zip"
  }
}

data "local_file" "StudentLambda" {
  filename   = "../src/Lambda/StudentLambda/src/StudentLambda/bin/StudentLambda.zip"
  depends_on = [null_resource.lambda_build]
}

resource "aws_lambda_function" "student_function" {
  filename         = data.local_file.StudentLambda.filename
  source_code_hash = data.local_file.StudentLambda.content_base64sha256
  function_name    = "StudentLambda"
  role             = "arn:aws:iam::194722443726:role/lambda-administrator-role"
  handler          = "StudentLambda::StudentLambda.Function::FunctionHandler"
  runtime          = "dotnet8"
  depends_on       = [null_resource.lambda_build]
}
*/
