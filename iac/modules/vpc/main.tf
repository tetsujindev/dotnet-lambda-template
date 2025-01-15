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

resource "aws_security_group" "rds_sg" {
  name   = var.rds_sg_name
  vpc_id = data.aws_vpc.default.id
}

resource "aws_security_group_rule" "allow_postgresql_inbound" {
  type              = "ingress"
  security_group_id = aws_security_group.rds_sg.id

  cidr_blocks = ["106.73.23.33/32"]
  from_port   = "5432"
  protocol    = "tcp"
  to_port     = "5432"
}

resource "aws_security_group_rule" "allow_all_inbound_from_self" {
  type              = "ingress"
  security_group_id = aws_security_group.rds_sg.id

  self        = true
  from_port   = 0 // すべてのポートを許可
  to_port     = 0 // すべてのポートを許可
  protocol    = "-1"  // -1 はすべてのプロトコルを意味します
}

resource "aws_security_group_rule" "allow_all_outbound" {
  type              = "egress"
  security_group_id = aws_security_group.rds_sg.id

  from_port   = 0 // すべてのポートを許可
  to_port     = 0 // すべてのポートを許可
  protocol    = "-1"  # -1 はすべてのプロトコルを意味します
  cidr_blocks = ["0.0.0.0/0"]  # すべてのIPアドレス範囲を指定
}
