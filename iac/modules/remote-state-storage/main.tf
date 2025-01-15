terraform {
  required_version = ">= 1.0.0, < 2.0.0"

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.83.1"
    }
  }
}

resource "aws_s3_bucket" "remote_state_bucket" {
  bucket = var.remote_state_bucket_name
  // lifecycle { prevent_destroy = true } // リリース後はバケットの削除を禁止
  force_destroy = true // バケットの削除時に中身があっても削除する。検証時のみ有効にする
}

resource "aws_s3_bucket_versioning" "remote_state_bucket" {
  bucket = aws_s3_bucket.remote_state_bucket.id
  versioning_configuration {
    status = "Enabled"
  }
}

resource "aws_s3_bucket_public_access_block" "remote_state_bucket" {
  bucket                  = aws_s3_bucket.remote_state_bucket.id
  block_public_acls       = true
  block_public_policy     = true
  ignore_public_acls      = true
  restrict_public_buckets = true
}

resource "aws_dynamodb_table" "remote_state_lock_table" {
  name         = var.remote_state_lock_table_name
  billing_mode = "PAY_PER_REQUEST"
  hash_key     = "LockID"

  attribute {
    name = "LockID"
    type = "S"
  }
}

output "s3_bucket_arn" {
  value = aws_s3_bucket.remote_state_bucket.arn
}

output "dynamodb_table_name" {
  value = aws_dynamodb_table.remote_state_lock_table.name
}
