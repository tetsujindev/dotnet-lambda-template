terraform {
  required_version = ">= 1.0.0, < 2.0.0"

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.83.1"
    }
  }
}

resource "null_resource" "package_book_lambda" {
  triggers = {
    always_run = timestamp()
  }

  provisioner "local-exec" {
    command = "dotnet lambda package --project-location ../../../../src/Lambda/BookLambda/src/BookLambda/ --output-package ../../../../src/Lambda/BookLambda/src/BookLambda/bin/Release/net8.0/publish/BookLambda.zip"
  }
}

data "local_file" "BookLambda" {
  filename   = "../../../../src/Lambda/BookLambda/src/BookLambda/bin/Release/net8.0/publish/BookLambda.zip"
  depends_on = [null_resource.package_book_lambda]
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

resource "aws_lambda_function" "get_books" {
  filename         = data.local_file.BookLambda.filename
  source_code_hash = data.local_file.BookLambda.content_base64sha256
  function_name    = "GetBooks"
  role             = "arn:aws:iam::194722443726:role/lambda-administrator-role"
  handler          = "BookLambda::BookLambda.Function::GetBooks"
  runtime          = "dotnet8"
  timeout          = "30"

  vpc_config {
    ipv6_allowed_for_dual_stack = "false"
    security_group_ids          = [var.vpc_security_group_id]
    subnet_ids                  = data.aws_subnets.default.ids
  }

  environment {
    variables = {
      connectionstring = var.db_connectionstring
    }
  }

  depends_on = [data.local_file.BookLambda]
}
