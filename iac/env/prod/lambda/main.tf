provider "aws" {
  region = "ap-northeast-1"
}

terraform {
  required_version = ">= 1.0.0, < 2.0.0"

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.83.1"
    }
  }

  backend "s3" {
    bucket         = "remote-state-bucket-prod-194722443726"
    key            = "lambda/terraform.tfstate" // モジュールごとに異なる値を設定する
    region         = "ap-northeast-1"
    dynamodb_table = "remote-state-lock-table-prod"
    encrypt        = true
  }
}

module "lambda" {
  source = "../../../modules/lambda"

  vpc_security_group_id = data.terraform_remote_state.vpc.outputs.security_group_id
  db_connectionstring = var.db_connectionstring_prod
  function_name = "GetBooksProd"
}

data "terraform_remote_state" "vpc" {
  backend = "s3"

  config = {
    bucket = "remote-state-bucket-prod-194722443726"
    key    = "vpc/terraform.tfstate"
    region = "ap-northeast-1"
  }
}
