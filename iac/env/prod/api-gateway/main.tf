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
    key            = "api-gateway/terraform.tfstate"  // モジュールごとに異なる値を設定する
    region         = "ap-northeast-1"
    dynamodb_table = "remote-state-lock-table-prod"
    encrypt        = true
  }
}

module "api_gateway" {
  source = "../../../modules/api-gateway"
  
  api_name = "prod"
  get_books_invoke_arn = data.terraform_remote_state.lambda.outputs.get_books_invoke_arn
  get_books_function_name = "GetBooksProd"
}

data "terraform_remote_state" "lambda" {
  backend = "s3"

  config = {
    bucket = "remote-state-bucket-prod-194722443726"
    key    = "lambda/terraform.tfstate"
    region = "ap-northeast-1"
  }
}
