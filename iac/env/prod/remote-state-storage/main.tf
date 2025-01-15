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
  /*
  backend "s3" {
    bucket         = "remote-state-bucket-prod-194722443726"
    key            = "remote-state-storage/terraform.tfstate"
    region         = "ap-northeast-1"
    dynamodb_table = "remote-state-lock-table-prod"
    encrypt        = true
  }
  // */
}

module "remote_state_storage" {
  source = "../../../modules/remote-state-storage"

  remote_state_bucket_name     = "remote-state-bucket-prod-194722443726"
  remote_state_lock_table_name = "remote-state-lock-table-prod"
}
