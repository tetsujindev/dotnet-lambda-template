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
    bucket         = "remote-state-bucket-stage-194722443726"
    key            = "vpc/terraform.tfstate"  // モジュールごとに異なる値を設定する
    region         = "ap-northeast-1"
    dynamodb_table = "remote-state-lock-table-stage"
    encrypt        = true
  }
}

module "rds_sg" {
  source = "../../../modules/vpc"
  
  rds_sg_name = "rds-sg-stage"
}
