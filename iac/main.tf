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

resource "aws_db_subnet_group" "default" {
  name       = "default-db-subnet-group"
  subnet_ids = data.aws_subnets.default.ids
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
